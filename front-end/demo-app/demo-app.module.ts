import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';

import { FrontEndModule } from '../src';
import {
    RootDemoComponent,
    LayoutDemoComponent,
    ValidatorsDemoComponent
} from './components';

const frontEndGroup = 'Front End';

/**
 *  Used to test and develop components in isolation. Not to be used for production purposes.
 */
/* tslint:disable:max-line-length */
@NgModule({
    declarations: [
        RootDemoComponent,
        LayoutDemoComponent,
        ValidatorsDemoComponent
    ],
    imports: [
        BrowserModule,
        RouterModule.forRoot([
            // when developing, set the redirect to what you are working on.
            { path: '', pathMatch: 'full', redirectTo: 'component/lvl-validators' },
            { path: 'component/lvl-layout', component: LayoutDemoComponent, data: { title: 'Layout', icon: 'subject', showInNavigation: true, group: frontEndGroup } },
            { path: 'component/lvl-validators', component: ValidatorsDemoComponent, data: { title: 'Validators', icon: 'spellcheck', showInNavigation: true, group: frontEndGroup } }
        ]),
        FrontEndModule.useWithOptions({
            resourceServerUrl: 'http://localhost:5000'
        })
    ],
    bootstrap: [RootDemoComponent]
})
export class DemoAppModule { }
