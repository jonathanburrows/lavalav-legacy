import { Type } from '@angular/core';
import { ValidatorFn } from '@angular/forms';

import { Range } from './range.decorator';
import { ValidationKey } from './validation-factory';

describe(Range.name, () => {
    it('is valid if not defined', () => {
        const validator = getValidator(Person, 'income');
        const control: any = { value: null };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid if value is smaller than minimum', () => {
        const validator = getValidator(Person, 'income');
        const control: any = { value: -1 };

        const errors = validator(control);

        expect(errors['range']).toBeDefined();
    });

    it('is valid if value is equal to minimum', () => {
        const validator = getValidator(Person, 'income');
        const control: any = { value: 0 };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid if value is larger than minimum and smaller than maximum', () => {
        const validator = getValidator(Person, 'income');
        const control: any = { value: 1 };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid if value is equal to maximum', () => {
        const validator = getValidator(Person, 'income');
        const control: any = { value: 1000000 };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid if value is larger than the maximum', () => {
        const validator = getValidator(Person, 'income');
        const control: any = { value: 1000001 };

        const errors = validator(control);

        expect(errors['range']).toBeDefined();
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
    @Range(0, 1000000) income: number;
}
