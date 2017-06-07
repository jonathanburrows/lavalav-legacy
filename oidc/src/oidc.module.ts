import { NgModule, ModuleWithProviders } from '@angular/core';

import {
    FrontEndModule,
    HeadersService,
    Navigation
} from '@lvl/front-end';
import {
    AccountMenuComponent,
    ChangePasswordComponent,
    PersonalDetailComponents,
    RecoverUsernameComponent,
    RegisterAccountComponent,
    RequestResetPasswordComponent,
    ResourceOwnerSigninComponent,
    SingleSignoutComponent,
    UserAdministrationComponent,
    UserRolesComponent
} from './components';
import { oidcRouterModule } from './oidc.router.module';
import {
    BearerHeadersService,
    OidcNavigationService,
    OidcOptions,
    PersonalDetailsService,
    RecoverUsernameService,
    ResetPasswordService,
    ResourceOwnerSecurityService,
    SecurityService,
    TokenService,
    UserService,
    UserAdministrationService
} from './services';

/**
 *  Provides services and components for security.
 */
@NgModule({
    providers: [
        { provide: HeadersService, useClass: BearerHeadersService },
        { provide: Navigation, useClass: OidcNavigationService },
        PersonalDetailsService,
        RecoverUsernameService,
        ResetPasswordService,
        TokenService,
        UserService,
        UserAdministrationService
    ],
    declarations: [
        AccountMenuComponent,
        ChangePasswordComponent,
        PersonalDetailComponents,
        RecoverUsernameComponent,
        RegisterAccountComponent,
        RequestResetPasswordComponent,
        ResourceOwnerSigninComponent,
        SingleSignoutComponent,
        UserAdministrationComponent,
        UserRolesComponent
    ],
    exports: [
        AccountMenuComponent,
        ChangePasswordComponent,
        PersonalDetailComponents,
        RecoverUsernameComponent,
        RegisterAccountComponent,
        RequestResetPasswordComponent,
        ResourceOwnerSigninComponent,
        SingleSignoutComponent,
        UserAdministrationComponent,
        UserRolesComponent
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
