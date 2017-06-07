import {
    Component,
    EventEmitter,
    Input,
    Output,
    OnInit
} from '@angular/core';

/**
 *  An input that will allow users to search for entities in the master list.
 */
@Component({
    selector: 'lvl-master-search',
    templateUrl: 'master-search.component.html'
})
export class MasterSearchComponent implements OnInit {
    /** Propagates the change of the search input. */
    @Output() change = new EventEmitter<any>();

    /** Triggered when the search icon is clicked, or enter is pressed while in focus. */
    @Output() submit = new EventEmitter<any>();

    /* Will bind the inputs value to this. */
    @Input() model: SearchModel;

    ngOnInit() {
        if (!this.model) {
            throw new Error('model is null.');
        }
    }
}

export interface SearchModel {
    filter?: string;
}
