import {
    ContentChildren,
    Directive,
    QueryList
} from '@angular/core';

import { MasterItemDirective } from './master-item.directive';

/**
 *  Represents the right hand panel, which will contain a list of items which can be selected.
 */
@Directive({
    selector: '[lvl-master]',
    host: {
        '[class.lvl-master]': 'true',
        '[class.lvl-master--empty]': 'isEmpty',
        '[class.mat-elevation-z4]': 'true'
    }
})
export class MasterDirective {
    @ContentChildren(MasterItemDirective) items: QueryList<MasterItemDirective>;

    /** Was placed here instead of master-detail because the parent cant load the directives. */
    get itemsAreSelected() {
        return this.items.some(i => i.selected);
    }

    /** Determines if there are any menu items, so that an empty state can be show. */
    get isEmpty() {
        return !this.items || this.items.length === 0;
    }
}
