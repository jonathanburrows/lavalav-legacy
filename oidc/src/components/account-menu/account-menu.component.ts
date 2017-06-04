import { Component } from '@angular/core';

import { SecurityService } from '../../services';

@Component({
    selector: 'lvl-oidc-account-menu',
    templateUrl: 'account-menu.component.html',
    styleUrls: ['account-menu.component.scss']
})
export class AccountMenuComponent {
    get username() {
        if (!this.securityService.isAuthorized) {
            return null;
        }

        return this.securityService.userInfo['name'];
    }

    constructor(public securityService: SecurityService) { }
}
