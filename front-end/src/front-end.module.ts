import { ModuleWithProviders, NgModule } from '@angular/core';

import {
    ApiService,
    FrontEndOptions,
    HeadersService,
    LocalStorageService,
    StorageService
} from './services';

@NgModule({
    providers: [
        ApiService,
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
