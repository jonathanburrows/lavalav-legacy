import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {
    ContentComponent,
    ContentAvatarDirective,
    ContentBodyDirective,
    ContentSubtitleDirective,
    ContentTitleDirective,
    LayoutComponent,
    SearchActionComponent
} from './components';

import {
    ApiService,
    FrontEndOptions,
    HeadersService,
    LocalStorageService,
    StorageService,
    ValidationBuilder
} from './services';

@NgModule({
    declarations: [
        ContentComponent,
        ContentAvatarDirective,
        ContentBodyDirective,
        ContentSubtitleDirective,
        ContentTitleDirective,
        LayoutComponent,
        SearchActionComponent
    ],
    exports: [
        ContentComponent,
        ContentAvatarDirective,
        ContentBodyDirective,
        ContentSubtitleDirective,
        ContentTitleDirective,
        LayoutComponent,
        SearchActionComponent,

        // vendor modules
        CommonModule,
        ReactiveFormsModule,
        MaterialModule
    ],
    providers: [
        ApiService,
        HeadersService,
        { provide: StorageService, useClass: LocalStorageService },
        ValidationBuilder
    ],
    imports: [
        BrowserAnimationsModule,
        CommonModule,
        FormsModule,
        HttpModule,
        MaterialModule,
        ReactiveFormsModule,
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
