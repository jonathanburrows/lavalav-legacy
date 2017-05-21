import { ClientEntity } from './client-entity';
import { Entity, IAggregateScope, Required } from '@lvl/front-end';

export class RedirectUri extends Entity implements IAggregateScope<ClientEntity> {
    @Required() public name: string;
    public id: number;

    constructor(options?: RedirectUri) {
        super();
        if (options) {
            this.name = options.name;
            this.id = options.id;
        }
    }
}
