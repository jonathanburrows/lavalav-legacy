import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ValidationModel } from '../../models/validation-model';
import { ValidatableForm, ValidationBuilder } from '../../../src';

/**
 *  This component will render a form with validation, for development purpose.
 */
@Component({
    selector: 'lvl-e2e-validators',
    templateUrl: 'validators.e2e.component.html',
    styleUrls: ['validators.e2e.component.scss']
})
export class ValidatorsE2eComponent implements OnInit {
    model: ValidationModel;
    form: ValidatableForm;

    constructor(private validationBuilder: ValidationBuilder) { }

    ngOnInit() {
        this.model = new ValidationModel();
        this.form = this.validationBuilder.formFor(this.model, [
            'creditCard',
            'emailAddress',
            'maxLength',
            'minLength',
            'phone',
            'range',
            'regularExpression',
            'required'
        ]);
    }
}
