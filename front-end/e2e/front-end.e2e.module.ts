import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { FrontEndModule } from '../src';

import {
    AppE2eComponent
} from './components';

/**
 *  Used to test and develop components in isolation. Not to be used for production purposes.
 */
@NgModule({
    declarations: [
        AppE2eComponent
    ],
    imports: [
        BrowserModule,
        RouterModule.forRoot([]),
        FrontEndModule.useWithOptions({
            resourceServerUrl: 'http://localhost:5000'
        })
    ],
    bootstrap: [AppE2eComponent]
})
export class FrontEndE2eModule { }
