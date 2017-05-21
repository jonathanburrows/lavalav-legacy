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
    public id: number;

    constructor(options?: ApiResourceEntity) {
        super();
        if (options) {
            this.name = options.name;
            this.enabled = options.enabled;
            this.displayName = options.displayName;
            this.description = options.description;
            if (options.userClaims) {
                this.userClaims = options.userClaims.map(p => new UserClaim(p));
            }
            if (options.apiSecrets) {
                this.apiSecrets = options.apiSecrets.map(p => new SecretEntity(p));
            }
            if (options.scopes) {
                this.scopes = options.scopes.map(p => new ScopeEntity(p));
            }
            this.id = options.id;
        }
    }
}
