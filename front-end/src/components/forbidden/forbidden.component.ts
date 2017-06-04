import { Component } from '@angular/core';

import { Navigatable } from '../../decorators';

/**
 *  Provides a page to tell users they are accessing a resource they dont have permissions to.
 */
@Component({
    selector: 'lvl-oidc-forbidden',
    templateUrl: 'forbidden.component.html',
    styleUrls: ['forbidden.component.scss']
})
@Navigatable({
    hideInMenu: true,
    title: 'Forbidden'
})
export class ForbiddenComponent { }
