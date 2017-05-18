import { Type } from '@angular/core';
import { ValidatorFn } from '@angular/forms';

import { Phone } from './phone.decorator';
import { ValidationKey } from './validation-factory';

describe(Phone.name, () => {
    it('is valid if not defined', () => {
        const validator = getValidator(Person, 'phoneNumber');
        const control: any = { value: null };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid if less than 7 digits', () => {
        const validator = getValidator(Person, 'phoneNumber');
        const control: any = { value: 123456 };

        const errors = validator(control);

        expect(errors['phone']).toBeDefined();
    });

    it('is valid if 7 digits', () => {
        const validator = getValidator(Person, 'phoneNumber');
        const control: any = { value: 1234567 };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid if more than 7 digits', () => {
        const validator = getValidator(Person, 'phoneNumber');
        const control: any = { value: 12345678 };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    function getValidator<T>(type: Type<T>, property: string): ValidatorFn {
        const validators = Reflect.getMetadata(ValidationKey, type.prototype, property);
        if (!validators) {
            throw new Error(`No validator for ${type.name}.${property}`);
        }

        return validators[0];
    }
});

class Person {
    @Phone() phoneNumber: number;
}
