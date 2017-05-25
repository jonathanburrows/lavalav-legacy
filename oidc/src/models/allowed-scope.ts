import { ClientEntity } from './client-entity';
import { Entity, IAggregateScope, Required } from '@lvl/front-end';

export class AllowedScope extends Entity implements IAggregateScope<ClientEntity> {
    @Required() public name: string;

    constructor(options?: IAllowedScopeOptions) {
        options = options || {};
        super(options);
        this.name = options!.name;
    }
}

interface IAllowedScopeOptions {
    name?: string;
    id?: number;
}
