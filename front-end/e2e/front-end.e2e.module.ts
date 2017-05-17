import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';

import { FrontEndModule } from '../src';
import {
    AppE2eComponent,
    LayoutE2eComponent
} from './components';

const frontEndGroup = 'Front End';

/**
 *  Used to test and develop components in isolation. Not to be used for production purposes.
 */
/* tslint:disable:max-line-length */
@NgModule({
    declarations: [
        AppE2eComponent,
        LayoutE2eComponent
    ],
    imports: [
        BrowserModule,
        RouterModule.forRoot([
            // when developing, set the redirect to what you are working on.
            { path: '', pathMatch: 'full', redirectTo: 'lvl-layout' },
            { path: 'lvl-layout', component: LayoutE2eComponent, data: { title: 'Layout', icon: 'subject', showInNavigation: true, group: frontEndGroup } }
        ]),
        FrontEndModule.useWithOptions({
            resourceServerUrl: 'http://localhost:5000'
        })
    ],
    bootstrap: [AppE2eComponent]
})
export class FrontEndE2eModule { }
