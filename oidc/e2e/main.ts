import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { platformBrowser } from '@angular/platform-browser';
import { environment } from '@lvl/front-end';

import { OidcE2eModule } from './oidc.e2e.module';

if (environment.production) {
    enableProdMode();
}

// Loads components in a way that they can be developed in isolation.
platformBrowserDynamic().bootstrapModule(OidcE2eModule);
