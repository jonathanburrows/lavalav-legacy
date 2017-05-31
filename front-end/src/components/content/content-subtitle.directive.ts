import { Directive} from '@angular/core';

/**
 *  Denotes the non-primary title.
 */
@Directive({
    selector: 'lvl-content-subtitle',
    host: {
        '[class.lvl-content-subtitle]': 'true'
    }
})
export class ContentSubtitleDirective { }
