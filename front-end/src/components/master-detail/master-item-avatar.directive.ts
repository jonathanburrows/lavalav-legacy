import { Directive } from '@angular/core';

/**
 *  Denotes a icon to display in the master list.
 *  @remarks - used to apply styles, was preferred over adding a class.
 */
@Directive({
    selector: '[lvl-master-item-avatar]',
    host: { '[class.lvl-master-item-avatar]': 'true' }
})
export class MasterItemAvatarDirective { }
