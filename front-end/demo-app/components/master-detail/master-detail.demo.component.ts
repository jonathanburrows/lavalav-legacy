import { Component } from '@angular/core';

import { Navigatable } from '../../../src';

@Component({
    selector: 'lvl-demo-master-detail',
    templateUrl: 'master-detail.demo.component.html'
})
@Navigatable({
    group: 'Front End',
    icon: 'view_quilt',
    title: 'Master Detail'
})
export class MasterDetailDemoComponent { }
