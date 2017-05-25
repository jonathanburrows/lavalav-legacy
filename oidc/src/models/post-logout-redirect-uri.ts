import { ClientEntity } from './client-entity';
import { Entity, IAggregateScope, Required } from '@lvl/front-end';

export class PostLogoutRedirectUri extends Entity implements IAggregateScope<ClientEntity> {
    @Required() public name: string;

    constructor(options?: IPostLogoutRedirectUriOptions) {
        options = options || {};
        super(options);
        this.name = options!.name;
    }
}

interface IPostLogoutRedirectUriOptions {
    name?: string;
    id?: number;
}
