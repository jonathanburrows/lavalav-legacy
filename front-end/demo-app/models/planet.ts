import { Entity, IAggregateRoot } from '../../src';
import { Moon } from './moon';

export class Planet extends Entity implements IAggregateRoot {
    public name: string;
    public supportsLife: boolean;
    public mass: number;
    public astronomicalUnits: number;
    public discoveredOn: Date;
    public moons: Moon[];

    constructor(options?: IPlanetOptions) {
        options = options || {};
        super(options);
        this.name = options!.name;
        this.supportsLife = options!.supportsLife;
        this.mass = options!.mass;
        this.astronomicalUnits = options!.astronomicalUnits;
        this.discoveredOn = options!.discoveredOn;
        this.moons = options.moons ? options.moons.map(p => new Moon(p)) : [];
    }
}

interface IPlanetOptions {
    name?: string;
    supportsLife?: boolean;
    mass?: number;
    astronomicalUnits?: number;
    discoveredOn?: Date;
    moons?: Moon[];
    id?: number;
}
