import { DefineValidationMetadata } from './validation-factory';

/** Provides an attribute that compares two properties.
 *  @param otherProperty Gets the property to compare with the current property.
 *  @throws otherProperty was not given a value.
 */
export function Compare(otherProperty: string): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!otherProperty) {
            throw new Error(`@${Compare.name} on ${target.constructor.name}.${propertyKey} has an undefined otherProperty.`);
        }

        const isValid = (validating) => validating[propertyKey] === validating[otherProperty];

        DefineValidationMetadata(Compare.name, isValid, target, propertyKey);
    }
}