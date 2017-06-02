import { EventEmitter, Output } from '@angular/core';
import {
    AbstractControl,
    AsyncValidatorFn,
    FormGroup,
    ValidatorFn
} from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';

import { ValidationBuilder } from './validation-builder.service';

/**
 *  Validation form which has error messages for each property.
 *  @remarks errorMessages will be updated when a change occurs. this is set up in ValidationBuilder.
 */
export class ValidatableForm<T> extends FormGroup {
    /**
     *  Will contain the error messages for all the controls in the form.
     *  If a control is valid, it will have no entry in model errors.
     *  If a control has more than one error, the messages are concatonated with commas.
     *  When a value is changed, the model errors are automatically updated.
     */
    public modelErrors: { [property: string]: string };

    @Output() saveStarted = new EventEmitter<T>();
    @Output() saveCompleted = new EventEmitter<T>();

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

    /**
     *  Will update the fields in the form to reflect a given model.
     *  @param model The model whos properties will be used to populate the form.
     */
    public updateFieldsWithModel(model: T) {
        if (!model) {
            throw new Error('model is null.');
        }

        for (const controlName of Object.keys(this.controls)) {
            const control = this.controls[controlName];
            control.setValue(model[controlName]);
        }
    }

    /**
     *  Helper function to return all controls.
     */
    public getControls() {
        return Object.keys(this.controls).map(controlName => this.get(controlName));
    }

    /**
     *  Perform an asynchonous save request. Events are triggered to denote the start and end of the request.
     *  @param saveFunction The function which will contact the server for the saving.
     *  @throws {Error} saveFunction is null.
     *  @remarks Will only submit the request if the form is valid and modified. Otherwise, events dont trigger.
     */
    public saveAsync(saveFunction: () => Observable<T>): Observable<T> {
        if (!saveFunction) {
            throw new Error('saveFunction is null.');
        }

        if (this.valid && this.touched) {
            const successAction = (savedModel: T) => {
                this.markAsPristine();
                this.saveCompleted.emit(savedModel);
            };

            const failureAction = response => {
                this.setModelErrors(response);
                this.saveCompleted.emit(this.value);
            };

            this.saveStarted.emit(this.value);
            const saveRequest = saveFunction();
            saveRequest.subscribe(successAction, failureAction);

            return saveRequest;
        } else {
            const completedSubject = new Subject<T>();
            completedSubject.complete();
            return completedSubject;
        }
    }
}
