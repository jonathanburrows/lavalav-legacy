import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { CoreModule } from '@lvl/core';

import { AppComponent, OidcModule } from '../src';

@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        CoreModule.useWithOptions({
            resourceServerUrl: 'http://localhost:56182'
        }),
        OidcModule.useResourceOwnerFlow({
            authorizationServerUrl: 'http://localhost:65170',
            clientUrl: 'http://localhost:4200',
            clientId: 'test-resource-owner-client',
            clientSecret: 'secret',
            scopes: ['openid', 'profile', 'offline_access', 'test-resource-server']
        }),
        RouterModule.forRoot([])
    ],
    bootstrap: [AppComponent]
})
export class OidcE2eModule { }
