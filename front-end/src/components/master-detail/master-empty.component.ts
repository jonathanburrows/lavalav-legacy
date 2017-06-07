import { Component, Input, OnInit } from '@angular/core';

/**
 *  Will display a message when there are no lvl-master-items in the master list.
 */
@Component({
    selector: 'lvl-master-empty',
    templateUrl: 'master-empty.component.html',
    host: {
        '[class.lvl-master-empty]': 'true'
    }
})
export class MasterEmptyComponent implements OnInit {
    /** Explain what cant be shown to the user. */
    @Input() message: string;

    ngOnInit() {
        if (!this.message) {
            throw new Error(`${MasterEmptyComponent.name} was instantiated without a message.`);
        }
    }
}
