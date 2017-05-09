import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { platformBrowser } from '@angular/platform-browser';
import { environment } from '../environments';

import { FrontEndE2eModule } from './front-end.e2e.module';

if (environment.production) {
    enableProdMode();
}

// Loads components in a way that they can be developed in isolation.
platformBrowserDynamic().bootstrapModule(FrontEndE2eModule);
