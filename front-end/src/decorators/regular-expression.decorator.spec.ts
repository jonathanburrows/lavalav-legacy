import { Type } from '@angular/core';
import { ValidatorFn } from '@angular/forms';

import { RegularExpression } from './regular-expression.decorator';
import { ValidationKey } from './validation-factory';

describe(RegularExpression.name, () => {
    it('is valid when value is null', () => {
        const validator = getValidator(Person, 'description');
        const control: any = { value: null };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid when the pattern doesnt match', () => {
        const validator = getValidator(Person, 'description');
        const control: any = { value: 'hello' };

        const errors = validator(control);

        expect(errors['regularExpression']).toBeDefined();
    });

    it('is valid when the pattern matches', () => {
        const validator = getValidator(Person, 'description');
        const control: any = { value: 'hello, world!' };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid when case insensitivity is detected', () => {
        const validator = getValidator(Person, 'description');
        const control: any = { value: 'HELLO, WORLD!' };

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
    @RegularExpression(/^hello, world!$/i) description: string;
}
