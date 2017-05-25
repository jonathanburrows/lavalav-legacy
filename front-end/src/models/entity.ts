export abstract class Entity {
    public id: number;

    constructor(options?: IEntityOptions) {
        options = options || {};
        this.id = options!.id;
    }
}

interface IEntityOptions {
    id?: number;
}
