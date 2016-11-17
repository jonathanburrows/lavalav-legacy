## Goals
To provide a way to register all models referenced by the executing application, so they can be queried.

## Requirements
Must provide a method, AddDomains, which
1. registers the nhibernate config in the service provider
2. adds all models in assemblies referenced by the executing assembly to the NHibernate config.
3. adds all models in the executing assembly to the NHibernate config
4. will avoid adding duplicate models if they are already added

## Technical Considerations
There shouldnt be any database specific actions occuring in this extension method.

## Performance Considerations
This will be run on startup, so consider optimizing as this could be a bottleneck.