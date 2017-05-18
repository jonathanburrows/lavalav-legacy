import { AbstractControl, ValidatorFn } from '@angular/forms';

import { DefineValidationMetadata } from './validation-factory';

/** Specifies the maximum length of array or string data allowed in a property.
 *  @throws length has not been given a value.
 *  @throws length is not a positive number.
 */
export function MaxLength(length: number): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!length) {
            throw new Error(`@${MaxLength.name} on ${target.constructor.name} has an undefined length.`);
        }
        if (length < 0) {
            throw new Error(`@${MaxLength.name} on ${target.constructor.name} has a negative number.`);
        }

        const validator = maxLengthValidator(length);
        DefineValidationMetadata(MaxLength.name, validator, target, propertyKey);
    };
}

function maxLengthValidator(length: number): ValidatorFn {
    return (control: AbstractControl): { [name: string]: any } => {
        if (!control) {
            throw new Error('Control is null.');
        }

        if (control.value && control.value.length > length) {
            return {
                'maxLength': `${control.value.length}/${length}`
            };
        } else {
            return null;
        }
    };
}
