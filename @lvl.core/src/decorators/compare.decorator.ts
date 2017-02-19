import { DefineValidationMetadata } from './validation-factory';

/** Provides an attribute that compares two properties.
 *  @param otherProperty Gets the property to compare with the current property.
 */
export function Compare(otherProperty: string): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const isValid = (validating) => validating[propertyKey] === validating[otherProperty];

        DefineValidationMetadata(Compare.name, isValid, target, propertyKey);
    }
}