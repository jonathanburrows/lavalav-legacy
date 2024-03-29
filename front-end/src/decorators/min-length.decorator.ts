﻿import { AbstractControl, ValidatorFn } from '@angular/forms';

import { DefineValidationMetadata } from './validation-factory';

/** Specifies the minimum length of array or string data allowed in a property.
 *  @param length {number} The minimum (inclusive) the value must be to be valid.
 *  @throws length has not been given a value.
 *  @throws length is not a positive number */
export function MinLength(length: number): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!length) {
            throw new Error(`@${MinLength.name} on ${target.constructor.name} has an undefined length.`);
        }
        if (length < 0) {
            throw new Error(`@${MinLength.name} on ${target.constructor.name} has a negative number.`);
        }

        const validator = minLengthValidator(length);
        DefineValidationMetadata(MinLength.name, validator, target, propertyKey);
    };
}

function minLengthValidator(length: number): ValidatorFn {
    return (control: AbstractControl): { [name: string]: any } => {
        if (!control) {
            throw new Error('Control is null.');
        }

        if (control.value && control.value.length < length) {
            return {
                'minLength': `${control.value.length}/${length}`
            };
        } else {
            return null;
        }
    };
}
