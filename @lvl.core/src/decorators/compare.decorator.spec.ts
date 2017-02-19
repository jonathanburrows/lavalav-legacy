import { Compare } from './compare.decorator';
import { GetValidationRules } from './validation-factory';

describe('Compare', () => {
    it('is true when both are null', () => {
        const person = new Person();

        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));
        expect(isValid).toBe(true);
    });

    it('is false when one is null and the other has a value', () => {
        const person = new Person();
        person.name = 'Smith';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is true when both have matching values', () => {
        const person = new Person();
        const name = 'Smith';
        person.name = name;
        person.nickname = name;
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is true when both have matching values', () => {
        const person = new Person();
        person.name = 'Smith';
        person.nickname = 'Johnson';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });
});

class Person {
    @Compare('nickname') name: string;
    nickname: string;
}