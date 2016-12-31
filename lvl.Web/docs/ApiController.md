## Goals
Provide a single controller which serves restful endpoints for all entities.


## Requirements
Provides 5 methods: get single, get collection, post, put, and delete

### Get collection
Provides a method which retreives all entities of a given type:
1. All entities returned are of the given type
2. If the type is not mapped, a 500 status code is returned
3. If a null value is given for the type, a 500 status code is returned

### Get single
Provides a method which retreives an entity from a given ID:
1. If there is an entity with a matching ID, that entity is returned
2. If there is no entity with a matching ID, a 500 status code is returned
3. If the request is a post, then a 404 is returned
4. If the request is a put, then a 404 is returned
5. If the request is a delete, then a 404 is returned
6. If an entity is returned, then the entity is returned as a json object
7. If the given entity type is not registered, a 500 status code is returned

### Post
Provide a method which adds an entity:
1. After calling the method, the entity is stored persistently
2. If the entity is null, an ArgumentNullException is thrown
3. If the entity type is null, an ArgumentNullException is thrown
4. If the entity type is not mapped, an InvalidOperationException is thrown
5. If the entity cannot deserialize to the entity type, an ArgumentException is thrown
6. If the entity already has an identifier, an InvalidOperationException is thrown
7. The returned entity has a populated identifier

### Put
Provide a method which updates an existing entity:
1. After calling the method (as a PUT request), the entitys fields are updated persistently
2. If the entity is null, an ArgumentNullException is thrown
3. If the entity type is null, an ArgumentNullException is thrown
4. If the entity type is not mapped, an InvalidOperationException is thrown
5. If the entity cannot deserialize to the entity type, an ArgumentException is thrown
6. If there is no matching entity, an InvalidOperationException is thrown

### Delete
Provide a method which deletes an existing entity:
1. After calling the method (as a DELETE request), the entity is removed from the persistent collection
2. If the entity is null, an ArgumentNullException is thrown
3. If the entity type is null, an ArgumentNullException is thrown
4. If the entity type is not mapped, an InvalidOperationException is thrown
5. If the entity cannot deserialize to the entity type, an ArgumentException is thrown
6. If there is no matching entity, an InvalidOperationException is thrown

## Performance Considerations
This could be a potential bottleneck, be sure to not add slow code.