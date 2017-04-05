import { ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';

import { CoreModule } from '@lvl/core';

import {
    AppComponent,
    LoginComponent
} from './components';

import {
    ImplicitSecurityService,
    OidcConfiguration,
    SecurityService,
    TokenService
} from './services';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent
    ],
    exports: [
        LoginComponent
    ],
    imports: [
        BrowserModule,
        CoreModule,
        FormsModule,
        HttpModule
    ],
    providers: [
        TokenService
    ],
    bootstrap: [AppComponent]
})
export class OidcModule {
    static useImplicitFlow(options: (configuration: OidcConfiguration) => void): ModuleWithProviders {
        const oidcConfiguration = options(new OidcConfiguration());

        return {
            ngModule: OidcModule,
            providers: [
                { provide: SecurityService, useClass: ImplicitSecurityService },
                { provide: OidcConfiguration, useValue: oidcConfiguration }
            ]
        };
    }
}
