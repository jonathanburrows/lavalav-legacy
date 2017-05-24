import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';

import { FrontEndModule } from '@lvl/front-end';
import {
    RootComponent,
    SecretComponent
} from './components';
import { OidcModule } from '../src';

const oidcGroup = 'Openid';

/**
 *  Used to test and develop components in isolation. Not to be used for production purposes.
 */
/* tslint:disable:max-line-length */
@NgModule({
    declarations: [
        RootComponent,
        SecretComponent
    ],
    imports: [
        RouterModule.forRoot([
            // when developing, set the redirect to what you are working on.
            { path: '', pathMatch: 'full', redirectTo: '/demo/secret' },
            { path: 'demo/secret', component: SecretComponent, data: { group: oidcGroup, title: 'Secret', icon: 'add_alert', showInNavigation: true } }
        ]),
        BrowserModule,
        FrontEndModule.useWithOptions({
            resourceServerUrl: 'http://localhost:5000'
        }),
        OidcModule.useResourceOwnerFlow({
            clientId: 'test-resource-owner-client',
            authorizationServerUrl: 'http://localhost:5004',
            clientSecret: 'secret',
            scopes: ['test-resource-server']
        })
    ],
    bootstrap: [RootComponent]
})
export class DemoAppModule { }
