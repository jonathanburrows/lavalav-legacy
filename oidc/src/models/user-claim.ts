import { Entity, IAggregateRoot, Required } from '@lvl/front-end';

export class UserClaim extends Entity implements IAggregateRoot {
    @Required() public name: string;

    constructor(options?: UserClaim) {
        super(options);
        this.name = options!.name;
    }
}
