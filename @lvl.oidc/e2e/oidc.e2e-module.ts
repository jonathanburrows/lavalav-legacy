import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { AppComponent, OidcModule } from '../src';

@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        OidcModule.useResourceOwnerFlow({
            authorizationServerUrl: 'http://localhost:57545',
            clientUrl: 'http://localhost:4200',
            clientId: 'test-resource-owner-client',
            clientSecret: 'secret',
            scopes: ['openid', 'profile', 'test-resource-server'],
            resourceServerUrl: null
        }),
        RouterModule.forRoot([])
    ],
    bootstrap: [AppComponent]
})
export class OidcE2eModule { }
