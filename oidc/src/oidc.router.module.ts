import { RouterModule } from '@angular/router';

import { Navigation } from '@lvl/front-end';
import {
    ChangePasswordComponent,
    PersonalDetailComponent,
    RecoverUsernameComponent,
    RegisterAccountComponent,
    RequestResetPasswordComponent,
    ResourceOwnerSigninComponent,
    SingleSignoutComponent,
    UserAdministrationComponent
} from './components';

export const oidcRouterModule = RouterModule.forChild([
    { path: 'oidc/sign-in', component: ResourceOwnerSigninComponent, canActivate: [Navigation] },
    { path: 'oidc/recover-username', component: RecoverUsernameComponent, canActivate: [Navigation] },
    { path: 'oidc/register-account', component: RegisterAccountComponent, canActivate: [Navigation] },
    { path: 'oidc/request-reset-password', component: RequestResetPasswordComponent, canActivate: [Navigation] },
    { path: 'oidc/single-signout', component: SingleSignoutComponent, canActivate: [Navigation] },
    { path: 'oidc/personal-details', component: PersonalDetailComponent, canActivate: [Navigation] },
    { path: 'oidc/change-password', component: ChangePasswordComponent, canActivate: [Navigation] },
    { path: 'oidc/user-administration/:id', component: UserAdministrationComponent, canActivate: [Navigation] },
    { path: 'oidc/user-administration', component: UserAdministrationComponent, canActivate: [Navigation] }
]);
