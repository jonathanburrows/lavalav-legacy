import { RouterModule } from '@angular/router';

import { Navigation } from '@lvl/front-end';
import {
    PersonalDetailComponents,
    RecoverUsernameComponent,
    RegisterAccountComponent,
    RequestResetPasswordComponent,
    ResourceOwnerSigninComponent,
    SingleSignoutComponent
} from './components';

export const oidcRouterModule = RouterModule.forChild([
    { path: 'oidc/sign-in', component: ResourceOwnerSigninComponent, canActivate: [Navigation] },
    { path: 'oidc/recover-username', component: RecoverUsernameComponent, canActivate: [Navigation] },
    { path: 'oidc/register-account', component: RegisterAccountComponent, canActivate: [Navigation] },
    { path: 'oidc/request-reset-password', component: RequestResetPasswordComponent, canActivate: [Navigation] },
    { path: 'oidc/single-signout', component: SingleSignoutComponent, canActivate: [Navigation] },
    { path: 'oidc/personal-details', component: PersonalDetailComponents, canActivate: [Navigation] }
]);
