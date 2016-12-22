## Goals
To provide a CRUD operations for entities, and to create a fasade for database access.

## Requirements
Must provide get collection, get single, create, update, and delete methods for entities. 
Each method will have a generic and non-generic counterpart.

Get (collection)
1. Will return all items in the repository

Get (single) 
Finds an element with a matching id from the repository:
1. If an element with a matching id is found, it is returned
2. If no elements with that id are found, null
3. Throws an InvalidOperationException if multiple elements with that id are found

Create:
Adds an element to the repository, updates the properties with any generated values, and returns the created entity.
1. If successful, the model is added to the repository, updated with any generated properties, and returned
2. Throws an ArgumentNullException if the given element is null
3. Throws an ArgumentException if the given element is not of the repository type
4. The create action is wrapped in a transaction
5. Any child items which have been added will be created
6. Any child items which have been edited will be updated

Update:
Updates an entity in the repository with an id matching the given element, with the given entities properties:
1. If successful, the given entity is updated with generated properties, then returned
2. Throws an ArgumentNullException if the given entity is null
3. Throws an ArgumentException if the given entity is not of the repository type
4. Throws an InvalidOperationException if no matching entity is found
5. The update action is wrapped in a transaction
6. Any child items which have been added will be created
7. Any child items which have been edited will be updated

Delete:
Removes an entity from the repostory with an id matching the given entity
1. If successful, the given entity is returned
2. Throws an ArgumentNullException if the given entity is null
3. Throws an ArgumentException if the given entity is not of the repository type
4. Throws an InvalidOperationException if there is no matching entity
5. The delete action is wrapped in a transaction
6. The delete will cascade to the child elements

## Technical Considerations
Because there is generic and non-generic support, the non-generic methods should call the generics.
