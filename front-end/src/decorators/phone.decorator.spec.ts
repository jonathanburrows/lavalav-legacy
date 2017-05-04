import { Phone } from './phone.decorator';
import { GetValidationRules } from './validation-factory';

describe(Phone.name, () => {
    it('is valid if not defined', () => {
        const person = new Person();
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is invalid if less than 7 digits', () => {
        const person = new Person();
        person.phoneNumber = 123456;
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is valid if 7 digits', () => {
        const person = new Person();
        person.phoneNumber = 1234567;
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid if more than 7 digits', () => {
        const person = new Person();
        person.phoneNumber = 12345678;
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });
});

class Person {
    @Phone() phoneNumber: number;
}
