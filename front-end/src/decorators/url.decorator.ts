import { AbstractControl, ValidatorFn } from '@angular/forms';

import { DefineValidationMetadata } from './validation-factory';

/** Provides URL validation. */
export function Url(): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const validator = urlValidator();
        DefineValidationMetadata(Url.name, validator, target, propertyKey);
    };
}

function urlValidator(): ValidatorFn {
    return (control: AbstractControl): { [name: string]: any } => {
        if (!control) {
            throw new Error('Control is null.');
        }

        const urlRegex = /^(ftp|http|https):\/\/[^ "]+$/i;

        if (control.value && !urlRegex.test(control.value)) {
            return {
                'url': 'Invalid url'
            };
        } else {
            return null;
        }
    };
}
