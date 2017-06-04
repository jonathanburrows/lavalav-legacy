import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Rx';

import {
    Navigatable,
    ValidationBuilder,
    ValidatableForm
} from '@lvl/front-end';
import {
    PersonalDetailsService,
    PersonalDetailsViewModel,
    TokenService
} from '../../services';

/**
 *  Allows users to modify their claims, so they can change what their identity is.
 */
@Component({
    selector: 'lvl-oidc-personal-details',
    templateUrl: 'personal-details.component.html',
    styleUrls: ['personal-details.component.scss']
})
@Navigatable({
    hideInMenu: true,
    title: 'Personal Details'
})
export class PersonalDetailComponents {
    form: ValidatableForm<PersonalDetailsViewModel>;
    model: PersonalDetailsViewModel;

    constructor(private validationBuilder: ValidationBuilder, private personalDetailsService: PersonalDetailsService) { }

    ngOnInit() {
        // set up the form.
        this.model = new PersonalDetailsViewModel();
        this.form = this.validationBuilder.formFor(this.model, [
            'email',
            'firstName',
            'lastName',
            'phoneNumber',
            'job',
            'location'
        ]);

        this.personalDetailsService.get().subscribe(this.form.updateFieldsWithModel.bind(this.form));
    }

    save() {
        this.form.saveAsync(() => this.personalDetailsService.update(this.model));
    }
}
