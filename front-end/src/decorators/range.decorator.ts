import { AbstractControl, ValidatorFn } from '@angular/forms';

import { DefineValidationMetadata } from './validation-factory';

/** Specifies the numeric range constraints for the value of a data field.
 *  @param minimum {number} The minimum value (inclusive) the property must be.
 *  @param maximum {number} The maximum value (inclusive) the property can be.
 *  @throws minimum has not been given a value.
 *  @throws maximum has not been given a value.
 *  @throws minimum equals maximum.
 *  @throws minimum is larger than maximum. */
export function Range(minimum: number, maximum: number): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!minimum && isNaN(minimum)) {
            throw new Error(`@${Range.name} on ${target.constructor.name} has an undefined minimum.`);
        }
        if (!maximum && isNaN(maximum)) {
            throw new Error(`@${Range.name} on ${target.constructor.name} has an undefined maximum.`);
        }
        if (minimum === maximum) {
            throw new Error(`@${Range.name} on ${target.constructor.name} has an identical min and max value of ${minimum}`);
        }
        if (minimum > maximum) {
            // tslint:disable-next-line
            throw new Error(`@${Range.name} on ${target.constructor.name} has a minimum ${minimum} which is larger than the maximum ${maximum}`);
        }

        const validator = rangeValidator(minimum, maximum);
        DefineValidationMetadata(Range.name, validator, target, propertyKey);
    };
}

function rangeValidator(minimum: number, maximum: number): ValidatorFn {
    return (control: AbstractControl): { [name: string]: any } => {
        if (!control) {
            throw new Error('Control is null.');
        }

        if (control.value >= minimum && control.value <= maximum) {
            return null;
        } else {
            return {
                'range': `Not between ${minimum} and ${maximum}`
            };
        }
    };
}
