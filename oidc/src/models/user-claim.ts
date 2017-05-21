import { Entity, IAggregateRoot, Required } from '@lvl/front-end';

export class UserClaim extends Entity implements IAggregateRoot {
    @Required() public name: string;
    public id: number;

    constructor(options?: UserClaim) {
        super();
        if (options) {
            this.name = options.name;
            this.id = options.id;
        }
    }
}
