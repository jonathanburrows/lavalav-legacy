import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';

import {
    FrontEndModule,
    Navigation
} from '@lvl/front-end';
import {
    AdminProtectedComponent,
    RootComponent,
    SecretComponent
} from './components';
import { environment } from './environments';
import { OidcModule } from '../src';

const oidcGroup = 'Openid';

/**
 *  Used to test and develop components in isolation. Not to be used for production purposes.
 */
/* tslint:disable:max-line-length */
@NgModule({
    declarations: [
        AdminProtectedComponent,
        RootComponent,
        SecretComponent
    ],
    imports: [
        RouterModule.forRoot([
            // when developing, set the redirect to what you are working on.
            { path: '', pathMatch: 'full', redirectTo: '/demo/secret', canActivate: [Navigation] },
            { path: 'demo/secret', component: SecretComponent, canActivate: [Navigation] },
            { path: 'demo/admin-protected', component: AdminProtectedComponent, canActivate: [Navigation] }
        ]),
        BrowserModule,
        FrontEndModule.useWithOptions(environment),
        OidcModule.useResourceOwnerFlow(environment)
    ],
    bootstrap: [RootComponent]
})
export class DemoAppModule { }
