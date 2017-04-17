import { RouterModule } from '@angular/router';

import { CredentialsLoginComponent } from './components';

export const OidcRouterModule = RouterModule.forChild([
    { path: 'oidc/login', component: CredentialsLoginComponent }
]);
