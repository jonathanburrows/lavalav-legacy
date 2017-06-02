import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { HeadersService, StorageService } from '@lvl/front-end';

import { OidcOptions } from '../oidc-options';
import { Credentials } from './credentials';
import { BearerToken, TokenService } from '../token';

/**
 *  Abstraction of logic required by all the authentication flows. Contains logic on how to redirect after login.
 *  @remarks - each authentication flow should have it's on implementation.
 */
@Injectable()
export abstract class SecurityService {
    public get isAuthorized() {
        return this.tokenService.bearerToken && !this.tokenService.tokenIsExpired();
    }

    private postLoginRedirectUrlKey = 'oidc:postLoginRedirectUrl';

    /**
     *  The claims of the user.
     *  @remarks claims are placed here instead of the token service because they are accessed in different ways based on the flow.
     */
    public abstract userInfo: { [key: string]: any };

    /**
     *  Location on where to return to after authentication is complete.
     *  @remarks - it is stored in local storage because a redirect off site may take place.
     */
    private _postLoginRedirectUrl: string;
    public get postLoginRedirectUrl() {
        if (!this._postLoginRedirectUrl) {
            this._postLoginRedirectUrl = this.storageService.getItem(this.postLoginRedirectUrlKey);
        }

        // If no route is given, take it to the root of the application.
        return this._postLoginRedirectUrl || '/';
    }
    public set postLoginRedirectUrl(value: string) {
        this._postLoginRedirectUrl = value;

        if (value) {
            this.storageService.setItem(this.postLoginRedirectUrlKey, value);
        } else {
            this.storageService.removeItem(this.postLoginRedirectUrlKey);
        }
    }

    constructor(
        protected http: Http,
        protected router: Router,
        protected tokenService: TokenService,
        protected oidcOptions: OidcOptions,
        protected storageService: StorageService,
        protected headersService: HeadersService
    ) {
        this.tokenService.tokenHalflife.subscribe(_ => this.refreshToken());
    }

    /**
     *  Redirects the user to a login page where they can enter their credentials.
     */
    public abstract redirectToLogin();

    /**
     *  Exchanges credentials or a access code for a token.
     *  @param credentials The user entered credentials.
     *  @remarks credentials is not required by all resource flows, but for consistency, it was added here.
     */
    public abstract login(credentials?: Credentials): Observable<BearerToken>;

    /**
     *  Invalides a user's tokens and redirects them to an anonymous page.
     */
    public abstract logout();

    /**
     *  Invalidates tokens stored in local storage. Allows for single sign off from other applications.
     */
    public abstract logoutLocal();

    /**
     *  Trades a refresh token for a new bearer token.
     */
    public abstract refreshToken(): Observable<BearerToken>;

    /**
     *  Gets the user's claims.
     */
    protected getUserData(): Observable<string[]> {
        const headers = this.headersService.getHeaders();
        const userInfoUrl = `${this.oidcOptions.authorizationServerUrl}/connect/userinfo`;
        return this.http.get(userInfoUrl, { headers: headers }).map(result => result.json());
    }

    /**
     *  Redirects the user when an error occurs.
     *  @param error response from the server.
     */
    protected handleError(error: any) {
        if (error!.status === 403) {
            this.router.navigate(['/Forbidden']);
        } else if (error!.status === 401) {
            this.tokenService.bearerToken = null;
            this.router.navigate(['/Unauthorized']);
        }
    }
}
