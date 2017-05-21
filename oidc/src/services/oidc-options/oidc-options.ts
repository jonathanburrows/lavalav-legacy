import { Injectable } from '@angular/core';

/**
 *  Options required for the client to get tokens from an authorization server.
 */
@Injectable()
export class OidcOptions {
    /* Server which issues tokens. */
    public authorizationServerUrl: string;

    /* Identifier of the client registered on the authentication server. */
    public clientId: string;

    /* The secret to validate the client identity. */
    public clientSecret: string;

    /**
     *  The required scopes and permissions needed by the app to function.
     *  @remarks - openid and token are added by default in the security services.
     */
    public scopes: string[];
}
