## Goals
To provide a way to construct a repository.

## Requirements
Must provide a generic and non-generic method that takes a type, which:
1. Creates a repository for of the given type
2. Throws an ArgumentNullException if the given type is null
3. Throws an ArgumentException if the type does not implement IEntity
4. Throws an InvalidOperationException if the type is an entity, but not mapped

## Technical Considerations
To keep the code to a mimimum, try to minimize the amount of reflection.