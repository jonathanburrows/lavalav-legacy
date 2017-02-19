import { DefineValidationMetadata } from './validation-factory';

/** Specifies that a data field value in ASP.NET Dynamic Data must match the specified
 *  regular expression.
 *  @param pattern {RegExp} the pattern the property must adhere to.
 *  @throws pattern has been been assigned a value. */
export function RegularExpression(pattern: RegExp): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!pattern) {
            throw new Error(`@${RegularExpression.name} on ${target.constructor.name} did not define a pattern.`);
        }

        const isValid = (validating) => {
            const value = validating[propertyKey];

            if (!value) {
                return true;
            }
            return pattern.test(value);
        };

        DefineValidationMetadata(RegularExpression.name, isValid, target, propertyKey);
    };
}