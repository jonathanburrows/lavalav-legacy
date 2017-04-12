import { EventEmitter, Injectable } from '@angular/core';
import {
    Headers,
    Http,
    Response
} from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/map';

import { StorageService } from '@lvl/core';

import { OidcConfiguration } from '../oidc-configuration';
import { ImplicitAuthorizationOptions } from './implicit-authorization-options';
import { ImplicitLogoffOptions } from './implicit-logoff-options';
import { SecurityService } from '../security';
import { BearerToken, IdToken } from '../token';

@Injectable()
export class ImplicitSecurityService extends SecurityService {
    private nonceKey = 'oidc:nonce';
    private stateKey = 'oidc:state';

    public redirectToLogin() {
        this.postLoginRedirectUrl = this.router.routerState.snapshot.url;
        window.location.href = this.getRedirectUrl();
    }

    private getRedirectUrl(idTokenHint?: string) {
        const nonce = 'N' + Math.random() + '' + Date.now();
        this.storageService.setItem(this.nonceKey, nonce);

        const state = Date.now() + '' + Math.random();
        this.storageService.setItem(this.stateKey, state);

        const scopes = ['openid', 'token', ...this.oidcConfiguration.scopes].join(' ');

        const options: ImplicitAuthorizationOptions = {
            client_id: this.oidcConfiguration.clientSecret,
            redirect_uri: `${this.oidcConfiguration.clientUrl}/account/signin-oidc`,
            response_type: 'id_token token',
            scope: scopes,
            id_token_hint: idTokenHint,
            prompt: idTokenHint ? 'none' : undefined,
            nonce: nonce,
            state: state
        };

        const queryParameters = this.convertLiteralToQueryParameters(options);
        return `${this.oidcConfiguration.authorizationServerUrl}/connect/authorize?${queryParameters}`;
    }

    public login() {
        const hash = window.location.hash.substr(1);

        const bearerToken = <BearerToken>hash.split('&').reduce((r: {}, item: string) => {
            const keyValue = item.split('=');
            r[keyValue[0]] = keyValue[1];
            return r;
        }, {});
        const idToken = this.tokenService.convertJwtToIdToken(bearerToken.id_token);

        const originalNonce = this.storageService.getItem(this.nonceKey);
        const originalState = this.storageService.getItem(this.stateKey);


        if (bearerToken.error) {
            this.tokenService.bearerToken = null;
            this.router.navigate(['/account/unauthorized']);
        } else if (originalState !== bearerToken.state || originalNonce !== idToken.nonce) {
            this.tokenService.bearerToken = null;
            this.router.navigate(['/account/malicious-warning']);
        } else {
            this.tokenService.bearerToken = bearerToken;
            this.router.navigate([this.postLoginRedirectUrl]);
        }
    }

    public logout() {
        const options: ImplicitLogoffOptions = {
            id_token_hint: this.tokenService.bearerToken.id_token,
            post_logout_redirect_uri: this.oidcConfiguration.clientUrl
        };

        const queryParameters = this.convertLiteralToQueryParameters(options);
        const url = `${this.oidcConfiguration.authorizationServerUrl}/connect/endsession?${queryParameters}`;

        this.tokenService.bearerToken = null;
        window.location.href = url;
    }

    public logoutLocal() {
        this.tokenService.bearerToken = null;
    }

    public refreshToken() {
        const refreshAction = new Subject<BearerToken>();

        const renewIframe = window.document.createElement('iframe');
        renewIframe.style.display = 'none';
        window.document.body.appendChild(renewIframe);

        renewIframe.onload = () => {
            const serialized = this.storageService.getItem(this.tokenService.bearerTokenKey);
            this.tokenService.bearerToken = JSON.parse(serialized);
            refreshAction.next(this.tokenService.bearerToken);
            renewIframe.remove();
        };

        const hint = this.tokenService.bearerToken ? this.tokenService.bearerToken.id_token : null;
        renewIframe.src = this.getRedirectUrl(hint);

        return refreshAction;
    }

    private convertLiteralToQueryParameters(literal: Object) {
        const parameters = [];
        for (const i in literal) {
            if (literal[i]) {
                const encodedValue = encodeURI(literal[i]);
                parameters.push(`${i}=${encodedValue}`);
            }
        }

        return parameters.join('&');
    }
}
