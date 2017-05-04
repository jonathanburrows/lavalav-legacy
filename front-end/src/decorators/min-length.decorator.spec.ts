import { MinLength } from './min-length.decorator';
import { GetValidationRules } from './validation-factory';

describe(MinLength.name, () => {
    it('is valid if value is null', () => {
        const person = new Person();
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is invalid if value is less than MinLength', () => {
        const person = new Person();
        person.fullName = '12';
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is valid if value is equal to MinLength', () => {
        const person = new Person();
        person.fullName = '123';
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid if value is larger than MinLength', () => {
        const person = new Person();
        person.fullName = '1234';
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });
});

class Person {
    @MinLength(3) fullName: string;
}
