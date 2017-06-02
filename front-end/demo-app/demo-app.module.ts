import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';

import { environment } from './environments';
import { FrontEndModule } from '../src';
import {
    ContentDemoComponent,
    LayoutDemoComponent,
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
        RootDemoComponent,
        SaveButtonDemoComponent,
        ValidatorsDemoComponent
    ],
    imports: [
        BrowserModule,
        RouterModule.forRoot([
            // when developing, set the redirect to what you are working on.
            { path: '', pathMatch: 'full', redirectTo: 'component/lvl-save-button' },
            { path: 'component/lvl-layout', component: LayoutDemoComponent, data: { title: 'Layout', icon: 'subject', showInNavigation: true, group: frontEndGroup } },
            { path: 'component/lvl-validators', component: ValidatorsDemoComponent, data: { title: 'Validators', icon: 'spellcheck', showInNavigation: true, group: frontEndGroup } },
            { path: 'component/lvl-content', component: ContentDemoComponent, data: { title: 'Content', icon: 'content_copy', showInNavigation: true, group: frontEndGroup } },
            { path: 'component/lvl-save-button', component: SaveButtonDemoComponent, data: { title: 'Save Button', icon: 'content_copy', showInNavigation: true, group: frontEndGroup } }
        ]),
        FrontEndModule.useWithOptions(environment)
    ],
    bootstrap: [RootDemoComponent]
})
export class DemoAppModule { }
