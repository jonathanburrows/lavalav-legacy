## Goals
To provide a way to register all classes related to generating a repository.

## Requirements
Must provide a parameterless method, AddRepositories, which
1. registers a TypeResolver, RepositoryFactory, and a repository for every registered entity
2. throws an exception if the service collection is null

## Technical Considerations
The convention for service collection extensions is to have the namespace Microsoft.Extensions.DependencyInjection, please follow that pattern.

## Performance Considerations
Consider making classes which are expensive to initiate singletons.