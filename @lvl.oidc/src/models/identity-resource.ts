import { IEntity } from '@lvl/core';

export class IdentityResource implements IEntity {
    public id: number;
    public enabled: boolean;
    public name: string;
    public displayName: string;
    public description: string;
    public required: boolean;
    public emphasize: boolean;
    public showInDiscoveryDocument: boolean;
    public userClaims: string[];
}
