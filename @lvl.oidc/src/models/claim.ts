import { IEntity } from '@lvl/core';

export class Claim implements IEntity {
    public id: number;
    public type: string;
    public issuer: string;
    public valueType: string;
    public value: string;
}
