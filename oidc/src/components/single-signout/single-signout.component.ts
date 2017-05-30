import { Component, OnInit } from '@angular/core';

import { SecurityService } from '../../services';

/**
 *  Will clear tokens from local service, so single signout can be supported.
 */
@Component({
    selector: 'lvl-oidc-single-signout',
    template: ''
})
export class SingleSignoutComponent implements OnInit {
    constructor(private securityService: SecurityService) { }

    ngOnInit() {
        this.securityService.logoutLocal();
    }
}
