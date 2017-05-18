import { AbstractControl, ValidatorFn } from '@angular/forms';

import { DefineValidationMetadata } from './validation-factory';

/** Checks if a value has been assigned to a property. */
export function Required(): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const validator = requiredValidator();
        DefineValidationMetadata(Required.name, validator, target, propertyKey);
    };
}

function requiredValidator(): ValidatorFn {
    return (control: AbstractControl): { [name: string]: any } => {
        if (!control) {
            throw new Error('Control is null.');
        }

        const value = control.value;
        if (typeof value === 'undefined' || value === null || value === '') {
            return {
                'required': 'Required'
            };
        } else {
            return null;
        }
    };
}
