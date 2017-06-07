import { Directive } from '@angular/core';

/**
 *  Represents the details of a selected entity. Will be rendered after a lvl-master directive.
 */
@Directive({
    selector: '[lvl-detail]',
    host: { '[class.lvl-detail]': 'true' }
})
export class DetailDirective { }
