import { Type } from '@angular/core';
import { AbstractControl, ValidatorFn } from '@angular/forms';

import { DefineValidationMetadata } from './validation-factory';

/** Specifies the maximum length of array or string data allowed in a property.
 *  @param dataType {Function} the type which the property value must be. */
export function DataType(dataType: Function): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        if (!dataType) {
            throw new Error(`@${DataType.name} on ${target.constructor.name}.${propertyKey} has no value.`);
        }

        const validator = dataTypeValidator(dataType);
        DefineValidationMetadata(DataType.name, validator, target, propertyKey);
    };
}

function dataTypeValidator(dataType: Function): ValidatorFn {
    return (control: AbstractControl): { [name: string]: any } => {
        if (!control) {
            throw new Error('Control is null.');
        }

        if (!isValidDataType(control.value, dataType)) {
            return {
                'dataType': `isn't a ${dataType.name}`
            };
        } else {
            return null;
        }
    };
}

function isValidDataType(value: any, dataType: Function) {
    if (!value) {
        return true;
    }

    // Need to make sure numbers primative types are considered
    if (dataType === Number) {
        if (value === true || value === false) {
            return false;
        }
        if (value instanceof Date) {
            return false;
        }
        if (value instanceof dataType) {
            return true;
        }
        return !isNaN(value);
    }

    // Need to make sure boolean primatives are considered
    if (dataType === Boolean) {
        if (value instanceof dataType) {
            return true;
        }
        if (value === true) {
            return true;
        }
        if (value === false) {
            return true;
        }
        return false;
    }

    return value instanceof dataType;
}
