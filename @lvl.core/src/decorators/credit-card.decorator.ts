import { DefineValidationMetadata } from './validation-factory';

/**
 * Specifies that a data field value is a credit card number.
 */
export function CreditCard(): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const isValid = (validating) => {
            const creditCardRegex = /^(?:(4[0-9]{12}(?:[0-9]{3})?)|(5[1-5][0-9]{14})|(6(?:011|5[0-9]{2})[0-9]{12})|(3[47][0-9]{13})|(3(?:0[0-5]|[68][0-9])[0-9]{11})|((?:2131|1800|35[0-9]{3})[0-9]{11}))$/
            const value = validating[propertyKey];

            if (!value) {
                return true;
            }
            return creditCardRegex.test(value);
        };

        DefineValidationMetadata(CreditCard.name, isValid, target, propertyKey);
    };
}