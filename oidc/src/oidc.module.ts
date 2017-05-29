import { NgModule, ModuleWithProviders } from '@angular/core';

import { FrontEndModule, HeadersService } from '@lvl/front-end';
import {
    AccountMenuComponent,
    CredentialsSigninComponent,
    RecoverUsernameComponent,
    RegisterAccountComponent,
    ResetPasswordComponent
 } from './components';
import { oidcRouterModule } from './oidc.router.module';
import {
    BearerHeadersService,
    OidcOptions,
    RecoverUsernameService,
    ResourceOwnerSecurityService,
    SecurityService,
    TokenService,
    UserService
} from './services';

/**
 *  Provides services and components for security.
 */
@NgModule({
    providers: [
        { provide: HeadersService, useClass: BearerHeadersService },
        RecoverUsernameService,
        TokenService,
        UserService
    ],
    declarations: [
        AccountMenuComponent,
        CredentialsSigninComponent,
        RecoverUsernameComponent,
        RegisterAccountComponent,
        ResetPasswordComponent
    ],
    exports: [
        AccountMenuComponent,
        CredentialsSigninComponent,
        RecoverUsernameComponent,
        RegisterAccountComponent,
        ResetPasswordComponent
    ],
    imports: [
        FrontEndModule,
        oidcRouterModule
    ]
})
export class OidcModule {
    /**
     *  Provides services and components for security using the resource owner flow.
     *  @param oidcOptions Options for how to connect to the authorization server.
     *  @returns A module with services and components registered required for resource owner security.
     */
    static useResourceOwnerFlow(oidcOptions: OidcOptions): ModuleWithProviders {
        // null checks and logic were not put in this method as it causes a compiler error for angular.

        return {
            ngModule: OidcModule,
            providers: [
                { provide: OidcOptions, useValue: oidcOptions },
                { provide: SecurityService, useClass: ResourceOwnerSecurityService }
            ]
        };
    }
}
