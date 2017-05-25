import { Entity, IAggregateRoot } from '../../src';
import { Planet } from './planet';

export class Moon extends Entity implements IAggregateRoot {
    public name: string;
    public radius: number;
    public orbitalDistance: number;
    public mass: number;
    public planet: Planet;
    public firstPersonToStepFootId: number;

    constructor(options?: IMoonOptions) {
        options = options || {};
        super(options);
        this.name = options!.name;
        this.radius = options!.radius;
        this.orbitalDistance = options!.orbitalDistance;
        this.mass = options!.mass;
        this.planet = new Planet(options!.planet);
        this.firstPersonToStepFootId = options!.firstPersonToStepFootId;
    }
}

interface IMoonOptions {
    name?: string;
    radius?: number;
    orbitalDistance?: number;
    mass?: number;
    planet?: Planet;
    firstPersonToStepFootId?: number;
    id?: number;
}
