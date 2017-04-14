using IdentityServer4.Services;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using lvl.Oidc.AuthorizationServer.ViewModels;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    [Route("oidc/[controller]")]
    public class LoginController : Controller
    {
        private IIdentityServerInteractionService InteractionService { get; }
        private UserStore UserStore { get; }
        private ExternalProviderNegotiator ExternalProviderNegotiator { get; }
        private OidcAuthorizationServerOptions Options { get; }

        public LoginController(IIdentityServerInteractionService interactionService, UserStore userStore, ExternalProviderNegotiator externalProviderNegotiator, OidcAuthorizationServerOptions options)
        {
            InteractionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            ExternalProviderNegotiator = externalProviderNegotiator ?? throw new ArgumentNullException(nameof(externalProviderNegotiator));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            if (returnUrl == null)
            {
                throw new ArgumentNullException(nameof(returnUrl));
            }
            if (!InteractionService.IsValidReturnUrl(returnUrl))
            {
                throw new ArgumentException($"'{returnUrl}' is not a valid return url.");
            }

            var authorizationContext = await InteractionService.GetAuthorizationContextAsync(returnUrl);
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                Username = authorizationContext?.LoginHint
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!InteractionService.IsValidReturnUrl(model.ReturnUrl))
            {
                throw new InvalidOperationException($"The url of '{model.ReturnUrl}' is not a valid return url.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserStore.FindByUsernameAsync(model.Username);
            if(user == null)
            {
                ModelState.AddModelError(nameof(LoginViewModel.Username), "Can't find user");
                return View(model);
            }

            if(!await UserStore.ValidateCredentialsAsync(model.Username, model.Password))
            {
                ModelState.AddModelError(nameof(LoginViewModel.Password), "Try again");
                return View(model);
            }

            var expiry = DateTimeOffset.UtcNow.Add(Options.RefreshTokenLifetime);
            var props = model.RememberLogin ? new AuthenticationProperties { ExpiresUtc = expiry, IsPersistent = true } : null;
            await HttpContext.Authentication.SignInAsync(user.SubjectId, user.Username, props);

            return Redirect(model.ReturnUrl);
        }
    }
}
