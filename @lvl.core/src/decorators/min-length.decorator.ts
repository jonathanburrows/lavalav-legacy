import { DefineValidationMetadata } from './validation-factory';

/**
 * Specifies the minimum length of array or string data allowed in a property.
 */
export function MinLength(length: number): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!length) {
            throw new Error(`@${MinLength.name} on ${target.constructor.name} has an undefined length.`);
        }
        if (length < 0) {
            throw new Error(`@${MinLength.name} on ${target.constructor.name} has a negative number.`);
        }

        const isValid = (validating) => {
            const value = validating[propertyKey];

            if (!value) {
                return true;
            }

            return value.toString().length >= length;
        };

        DefineValidationMetadata(MinLength.name, isValid, target, propertyKey);
    };
}