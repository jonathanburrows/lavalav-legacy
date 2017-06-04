import { Component } from '@angular/core';

import { Navigatable } from '../../../src';

/**
 * No template is given, as layout is loaded by default.
 */
@Component({
    selector: 'lvl-demo-layout',
    template: ''
})
@Navigatable({
    group: 'Front End',
    title: 'Layout',
    icon: 'subject'
})
export class LayoutDemoComponent { }
