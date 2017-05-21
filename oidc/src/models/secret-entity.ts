import { Entity, IAggregateRoot } from '@lvl/front-end';

export class SecretEntity extends Entity implements IAggregateRoot {
    public description: string;
    public value: string;
    public expiration: Date;
    public type: string;

    constructor(options?: SecretEntity) {
        super(options);
        this.description = options!.description;
        this.value = options!.value;
        this.expiration = options!.expiration;
        this.type = options!.type;
    }
}
