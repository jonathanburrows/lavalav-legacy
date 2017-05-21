import { ClientEntity } from './client-entity';
import { Entity, IAggregateScope, Required } from '@lvl/front-end';

export class GrantTypeEntity extends Entity implements IAggregateScope<ClientEntity> {
    @Required() public name: string;

    constructor(options?: GrantTypeEntity) {
        super(options);
        this.name = options!.name;
    }
}
