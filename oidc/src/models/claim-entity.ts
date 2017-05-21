import { Entity, IAggregateRoot } from '@lvl/front-end';

export class ClaimEntity extends Entity implements IAggregateRoot {
    public type: string;
    public issuer: string;
    public valueType: string;
    public value: string;
    public id: number;

    constructor(options?: ClaimEntity) {
        super();
        this.type = options!.type;
        this.issuer = options!.issuer;
        this.valueType = options!.valueType;
        this.value = options!.value;
        this.id = options!.id;
    }
}
