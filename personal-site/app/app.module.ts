import { NgModule, ModuleWithProviders } from '@angular/core';
import { RouterModule } from '@angular/router';

import {
    FrontEndModule,
    HeadersService,
    Navigation
} from '@lvl/front-end';
import { RootComponent } from './components';
import { PersonalSiteModule } from '../src';

/**
 *  Provides services and components for security.
 */
@NgModule({
    declarations: [
        RootComponent
    ],
    imports: [
        PersonalSiteModule,
        RouterModule.forRoot([
            { path: '', pathMatch: 'full', redirectTo: 'about-me/personality' }
        ])
    ],
    bootstrap: [RootComponent]
})
export class AppModule { }
