import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { oidcE2eRouterModule } from './oidc.router.e2e-module';
import { AppComponent, OidcModule } from '../src';
import { ConsentE2eComponent } from '../src/components/consent/consent.component.spec-e2e';
import { CredentialsLoginE2eComponent } from '../src/components/credentials-login/credentials-login.component.e2e-spec';

@NgModule({
    declarations: [
        ConsentE2eComponent,
        CredentialsLoginE2eComponent
    ],
    imports: [
        BrowserModule,
        HttpModule,
        OidcModule.useResourceOwnerFlow(options => {
            options.authorizationServerUrl = 'http://localhost:57545';
            options.clientUrl = 'http://localhost:4200';
            options.clientId = 'test-resource-owner-client';
            options.clientSecret = 'secret';
            options.scopes = ['openid', 'profile', 'test-resource-server'];
        }),
        oidcE2eRouterModule,
        RouterModule
    ],
    bootstrap: [AppComponent]
})
export class OidcE2eModule { }

