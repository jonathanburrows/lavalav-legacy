export abstract class Entity {
    public id: number;

    constructor(options?: Entity) {
        this.id = options!.id;
    }
}
