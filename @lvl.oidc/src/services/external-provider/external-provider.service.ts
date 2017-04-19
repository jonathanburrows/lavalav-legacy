import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { ExternalProvider } from './external-provider';
import { OidcConfiguration } from '../oidc-configuration';
import { SecurityService } from '../security';

@Injectable()
export class ExternalProviderService {
    constructor(
        private http: Http,
        private oidcConfiguration: OidcConfiguration,
        private router: Router,
        private securityService: SecurityService
    ) { }

    public getProviders(): Observable<ExternalProvider[]> {
        const url = `${this.oidcConfiguration.authorizationServerUrl}/oidc/external-login/providers?returnUrl=${this.oidcConfiguration.clientUrl}`;
        return this.http.get(url).map(response => response.json());
    }

    public login(authenticationScheme: string) {
        this.securityService.postLoginRedirectUrl = this.router.routerState.snapshot.url;
        const loginCallbackUrl = `${this.oidcConfiguration.clientUrl}/oidc/login-callback`;
        window.location.href = `${this.oidcConfiguration.authorizationServerUrl}/oidc/external-login/login?provider=${authenticationScheme}&returnUrl=${loginCallbackUrl}`;
    }
}
