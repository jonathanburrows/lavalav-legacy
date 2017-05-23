import { ApiResourceEntity } from './api-resource-entity';
import { UserClaim } from './user-claim';
import { Entity, IAggregateScope, Required } from '@lvl/front-end';

export class ScopeEntity extends Entity implements IAggregateScope<ApiResourceEntity> {
    @Required() public name: string;
    public displayName: string;
    public description: string;
    public required: boolean;
    public emphasize: boolean;
    public showInDiscoveryDocument: boolean;
    public userClaims: UserClaim[];

    constructor(options?: ScopeEntity) {
        super(options);
        this.name = options!.name;
        this.displayName = options!.displayName;
        this.description = options!.description;
        this.required = options!.required;
        this.emphasize = options!.emphasize;
        this.showInDiscoveryDocument = options!.showInDiscoveryDocument;
        this.userClaims = options!.userClaims!.map(p => new UserClaim(p));
    }
}