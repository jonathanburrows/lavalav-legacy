import { Component } from '@angular/core';

/**
 * Used to route to components so they can be tested in isolation.
 */
@Component({
    selector: 'lvl-e2e-app',
    template: '<lvl-layout siteTitle="lavalav"><lvl-layout>'
})
export class AppE2eComponent { }
