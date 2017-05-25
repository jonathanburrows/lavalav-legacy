import { Entity } from '../../src';

export class Astronaut extends Entity {
    public name: string;
    public birthDate: Date;

    constructor(options?: IAstronautOptions) {
        options = options || {};
        super(options);
        this.name = options!.name;
        this.birthDate = options!.birthDate;
    }
}

interface IAstronautOptions {
    name?: string;
    birthDate?: Date;
    id?: number;
}
