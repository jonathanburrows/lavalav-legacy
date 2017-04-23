import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { MaterialModule } from '@angular/material';
import { RouterModule } from '@angular/router';

import { CoreModule, HeadersService } from '@lvl/core';

import { OidcRouterModule } from './oidc.router.module';

import {
    AppComponent,
    ConsentComponent,
    CredentialsLoginComponent,
    LoginComponent,
    LoginCallbackComponent
} from './components';

import {
    BearerHeadersService,
    ExternalProviderService,
    ImplicitSecurityService,
    OidcOptions,
    ResourceOwnerSecurityService,
    SecurityService,
    TokenService
} from './services';

@NgModule({
    declarations: [
        AppComponent,
        ConsentComponent,
        CredentialsLoginComponent,
        LoginComponent,
        LoginCallbackComponent
    ],
    exports: [
        ConsentComponent,
        CredentialsLoginComponent,
        LoginComponent,
        LoginCallbackComponent
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
        ExternalProviderService,
        { provide: HeadersService, useClass: BearerHeadersService },
        TokenService
    ],
    bootstrap: [AppComponent]
})
export class OidcModule {
    static useImplicitFlow(/*options: (configuration: OidcConfiguration) => void*/): ModuleWithProviders {
        const oidcOptions = new OidcOptions();
        //options(oidcConfiguration);

        return {
            ngModule: OidcModule,
            providers: [
                { provide: SecurityService, useClass: ImplicitSecurityService },
                { provide: OidcOptions, useValue: oidcOptions }
            ]
        };
    }

    static useResourceOwnerFlow(oidcOptions: OidcOptions): ModuleWithProviders {
        return {
            ngModule: OidcModule,
            providers: [
                { provide: SecurityService, useClass: ResourceOwnerSecurityService },
                { provide: OidcOptions, useValue: oidcOptions }
            ]
        };
    }
}
