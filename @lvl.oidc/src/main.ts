import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { OidcModule } from './oidc.module';
import { OidcConfiguration } from './services/oidc-configuration';

enableProdMode();

platformBrowserDynamic().bootstrapModule(OidcModule);
