import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { MaterialModule } from '@angular/material';
import { RouterModule } from '@angular/router';

import { CoreModule } from '@lvl/core';

import { OidcRouterModule } from './oidc.router.module';


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
        CommonModule,
        CoreModule,
        FormsModule,
        HttpModule,
        MaterialModule,
        OidcRouterModule,
        ReactiveFormsModule,
        RouterModule
    ],
    providers: [
        TokenService
    ],
    bootstrap: [AppComponent]
})
export class OidcModule {
    static useImplicitFlow(/*options: (configuration: OidcConfiguration) => void*/): ModuleWithProviders {
        const oidcConfiguration = new OidcConfiguration();
        //options(oidcConfiguration);

        return {
            ngModule: OidcModule,
            providers: [
                { provide: SecurityService, useClass: ImplicitSecurityService },
                { provide: OidcConfiguration, useValue: oidcConfiguration }
            ]
        };
    }

    static useResourceOwnerFlow(oidcConfiguration: OidcConfiguration/*options: (configuration: OidcConfiguration) => void*/): ModuleWithProviders {
        //const oidcConfiguration = new OidcConfiguration();
        //options(oidcConfiguration);

        return {
            ngModule: OidcModule,
            providers: [
                { provide: SecurityService, useClass: ResourceOwnerSecurityService },
                { provide: OidcConfiguration, useValue: oidcConfiguration }
            ]
        };
    }
}
