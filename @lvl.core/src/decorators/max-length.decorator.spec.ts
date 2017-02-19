import { MaxLength } from './max-length.decorator';
import { GetValidationRules } from './validation-factory';

describe('MaxLength', () => {
    it('is valid if value is null', () => {
        const person = new Person();
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid if value is less than MaxLength', () => {
        const person = new Person();
        person.fullName = '12';
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(true);
    }); 

    it('is valid if value is equal to MaxLength', () => {
        const person = new Person();
        person.fullName = '123';
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is invalid if value is larger than MaxLength', () => {
        const person = new Person();
        person.fullName = '1234';
        const rules = GetValidationRules(person);

        const isValid = rules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });
});

class Person {
    @MaxLength(3)
    fullName: string;
}