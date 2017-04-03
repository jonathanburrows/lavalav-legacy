import { IEntity } from '@lvl/core';

export class Secret implements IEntity {
    public id: number;
    public description: string;
    public value: string;
    public expiration: Date;
    public type: string;
}
