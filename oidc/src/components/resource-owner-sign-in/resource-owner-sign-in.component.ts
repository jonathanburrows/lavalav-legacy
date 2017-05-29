import { Component, OnInit } from '@angular/core';

import { ValidatableForm, ValidationBuilder } from '@lvl/front-end';
import { SecurityService } from '../../services';
import { Credentials } from '../../services';

@Component({
    selector: 'lvl-oidc-resource-owner-sign-in',
    templateUrl: 'resource-owner-sign-in.component.html',
    styleUrls: ['resource-owner-sign-in.component.scss']
})
export class ResourceOwnerSigninComponent implements OnInit {
    form: ValidatableForm;
    model: Credentials;

    constructor(private securityService: SecurityService, private validationBuilder: ValidationBuilder) { }

    ngOnInit() {
        this.model = new Credentials();
        this.form = this.validationBuilder.formFor(this.model, [
            'username',
            'password'
        ]);
    }

    signIn() {
        if (this.form.valid) {
            this.securityService.login(this.model).subscribe(_ => { }, this.setValidationErrors.bind(this));
        }
    }

    private setValidationErrors(errorResponse: any) {
        if (errorResponse.status !== 400) {
            // Not a validation issue, no validation messages are attached.
            return;
        }

        const errorDetails = JSON.parse(<any>errorResponse._body);
        switch (errorDetails.error_description) {
            case 'invalid_user':
                this.setInputError('username', `Can't find user`);
                break;
            case 'invalid_password':
                this.setInputError('password', 'Incorrect password, try again');
                break;
            default:
                break;
        }
    }

    private setInputError(inputName: string, errorMessage: string) {
        this.form.modelErrors[inputName] = errorMessage;

        const input = this.form.get(inputName);
        input.setErrors({ 'server-validation': errorMessage });
        input.markAsDirty();
        input.markAsTouched();
    }
}
