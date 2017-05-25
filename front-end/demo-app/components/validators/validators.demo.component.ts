import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ValidationModel } from '../../models/validation-model';
import { ValidatableForm, ValidationBuilder } from '../../../src';

/**
 *  This component will render a form with validation, for development purpose.
 */
@Component({
    selector: 'lvl-demo-validators',
    templateUrl: 'validators.demo.component.html',
    styleUrls: ['validators.demo.component.scss']
})
export class ValidatorsDemoComponent implements OnInit {
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
