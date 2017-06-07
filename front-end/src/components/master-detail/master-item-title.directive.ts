import { Directive } from '@angular/core';

/**
 *  Represents information which can identify an entity.
 */
@Directive({
    selector: '[lvl-master-item-title]',
    host: { '[class.lvl-master-item-title]': 'true' }
})
export class MasterItemTitleDirective { }
