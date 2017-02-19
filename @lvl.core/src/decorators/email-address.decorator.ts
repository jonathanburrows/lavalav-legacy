import { DefineValidationMetadata } from './validation-factory';

/**
 * Validates an email address.
 */
export function EmailAddress(): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const isValid = (validating) => {
            const emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
            const value = validating[propertyKey];

            if (!value) {
                return true;
            }
            return emailRegex.test(value);
        };

        DefineValidationMetadata(EmailAddress.name, isValid, target, propertyKey);
    };
}