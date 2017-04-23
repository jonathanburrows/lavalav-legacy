import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

import { ApiService } from '@lvl/core';
import { Credentials, SecurityService } from '../../services';

@Component({
    selector: 'lvl-oidc-credentials-login',
    templateUrl: 'credentials-login.component.html',
    styleUrls: ['credentials-login.component.scss']
})
export class CredentialsLoginComponent implements OnInit {
    public model: Credentials;

    public usernameValidationMessage: string;
    public usernameControl: FormControl;

    public passwordValidationMessage: string;
    public passwordControl: FormControl;

    constructor(private securityService: SecurityService, private apiService: ApiService) {
        this.model = { username: null, password: null };
    }

    public ngOnInit() {
        this.usernameControl = new FormControl('', Validators.required);
        this.passwordControl = new FormControl('', Validators.required);
    }

    public login() {
        this.securityService.login(this.model);
    }

    public request() {
        debugger;
        this.apiService.get(Kitten).subscribe(x => {
            debugger;
        });
    }
}

class Kitten { }
