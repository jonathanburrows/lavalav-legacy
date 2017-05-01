import { Component, OnInit } from '@angular/core';

import { ConsentViewModel } from './consent-view-model';

declare const model: ConsentViewModel;

@Component({
    selector: 'lvl-oidc-consent',
    styleUrls: ['consent.component.scss'],
    templateUrl: 'consent.component.html'
})
export class ConsentComponent implements OnInit {
    public model: ConsentViewModel;

    ngOnInit() {
        if (typeof model === 'undefined') {
            throw new Error('There is no global variable named model.');
        }
        this.model = model;
    }
}
