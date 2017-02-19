import { DefineValidationMetadata } from './validation-factory';

/** Checks if a value has been assigned to a property. */
export function Required(): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const isValid = (validating) => {
            const value = validating[propertyKey];

            if (typeof value === 'undefined') {
                return false;
            }
            else if (value == null) {
                return false;
            }
            else if (value === '') {
                return false;
            }
            else {
                return true;
            }
        };

        DefineValidationMetadata(Required.name, isValid, target, propertyKey);
    };
}