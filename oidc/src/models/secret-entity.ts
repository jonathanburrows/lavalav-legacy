import { Entity, IAggregateRoot } from '@lvl/front-end';

export class SecretEntity extends Entity implements IAggregateRoot {
    public description: string;
    public value: string;
    public expiration: Date;
    public type: string;
    public id: number;

    constructor(options?: SecretEntity) {
        super();
        this.description = options!.description;
        this.value = options!.value;
        this.expiration = options!.expiration;
        this.type = options!.type;
        this.id = options!.id;
    }
}
