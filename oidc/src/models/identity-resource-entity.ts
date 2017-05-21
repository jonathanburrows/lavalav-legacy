import { UserClaim } from './user-claim';
import { Entity, IAggregateRoot, Required } from '@lvl/front-end';

export class IdentityResourceEntity extends Entity implements IAggregateRoot {
    public enabled: boolean;
    @Required() public name: string;
    public displayName: string;
    public description: string;
    public required: boolean;
    public emphasize: boolean;
    public showInDiscoveryDocument: boolean;
    public userClaims: UserClaim[];
    public id: number;

    constructor(options?: IdentityResourceEntity) {
        super();
        if (options) {
            this.enabled = options.enabled;
            this.name = options.name;
            this.displayName = options.displayName;
            this.description = options.description;
            this.required = options.required;
            this.emphasize = options.emphasize;
            this.showInDiscoveryDocument = options.showInDiscoveryDocument;
            if (options.userClaims) {
                this.userClaims = options.userClaims.map(p => new UserClaim(p));
            }
            this.id = options.id;
        }
    }
}
