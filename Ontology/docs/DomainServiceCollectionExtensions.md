## Goals
To provide a way to register all models referenced by the executing application, so they can be queried.

## Requirements
Must provide a method, AddDomains, which
1. registers the nhibernate config in the service provider
2. adds all models in assemblies referenced by the executing assembly to the NHibernate config.
3. adds all models in the executing assembly to the NHibernate config
4. will avoid adding duplicate models if they are already added
5. will set up database configurations based on the connection string
	- if no connection string is provided, in-memory database is configured
	- if an sql server connection string is provided, sql-server database is configured
	- if an oracle connection string is provided, oracle database is configured
	- if a connection string is given that does not match a database vendor, an exception is thrown

## Technical Considerations
There shouldnt be any database specific actions occuring in this extension method.

## Performance Considerations
This will be run on startup, so consider optimizing as this could be a bottleneck.