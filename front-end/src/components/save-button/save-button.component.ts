import {
    Component,
    OnInit,
    ElementRef,
    Output
} from '@angular/core';
import { FormGroupDirective } from '@angular/forms';

import { ValidatableForm } from '../../services';

/**
 *  Provide a save button that has different states, corresponding to a form.
 */
@Component({
    selector: 'lvl-save-button',
    templateUrl: 'save-button.component.html',
    styleUrls: ['save-button.component.scss'],
    host: {
        '[class.lvl-invalid-caused-by-user]': 'isInvalidCausedByUser',
        '[class.lvl-save-occurring]': 'saveOccurring',
        '[class.lvl-save-completed]': 'saveCompleted'
    }
})
export class SaveButtonComponent implements OnInit {
    /** Ensures that validation messages only show if an input is invalid from the user's actions. */
    get isInvalidCausedByUser(): boolean {
        if (this.parentFormGroup.valid) {
            return false;
        } else if (this.parentFormGroup.submitted) {
            return true;
        } else {
            const form = this.parentFormGroup.form;
            const inputs = Object.keys(form.controls).map(controlName => form.get(controlName));
            const invalidInputs = inputs.filter(i => i.status === 'INVALID');
            return invalidInputs.some(i => i.touched);
        }
    }

    /** Indicates the server is being contacted to update the form. */
    @Output() saveOccuring: boolean;

    /* Indicates the update action has been completed. */
    @Output() saveCompleted: boolean;

    constructor(private elementRef: ElementRef, private parentFormGroup: FormGroupDirective) { }

    ngOnInit() {
        if (!this.parentFormGroup) {
            throw new Error('[formGroup] has not been applied correctly on <lvl-save-button>');
        }

        if (this.parentFormGroup.form instanceof ValidatableForm) {
            // The saving/complete icons depend on events from the ValidatableForm class.
            const validatableForm = <ValidatableForm<any>>this.parentFormGroup.form;

            validatableForm.saveStarted.subscribe(_ => {
                // was dont manually since angular is not triggering event.
                this.elementRef.nativeElement.classList.add('lvl-save-occurring');
                this.saveCompleted = false;
                this.saveOccuring = true;
            });
            validatableForm.saveCompleted.subscribe(_ => {
                this.saveOccuring = false;
                this.saveCompleted = true;
                this.elementRef.nativeElement.classList.remove('lvl-save-occurring');
            });
        }
    }
}
