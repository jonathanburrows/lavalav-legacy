import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import {
    LoginComponent,
    OidcModule
} from '@lvl/oidc';

@NgModule({
    bootstrap: [LoginComponent],
    imports: [
        OidcModule,
        BrowserModule
    ]
})
export class LoginModule { }
