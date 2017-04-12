import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

import { ExternalProvider } from './external-provider';
import { LoginViewModel } from './login-view-model';

declare var externalProviders: ExternalProvider[];
declare var model: LoginViewModel;
declare var usernameValidationMessage: string;
declare var passwordValidationMessage: string;

@Component({
    selector: 'lvl-oidc-login', 
    templateUrl: 'login.component.html',
    styleUrls: ['login.component.scss']
})
export class LoginComponent implements OnInit {
    public externalProviders: ExternalProvider[];
    public model: LoginViewModel;

    public usernameValidationMessage: string;
    public usernameControl: FormControl;

    public passwordValidationMessage: string;
    public passwordControl: FormControl;

    public ngOnInit() {
        if (typeof externalProviders === 'undefined') {
            throw new Error('There is no global variable named externalProviders.');
        }
        this.externalProviders = externalProviders;

        if (typeof model === 'undefined') {
            throw new Error('There is no global variable named model.');
        }
        this.model = new LoginViewModel(model);

        if (typeof usernameValidationMessage === 'undefined') {
            throw new Error('There is no global variable called usernameValidationMessage');
        }
        this.usernameValidationMessage = usernameValidationMessage;

        if (typeof passwordValidationMessage === 'undefined') {
            throw new Error('There is no global variable called passwordValidationMessage');
        }
        this.passwordValidationMessage = passwordValidationMessage;

        this.usernameControl = new FormControl('', Validators.required);
        this.passwordControl = new FormControl('', Validators.required);
    }
}
