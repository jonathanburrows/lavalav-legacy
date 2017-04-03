import { IEntity } from '@lvl/core';

export class PersistedGrant implements IEntity {
    public id: number;
    public key: string;
    public type: string;
    public subjectId: string;
    public clientId: string;
    public creationTime: Date;
    public expiration: Date;
    public data: string;
}
