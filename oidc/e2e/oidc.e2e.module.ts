import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';

import { RootE2eComponent } from './components';
import { OidcModule } from '../src';

const frontEndGroup = 'Front End';

/**
 *  Used to test and develop components in isolation. Not to be used for production purposes.
 */
/* tslint:disable:max-line-length */
@NgModule({
    declarations: [
        RootE2eComponent
    ],
    imports: [
        BrowserModule,
        RouterModule.forRoot([
            // when developing, set the redirect to what you are working on.
        ]),
        OidcModule.useResourceOwnerFlow({
            clientId: 'test-resource-owner-client',
            authorizationServerUrl: 'http://localhost:5001',
            clientSecret: 'secret',
            scopes: ['test-resource-server']
        })
    ],
    bootstrap: [RootE2eComponent]
})
export class OidcE2eModule { }
