import { Claim } from './claim';
import { IEntity } from '@lvl/core';

export class User implements IEntity {
    public id: number;
    public subjectId: string;
    public username: string;
    public hashedPassword: string;
    public providerName: string;
    public providerSubjectId: string;
    public claims: Claim[];
}
