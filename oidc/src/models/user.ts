import { ClaimEntity } from './claim-entity';
import { Entity, IAggregateRoot, Required } from '@lvl/front-end';

export class User extends Entity implements IAggregateRoot {
    @Required() public subjectId: string;
    @Required() public username: string;
    public hashedPassword: string;
    public salt: string;
    public providerName: string;
    public providerSubjectId: string;
    public claims: ClaimEntity[];
    public id: number;

    constructor(options?: User) {
        super();
        this.subjectId = options!.subjectId;
        this.username = options!.username;
        this.hashedPassword = options!.hashedPassword;
        this.salt = options!.salt;
        this.providerName = options!.providerName;
        this.providerSubjectId = options!.providerSubjectId;
        this.claims = options!.claims!.map(p => new ClaimEntity(p));
        this.id = options!.id;
    }
}
