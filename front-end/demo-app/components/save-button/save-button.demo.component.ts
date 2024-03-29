﻿import { Component, OnInit, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Rx';

import {
    Navigatable,
    Required,
    ValidationBuilder,
    ValidatableForm
} from '../../../src';

@Component({
    selector: 'lvl-demo-save-button',
    templateUrl: 'save-button.demo.component.html'
})
@Navigatable({
    group: 'Front End',
    title: 'Save Button',
    icon: 'content_copy'
})
export class SaveButtonDemoComponent implements OnInit {
    model: SaveButtonModel;
    form: ValidatableForm<SaveButtonModel>;

    constructor(private validationBuilder: ValidationBuilder) { }

    ngOnInit() {
        this.model = new SaveButtonModel();
        this.form = this.validationBuilder.formFor(this.model, ['name', 'type']);
    }

    save() {
        this.form.saveAsync(() => <any>Observable.timer(2000));
    }
}

class SaveButtonModel {
    @Required() name: string;
    @Required() type: string;
}
