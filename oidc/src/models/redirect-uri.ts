import { ClientEntity } from './client-entity';
import { Entity, IAggregateScope, Required } from '@lvl/front-end';

export class RedirectUri extends Entity implements IAggregateScope<ClientEntity> {
    @Required() public name: string;

    constructor(options?: RedirectUri) {
        super(options);
        this.name = options!.name;
    }
}
