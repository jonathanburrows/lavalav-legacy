import { RouterModule } from '@angular/router';

import { ForbiddenComponent, NotFoundComponent } from './components';

export const frontEndRouterModule = RouterModule.forChild([
    { path: 'forbidden', component: ForbiddenComponent },
    { path: 'not-found', component: NotFoundComponent }
]);
