import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { LoginComponent } from './login.component';
import { CoreModule } from '@lvl/core';
import { OidcModule } from '@lvl/oidc';

@NgModule({
    declarations: [LoginComponent],
    bootstrap: [LoginComponent],
    imports: [
        BrowserModule,
        CoreModule,
        OidcModule
    ]
})
export class LoginModule { }
