import { Entity, IAggregateScope } from '../../src';
import { RocketShip } from './rocket-ship';

export class Booster extends Entity implements IAggregateScope<RocketShip> {
    public thrust: number;
    public burnTime: number;

    constructor(options?: IBoosterOptions) {
        options = options || {};
        super(options);
        this.thrust = options!.thrust;
        this.burnTime = options!.burnTime;
    }
}

interface IBoosterOptions {
    thrust?: number;
    burnTime?: number;
    id?: number;
}
