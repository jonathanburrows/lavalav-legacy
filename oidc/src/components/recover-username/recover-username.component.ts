import { Component, OnInit } from '@angular/core';

import { ValidatableForm, ValidationBuilder } from '@lvl/front-end';
import { RecoverUsernameViewModel } from './recover-username-view-model';
import { RecoverUsernameService } from '../../services';

/**
 *  Lets a user request an email reminding them of their username, so that they can remember forgotten accounts.
 */
@Component({
    selector: 'lvl-oidc-recover-username',
    templateUrl: 'recover-username.component.html',
    styleUrls: ['recover-username.component.scss']
})
export class RecoverUsernameComponent implements OnInit {
    model: RecoverUsernameViewModel;
    form: ValidatableForm;
    emailSent: boolean;

    constructor(private validationBuilder: ValidationBuilder, private recoverUsernameService: RecoverUsernameService) { }

    ngOnInit() {
        this.model = new RecoverUsernameViewModel();
        this.form = this.validationBuilder.formFor(this.model, ['email']);
    }

    recoverUsername() {
        if (this.form.valid) {
            const email = this.form.get('email').value;
            this.recoverUsernameService.recoverUsername(email).subscribe(
                this.showSuccess.bind(this),
                this.form.setModelErrors.bind(this.form)
            );
        }
    }

    private showSuccess() {
        this.emailSent = true;
        this.form.get('email').disable();
    }
}
