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

    /**
     *  Will update the model errors and their respective inputs with the response from the server.
     *  @param errorResponse The response which may contain server errors.
     *  @throws {Error} errorResponse is null.
     *  @remarks if the status code is not 400, then no model errors are updated
     */
    public setModelErrors(errorResponse: any) {
        if (!errorResponse) {
            throw new Error('errorResponse is null.');
        }
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
        this.modelErrors[inputName] = errorMessage;

        const input = this.get(inputName);
        input.setErrors({ 'server-validation': errorMessage });
        input.markAsDirty();
        input.markAsTouched();
    }
}
