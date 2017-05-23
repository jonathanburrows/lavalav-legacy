import { Entity, IAggregateRoot } from '../../src';
import { Moon } from './moon';

export class Planet extends Entity implements IAggregateRoot {
    public name: string;
    public supportsLife: boolean;
    public mass: number;
    public astronomicalUnits: number;
    public discoveredOn: Date;
    public moons: Moon[];
    public id: number;

    constructor(options?: Planet) {
        super();
        if (options) {
            this.name = options.name;
            this.supportsLife = options.supportsLife;
            this.mass = options.mass;
            this.astronomicalUnits = options.astronomicalUnits;
            this.discoveredOn = options.discoveredOn;
            if (options.moons) {
                this.moons = options.moons.map(p => new Moon(p));
            }
            this.id = options.id;
        }
    }
}
