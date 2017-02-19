import { DefineValidationMetadata } from './validation-factory';

/**
 * Validates an email address.
 */
export function Phone(): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const isValid = (validating) => {
            const value = validating[propertyKey];

            if (!value) {
                return true;
            }

            const length = value.toString().length;
            if (length < 7) {
                return false;
            }
            else {
                return true;
            }
        };

        DefineValidationMetadata(Phone.name, isValid, target, propertyKey);
    };
}