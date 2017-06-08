using FluentNHibernate.Data;
using IdentityServer4;
using IdentityServer4.Models;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using lvl.Ontology.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Models an OpenID Connect or OAuth2 client
    /// </summary>
    [Table(nameof(Client), Schema = "oidc")]
    [HiddenFromApi]
    public class ClientEntity : Entity, IAggregateRoot
    {
        /// <summary>
        ///     Specifies if client is enabled (defaults to true)
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        ///     Unique ID of the client
        /// </summary>
        [Unique, Required]
        public string ClientId { get; set; }

        /// <summary>
        ///     Gets or sets the protocol type.
        /// </summary>
        /// <value>
        ///     The protocol type.
        /// </value>
        [Required]
        public string ProtocolType { get; set; } = IdentityServerConstants.ProtocolTypes.OpenIdConnect;

        /// <summary>
        ///     Client secrets - only relevant for flows that require a secret
        /// </summary>
        public IEnumerable<SecretEntity> ClientSecrets { get; set; }

        /// <summary>
        ///     If set to false, no client secret is needed to request tokens at the token endpoint (defaults to true)
        /// </summary>
        public bool RequireClientSecret { get; set; } = true;

        /// <summary>
        ///     Client display name (used for logging and consent screen)
        /// </summary>
        [Required]
        public string ClientName { get; set; }

        /// <summary>
        ///     URI to further information about client (used on consent screen)
        /// </summary>
        [Required]
        public string ClientUri { get; set; }

        /// <summary>
        ///     URI to client logo (used on consent screen)
        /// </summary>
        public string LogoUri { get; set; }

        /// <summary>
        ///     Specifies whether a consent screen is required (defaults to true)
        /// </summary>
        public bool RequireConsent { get; set; } = true;

        /// <summary>
        ///     Specifies whether user can choose to store consent decisions (defaults to true)
        /// </summary>
        public bool AllowRememberConsent { get; set; } = true;

        /// <summary>
        ///     Specifies the allowed grant types (legal combinations of AuthorizationCode, Implicit, Hybrid, ResourceOwner, ClientCredentials). Defaults to Implicit.
        /// </summary>
        public IEnumerable<GrantTypeEntity> AllowedGrantTypes { get; set; } = GrantTypes.Implicit.Select(gt => new GrantTypeEntity { Name = gt });

        /// <summary>
        ///     Specifies whether a proof key is required for authorization code based token requests
        /// </summary>
        public bool RequirePkce { get; set; } = false;

        /// <summary>
        ///     Specifies whether a proof key can be sent using plain method (not recommended and default to false)
        /// </summary>
        public bool AllowPlainTextPkce { get; set; } = false;

        /// <summary>
        ///     Controls whether access tokens are transmitted via the browser for this client (defaults to false).
        ///     This can prevent accidental leakage of access tokens when multiple response types are allowed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if access tokens can be transmitted via the browser; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAccessTokensViaBrowser { get; set; } = false;

        /// <summary>
        ///     Specifies allowed URIs to return tokens or authorization codes to
        /// </summary>
        public IEnumerable<RedirectUri> RedirectUris { get; set; }

        /// <summary>
        ///     Specifies allowed URIs to redirect to after logout
        /// </summary>
        public IEnumerable<PostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }

        /// <summary>
        ///     Specifies logout URI at client for HTTP based logout.
        /// </summary>
        public string LogoutUri { get; set; }

        /// <summary>
        ///     Specifies is the user's session id should be sent to the LogoutUri. Defaults to true.
        /// </summary>
        public bool LogoutSessionRequired { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether [allow offline access].
        /// </summary>
        public bool AllowOfflineAccess { get; set; } = false;

        /// <summary>
        ///     Specifies the api scopes that the client is allowed to request. If empty, the client can't access any scope
        /// </summary>
        public IEnumerable<AllowedScope> AllowedScopes { get; set; }

        /// <summary>
        ///     When requesting both an id token and access token, should the user claims always be added to the id token instead of requring the client to use the userinfo endpoint.
        /// </summary>
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; } = false;

        /// <summary>
        ///     Lifetime of identity token in seconds (defaults to 300 seconds / 5 minutes)
        /// </summary>
        public int IdentityTokenLifetime { get; set; } = 300;

        /// <summary>
        ///     Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
        /// </summary>
        public int AccessTokenLifetime { get; set; } = 3600;

        /// <summary>
        ///     Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
        /// </summary>
        public int AuthorizationCodeLifetime { get; set; } = 300;

        /// <summary>
        ///     Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds / 30 days
        /// </summary>
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        /// <summary>
        ///     Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds / 15 days
        /// </summary>
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        /// <summary>
        ///     ReUse: the refresh token handle will stay the same when refreshing tokens
        ///     OneTime: the refresh token handle will be updated when refreshing tokens
        /// </summary>
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;

        /// <summary>
        ///     Gets or sets a value indicating whether the access token (and its claims) should be updated on a refresh token request.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the token should be updated; otherwise, <c>false</c>.
        /// </value>
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; } = false;

        /// <summary>
        ///     Absolute: the refresh token will expire on a fixed point in time (specified by the AbsoluteRefreshTokenLifetime)
        ///     Sliding: when refreshing the token, the lifetime of the refresh token will be renewed (by the amount specified in SlidingRefreshTokenLifetime). The lifetime will not exceed AbsoluteRefreshTokenLifetime.
        /// </summary>        
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;

        /// <summary>
        ///     Specifies whether the access token is a reference token or a self contained JWT token (defaults to Jwt).
        /// </summary>
        public int AccessTokenType { get; set; } = (int)IdentityServer4.Models.AccessTokenType.Jwt;

        /// <summary>
        ///     Gets or sets a value indicating whether the local login is allowed for this client. Defaults to <c>true</c>.
        /// </summary>
        /// <value>
        ///     <c>true</c> if local logins are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary>
        ///     Specifies which external IdPs can be used with this client (if list is empty all IdPs are allowed). Defaults to empty.
        /// </summary>
        public IEnumerable<IdentityProviderRestriction> IdentityProviderRestrictions { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether JWT access tokens should include an identifier
        /// </summary>
        /// <value>
        ///     <c>true</c> to add an id; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeJwtId { get; set; } = false;

        /// <summary>
        ///     Allows settings claims for the client (will be included in the access token).
        /// </summary>
        /// <value>
        ///     The claims.
        /// </value>
        public IEnumerable<ClaimEntity> Claims { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether client claims should be always included in the access tokens - or only for client credentials flow.
        /// </summary>
        /// <value>
        ///     <c>true</c> if claims should always be sent; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysSendClientClaims { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether all client claims should be prefixed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if client claims should be prefixed; otherwise, <c>false</c>.
        /// </value>
        public bool PrefixClientClaims { get; set; } = true;

        /// <summary>
        ///     Gets or sets the allowed CORS origins for JavaScript clients.
        /// </summary>
        /// <value>
        ///     The allowed CORS origins.
        /// </value>
        public IEnumerable<CorsOrigin> AllowedCorsOrigins { get; set; }

        /// <summary>
        ///     Converts the POCO into something that can be used by Identity Server
        /// </summary>
        public Client ToIdentityClient()
        {
            return new Client
            {
                AbsoluteRefreshTokenLifetime = AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = AccessTokenLifetime,
                AccessTokenType = (AccessTokenType)AccessTokenType,
                AllowAccessTokensViaBrowser = AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = AllowedCorsOrigins.Select(co => co.Name).ToList(),
                AllowedGrantTypes = AllowedGrantTypes.Select(gt => gt.Name).ToList(),
                AllowedScopes = AllowedScopes.Select(co => co.Name).ToList(),
                AllowOfflineAccess = AllowOfflineAccess,
                AllowPlainTextPkce = AllowPlainTextPkce,
                AllowRememberConsent = AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = AlwaysSendClientClaims,
                AuthorizationCodeLifetime = AuthorizationCodeLifetime,
                Claims = Claims.Select(c => c.ToSecurityClaim()).ToList(),
                ClientId = ClientId,
                ClientName = ClientName,
                ClientSecrets = ClientSecrets.Select(c => c.ToIdentityServer()).ToList(),
                ClientUri = ClientUri,
                Enabled = Enabled,
                EnableLocalLogin = EnableLocalLogin,
                IdentityProviderRestrictions = IdentityProviderRestrictions.Select(ipr => ipr.Name).ToList(),
                IdentityTokenLifetime = IdentityTokenLifetime,
                IncludeJwtId = IncludeJwtId,
                LogoUri = LogoUri,
                LogoutSessionRequired = LogoutSessionRequired,
                LogoutUri = LogoutUri,
                PostLogoutRedirectUris = PostLogoutRedirectUris.Select(ru => ru.Name).ToList(),
                PrefixClientClaims = PrefixClientClaims,
                ProtocolType = ProtocolType,
                RedirectUris = RedirectUris.Select(ru => ru.Name).ToList(),
                RefreshTokenExpiration = (TokenExpiration)RefreshTokenExpiration,
                RefreshTokenUsage = (TokenUsage)RefreshTokenUsage,
                RequireClientSecret = RequireClientSecret,
                RequireConsent = RequireConsent,
                RequirePkce = RequirePkce,
                SlidingRefreshTokenLifetime = SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = UpdateAccessTokenClaimsOnRefresh
            };
        }

        /// <summary>
        ///     Determines if the client has the given domain registered.
        /// </summary>
        /// <param name="origin">The domain being requested.</param>
        /// <returns>True if the client expects the given domain, false otherwise.</returns>
        public bool AllowsOrigin(string origin)
        {
            if(origin == null)
            {
                throw new ArgumentNullException(nameof(origin));
            }

            return AllowedCorsOrigins.Any(corsOrigin => corsOrigin.Name.Equals(origin, StringComparison.OrdinalIgnoreCase));
        }
    }
}
