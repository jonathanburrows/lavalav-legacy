import { Type } from '@angular/core';
import { ValidatorFn } from '@angular/forms';

import { EmailAddress } from './email-address.decorator';
import { ValidationKey } from './validation-factory';

describe('EmailAddress', () => {
    it('is true when value is null', () => {
        const validator = getValidator(Person, 'emailAddress');
        const control: any = { value: null };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is false when no domain', () => {
        const validator = getValidator(Person, 'emailAddress');
        const control: any = { value: 'john@smith' };

        const errors = validator(control);

        expect(errors['emailAddress']).toBeDefined();
    });

    it('is false when no host', () => {
        const validator = getValidator(Person, 'emailAddress');
        const control: any = { value: 'john@.com' };

        const errors = validator(control);

        expect(errors['emailAddress']).toBeDefined();
    });

    it('is false when no address', () => {
        const validator = getValidator(Person, 'emailAddress');
        const control: any = { value: 'smith.com' };

        const errors = validator(control);

        expect(errors['emailAddress']).toBeDefined();
    });

    it('is true when address, host, and domain are provided', () => {
        const validator = getValidator(Person, 'emailAddress');
        const control: any = { value: 'john@smith.com' };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is true when all caps', () => {
        const validator = getValidator(Person, 'emailAddress');
        const control: any = { value: 'JOHN@SMITH.COM' };

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
    @EmailAddress() emailAddress: string;
}
