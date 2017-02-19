# lavalav Roadmap


Below is the roadmap of lavalav. The order of these plans are subject to change.

### Current version: 0.3


## Release Features

### 0.1 Repositories, Ontology, and Database Generation Executable
This will aim to set up the repository pattern and the ability to:
- Automatically register entities inheriting from a base class
- Connect to SQLite, Oracle, and MsSql databases based on connection string
- Safely fetching a registered C# type from a string
- Resolving a repository of a given type from the Service Provider
- A factory for constructing repositories
- Ability to get, create, update, and delete models to a database
- Configuration on which database to use
- A sample domain for test purposes

The database generator aims to set up a programatic way of deploying databases, and the ability to:
- Register classes needed to create and migrate a database
- Create SQLite, MsSql, and Oracle database Generation
- Launch an executable to construct a database from a given dll and connection string
- Make updates to an existing database without changing any Database
- Ability to run scripts before and after the automatic generation
- Ability to report on any potential issues a deployment could have

Amendment: due to dependencies of SQLite on database generation, Database Generation was merged in with this release.


### 0.2 Web Library
This will be a set of generic web functions, not specific to any domain, and used across all APIs.
- RestFUL API endpoints for all registered models
- OData endpoints for all registered models
- Tracing and Logging for errors
- Logging and change tracking for entities
- Ability to register all the above functionality


### 0.3 Angular 2 Build Pipeline
A template of configurations and code for building angular 2 projects and libraries. 
The build pipeline aims to provide the following features:
- Building a project from source code
- Building a project from distributables
- Option to lazy load
- Option to Ahead of Time compile
- Run spec tests
- Run spec tests from child libraries
- Run e2e tests
- Run e2e tests from child libraries
- Run the project for development
- Have changes be automatically updated in the browser when running development
- Have changes from a child library automatically update the browser when running development
- Support Sass
- Support having a global Sass file override default Sass variables, per project
- Have error messaging on how a project failed to build


### 0.4 TypeScript Generator
Provides the ability convert the POCOs of a C# assembly to TypeScript.
This will aim to convert a all models in a single C# assembly into TypeScript models, and output them into a directory. It will also:
- Provide inheritence
- Strongly type child properties
- Allow for referencing of models in different libraries
- Decorate the properties with validation attributes
- Support interfaces and abstract classes
- Support abstract properties
- Allow for mapping of C# namespaces to TypeScript resolvable paths
- Provide a set of decorators for validation
- Be distrubuted via an npm package


### 0.5 Build server
A set of components which may be used for hosting a build server.


### 0.6 Package Manager Server
A set of components for hosting npm, nuget, and chocolately packages.


### 0.7 Deployment server
A set of components which may be used for a deployment server.


### 0.8 Semantic Versioning API
An executable used to version code in accordance to SemVer 2.0.


### 0.9 Agile Management Application
A set of components for managing sprints.


### 0.10 Change Management Application
A set of components to track changes made to software.


### 0.11 Environment Management Application
A set of components for distrubuting and virtualizing environments.


### 0.12 Build Server
A set of components which may be used for hosting a build server


### 0.13 Portfolio
A set of components which highlight the work done.