import { AbstractControl, ValidatorFn } from '@angular/forms';

import { DefineValidationMetadata } from './validation-factory';

/**
 * Validates a phone address.
 */
export function Phone(): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const validator = phoneValidator(length);
        DefineValidationMetadata(Phone.name, validator, target, propertyKey);
    };
}

function phoneValidator(length: number): ValidatorFn {
    return (control: AbstractControl): { [name: string]: any } => {
        if (!control) {
            throw new Error('Control is null.');
        }

        const minimumPhoneNumberLength = 7;
        if (control.value && control.value.toString().length < minimumPhoneNumberLength) {
            return {
                'phone': 'Invalid phone number'
            };
        } else {
            return null;
        }
    };
}
