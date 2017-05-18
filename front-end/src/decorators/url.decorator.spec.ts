import { Type } from '@angular/core';
import { ValidatorFn } from '@angular/forms';

import { Url } from './url.decorator';
import { ValidationKey } from './validation-factory';

describe(Url.name, () => {
    it('is valid when the url has no value', () => {
        const validator = getValidator(Person, 'url');
        const control: any = { value: null };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid when missing protocol', () => {
        const validator = getValidator(Person, 'url');
        const control: any = { value: 'google.com' };

        const errors = validator(control);

        expect(errors['url']).toBeDefined();
    });

    it('is valid when given http protocol', () => {
        const validator = getValidator(Person, 'url');
        const control: any = { value: 'http://google.com' };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid when given https protocol', () => {
        const validator = getValidator(Person, 'url');
        const control: any = { value: 'https://google.com' };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid when given ftp protocol', () => {
        const validator = getValidator(Person, 'url');
        const control: any = { value: 'ftp://google.com' };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid when protocol is all uppercase', () => {
        const validator = getValidator(Person, 'url');
        const control: any = { value: 'HTTP://google.com' };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid when using an unknown protocol', () => {
        const validator = getValidator(Person, 'url');
        const control: any = { value: 'hggp://google.com' };

        const errors = validator(control);

        expect(errors['url']).toBeDefined();
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
    @Url() url: string;
}
