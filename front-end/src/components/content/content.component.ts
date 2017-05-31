import { Component, ViewEncapsulation } from '@angular/core';

/**
 *  Will organize the content of a page, and apply styles to it, so all pages look consistent.
 */
@Component({
    selector: 'lvl-content',
    templateUrl: 'content.component.html',
    styleUrls: ['content.component.scss'],

    // need to have styles cascade to child directives.
    encapsulation: ViewEncapsulation.None,

    host: {
        '[class.lvl-content]': 'true'
    }
})
export class ContentComponent { }
