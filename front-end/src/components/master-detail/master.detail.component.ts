import {
    AfterContentInit,
    ContentChildren,
    ContentChild,
    ViewChild,
    Component,
    QueryList,
    ViewEncapsulation
} from '@angular/core';

import { MasterItemDirective } from './master-item.directive';
import { MasterDirective } from './master.directive';
import { MasterSearchComponent } from './master-search.component';

/**
 *  A container around master/detail directives, so that styles can be applied.
 */
@Component({
    selector: 'lvl-master-detail',
    templateUrl: 'master.detail.component.html',
    styleUrls: ['master.detail.component.scss'],
    host: {
        '[class.lvl-master-detail]': 'true',
        '[class.lvl-master-detail--items-are-selected]': 'itemsAreSelected'
    },
    encapsulation: ViewEncapsulation.None
})
export class MasterDetailComponent {
    @ContentChild(MasterDirective) master: MasterDirective;

    /** Allows the master to be hidden on mobile devices. */
    get itemsAreSelected() {
        return this.master && this.master.itemsAreSelected;
    }
}
