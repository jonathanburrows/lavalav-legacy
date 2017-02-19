import { DefineValidationMetadata } from './validation-factory';

/**
 * Specifies the numeric range constraints for the value of a data field.
 */
export function Range(minimum: number, maximum: number): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!minimum && isNaN(minimum)) {
            throw new Error(`@${Range.name} on ${target.constructor.name} has an undefined minimum.`);
        }
        if (!maximum && isNaN(maximum)) {
            throw new Error(`@${Range.name} on ${target.constructor.name} has an undefined maximum.`);
        }
        if (minimum === maximum) {
            throw new Error(`@${Range.name} on ${target.constructor.name} has an identical min and max value of ${minimum}`);
        }
        if (minimum > maximum) {
            throw new Error(`@${Range.name} on ${target.constructor.name} has a minimum ${minimum} which is larger than the maximum ${maximum}`);
        }

        const isValid = (validating) => {
            const value = validating[propertyKey];

            if (!value) {
                return true;
            }

            return value >= minimum && value <= maximum;
        };

        DefineValidationMetadata(Range.name, isValid, target, propertyKey);
    };
}