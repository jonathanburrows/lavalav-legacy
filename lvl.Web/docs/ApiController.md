## Goals
Provide a single controller which serves restful endpoints for all entities.


## Requirements
Provides 5 methods: get single, get collection, create, update, and delete

### Get single
Provides a method which retreives an entity from a given ID:
1. If there is an entity with a matching ID, that entity is returned
2. If there is no entity with a matching ID, a 500 status code is returned
3. If the request is a post, then a 404 is returned
4. If the request is a put, then a 404 is returned
5. If the request is a delete, then a 404 is returned
6. If an entity is returned, then the entity is returned as a json object
7. If the given entity type is not registered, a 500 status code is returned


## Performance Considerations
This could be a potential bottleneck, be sure to not add slow code.