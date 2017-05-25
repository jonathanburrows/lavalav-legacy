import {
    animate,
    state,
    style,
    transition,
    trigger
} from '@angular/core';

const passiveStyle = style({
    'white-space': 'normal'
});

const enterStyle = style({
    'white-space': 'normal'
});

const leaveStyle = style({
    'white-space': 'normal'
});

const easeInBezier = '0s cubic-bezier(0.0,0.0,0.2,1)';
const easeOutBezier = '5s cubic-bezier(0.4,0.0,1,1)';

export const routerTransition = trigger('routerTransition', [
    state('*', passiveStyle),
    transition(':enter', [enterStyle, animate(easeInBezier)]),
    transition(':leave', [animate(easeOutBezier, leaveStyle)])
]);

export const routerHostBindings = {
    '[@routerTransition]': '',
    '(@routerTransition.start)': `$event.fromState === 'void'? $event.element.classList.add('entering') : $event.element.classList.add('leaving')`,
    '(@routerTransition.done)': `$event.fromState === 'void'? $event.element.classList.add('entered') : $event.element.classList.add('left')`
};
