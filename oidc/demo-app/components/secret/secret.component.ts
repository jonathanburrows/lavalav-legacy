import { Component } from '@angular/core';

import { Navigatable } from '@lvl/front-end';

@Component({
    selector: 'lvl-oidc-demo-secret',
    templateUrl: 'secret.component.html'
})
@Navigatable({
    group: 'Open ID',
    title: 'Secret',
    icon: 'add_alert',
    roles: ['oidc']
})
export class SecretComponent { }
