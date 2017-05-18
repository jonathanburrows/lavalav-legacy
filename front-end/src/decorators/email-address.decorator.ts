import { AbstractControl, ValidatorFn } from '@angular/forms';

import { DefineValidationMetadata } from './validation-factory';

/** Validates an email address. */
export function EmailAddress(): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const validator = emailAddressValidator();
        DefineValidationMetadata(EmailAddress.name, validator, target, propertyKey);
    };
}

function emailAddressValidator(): ValidatorFn {
    return (control: AbstractControl): { [name: string]: any } => {
        if (!control) {
            throw new Error('Control is null.');
        }

        // tslint:disable-next-line
        const emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;

        if (control.value && !emailRegex.test(control.value)) {
            return {
                'emailAddress': 'Invalid email'
            };
        } else {
            return null;
        }
    };
}
