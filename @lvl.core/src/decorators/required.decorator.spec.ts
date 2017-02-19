import { Required } from './required.decorator';
import { GetValidationRules } from './validation-factory';

describe(Required.name, () => {
    it('is invalid if value is undefined', () => {
        const person = new Person();
        person.firstName = undefined;
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is invalid if value is null', () => {
        const person = new Person();
        person.firstName = null;
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is invalid if value is empty string', () => {
        const person = new Person();
        person.firstName = '';
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is valid if value is zero', () => {
        const person = new Person();
        person.firstName = 0;
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid if value is false', () => {
        const person = new Person();
        person.firstName = false;
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid if non-empty string', () => {
        const person = new Person();
        person.firstName = 'hello, world!';
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });
});

class Person {
    @Required() firstName: any;
}
