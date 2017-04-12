import {
    animate,
    state,
    style,
    transition,
    trigger
} from '@angular/core';

const passiveStyle = style({
    opacity: 1,
    transform: 'translateX(0)'
});

const enterStyle = style({
    opacity: 0,
    transform: 'translateX(-100%)'
});

const leaveStyle = style({
    opacity: 0,
    transform: 'translateX(100%)'
});

const easeInBezier = '0.8s cubic-bezier(0.55, 0, 0.55, 0.2)';
const easeOutBezier = '0.8s cubic-bezier(0.25, 0.8, 0.25, 1)';

export const slideInAnimation = trigger('routeAnimation', [
    state('*', passiveStyle),
    transition(':enter', [enterStyle, animate(easeInBezier)]),
    transition(':leave', [animate(easeOutBezier, leaveStyle)])
]);
