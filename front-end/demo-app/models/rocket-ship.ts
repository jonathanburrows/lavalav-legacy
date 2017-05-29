import { Entity, IAggregateRoot, Required } from '../../src';
import { Booster } from './booster';

export class RocketShip extends Entity implements IAggregateRoot {
    public expectedLaunchDate: Date;
    public cost: number;
    public maximumWeight: number;
    @Required() public inventingUserId: string;
    public targetPlanetId: number;
    public boosters: Booster[];

    constructor(options?: IRocketShipOptions) {
        options = options || {};
        super(options);
        this.expectedLaunchDate = options!.expectedLaunchDate;
        this.cost = options!.cost;
        this.maximumWeight = options!.maximumWeight;
        this.inventingUserId = options!.inventingUserId;
        this.targetPlanetId = options!.targetPlanetId;
        this.boosters = options.boosters ? options.boosters.map(p => new Booster(p)) : [];
    }
}

interface IRocketShipOptions {
    expectedLaunchDate?: Date;
    cost?: number;
    maximumWeight?: number;
    inventingUserId?: string;
    targetPlanetId?: number;
    boosters?: Booster[];
    id?: number;
}
