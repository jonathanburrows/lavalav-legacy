import { Directive } from '@angular/core';

/**
 *  Used to denote non-primary information about a menu item.
 */
@Directive({
    selector: '[lvl-master-item-subtitle]',
    host: { '[class.lvl-master-item-subtitle]': 'true' }
})
export class MasterItemSubtitleDirective { }
