import { ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { MaterialModule } from '@angular/material';
import { RouterModule } from '@angular/router';

import { CoreModule } from '@lvl/core';

import { oidcRouterModule } from './oidc.router.module';


import {
    AppComponent,
    ConsentComponent,
    CredentialsLoginComponent,
    LoginComponent
} from './components';

import {
    ImplicitSecurityService,
    OidcConfiguration,
    ResourceOwnerSecurityService,
    SecurityService,
    TokenService
} from './services';

@NgModule({
    declarations: [
        AppComponent,
        ConsentComponent,
        CredentialsLoginComponent,
        LoginComponent
    ],
    exports: [
        ConsentComponent,
        CredentialsLoginComponent,
        LoginComponent
    ],
    imports: [
        BrowserModule,
        CoreModule,
        FormsModule,
        HttpModule,
        MaterialModule,
        ReactiveFormsModule,
        RouterModule,

        oidcRouterModule
    ],
    providers: [
        TokenService
    ],
    bootstrap: [AppComponent]
})
export class OidcModule {
    static useImplicitFlow(options: (configuration: OidcConfiguration) => void): ModuleWithProviders {
        const oidcConfiguration = new OidcConfiguration();
        options(oidcConfiguration);

        return {
            ngModule: OidcModule,
            providers: [
                { provide: SecurityService, useClass: ImplicitSecurityService },
                { provide: OidcConfiguration, useValue: oidcConfiguration }
            ]
        };
    }

    static useResourceOwnerFlow(options: (configuration: OidcConfiguration) => void): ModuleWithProviders {
        const oidcConfiguration = new OidcConfiguration();
        options(oidcConfiguration);

        return {
            ngModule: OidcModule,
            providers: [
                { provide: SecurityService, useClass: ResourceOwnerSecurityService },
                { provide: OidcConfiguration, useValue: oidcConfiguration }
            ]
        };
    }
}
