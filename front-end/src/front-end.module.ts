import { NgModule } from '@angular/core';

import {
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
export class FrontEndModule { }
