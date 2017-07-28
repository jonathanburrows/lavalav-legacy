import { Component } from '@angular/core';

import { Navigatable } from '@lvl/front-end';

@Component({
    selector: 'lvl-personal-personality',
    templateUrl: 'personality.component.html',
    styleUrls: ['personality.component.scss']
})
@Navigatable({
    group: 'About Me',
    title: 'Personality',
    icon: 'tag_faces'
})
export class PersonalityComponent { }
