import { CreditCard } from './credit-card.decorator';
import { GetValidationRules } from './validation-factory';

describe('CreditCard', () => {
    const validCreditCards: { number: number, vendor: string }[] = [
        { number: 378282246310005, vendor: 'American Express' },
        { number: 371449635398431, vendor: 'American Express' },
        { number: 378734493671000, vendor: 'American Express Corporate' },
        { number: 30569309025904, vendor: 'Diners Club' },
        { number: 38520000023237, vendor: 'Diners Club' },
        { number: 6011111111111117, vendor: 'Discover' },
        { number: 6011000990139424, vendor: 'Discover' },
        { number: 3530111333300000, vendor: 'JCB' },
        { number: 3566002020360505, vendor: 'JCB' },
        { number: 3566002020360505, vendor: 'MasterCard' },
        { number: 5105105105105100, vendor: 'MasterCard' },
        { number: 4111111111111111, vendor: 'Visa' },
        { number: 4012888888881881, vendor: 'Visa' },
        { number: 4222222222222, vendor: 'Visa' }
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
    @CreditCard() creditCardNumber: number;
}
