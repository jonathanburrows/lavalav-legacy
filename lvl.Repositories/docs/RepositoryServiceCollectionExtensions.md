## Goals
To provide a way to register all classes related to generating a repository.

## Requirements
Must provide a parameterless method, AddRepositories, which
1. registers a TypeResolver, RepositoryFactory, SessionManager and a repository for every registered entity
2. Throws an ArgumentNullException if the service collection is null
3. Throws an InvalidOperationException if the AddDomains method hasnt been called before AddRepositories
3. Provides a way to override a repository. When resolving through the service provider or factory, it is returned instead
4. If the database is SQL Lite, then SQLiteSessionManager is registered

## Technical Considerations
The convention for service collection extensions is to have the namespace Microsoft.Extensions.DependencyInjection, please follow that pattern.

## Performance Considerations
Consider making classes which are expensive to initiate singletons.