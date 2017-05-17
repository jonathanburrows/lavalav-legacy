import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { LayoutComponent } from './components';
import {
    ApiService,
    FrontEndOptions,
    HeadersService,
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
        ApiService,
        HeadersService,
        { provide: StorageService, useClass: LocalStorageService }
    ],
    imports: [
        BrowserAnimationsModule,
        CommonModule,
        FormsModule,
        HttpModule,
        MaterialModule,
        RouterModule
    ]
})
export class FrontEndModule {
    public static useWithOptions(options: FrontEndOptions): ModuleWithProviders {
        return {
            ngModule: FrontEndModule,
            providers: [
                { provide: FrontEndOptions, useValue: options }
            ]
        };
    }
}
