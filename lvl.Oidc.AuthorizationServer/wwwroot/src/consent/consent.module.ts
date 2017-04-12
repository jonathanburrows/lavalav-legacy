import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { ConsentComponent } from './consent.component';
import { CoreModule } from '@lvl/core';
import { OidcModule } from '@lvl/oidc';

@NgModule({
    declarations: [ConsentComponent],
    bootstrap: [ConsentComponent],
    imports: [
        BrowserModule,
        CoreModule,
        OidcModule
    ]
})
export class ConsentModule { }
