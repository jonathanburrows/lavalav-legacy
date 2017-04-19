import { RouterModule } from '@angular/router';

import {
    CredentialsLoginComponent,
    LoginCallbackComponent
} from './components';

export const OidcRouterModule = RouterModule.forChild([
    { path: 'oidc/login', component: CredentialsLoginComponent },
    { path: 'oidc/login-callback', component: CredentialsLoginComponent }
]);
