import { Entity, IAggregateRoot, Required } from '@lvl/front-end';

export class PersistedGrantEntity extends Entity implements IAggregateRoot {
    @Required() public key: string;
    @Required() public type: string;
    @Required() public subjectId: string;
    @Required() public clientId: string;
    public creationTime: Date;
    public expiration: Date;
    public data: string;

    constructor(options?: IPersistedGrantEntityOptions) {
        options = options || {};
        super(options);
        this.key = options!.key;
        this.type = options!.type;
        this.subjectId = options!.subjectId;
        this.clientId = options!.clientId;
        this.creationTime = options!.creationTime;
        this.expiration = options!.expiration;
        this.data = options!.data;
    }
}

interface IPersistedGrantEntityOptions {
    key?: string;
    type?: string;
    subjectId?: string;
    clientId?: string;
    creationTime?: Date;
    expiration?: Date;
    data?: string;
    id?: number;
}
