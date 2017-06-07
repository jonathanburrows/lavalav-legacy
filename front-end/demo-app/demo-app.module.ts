import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';

import { environment } from './environments';
import { FrontEndModule, Navigation } from '../src';
import {
    ContentDemoComponent,
    LayoutDemoComponent,
    MasterDetailDemoComponent,
    RootDemoComponent,
    SaveButtonDemoComponent,
    ValidatorsDemoComponent
} from './components';

const frontEndGroup = 'Front End';

/**
 *  Used to test and develop components in isolation. Not to be used for production purposes.
 */
/* tslint:disable:max-line-length */
@NgModule({
    declarations: [
        ContentDemoComponent,
        LayoutDemoComponent,
        MasterDetailDemoComponent,
        RootDemoComponent,
        SaveButtonDemoComponent,
        ValidatorsDemoComponent
    ],
    imports: [
        BrowserModule,
        RouterModule.forRoot([
            // when developing, set the redirect to what you are working on.
            { path: '', pathMatch: 'full', redirectTo: 'component/lvl-master' },
            { path: 'component/lvl-layout', component: LayoutDemoComponent, canActivate: [Navigation] },
            { path: 'component/lvl-validators', component: ValidatorsDemoComponent, canActivate: [Navigation] },
            { path: 'component/lvl-content', component: ContentDemoComponent, canActivate: [Navigation] },
            { path: 'component/lvl-save-button', component: SaveButtonDemoComponent, canActivate: [Navigation] },
            { path: 'component/lvl-master', component: MasterDetailDemoComponent, canActivate: [Navigation] }
        ]),
        FrontEndModule.useWithOptions(environment)
    ],
    bootstrap: [RootDemoComponent]
})
export class DemoAppModule { }
