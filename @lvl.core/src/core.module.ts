import { ModuleWithProviders, NgModule } from '@angular/core';
import { MaterialModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {
    LayoutComponent
} from './components';
import {
    ApiService,
    CoreOptions,
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
        MaterialModule
    ]
})
export class CoreModule {
    static useWithOptions(coreOptions: CoreOptions): ModuleWithProviders {
        return {
            ngModule: CoreModule,
            providers: [
                { provide: CoreOptions, useValue: coreOptions }
            ]
        };
    }
}
