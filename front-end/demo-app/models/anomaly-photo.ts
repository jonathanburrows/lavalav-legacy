import { Entity, IAggregateRoot, Required } from '../../src';

export class AnomalyPhoto extends Entity implements IAggregateRoot {
    @Required() public name: string;
    public views: number;
    @Required() public takenByUserId: string;

    constructor(options?: IAnomalyPhotoOptions) {
        options = options || {};
        super(options);
        this.name = options!.name;
        this.views = options!.views;
        this.takenByUserId = options!.takenByUserId;
    }
}

interface IAnomalyPhotoOptions {
    name?: string;
    views?: number;
    takenByUserId?: string;
    id?: number;
}
