import { RouterModule } from '@angular/router';

import { Navigation } from '@lvl/front-end';
import { PersonalityComponent } from './components';

export const personalSiteRouterModule = RouterModule.forChild([
    { path: 'about-me/personality', component: PersonalityComponent, canActivate: [Navigation] }
]);
