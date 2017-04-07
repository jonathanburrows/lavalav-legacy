using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using IdentityServer4.Models;
using lvl.Ontology;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Models
{
    public class ClientEntity : IEntity, IAggregateRoot
    {
        public int Id { get; set; }

        //
        // Summary:
        //     Specifies is the user's session id should be sent to the LogoutUri. Defaults
        //     to true.
        public bool LogoutSessionRequired { get; set; }

        //
        // Summary:
        //     When requesting both an id token and access token, should the user claims always
        //     be added to the id token instead of requring the client to use the userinfo endpoint.
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        //
        // Summary:
        //     Lifetime of identity token in seconds (defaults to 300 seconds / 5 minutes)
        public int IdentityTokenLifetime { get; set; }

        //
        // Summary:
        //     Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
        public int AccessTokenLifetime { get; set; }

        //
        // Summary:
        //     Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
        public int AuthorizationCodeLifetime { get; set; }

        //
        // Summary:
        //     Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds /
        //     30 days
        public int AbsoluteRefreshTokenLifetime { get; set; }

        //
        // Summary:
        //     Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds /
        //     15 days
        public int SlidingRefreshTokenLifetime { get; set; }

        //
        // Summary:
        //     ReUse: the refresh token handle will stay the same when refreshing tokens OneTime:
        //     the refresh token handle will be updated when refreshing tokens
        public TokenUsage RefreshTokenUsage { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether the access token (and its claims) should
        //     be updated on a refresh token request.
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        //
        // Summary:
        //     Absolute: the refresh token will expire on a fixed point in time (specified by
        //     the AbsoluteRefreshTokenLifetime) Sliding: when refreshing the token, the lifetime
        //     of the refresh token will be renewed (by the amount specified in SlidingRefreshTokenLifetime).
        //     The lifetime will not exceed AbsoluteRefreshTokenLifetime.
        public TokenExpiration RefreshTokenExpiration { get; set; }

        //
        // Summary:
        //     Specifies whether the access token is a reference token or a self contained JWT
        //     token (defaults to Jwt).
        public AccessTokenType AccessTokenType { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether the local login is allowed for this client.
        //     Defaults to true.
        public bool EnableLocalLogin { get; set; }

        //
        // Summary:
        //     Specifies which external IdPs can be used with this client (if list is empty
        //     all IdPs are allowed). Defaults to empty.
        public ICollection<string> IdentityProviderRestrictions { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether JWT access tokens should include an identifier
        public bool IncludeJwtId { get; set; }

        //
        // Summary:
        //     Allows settings claims for the client (will be included in the access token).
        public ICollection<ClaimEntity> Claims { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether client claims should be always included
        //     in the access tokens - or only for client credentials flow.
        public bool AlwaysSendClientClaims { get; set; }

        //
        // Summary:
        //     Specifies the api scopes that the client is allowed to request. If empty, the
        //     client can't access any scope
        public ICollection<string> AllowedScopes { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether [allow offline access].
        public bool AllowOfflineAccess { get; set; }

        //
        // Summary:
        //     Gets or sets the allowed CORS origins for JavaScript clients.
        public ICollection<string> AllowedCorsOrigins { get; set; }

        //
        // Summary:
        //     Specifies logout URI at client for HTTP based logout.
        public string LogoutUri { get; set; }

        //
        // Summary:
        //     Specifies if client is enabled (defaults to true)
        public bool Enabled { get; set; }

        //
        // Summary:
        //     Unique ID of the client
        public string ClientId { get; set; }

        //
        // Summary:
        //     Gets or sets the protocol type.
        public string ProtocolType { get; set; }

        //
        // Summary:
        //     Client secrets - only relevant for flows that require a secret
        public ICollection<SecretEntity> ClientSecrets { get; set; }

        //
        // Summary:
        //     If set to false, no client secret is needed to request tokens at the token endpoint
        //     (defaults to true)
        public bool RequireClientSecret { get; set; }

        //
        // Summary:
        //     Client display name (used for logging and consent screen)
        public string ClientName { get; set; }

        //
        // Summary:
        //     URI to further information about client (used on consent screen)
        public string ClientUri { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether all client claims should be prefixed.
        public bool PrefixClientClaims { get; set; }

        //
        // Summary:
        //     URI to client logo (used on consent screen)
        public string LogoUri { get; set; }

        //
        // Summary:
        //     Specifies whether user can choose to store consent decisions (defaults to true)
        public bool AllowRememberConsent { get; set; }

        //
        // Summary:
        //     Specifies the allowed grant types (legal combinations of AuthorizationCode, Implicit,
        //     Hybrid, ResourceOwner, ClientCredentials). Defaults to Implicit.
        public IEnumerable<string> AllowedGrantTypes { get; set; }

        //
        // Summary:
        //     Specifies whether a proof key is required for authorization code based token
        //     requests
        public bool RequirePkce { get; set; }

        //
        // Summary:
        //     Specifies whether a proof key can be sent using plain method (not recommended
        //     and default to false)
        public bool AllowPlainTextPkce { get; set; }

        //
        // Summary:
        //     Controls whether access tokens are transmitted via the browser for this client
        //     (defaults to false). This can prevent accidental leakage of access tokens when
        //     multiple response types are allowed.
        public bool AllowAccessTokensViaBrowser { get; set; }

        //
        // Summary:
        //     Specifies allowed URIs to return tokens or authorization codes to
        public ICollection<string> RedirectUris { get; set; }

        //
        // Summary:
        //     Specifies allowed URIs to redirect to after logout
        public ICollection<string> PostLogoutRedirectUris { get; set; }

        //
        // Summary:
        //     Specifies whether a consent screen is required (defaults to true)
        public bool RequireConsent { get; set; }

        public Client ToIdentityClient()
        {
            return new Client
            {
                AbsoluteRefreshTokenLifetime = AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = AccessTokenLifetime,
                AccessTokenType = AccessTokenType,
                AllowAccessTokensViaBrowser = AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = AllowedCorsOrigins,
                AllowedGrantTypes = AllowedGrantTypes,
                AllowedScopes = AllowedScopes,
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
                IdentityProviderRestrictions = IdentityProviderRestrictions,
                IdentityTokenLifetime = IdentityTokenLifetime,
                IncludeJwtId = IncludeJwtId,
                LogoUri = LogoUri,
                LogoutSessionRequired = LogoutSessionRequired,
                LogoutUri = LogoutUri,
                PostLogoutRedirectUris = PostLogoutRedirectUris,
                PrefixClientClaims = PrefixClientClaims,
                ProtocolType = ProtocolType,
                RedirectUris = RedirectUris,
                RefreshTokenExpiration = RefreshTokenExpiration,
                RefreshTokenUsage = RefreshTokenUsage,
                RequireClientSecret = RequireClientSecret,
                RequireConsent = RequireConsent,
                RequirePkce = RequirePkce,
                SlidingRefreshTokenLifetime = SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = UpdateAccessTokenClaimsOnRefresh
            };
        }

        public bool AllowsOrigin(string origin)
        {
            return AllowedCorsOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
        }
    }

    public class ClientOverride : IAutoMappingOverride<ClientEntity>
    {
        public void Override(AutoMapping<ClientEntity> mapping)
        {
            mapping.HasMany(identity => identity.IdentityProviderRestrictions).Element("Value");
            mapping.HasMany(identity => identity.AllowedScopes).Element("Value");
            mapping.HasMany(identity => identity.AllowedCorsOrigins).Element("Value");
            mapping.HasMany(identity => identity.AllowedGrantTypes).Element("Value");
            mapping.HasMany(identity => identity.RedirectUris).Element("Value");
            mapping.HasMany(identity => identity.PostLogoutRedirectUris).Element("Value");
        }
    }
}
