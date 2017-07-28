import { NgModule, ModuleWithProviders } from '@angular/core';

import {
    FrontEndModule,
    HeadersService,
    Navigation
} from '@lvl/front-end';
import {
    PersonalityComponent
} from './components';
import { personalSiteRouterModule } from './personal-site.router.module';

/**
 *  Provides services and components for security.
 */
@NgModule({
    declarations: [
        PersonalityComponent
    ],
    exports: [
        PersonalityComponent,
        FrontEndModule
    ],
    imports: [
        FrontEndModule,
        personalSiteRouterModule
    ]
})
export class PersonalSiteModule { }
