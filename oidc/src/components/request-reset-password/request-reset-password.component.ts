import { Component, OnInit } from '@angular/core';

import { ValidatableForm, ValidationBuilder } from '@lvl/front-end';
import { RequestResetPasswordViewModel } from './request-reset-password-view-model';
import { ResetPasswordService } from '../../services';

@Component({
    selector: 'lvl-oidc-request-reset-password',
    templateUrl: 'request-reset-password.component.html',
    styleUrls: ['request-reset-password.component.scss']
})
export class RequestResetPasswordComponent implements OnInit {
    model: RequestResetPasswordViewModel;
    form: ValidatableForm;
    emailSent: boolean;

    constructor(private resetPasswordService: ResetPasswordService, private validationBuilder: ValidationBuilder) { }

    ngOnInit() {
        this.model = new RequestResetPasswordViewModel();
        this.form = this.validationBuilder.formFor(this.model, ['username']);
    }

    requestReset() {
        if (this.form.valid) {
            const username = this.form.get('username').value;
            this.resetPasswordService.requestReset(username).subscribe(
                this.showSuccess.bind(this),
                this.form.setModelErrors.bind(this.form)
            );
        }
    }

    private showSuccess() {
        this.emailSent = true;
        this.form.get('username').disable();
    }
}
