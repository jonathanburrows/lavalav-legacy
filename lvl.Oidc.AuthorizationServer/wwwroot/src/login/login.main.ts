import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { environment } from '@lvl/core';
import { LoginModule } from './login.module';

if (environment.production) {
    enableProdMode();
}

platformBrowserDynamic().bootstrapModule(LoginModule);
