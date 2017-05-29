import { Component } from '@angular/core';

/**
 *  Provides a widget which slides open, and can redirect the user to a search page with a query.
 *  @remarks This search is intended for the entire system, and will appear in the top right of the app bar.
 *           Should be decorated with [lvl-toolbar-action] to show up in the correct place.
 */
@Component({
    selector: 'lvl-search-action',
    templateUrl: 'search-action.component.html',
    styleUrls: ['search-action.component.scss']
})
export class SearchActionComponent {
    /** Will expand the search to include the input. */
    showSearch: boolean;

    /**
     *  Will show the search input in the application bar.
     */
    openSearch() {
        this.showSearch = true;
    }

    /**
     *  Will hide the search input in the application bar.
     */
    closeSearch() {
        this.showSearch = false;
    }
}
