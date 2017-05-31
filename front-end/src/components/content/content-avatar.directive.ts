import { Directive } from '@angular/core';

/**
 *  Will place an avatar in the first column of a row, so that it is horizontally aligned with the menu icon.
 */
@Directive({
    selector: '[lvl-content-avatar]',
    host: {
        '[class.lvl-content-avatar]': 'true'
    }
})
export class ContentAvatarDirective { }
