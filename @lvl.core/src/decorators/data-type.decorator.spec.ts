import { DataType } from './data-type.decorator';
import { GetValidationRules } from './validation-factory';

describe(DataType.name, () => {
    it('is valid if property is null', () => {
        const dateContainer = new DateContainer();
        const rules = GetValidationRules(dateContainer);

        const isValid = rules.every(rule => rule(dateContainer));

        expect(isValid).toBe(true);
    });

    it('is invalid if date is a number', () => {
        const dateContainer = new DateContainer();
        dateContainer.date = 3;
        const rules = GetValidationRules(dateContainer);

        const isValid = rules.every(rule => rule(dateContainer));

        expect(isValid).toBe(false);
    });

    it('is valid if date is a date', () => {
        const dateContainer = new DateContainer();
        dateContainer.date = new Date();
        const rules = GetValidationRules(dateContainer);

        const isValid = rules.every(rule => rule(dateContainer));

        expect(isValid).toBe(true);
    });

    it('is invalid if number is a boolean', () => {
        const numberContainer = new NumberContainer();
        numberContainer.number = true;
        const rules = GetValidationRules(numberContainer);

        const isValid = rules.every(rule => rule(numberContainer));

        expect(isValid).toBe(false);
    });

    it('is valid if number is primative number', () => {
        const numberContainer = new NumberContainer();
        numberContainer.number = 5;
        const rules = GetValidationRules(numberContainer);

        const isValid = rules.every(rule => rule(numberContainer));

        expect(isValid).toBe(true);
    });

    it('is valid if number is object number', () => {
        const numberContainer = new NumberContainer();
        // tslint:disable-next-line
        numberContainer.number = new Number(5);
        const rules = GetValidationRules(numberContainer);

        const isValid = rules.every(rule => rule(numberContainer));

        expect(isValid).toBe(true);
    });

    it('is invalid if boolean is string', () => {
        const booleanContainer = new BooleanContainer();
        booleanContainer.boolean = 'hello';
        const rules = GetValidationRules(booleanContainer);

        const isValid = rules.every(rule => rule(booleanContainer));

        expect(isValid).toBe(false);
    });

    it('is valid if boolean is primative true', () => {
        const booleanContainer = new BooleanContainer();
        booleanContainer.boolean = true;
        const rules = GetValidationRules(booleanContainer);

        const isValid = rules.every(rule => rule(booleanContainer));

        expect(isValid).toBe(true);
    });

    it('is valid if boolean is primative false', () => {
        const booleanContainer = new BooleanContainer();
        booleanContainer.boolean = false;
        const rules = GetValidationRules(booleanContainer);

        const isValid = rules.every(rule => rule(booleanContainer));

        expect(isValid).toBe(true);
    });

    it('is invalid if boolean is true object', () => {
        const booleanContainer = new BooleanContainer();
        // tslint:disable-next-line
        booleanContainer.boolean = new Boolean(true);
        const rules = GetValidationRules(booleanContainer);

        const isValid = rules.every(rule => rule(booleanContainer));

        expect(isValid).toBe(true);
    });

    it('is invalid if boolean is false object', () => {
        const booleanContainer = new BooleanContainer();
        // tslint:disable-next-line
        booleanContainer.boolean = new Boolean(false);
        const rules = GetValidationRules(booleanContainer);

        const isValid = rules.every(rule => rule(booleanContainer));

        expect(isValid).toBe(true);
    });

    it('is invalid if complex type is not correct', () => {
        const parent = new Parent();
        parent.child = 1;
        const rules = GetValidationRules(parent);

        const isValid = rules.every(rule => rule(parent));

        expect(isValid).toBe(false);
    });

    it('is valid if complex type is correct', () => {
        const parent = new Parent();
        parent.child = new Child();
        const rules = GetValidationRules(parent);

        const isValid = rules.every(rule => rule(parent));

        expect(isValid).toBe(true);
    });
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
