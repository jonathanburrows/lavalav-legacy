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

    constructor(options?: IIdentityResourceEntityOptions) {
        options = options || {};
        super(options);
        this.enabled = options!.enabled;
        this.name = options!.name;
        this.displayName = options!.displayName;
        this.description = options!.description;
        this.required = options!.required;
        this.emphasize = options!.emphasize;
        this.showInDiscoveryDocument = options!.showInDiscoveryDocument;
        this.userClaims = options.userClaims ? options.userClaims.map(p => new UserClaim(p)) : []; // tslint:disable-line
    }
}

interface IIdentityResourceEntityOptions {
    enabled?: boolean;
    name?: string;
    displayName?: string;
    description?: string;
    required?: boolean;
    emphasize?: boolean;
    showInDiscoveryDocument?: boolean;
    userClaims?: UserClaim[];
    id?: number;
}
