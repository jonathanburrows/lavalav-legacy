import { Component, OnInit } from '@angular/core';

import { Navigatable, ValidatableForm, ValidationBuilder } from '@lvl/front-end';
import { ResetPasswordService, ResetPasswordViewModel } from '../../services';

/**
 *  Provide a way for users to update their password, so they can remain secure if their password has been compromised.
 */
@Component({
    selector: 'lvl-oidc-change-password',
    templateUrl: 'change-password.component.html',
    styleUrls: ['change-password.component.scss']
})
@Navigatable({
    hideInMenu: true,
    title: 'Change Password'
})
export class ChangePasswordComponent implements OnInit {
    form: ValidatableForm<ResetPasswordViewModel>;
    model: ResetPasswordViewModel;

    constructor(private validationBuilder: ValidationBuilder, private resetPasswordService: ResetPasswordService) { }

    ngOnInit() {
        this.model = new ResetPasswordViewModel();
        this.form = this.validationBuilder.formFor(this.model, ['oldPassword', 'newPassword']);
    }

    save() {
        this.form.saveAsync(() => <any>this.resetPasswordService.reset(this.model));
    }
}
