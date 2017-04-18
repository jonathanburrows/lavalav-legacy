import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { StorageService } from '@lvl/core';

import { OidcConfiguration } from '../oidc-configuration';
import { Credentials } from './credentials';
import { BearerToken, TokenService } from '../token';

@Injectable()
export abstract class SecurityService {
    public get isAuthorized() {
        return this.tokenService.bearerToken && !this.tokenService.isTokenExpired();
    }

    private postLoginRedirectUrlKey = 'oidc:postLoginRedirectUrl';

    private _postLoginRedirectUrl: string;
    public get postLoginRedirectUrl() {
        if (!this._postLoginRedirectUrl) {
            this._postLoginRedirectUrl = this.storageService.getItem(this.postLoginRedirectUrlKey);
        }

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
        protected oidcConfiguration: OidcConfiguration,
        protected storageService: StorageService
    ) {

        this.tokenService.tokenHalflife.subscribe(_ => this.refreshToken());
    }

    public abstract redirectToLogin();

    public abstract login(credentials?: Credentials);

    public abstract logout();

    public abstract logoutLocal();

    public abstract refreshToken(): Observable<BearerToken>;

    protected getUserData(): Observable<string[]> {
        const headers = this.getHeaders();
        const userInfoUrl = `${this.oidcConfiguration.authorizationServerUrl}/connect/userinfo`;
        return this.http.get(userInfoUrl, { headers: headers }).map(result => result.json());
    }

    public getHeaders() {
        const headers = new Headers({
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        });

        if (this.tokenService.bearerToken) {
            headers.append('Authorization', `Bearer ${this.tokenService.bearerToken.access_token}`);
        }

        return headers;
    }

    protected handleError(error: any) {
        if (error.status === 403) {
            this.router.navigate(['/Forbidden']);
        } else if (error.status === 401) {
            this.tokenService.bearerToken = null;
            this.router.navigate(['/Unauthorized']);
        }
    }
}
