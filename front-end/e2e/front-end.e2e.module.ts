import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

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
        RouterModule.forRoot([]),
        BrowserModule
    ],
    bootstrap: [AppE2eComponent]
})
export class FrontEndE2eModule { }
