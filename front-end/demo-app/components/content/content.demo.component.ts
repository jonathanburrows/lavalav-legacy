import { Component } from '@angular/core';

import { Navigatable } from '../../../src';

@Component({
    selector: 'lvl-demo-content-component',
    templateUrl: 'content.demo.component.html'
})
@Navigatable({
    group: 'Front End',
    title: 'Content',
    icon: 'content_copy'
})
export class ContentDemoComponent { }
