import { FormBuilder, FormGroup } from '@angular/forms';
import { ValidationBuilder } from './validation-builder.service';
import { MaxLength, MinLength } from '../../decorators';

describe(ValidationBuilder.name, () => {
    const formBuilder = new FormBuilder();
    const validationBuilder = new ValidationBuilder(formBuilder);

    describe('formFor', () => {
        it('will throw an error if parent is null', () => {
            expect(() => validationBuilder.formFor(null, ['property'])).toThrowError();
        });

        it('will throw an error if parent is an object literal', () => {
            expect(() => validationBuilder.formFor({}, ['property'])).toThrowError();
        });

        it('will throw an error if properties is null', () => {
            expect(() => validationBuilder.formFor(new BlankClass(), null)).toThrowError();
        });

        it('will throw an error if the same property is specified twhice', () => {
            expect(() => validationBuilder.formFor(new BlankClass(), ['property', 'property'])).toThrowError();
        });

        it('will return a form with a control for each property', () => {
            const formGroup = validationBuilder.formFor(new DoublePropertiesNoValidation(), ['firstProperty', 'secondProperty']);
            const controls = getControls(formGroup);

            expect(controls.length).toBe(2);
        });

        class DoublePropertiesNoValidation {
            firstProperty: string;
            secondProperty: string;
        }

        it('will populate a control with no validators', () => {
            const formGroup = validationBuilder.formFor(new SinglePropertyNoValidation(), ['onlyProperty']);
            const controls = getControls(formGroup);

            expect(controls.length).toBe(1);
        });

        class SinglePropertyNoValidation {
            onlyProperty: string;
        }

        it('will populate a control with 1 validator', () => {
            const formGroup = validationBuilder.formFor(new SinglePropertySingleValidator(), ['property']);
            const control = formGroup.controls['property'];

            expect(control.validator.length).toBe(1);
        });

        class SinglePropertySingleValidator {
            @MaxLength(2) property: string;
        }

        it('will populate a control with 2 validators', () => {
            const formGroup = validationBuilder.formFor(new SinglePropertyDoubleValidator(), ['property']);
            const control = formGroup.controls['property'];

            expect(control.validator.length).toBe(1);
        });

        class SinglePropertyDoubleValidator {
            @MinLength(1) @MaxLength(5) property: string;
        }

        it('will populate two control with different validators', () => {
            const formGroup = validationBuilder.formFor(new DoublePropertyDifferentValidators(), ['firstProperty', 'secondProperty']);
            const firstControl = formGroup.controls['firstProperty'];
            const secondControl = formGroup.controls['secondProperty'];

            expect(firstControl.validator).not.toEqual(secondControl.validator);
        });

        class DoublePropertyDifferentValidators {
            @MaxLength(1) firstProperty: string;
            @MinLength(2) secondProperty: string;
        }

        it('will populate the controls value', () => {
            const model = new SinglePropertyNoValidation();
            model.onlyProperty = 'hello, world!';

            const form = validationBuilder.formFor(model, ['onlyProperty']);

            const control = form.controls['onlyProperty'];
            expect(control.value).toBe(model.onlyProperty);
        });
    });

    describe('controlFor', () => {
        it('will throw an error if parent is null', () => {
            expect(() => validationBuilder.controlFor(null, 'property')).toThrowError();
        });

        it('will throw an error if parent is an object literal', () => {
            expect(() => validationBuilder.controlFor({}, 'property')).toThrowError();
        });

        it('will throw an error if property is null', () => {
            expect(() => validationBuilder.controlFor(new BlankClass(), null)).toThrowError();
        });

        it('will return a control if there is no validators', () => {
            const control = validationBuilder.controlFor(new SinglePropertyNoValidator(), 'property');

            expect(control.validator).toBeNull();
        });

        class SinglePropertyNoValidator {
            property: string;
        }

        it('will return a control if there is 1 validator', () => {
            const control = validationBuilder.controlFor(new SinglePropertySingleValidator(), 'property');

            expect(control.validator.length).toBe(1);
        });

        class SinglePropertySingleValidator {
            @MaxLength(10) property: string;
        }

        it('will return a control if there are 2 validators', () => {
            const control = validationBuilder.controlFor(new SinglePropertyDoubleValidator(), 'property');

            expect(control.validator.length).toBe(1);
        });

        class SinglePropertyDoubleValidator {
            @MinLength(1) @MaxLength(10) property: string;
        }

        it('will populate the controls value', () => {
            const model = new SinglePropertyNoValidator();
            model.property = 'hello, world!';
            const control = validationBuilder.controlFor(model, 'property');

            expect(control.value).toBe(model.property);
        });
    });

    describe('getValidationMessages', () => {
        it('will throw an error if the form group is null', () => {
            expect(() => validationBuilder.getErrors(null)).toThrowError();
        });

        it('will return an empty object if all inputs are valid', () => {
            const form = validationBuilder.formFor(new ValidByDefault(), ['property']);
            form.controls['property'].markAsDirty();

            const errorMessages = validationBuilder.getErrors(form);

            const flattenedErrors = flattenErrorMessages(errorMessages);
            expect(flattenedErrors.length).toBe(0);
        });

        class ValidByDefault {
            @MaxLength(1) property: string;
        }

        it('will return a single property with an array of 1 if there is a single error', () => {
            const form = validationBuilder.formFor(new OneErrorByDefault(), ['property']);
            form.controls['property'].markAsDirty();

            const errorMessages = validationBuilder.getErrors(form);

            const flattenedErrors = flattenErrorMessages(errorMessages);
            expect(flattenedErrors.length).toBe(1);
        });

        class OneErrorByDefault {
            @MinLength(2) property = '1';
        }

        it('will return a single property with an array of 2 if there is a single property with 2 errors', () => {
            const form = validationBuilder.formFor(new SinglePropertyTwoErrorsByDefault(), ['property']);
            form.controls['property'].markAsDirty();

            const errorMessages = validationBuilder.getErrors(form);

            const flattenedErrors = flattenErrorMessages(errorMessages);
            expect(flattenedErrors.length).toBe(1);
        });

        class SinglePropertyTwoErrorsByDefault {
            @MinLength(2) @MinLength(3) property = '1';
        }

        it('will return 2 properties with an array of 1 each if there are two properties with a single error each', () => {
            const form = validationBuilder.formFor(new TwoPropertiesTwoErrorsByDefault(), ['firstProperty', 'secondProperty']);
            form.controls['firstProperty'].markAsDirty();
            form.controls['secondProperty'].markAsDirty();

            const errorMessages = validationBuilder.getErrors(form);

            const flattenedErrors = flattenErrorMessages(errorMessages);
            expect(flattenedErrors.length).toBe(2);
        });

        class TwoPropertiesTwoErrorsByDefault {
            @MinLength(2) firstProperty = '1';
            @MinLength(3) secondProperty = '1';
        }
    });

    class BlankClass { }

    function getControls(formGroup: FormGroup) {
        const controls = [];

        for (const key in formGroup.controls) {
            if (key) {
                const control = formGroup.get(key);
                if (control) {
                    controls.push(control);
                }
            }
        }

        return controls;
    }

    function flattenErrorMessages(errorMessages: { [property: string]: any }): any[] {
        const errors = [];

        for (const property in errorMessages) {
            if (errorMessages[property]) {
                errors.push(errorMessages[property]);
            }
        }

        return errors;
    }
});
