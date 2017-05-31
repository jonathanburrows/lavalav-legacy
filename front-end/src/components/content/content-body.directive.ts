import { Directive } from '@angular/core';

/**
 *  Container for text and inputs.
 */
@Directive({
    selector: 'lvl-content-body',
    host: {
        '[class.lvl-content-body]': 'true'
    }
})
export class ContentBodyDirective { }
