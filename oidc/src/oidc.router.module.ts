﻿import { RouterModule } from '@angular/router';

import {
    RecoverUsernameComponent,
    RegisterAccountComponent,
    RequestResetPasswordComponent,
    ResourceOwnerSigninComponent
} from './components';

export const oidcRouterModule = RouterModule.forChild([
    { path: 'oidc/sign-in', component: ResourceOwnerSigninComponent },
    { path: 'oidc/recover-username', component: RecoverUsernameComponent },
    { path: 'oidc/register-account', component: RegisterAccountComponent },
    { path: 'oidc/request-reset-password', component: RequestResetPasswordComponent }
]);
