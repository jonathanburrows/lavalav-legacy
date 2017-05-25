import { ScopeEntity } from './scope-entity';
import { SecretEntity } from './secret-entity';
import { UserClaim } from './user-claim';
import { Entity, IAggregateRoot, Required } from '@lvl/front-end';

export class ApiResourceEntity extends Entity implements IAggregateRoot {
    @Required() public name: string;
    public enabled: boolean;
    public displayName: string;
    public description: string;
    public userClaims: UserClaim[];
    public apiSecrets: SecretEntity[];
    public scopes: ScopeEntity[];

    constructor(options?: IApiResourceEntityOptions) {
        options = options || {};
        super(options);
        this.name = options!.name;
        this.enabled = options!.enabled;
        this.displayName = options!.displayName;
        this.description = options!.description;
        this.userClaims = options!.userClaims!.map(p => new UserClaim(p));
        this.apiSecrets = options!.apiSecrets!.map(p => new SecretEntity(p));
        this.scopes = options!.scopes!.map(p => new ScopeEntity(p));
    }
}

interface IApiResourceEntityOptions {
    name?: string;
    enabled?: boolean;
    displayName?: string;
    description?: string;
    userClaims?: UserClaim[];
    apiSecrets?: SecretEntity[];
    scopes?: ScopeEntity[];
    id?: number;
}
