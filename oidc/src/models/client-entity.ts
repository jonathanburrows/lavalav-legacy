import { AllowedScope } from './allowed-scope';
import { ClaimEntity } from './claim-entity';
import { CorsOrigin } from './cors-origin';
import { GrantTypeEntity } from './grant-type-entity';
import { IdentityProviderRestriction } from './identity-provider-restriction';
import { PostLogoutRedirectUri } from './post-logout-redirect-uri';
import { RedirectUri } from './redirect-uri';
import { SecretEntity } from './secret-entity';
import { Entity, IAggregateRoot, Required } from '@lvl/front-end';

export class ClientEntity extends Entity implements IAggregateRoot {
    public enabled: boolean;
    @Required() public clientId: string;
    @Required() public protocolType: string;
    public clientSecrets: SecretEntity[];
    public requireClientSecret: boolean;
    @Required() public clientName: string;
    @Required() public clientUri: string;
    public logoUri: string;
    public requireConsent: boolean;
    public allowRememberConsent: boolean;
    public allowedGrantTypes: GrantTypeEntity[];
    public requirePkce: boolean;
    public allowPlainTextPkce: boolean;
    public allowAccessTokensViaBrowser: boolean;
    public redirectUris: RedirectUri[];
    public postLogoutRedirectUris: PostLogoutRedirectUri[];
    public logoutUri: string;
    public logoutSessionRequired: boolean;
    public allowOfflineAccess: boolean;
    public allowedScopes: AllowedScope[];
    public alwaysIncludeUserClaimsInIdToken: boolean;
    public identityTokenLifetime: number;
    public accessTokenLifetime: number;
    public authorizationCodeLifetime: number;
    public absoluteRefreshTokenLifetime: number;
    public slidingRefreshTokenLifetime: number;
    public refreshTokenUsage: number;
    public updateAccessTokenClaimsOnRefresh: boolean;
    public refreshTokenExpiration: number;
    public accessTokenType: number;
    public enableLocalLogin: boolean;
    public identityProviderRestrictions: IdentityProviderRestriction[];
    public includeJwtId: boolean;
    public claims: ClaimEntity[];
    public alwaysSendClientClaims: boolean;
    public prefixClientClaims: boolean;
    public allowedCorsOrigins: CorsOrigin[];
    public id: number;

    constructor(options?: ClientEntity) {
        super();
        if (options) {
            this.enabled = options.enabled;
            this.clientId = options.clientId;
            this.protocolType = options.protocolType;
            if (options.clientSecrets) {
                this.clientSecrets = options.clientSecrets.map(p => new SecretEntity(p));
            }
            this.requireClientSecret = options.requireClientSecret;
            this.clientName = options.clientName;
            this.clientUri = options.clientUri;
            this.logoUri = options.logoUri;
            this.requireConsent = options.requireConsent;
            this.allowRememberConsent = options.allowRememberConsent;
            if (options.allowedGrantTypes) {
                this.allowedGrantTypes = options.allowedGrantTypes.map(p => new GrantTypeEntity(p));
            }
            this.requirePkce = options.requirePkce;
            this.allowPlainTextPkce = options.allowPlainTextPkce;
            this.allowAccessTokensViaBrowser = options.allowAccessTokensViaBrowser;
            if (options.redirectUris) {
                this.redirectUris = options.redirectUris.map(p => new RedirectUri(p));
            }
            if (options.postLogoutRedirectUris) {
                this.postLogoutRedirectUris = options.postLogoutRedirectUris.map(p => new PostLogoutRedirectUri(p));
            }
            this.logoutUri = options.logoutUri;
            this.logoutSessionRequired = options.logoutSessionRequired;
            this.allowOfflineAccess = options.allowOfflineAccess;
            if (options.allowedScopes) {
                this.allowedScopes = options.allowedScopes.map(p => new AllowedScope(p));
            }
            this.alwaysIncludeUserClaimsInIdToken = options.alwaysIncludeUserClaimsInIdToken;
            this.identityTokenLifetime = options.identityTokenLifetime;
            this.accessTokenLifetime = options.accessTokenLifetime;
            this.authorizationCodeLifetime = options.authorizationCodeLifetime;
            this.absoluteRefreshTokenLifetime = options.absoluteRefreshTokenLifetime;
            this.slidingRefreshTokenLifetime = options.slidingRefreshTokenLifetime;
            this.refreshTokenUsage = options.refreshTokenUsage;
            this.updateAccessTokenClaimsOnRefresh = options.updateAccessTokenClaimsOnRefresh;
            this.refreshTokenExpiration = options.refreshTokenExpiration;
            this.accessTokenType = options.accessTokenType;
            this.enableLocalLogin = options.enableLocalLogin;
            if (options.identityProviderRestrictions) {
                this.identityProviderRestrictions = options.identityProviderRestrictions.map(p => new IdentityProviderRestriction(p));
            }
            this.includeJwtId = options.includeJwtId;
            if (options.claims) {
                this.claims = options.claims.map(p => new ClaimEntity(p));
            }
            this.alwaysSendClientClaims = options.alwaysSendClientClaims;
            this.prefixClientClaims = options.prefixClientClaims;
            if (options.allowedCorsOrigins) {
                this.allowedCorsOrigins = options.allowedCorsOrigins.map(p => new CorsOrigin(p));
            }
            this.id = options.id;
        }
    }
}
