import { Component, OnInit } from '@angular/core';
import {
    FormBuilder,
    FormGroup,
    Validators
} from '@angular/forms';

import { ValidationModel } from '../../models/validation-model';
import {
    Navigatable,
    ValidatableForm,
    ValidationBuilder
} from '../../../src';

/**
 *  This component will render a form with validation, for development purpose.
 */
@Component({
    selector: 'lvl-demo-validators',
    templateUrl: 'validators.demo.component.html',
    styleUrls: ['validators.demo.component.scss']
})
@Navigatable({
    group: 'Front End',
    title: 'Validators',
    icon: 'spellcheck'
})
export class ValidatorsDemoComponent implements OnInit {
    model: ValidationModel;
    form: ValidatableForm<ValidationModel>;

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
