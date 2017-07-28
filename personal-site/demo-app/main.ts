import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { platformBrowser } from '@angular/platform-browser';
import { InMemoryStorageService, StorageService } from '@lvl/front-end';

import { DemoAppModule } from './demo-app.module';
import { environment } from './environments';

if (environment.production) {
    enableProdMode();
}

platformBrowserDynamic().bootstrapModule(DemoAppModule);
