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
    ForbiddenComponent,
    LayoutComponent,
    NotFoundComponent,
    SaveButtonComponent,
    SearchActionComponent
} from './components';

import { frontEndRouterModule } from './front-end.router.module';

import { KeysPipe } from './pipes';

import {
    ApiService,
    FrontEndOptions,
    HeadersService,
    LocalStorageService,
    Navigation,
    StorageService,
    ValidationBuilder
} from './services';

@NgModule({
    declarations: [
        // components
        ContentComponent,
        ContentAvatarDirective,
        ContentBodyDirective,
        ContentSubtitleDirective,
        ContentTitleDirective,
        ForbiddenComponent,
        LayoutComponent,
        NotFoundComponent,
        SaveButtonComponent,
        SearchActionComponent,

        // pipes
        KeysPipe
    ],
    exports: [
        // components
        ContentComponent,
        ContentAvatarDirective,
        ContentBodyDirective,
        ContentSubtitleDirective,
        ContentTitleDirective,
        LayoutComponent,
        SaveButtonComponent,
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
        Navigation,
        ValidationBuilder
    ],
    imports: [
        BrowserAnimationsModule,
        CommonModule,
        FormsModule,
        frontEndRouterModule,
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
