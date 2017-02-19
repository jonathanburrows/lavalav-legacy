import { Range } from './range.decorator';
import { GetValidationRules } from './validation-factory';

describe(Range.name, () => {
    it('is valid if not defined', () => {
        const person = new Person();
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is invalid if value is smaller than minimum', () => {
        const person = new Person();
        person.income = -1;
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is valid if value is equal to minimum', () => {
        const person = new Person();
        person.income = 0;
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid if value is larger than minimum and smaller than maximum', () => {
        const person = new Person();
        person.income = 1;
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid if value is equal to maximum', () => {
        const person = new Person();
        person.income = 1000000;
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });
});

class Person {
    @Range(0, 1000000) income: number;
}
