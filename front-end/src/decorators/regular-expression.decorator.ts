import { AbstractControl, ValidatorFn } from '@angular/forms';

import { DefineValidationMetadata } from './validation-factory';

/** Specifies that a data field value in ASP.NET Dynamic Data must match the specified
 *  regular expression.
 *  @param pattern {RegExp} the pattern the property must adhere to.
 *  @throws pattern has not been been assigned a value. */
export function RegularExpression(pattern: RegExp): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!pattern) {
            throw new Error(`@${RegularExpression.name} on ${target.constructor.name} did not define a pattern.`);
        }

        const validator = regularExpressionValidator(pattern);
        DefineValidationMetadata(RegularExpression.name, validator, target, propertyKey);
    };
}


function regularExpressionValidator(pattern: RegExp): ValidatorFn {
    return (control: AbstractControl): { [name: string]: any } => {
        if (!control) {
            throw new Error('Control is null.');
        }

        if (control.value && !pattern.test(control.value)) {
            return {
                'regularExpression': `Doesnt match pattern '${pattern}'`
            };
        } else {
            return null;
        }
    };
}
