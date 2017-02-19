import { Url } from './url.decorator';
import { GetValidationRules } from './validation-factory';

describe(Url.name, () => {
    it('is valid when the url has no value', () => {
        const person = new Person();
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is invalid when missing protocol', () => {
        const person = new Person();
        person.url = 'google.com';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is valid when given http protocol', () => {
        const person = new Person();
        person.url = 'http://google.com';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid when given https protocol', () => {
        const person = new Person();
        person.url = 'https://google.com';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid when given ftp protocol', () => {
        const person = new Person();
        person.url = 'ftp://google.com';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is valid when protocol is all uppercase', () => {
        const person = new Person();
        person.url = 'HTTP://google.com';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });

    it('is invalid when using an unknown protocol', () => {
        const person = new Person();
        person.url = 'hggp://google.com';
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });
});

class Person {
    @Url() url: string;
}