import { AbstractControl, AsyncValidatorFn, FormGroup, ValidatorFn } from '@angular/forms';

import { ValidationBuilder } from './validation-builder.service';

/**
 *  Validation form which has error messages for each property.
 *  @remarks errorMessages will be updated when a change occurs. this is set up in ValidationBuilder.
 */
export class ValidatableForm extends FormGroup {
    /**
     *  Will contain the error messages for all the controls in the form.
     *  If a control is valid, it will have no entry in model errors.
     *  If a control has more than one error, the messages are concatonated with commas.
     *  When a value is changed, the model errors are automatically updated.
     */
    public modelErrors: { [property: string]: string };

    constructor(
        controls: { [key: string]: AbstractControl },
        validator?: ValidatorFn | null,
        asyncValidator?: AsyncValidatorFn | null) {

        super(controls, validator, asyncValidator);
        this.modelErrors = {};
    }
}
