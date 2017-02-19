import { DefineValidationMetadata } from './validation-factory';

/**
 * Specifies the maximum length of array or string data allowed in a property.
 */
export function MaxLength(length: number): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!length) {
            throw new Error(`@${MaxLength.name} on ${target.constructor.name} has an undefined length.`);
        }
        if (length < 0) {
            throw new Error(`@${MaxLength.name} on ${target.constructor.name} has a negative number.`);
        }

        const isValid = (validating) => {
            const value = validating[propertyKey];

            if (!value) {
                return true;
            }

            return value.toString().length <= length;
        };

        DefineValidationMetadata(MaxLength.name, isValid, target, propertyKey);
    };
}