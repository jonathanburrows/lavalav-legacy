import 'reflect-metadata';

export const ValidationKey = '__validation';

/** Adds a validation function to the metadata of a property.
 *  @param metadataKey the name of the validation to be added.
 *  @param validationFunction predicate to determine if property value is valid.
 *  @param target the type of object which contains the property to be validating.
 *  @param propertyKey the name of the property to have the metadata assigned.
 */
export function DefineValidationMetadata(metadataKey: string, validationFunction: ValidationFunction, target: Object, propertyKey: string) {
    if (!metadataKey) {
        throw new Error('metadataKey has no value.');
    }
    if (!validationFunction) {
        throw new Error('validationFunction has no value.');
    }
    if (!target) {
        throw new Error('target has no value.');
    }
    if (!propertyKey) {
        throw new Error('propertyKey has no value.');
    }

    Reflect.defineMetadata(metadataKey, validationFunction, target, propertyKey);

    const validationRules: ValidationFunction[] = Reflect.getMetadata(ValidationKey, target, propertyKey) || [];
    validationRules.push(validationFunction);
    Reflect.defineMetadata(ValidationKey, validationRules, target, propertyKey);

    const parentValidationRules: ValidationFunction[] = Reflect.getMetadata(ValidationKey, target) || [];
    parentValidationRules.push(validationFunction);
    Reflect.defineMetadata(ValidationKey, validationRules, target);
}

export function GetValidationRules(target: Object): ValidationFunction[] {
    if (!target) {
        throw new Error('target is null');
    }

    return Reflect.getMetadata(ValidationKey, target) || [];
}

declare type ValidationFunction = (validating: Object) => boolean;
