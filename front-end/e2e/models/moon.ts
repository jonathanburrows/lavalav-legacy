import { Entity, IAggregateRoot } from '../../src';
import { Planet } from './planet';

export class Moon extends Entity implements IAggregateRoot {
    public name: string;
    public radius: number;
    public orbitalDistance: number;
    public mass: number;
    public planet: Planet;
    public firstPersonToStepFootId: number;
    public id: number;

    constructor(options?: Moon) {
        super();
        if (options) {
            this.name = options.name;
            this.radius = options.radius;
            this.orbitalDistance = options.orbitalDistance;
            this.mass = options.mass;
            this.planet = new Planet(options.planet);
            this.firstPersonToStepFootId = options.firstPersonToStepFootId;
            this.id = options.id;
        }
    }
}
