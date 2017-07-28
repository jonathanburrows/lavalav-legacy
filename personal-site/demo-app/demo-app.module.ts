import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';

import {
    FrontEndModule,
    Navigation
} from '@lvl/front-end';
import {
    RootComponent
} from './components';
import { environment } from './environments';
import { PersonalSiteModule } from '../src';

const oidcGroup = 'Openid';

/**
 *  Used to test and develop components in isolation. Not to be used for production purposes.
 */
/* tslint:disable:max-line-length */
@NgModule({
    declarations: [
        RootComponent
    ],
    imports: [
        RouterModule.forRoot([
        ]),
        BrowserModule,
        FrontEndModule.useWithOptions(environment),
        PersonalSiteModule
    ],
    bootstrap: [RootComponent]
})
export class DemoAppModule { }
