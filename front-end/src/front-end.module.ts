import { ModuleWithProviders, NgModule } from '@angular/core';

import {
    FrontEndOptions,
    HeadersService,
    LocalStorageService,
    StorageService
} from './services';

@NgModule({
    providers: [
        HeadersService,
        { provide: StorageService, useClass: LocalStorageService }
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
