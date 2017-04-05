import { NgModule } from '@angular/core';

import {
    LocalStorageService,
    StorageService
} from './services';

@NgModule({
    providers: [
        { provide: StorageService, useClass: LocalStorageService }
    ]
})
export class CoreModule { }
