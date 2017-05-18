import { Type } from '@angular/core';
import { ValidatorFn } from '@angular/forms';

import { CreditCard } from './credit-card.decorator';
import { ValidationKey } from './validation-factory';

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
            const validator = getValidator(Person, 'creditCardNumber');
            const control: any = { value: validCreditCard.number };

            const errors = validator(control);

            expect(errors).toBeNull();
        });
    }

    it('is false when not a known vendor', () => {
        const validator = getValidator(Person, 'creditCardNumber');
        const control: any = { value: 1111111111111111 };

        const errors = validator(control);

        expect(errors['creditCard']).toBeDefined();
    });

    it('is true when null', () => {
        const validator = getValidator(Person, 'creditCardNumber');
        const control: any = { value: null };

        const errors = validator(control);

        expect(errors).toBeNull();
    });

    function getValidator<T>(type: Type<T>, property: string): ValidatorFn {
        const validators = Reflect.getMetadata(ValidationKey, type.prototype, property);
        if (!validators) {
            throw new Error(`No validator for ${type.name}.${property}`);
        }

        return validators[0];
    }
});

class Person {
    @CreditCard() creditCardNumber: number;
}
