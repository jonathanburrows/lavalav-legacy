import { Scope } from './scope';
import { Secret } from './secret';
import { IEntity } from '@lvl/core';

export class ApiResource implements IEntity {
    public id: number;
    public enabled: boolean;
    public name: string;
    public displayName: string;
    public description: string;
    public apiSecrets: Secret[];
    public userClaims: string[];
    public scopes: Scope[];
}
