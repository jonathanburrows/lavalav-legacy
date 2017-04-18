using IdentityModel;
using IdentityServer4;
using lvl.Oidc.AuthorizationServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    public class ExternalLoginController : ControllerBase
    {
        private UserStore UserStore { get; }
        private OidcAuthorizationServerOptions Options { get; }

        public ExternalLoginController(UserStore userStore, OidcAuthorizationServerOptions options)
        {
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [HttpGet]
        public async Task<IActionResult> Login(string provider, string returnUrl)
        {
            var callbackUrl = Url.Action(nameof(LoginCallback), new { returnUrl = returnUrl });

            var windowsProvider = Options.WindowsProviders.FirstOrDefault(wp => wp.AuthenticationScheme.Equals(provider, StringComparison.OrdinalIgnoreCase));
            if (windowsProvider == null)
            {
                var externalAuthenticationProperties = new AuthenticationProperties
                {
                    RedirectUri = callbackUrl,
                    Items = { { "scheme", provider } }
                };
                return new ChallengeResult(provider, externalAuthenticationProperties);
            }
            else if (User is WindowsPrincipal)
            {
                var windowsAuthenticationProperties = new AuthenticationProperties
                {
                    Items = { ["scheme"] = windowsProvider.AuthenticationScheme }
                };

                var claimsIdentity = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.NameIdentifier, User.Identity.Name),
                    new Claim(ClaimTypes.Name, User.Identity.Name)
                });
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.Authentication.SignInAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme, claimsPrincipal, windowsAuthenticationProperties);
                return Redirect(callbackUrl);
            }
            else
            {
                var windowsProviders = Options.WindowsProviders.Select(wp => wp.AuthenticationScheme).ToList();
                return new ChallengeResult(windowsProviders);
            }

        }

        [HttpGet]
        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            if (returnUrl == null)
            {
                throw new ArgumentNullException(nameof(returnUrl));
            }

            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new InvalidOperationException("external authentication error.");
            }

            var claims = tempUser.Claims.ToList();
            var subject = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);
            var nameIdentifier = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var userIdClaim = subject ?? nameIdentifier;
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("unknown userid");
            }

            claims.Remove(userIdClaim);
            var provider = info.Properties.Items["scheme"];
            var userId = userIdClaim.Value;

            var user = await UserStore.FindByExternalProviderAsync(provider, userId) ?? await UserStore.AddExternalUser(provider, userId, claims);

            var sessionClaims = claims.Where(c => c.Type == JwtClaimTypes.SessionId).ToArray();
            var idToken = info.Properties.GetTokenValue("id_token");
            var properties = idToken != null ? new AuthenticationProperties(new Dictionary<string, string> { ["id_token"] = idToken }) : null;

            await HttpContext.Authentication.SignInAsync(user.SubjectId, user.Username, provider, properties, sessionClaims);
            await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            return Redirect(returnUrl);
        }
    }
}
