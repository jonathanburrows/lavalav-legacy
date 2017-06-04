import { Component } from '@angular/core';

import { Navigatable } from '../../decorators';

/**
 *  Provides a page to tell users they are trying to access resources that dont exist.
 */
@Component({
    selector: 'lvl-not-found',
    templateUrl: 'not-found.component.html',
    styleUrls: ['not-found.component.scss']
})
@Navigatable({
    hideInMenu: true,
    title: 'Not Found'
})
export class NotFoundComponent { }
