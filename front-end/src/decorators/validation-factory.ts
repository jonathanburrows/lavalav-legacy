import { ValidatorFn } from '@angular/forms';

export const ValidationKey = '__validation';

/** Adds a validation function to the metadata of a property.
 *  @param metadataKey the name of the validation to be added.
 *  @param validatorFn predicate to determine if property value is valid.
 *  @param target the type of object which contains the property to be validating.
 *  @param propertyKey the name of the property to have the metadata assigned.
 */
export function DefineValidationMetadata(metadataKey: string, validatorFn: ValidatorFn, target: Object, propertyKey: string) {
    if (!metadataKey) {
        throw new Error('metadataKey has no value.');
    }
    if (!validatorFn) {
        throw new Error('validationFunction has no value.');
    }
    if (!target) {
        throw new Error('target has no value.');
    }
    if (!propertyKey) {
        throw new Error('propertyKey has no value.');
    }

    const validationRules: ValidatorFn[] = Reflect.getMetadata(ValidationKey, target, propertyKey) || [];
    validationRules.push(validatorFn);
    Reflect.defineMetadata(ValidationKey, validationRules, target, propertyKey);
}
