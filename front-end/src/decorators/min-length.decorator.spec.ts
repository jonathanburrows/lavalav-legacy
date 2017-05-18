import { Type } from '@angular/core';
import { ValidatorFn } from '@angular/forms';

import { MinLength } from './min-length.decorator';
import { ValidationKey } from './validation-factory';

describe(MinLength.name, () => {
    it('is valid if value is null', () => {
        const validator = getValidator(Person, 'fullName');
        const control: any = { value: null };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid if value is less than MinLength', () => {
        const validator = getValidator(Person, 'fullName');
        const control: any = { value: '12' };

        const errors = validator(control);

        expect(errors['minLength']).toBeDefined();
    });

    it('is valid if value is equal to MinLength', () => {
        const validator = getValidator(Person, 'fullName');
        const control: any = { value: '123' };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid if value is larger than MinLength', () => {
        const validator = getValidator(Person, 'fullName');
        const control: any = { value: '1234' };

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
    @MinLength(3) fullName: string;
}
