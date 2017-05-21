import { ClientEntity } from './client-entity';
import { Entity, IAggregateScope, Required } from '@lvl/front-end';

export class CorsOrigin extends Entity implements IAggregateScope<ClientEntity> {
    @Required() public name: string;

    constructor(options?: CorsOrigin) {
        super(options);
        this.name = options!.name;
    }
}
