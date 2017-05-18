import { Type } from '@angular/core';
import { ValidatorFn } from '@angular/forms';

import { Required } from './required.decorator';
import { ValidationKey } from './validation-factory';

describe(Required.name, () => {
    it('is invalid if value is undefined', () => {
        const validator = getValidator(Person, 'firstName');
        const control: any = { value: undefined };

        const errors = validator(control);

        expect(errors['required']).toBeDefined();
    });

    it('is invalid if value is null', () => {
        const validator = getValidator(Person, 'firstName');
        const control: any = { value: null };

        const errors = validator(control);

        expect(errors['required']).toBeDefined();
    });

    it('is invalid if value is empty string', () => {
        const validator = getValidator(Person, 'firstName');
        const control: any = { value: '' };

        const errors = validator(control);

        expect(errors['required']).toBeDefined();
    });

    it('is valid if value is zero', () => {
        const validator = getValidator(Person, 'firstName');
        const control: any = { value: 0 };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid if value is false', () => {
        const validator = getValidator(Person, 'firstName');
        const control: any = { value: false };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid if non-empty string', () => {
        const validator = getValidator(Person, 'firstName');
        const control: any = { value: 'hello, world!' };

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
    @Required() firstName: any;
}
