import { EmailAddress } from './email-address.decorator';
import { GetValidationRules } from './validation-factory';

describe('EmailAddress', () => {
    it('is true when value is null', () => {
        const person = new Person();
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is false when no domain', () => {
        const person = new Person();
        person.emailAddress = 'john@smith';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is false when no host', () => {
        const person = new Person();
        person.emailAddress = 'john@.com';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is false when no address', () => {
        const person = new Person();
        person.emailAddress = 'smith.com';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is true when address, host, and domain are provided', () => {
        const person = new Person();
        person.emailAddress = 'john@smith.com';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is true when all caps', () => {
        const person = new Person();
        person.emailAddress = 'JOHN@SMITH.COM';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });
});

class Person {
    @EmailAddress()
    emailAddress: string;
}