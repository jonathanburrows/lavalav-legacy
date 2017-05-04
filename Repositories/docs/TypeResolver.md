## Goals
To provide a way to convert a string to a registered type, with clear exceptions.

## Requirements
Must provide a method, Resolve, which:
1. searches for a mapped class with a matching full name (case insensitive)
2. searches for a mapped class with a matching class name (case insensitive)
3. throws an exception if the given name is null
4. throws an exception if no class is found
5. throws an exception if more than one class is found
6. Is overridable

## Technical Considerations
If possible, use nhibernates mapped classes, and not reflection.

## Performance Considerations
This could be running on every request, consider using a caching mechanism.