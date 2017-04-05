﻿import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { OidcModule } from './oidc.module';
import { environment } from '@lvl/core';
import { OidcConfiguration } from './services/oidc-configuration';

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(OidcModule);
