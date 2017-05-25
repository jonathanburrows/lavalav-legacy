import { Entity, IAggregateRoot, Required } from '@lvl/front-end';

export class UserClaim extends Entity implements IAggregateRoot {
    @Required() public name: string;

    constructor(options?: IUserClaimOptions) {
        options = options || {};
        super(options);
        this.name = options!.name;
    }
}

interface IUserClaimOptions {
    name?: string;
    id?: number;
}
