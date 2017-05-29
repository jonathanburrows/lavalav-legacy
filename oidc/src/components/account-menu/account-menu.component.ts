import { Component, OnInit } from '@angular/core';

import { SecurityService } from '../../services';

@Component({
    selector: 'lvl-oidc-account-menu',
    templateUrl: 'account-menu.component.html',
    styleUrls: ['account-menu.component.scss']
})
export class AccountMenuComponent implements OnInit {
    constructor(public securityService: SecurityService) { }

    ngOnInit() {
    }
}
