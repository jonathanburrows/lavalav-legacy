import { Component } from '@angular/core';

import { Navigatable } from '@lvl/front-end';

@Component({
    selector: 'lvl-oidc-demo-admin-protected',
    template: 'Only administrators should see this.'
})
@Navigatable({
    group: 'Admin',
    title: 'Admin Protected Page',
    roles: ['administrator']
})
export class AdminProtectedComponent { }
