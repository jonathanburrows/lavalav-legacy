import { Directive, Input } from '@angular/core';

/**
 *  A row which will show up in the master list.
 */
@Directive({
    selector: '[lvl-master-item]',
    host: {
        '[class.lvl-master-item]': 'true',
        '[class.lvl-master-item--selected]': 'selected'
    }
})
export class MasterItemDirective {
    /** Denotes that a row is in focus on the detail panel. */
    @Input() selected: boolean;
}
