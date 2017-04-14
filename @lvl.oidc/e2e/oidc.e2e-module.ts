import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { oidcE2eRouterModule } from './oidc.router.e2e-module';
import { AppComponent, OidcModule } from '../src';
import { ConsentE2eComponent } from '../src/components/consent/consent.component.spec-e2e';

@NgModule({
    declarations: [
        ConsentE2eComponent
    ],
    imports: [
        BrowserModule,
        HttpModule,
        OidcModule,
        oidcE2eRouterModule,
        RouterModule
    ],
    bootstrap: [AppComponent]
})
export class OidcE2eModule { }
