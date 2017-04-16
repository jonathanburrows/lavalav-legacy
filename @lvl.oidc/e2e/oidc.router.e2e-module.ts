import { RouterModule } from '@angular/router';
import { ConsentE2eComponent } from '../src/components/consent/consent.component.spec-e2e';
import { CredentialsLoginE2eComponent } from '../src/components/credentials-login/credentials-login.component.e2e-spec';

export const oidcE2eRouterModule = RouterModule.forRoot([
    { path: 'oidc', loadChildren: '../src/oidc.module#OidcModule' },
    { path: 'test/lvl-oidc-consent-component', component: ConsentE2eComponent },
    { path: 'test/lvl-oidc-credentials-login', component: CredentialsLoginE2eComponent }
]);
