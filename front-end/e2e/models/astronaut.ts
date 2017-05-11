import { Entity } from '../../src';

export class Astronaut extends Entity {
    public name: string;
    public birthDate: Date;
    public id: number;

    constructor(options?: Astronaut) {
        super();
        if (options) {
            this.name = options.name;
            this.birthDate = options.birthDate;
            this.id = options.id;
        }
    }
}
