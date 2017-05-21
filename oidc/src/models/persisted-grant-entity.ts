import { Entity, IAggregateRoot, Required } from '@lvl/front-end';

export class PersistedGrantEntity extends Entity implements IAggregateRoot {
    @Required() public key: string;
    @Required() public type: string;
    @Required() public subjectId: string;
    @Required() public clientId: string;
    public creationTime: Date;
    public expiration: Date;
    public data: string;
    public id: number;

    constructor(options?: PersistedGrantEntity) {
        super();
        this.key = options!.key;
        this.type = options!.type;
        this.subjectId = options!.subjectId;
        this.clientId = options!.clientId;
        this.creationTime = options!.creationTime;
        this.expiration = options!.expiration;
        this.data = options!.data;
        this.id = options!.id;
    }
}
