import { ClientEntity } from './client-entity';
import { Entity, IAggregateScope, Required } from '@lvl/front-end';

export class IdentityProviderRestriction extends Entity implements IAggregateScope<ClientEntity> {
    @Required() public name: string;

    constructor(options?: IIdentityProviderRestrictionOptions) {
        options = options || {};
        super(options);
        this.name = options!.name;
    }
}

interface IIdentityProviderRestrictionOptions {
    name?: string;
    id?: number;
}
