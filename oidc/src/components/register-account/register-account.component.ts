import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ValidatableForm, ValidationBuilder } from '@lvl/front-end';
import {
    Credentials,
    SecurityService,
    UserService
} from '../../services';

@Component({
    selector: 'lvl-oidc-register',
    templateUrl: 'register-account.component.html',
    styleUrls: ['register-account.component.scss']
})
export class RegisterAccountComponent implements OnInit {
    form: ValidatableForm;
    model: Credentials;

    constructor(
        private validationBuilder: ValidationBuilder,
        private userService: UserService,
        private router: Router,
        private securityService: SecurityService) { }

    ngOnInit() {
        this.model = new Credentials();
        this.form = this.validationBuilder.formFor(this.model, [
            'username',
            'password'
        ]);
    }

    /**
     *  Attempts to create an account. If successful, logs in and redirects. Otherwise, error messages are displayed.
     */
    registerAccount() {
        if (this.form.valid) {
            // take a copy of the credentials, we dont want to log in using a tampered copy.
            const username = this.model.username;
            const password = this.model.password;

            const createRequest = this.userService.create(username, password).subscribe(
                () => this.securityService.login({ username: username, password: password }),
                this.setValidationErrors.bind(this)
            );
        }
    }

    private setValidationErrors(errorResponse: any) {
        if (errorResponse.status !== 400) {
            return;
        }

        if (!errorResponse._body) {
            return;
        }

        const errorDetails: { [inputName: string]: string[] } = JSON.parse(errorResponse._body);
        Object.keys(errorDetails).forEach(inputName => {
            const errors = errorDetails[inputName];

            if (errors.length) {
                const errorMessage = errors.join(', ');
                this.setInputError(inputName, errorMessage);
            }
        });
    }

    private setInputError(inputName: string, errorMessage: string) {
        this.form.modelErrors[inputName] = errorMessage;

        const input = this.form.get(inputName);
        input.setErrors({ 'server-validation': errorMessage });
        input.markAsDirty();
        input.markAsTouched();
    }
}
