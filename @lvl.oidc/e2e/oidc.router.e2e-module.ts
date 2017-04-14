import { RouterModule } from '@angular/router';

import { ConsentE2eComponent } from '../src/components/consent/consent.component.spec-e2e';

export const oidcE2eRouterModule = RouterModule.forRoot([
    { path: 'oidc', loadChildren: '../src/oidc.module#OidcModule' },
    { path: 'test/lvl-oidc-consent-component', component: ConsentE2eComponent }
]);
