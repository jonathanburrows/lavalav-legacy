import { Injectable, Type } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';

import { ValidatableForm } from './validatable-form';
import { ValidationKey } from '../../decorators';

/**
 *  Service to reduce the boilerplate code of Reactive Forms. Intended to reduce the number of string references to properties.
 */
@Injectable()
export class ValidationBuilder {
    constructor(private formBuilder: FormBuilder) { }

    /**
     *  Creates a form group with validation for the given properties of a model, when all inputs are on the same type.
     *
     *  @param parent The object whos prototype contains the validation metadata, and is used for the initial values.
     *  @param properties The properties which have inputs on the form.
     *
     *  @throws {Error} model is null.
     *  @throws {Error} model is an object literal.
     *  @throws {Error} properties is null.
     *  @throws {Error} the same property is specified more than once.
     *
     *  @remarks If the form binds inputs to multiple types, use controlFor instead.
     *  @remarks Make sure the object passed has the correct prototype.
     */
    formFor<T>(model: T, properties: string[]): ValidatableForm<T> {
        if (!model) {
            throw new Error('model is null.');
        }
        if (this.isObjectLiteral(model)) {
            throw new Error('model is an object literal.');
        }

        if (!properties) {
            throw new Error('properties is null');
        }

        for (const property of properties) {
            const matchingProperties = properties.filter(p => p === property);
            if (matchingProperties.length > 1) {
                throw new Error(`The property '${property}' is specified ${matchingProperties.length} times.`);
            }
        }

        const config = properties.reduce((reduced, property) => {
            const validators = Reflect.getMetadata(ValidationKey, model.constructor.prototype, property) || [];
            const value = model[property];
            const control = this.formBuilder.control(value, validators);

            control.valueChanges.subscribe(updatedValue => model[property] = updatedValue);

            reduced[property] = control;

            return reduced;
        }, {});

        const formGroup = new ValidatableForm<T>(config);
        formGroup.valueChanges.subscribe(() => formGroup.modelErrors = this.getErrors(formGroup));
        formGroup.modelErrors = this.getErrors(formGroup);

        return formGroup;
    }

    /**
     *  Creates a control for the given property, with validation taken from the metadata.
     *
     *  @param parent The object whos prototype contains the validation metadata, and has the initial value of the property.
     *  @param property Thr property which will be used in the control.
     *
     *  @throws {Error} parent is null.
     *  @throws {Error} parent is an object literal.
     *  @throws {Error} property is null.
     *
     *  @remarks Make sure the object passed has a prototype which contains the validation metadata.
     */
    controlFor(parent: Object, property: string): FormControl {
        if (!parent) {
            throw new Error('parent is null.');
        }
        if (this.isObjectLiteral(parent)) {
            throw new Error('parent is an object literal.');
        }

        if (!property) {
            throw new Error('property is null');
        }

        const validators = Reflect.getMetadata(ValidationKey, parent.constructor.prototype, property) || [];
        const value = parent[property];

        const control = this.formBuilder.control(value, validators, null);
        control.valueChanges.subscribe(updatedValue => parent[property] = updatedValue);

        return control;
    }

    /**
     *  Returns a set of error messages for invalid properties.
     *
     *  @param formGroup the form whos invalid inputs will have their validation messages returned.
     *
     *  @throws {Error} formGroup is null.
     */
    getErrors(formGroup: FormGroup): { [property: string]: any } {
        if (!FormGroup) {
            throw new Error('formGroup is null');
        }

        if (!formGroup) {
            throw new Error('formGroup is null.');
        }

        const modelErrors = {};

        for (const controlName in formGroup.controls) {
            if (controlName) {
                const control = formGroup.get(controlName);

                if (!control!.valid) {
                    const errors = [];

                    // tslint:disable-next-line
                    for (const error in control.errors) {
                        errors.push(control.errors[error]);
                    }

                    modelErrors[controlName] = errors.join(' ');
                }
            }
        }

        return modelErrors;
    }

    private isObjectLiteral(value: Object): boolean {
        return value.constructor === {}.constructor;
    }
}
