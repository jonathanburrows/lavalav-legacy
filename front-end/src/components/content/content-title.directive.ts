import { Directive } from '@angular/core';

/**
 *  Represents the main title of the page.
 */
@Directive({
    selector: 'lvl-content-title',
    host: {
        '[class.lvl-content-title]': 'true'
    }
})
export class ContentTitleDirective { }
