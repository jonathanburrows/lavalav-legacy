import { Type } from '@angular/core';
import { ValidatorFn } from '@angular/forms';

import { DataType } from './data-type.decorator';
import { ValidationKey } from './validation-factory';

describe(DataType.name, () => {
    it('is valid if property is null', () => {
        const validator = getValidator(DateContainer, 'date');
        const control: any = { value: null };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid if date is a number', () => {
        const validator = getValidator(DateContainer, 'date');
        const control: any = { value: 3 };

        const errors = validator(control);

        expect(errors['dataType']).toBeDefined();
    });

    it('is valid if date is a date', () => {
        const validator = getValidator(DateContainer, 'date');
        const control: any = { value: new Date() };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid if number is a boolean', () => {
        const validator = getValidator(NumberContainer, 'number');
        const control: any = { value: true };

        const errors = validator(control);

        expect(errors['dataType']).toBeDefined();
    });

    it('is valid if number is primative number', () => {
        const validator = getValidator(NumberContainer, 'number');
        const control: any = { value: 5 };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid if number is object number', () => {
        const validator = getValidator(NumberContainer, 'number');
        // tslint:disable-next-line
        const control: any = { value: new Number(5) };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid if boolean is string', () => {
        const validator = getValidator(BooleanContainer, 'boolean');
        const control: any = { value: 'hello' };

        const errors = validator(control);

        expect(errors['dataType']).toBeDefined();
    });

    it('is valid if boolean is primative true', () => {
        const validator = getValidator(BooleanContainer, 'boolean');
        const control: any = { value: true };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid if boolean is primative false', () => {
        const validator = getValidator(BooleanContainer, 'boolean');
        const control: any = { value: false };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is valid if boolean is true object', () => {
        const validator = getValidator(BooleanContainer, 'boolean');
        // tslint:disable-next-line
        const control: any = { value: new Boolean(true) };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid if boolean is false object', () => {
        const validator = getValidator(BooleanContainer, 'boolean');
        // tslint:disable-next-line
        const control: any = { value: new Boolean(false) };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    it('is invalid if complex type is not correct', () => {
        const validator = getValidator(Parent, 'child');
        const control: any = { value: 1 };

        const errors = validator(control);

        expect(errors['dataType']).toBeDefined();
    });

    it('is valid if complex type is correct', () => {
        const validator = getValidator(Parent, 'child');
        const control: any = { value: new Child() };

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

class DateContainer {
    @DataType(Date) date: any;
}

class NumberContainer {
    @DataType(Number) number: any;
}

class BooleanContainer {
    @DataType(Boolean) boolean: any;
}

class Child {}

class Parent {
    @DataType(Child) child: any;
}
