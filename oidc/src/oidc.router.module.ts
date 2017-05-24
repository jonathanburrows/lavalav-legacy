import { RouterModule } from '@angular/router';

import {
    CredentialsSigninComponent,
    RecoverUsernameComponent,
    RegisterAccountComponent,
    ResetPasswordComponent
} from './components';

export const oidcRouterModule = RouterModule.forChild([
    { path: 'oidc/credentials-signin', component: CredentialsSigninComponent },
    { path: 'oidc/recover-username', component: RecoverUsernameComponent },
    { path: 'oidc/register-account', component: RegisterAccountComponent },
    { path: 'oidc/reset-password', component: ResetPasswordComponent }
]);
