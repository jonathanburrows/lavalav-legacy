import { CreditCard } from './credit-card.decorator';
import { GetValidationRules } from './validation-factory';

describe('CreditCard', () => {
    const validCreditCards: { number: number, vendor: string }[] = [
        { number: 378282246310005, vendor: 'American Express' },
        { number: 5555555555554444, vendor: 'MasterCard' },
        { number: 4111111111111111, vendor: 'Visa' },
    ];

    for (const validCreditCard of validCreditCards) {
        it(`is true when valid ${validCreditCard.vendor}`, () => {
            const person = new Person();
            person.creditCardNumber = validCreditCard.number;
            const validationRules = GetValidationRules(person);

            const isValid = validationRules.every(rule => rule(person));

            expect(isValid).toBe(true);
        });
    }

    it('is false when not a known vendor', () => {
        const person = new Person();
        person.creditCardNumber = 1111111111111111;
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(false);
    });

    it('is true when null', () => {
        const person = new Person();
        const validationRules = GetValidationRules(person);

        const isValid = validationRules.every(rule => rule(person));

        expect(isValid).toBe(true);
    });
});

class Person {
    @CreditCard()
    creditCardNumber: number;
}