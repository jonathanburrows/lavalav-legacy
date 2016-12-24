## Goals
To provide a way to register all classes required by the middleware.


## Requirements
Must provide a method, AddWeb, which
1. Registers all classes required by the middleware
2. Calls Microsoft's AddMvc
3. Throws an exception if the service collection is null


## Technical considerations
It is critical that there are no depedencies on a specific domain.

Consider using the Microsoft.Extensions.DependencyInjection for consistency with Microsoft's pattern.