import { NgModule } from '@angular/core';
import { MaterialModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {
    LayoutComponent
} from './components';
import {
    LocalStorageService,
    StorageService
} from './services';

@NgModule({
    declarations: [
        LayoutComponent
    ],
    exports: [
        LayoutComponent
    ],
    providers: [
        { provide: StorageService, useClass: LocalStorageService }
    ],
    imports: [
        BrowserAnimationsModule,
        MaterialModule
    ]
})
export class CoreModule { }
