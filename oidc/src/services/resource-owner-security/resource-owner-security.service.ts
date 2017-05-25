import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { StorageService } from '@lvl/front-end';
import { OidcOptions } from '../oidc-options';
import { Credentials, SecurityService } from '../security';
import { BearerToken, TokenService } from '../token';
import { TokenRequestOptions } from './token-request-options';

/**
 *  Provides the resource owner flow.
 */
@Injectable()
export class ResourceOwnerSecurityService extends SecurityService {
    /**
     *  Saves the current url for the return trip, then navigates to the logic screen.
     */
    public redirectToLogin() {
        this.postLoginRedirectUrl = this.router.routerState.snapshot.url;
        this.router.navigate(['/account/login']);
    }

    /**
     *  Attempts a login against the authentication server using the provided credentials.
     *  @param credentials User entered credentials
     *  @throws {Error} credentials is null.
     */
    public login(credentials: Credentials): Observable<BearerToken> {
        if (!credentials) {
            throw new Error('credentials are null.');
        }

        const loginOptions: TokenRequestOptions = {
            grant_type: 'password',
            username: credentials.username,
            password: credentials.password
        };

        const request = this.requestToken(loginOptions);
        request.subscribe(bearerToken => {
            this.tokenService.bearerToken = bearerToken;
            this.router.navigate([this.postLoginRedirectUrl]);
        });
        return request;
    }

    /**
     *  Clears the token and navigates the user to the root of the app.
     */
    public logout() {
        this.tokenService.bearerToken = null;
        this.router.navigate(['/']);
    }

    /**
     *  Clears the token; used for single sign out.
     */
    public logoutLocal() {
        this.tokenService.bearerToken = null;
    }

    /**
     *  Exchanges a refresh token for a new access token.
     */
    public refreshToken() {
        const refreshOptions: TokenRequestOptions = {
            refresh_token: this.tokenService.bearerToken.refresh_token,
            grant_type: 'refresh_token'
        };
        const request = this.requestToken(refreshOptions);
        request.subscribe(bearerToken => this.tokenService.bearerToken = bearerToken);
        return request;
    }

    private requestToken(tokenOptions: TokenRequestOptions): Observable<BearerToken> {
        tokenOptions.client_id = this.oidcOptions.clientId;
        tokenOptions.client_secret = this.oidcOptions.clientSecret;

        const defaultScopes = ['openid', 'profile', 'offline_access'];
        tokenOptions.scope = defaultScopes.concat(this.oidcOptions.scopes).join(' ');

        const url = `${this.oidcOptions.authorizationServerUrl}/connect/token`;
        const headers = new Headers({ 'content-type': 'application/x-www-form-urlencoded' });
        const urlEncodedBody = this.urlEncodeBody(tokenOptions);

        return this.http.post(url, urlEncodedBody, { headers: headers }).map(token => token.json());
    }

    private urlEncodeBody(body: {}) {
        const parameters: { key: string, value: string }[] = [];
        for (const i in body) {
            if (body[i]) {
                parameters.push({ key: i, value: body[i] });
            }
        }
        return parameters.map(p => `${p.key}=${p.value}`).join('&');
    }
}
