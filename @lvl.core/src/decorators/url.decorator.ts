import { DefineValidationMetadata } from './validation-factory';

/** Provides URL validation. */
export function Url(): PropertyDecorator {
    return (target: Object, propertyKey: string) => {
        const isValid = (validating) => {
            const urlRegex = /^(ftp|http|https):\/\/[^ "]+$/i;
            const value = validating[propertyKey];

            if (!value) {
                return true;
            }
            return urlRegex.test(value);
        };

        DefineValidationMetadata(Url.name, isValid, target, propertyKey);
    };
}