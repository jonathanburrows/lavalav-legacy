export abstract class Entity {
    public id: number;

    constructor(options?: Entity) {
        options = options || <any>{};
        this.id = options!.id;
    }
}
