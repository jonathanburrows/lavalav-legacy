import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { platformBrowser } from '@angular/platform-browser';
import { environment } from '@lvl/core';

import { OidcModule } from './oidc.module';
import { OidcE2eModule } from '../e2e/oidc.e2e-module';

if (environment.production) {
    enableProdMode();
}

platformBrowserDynamic().bootstrapModule(OidcE2eModule);
