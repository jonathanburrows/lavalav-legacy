import { RegularExpression } from './regular-expression.decorator';
import { GetValidationRules } from './validation-factory';

describe(RegularExpression.name, () => {
    it('is valid when value is null', () => {
        const person = new Person();
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is invalid when the pattern doesnt match', () => {
        const person = new Person();
        person.description = 'hello';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is valid when the pattern matches', () => {
        const person = new Person();
        person.description = 'hello, world!';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid when case insensitivity is detected', () => {
        const person = new Person();
        person.description = 'HELLO, WORLD!';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });
});

class Person {
    @RegularExpression(/^hello, world!$/i) description: string;
}