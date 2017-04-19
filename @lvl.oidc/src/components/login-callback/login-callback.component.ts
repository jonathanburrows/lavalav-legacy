import { Component, OnInit } from '@angular/core';

import { SecurityService } from '../../services';

@Component({
    selector: 'lvl-oidc-login-callback',
    template: ''
})
export class LoginCallbackComponent implements OnInit {
    constructor(private securityService: SecurityService) { }

    ngOnInit() {
        this.securityService.login();
    }
}
