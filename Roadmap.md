# lavalav Roadmap


Below is the roadmap of lavalav. The order of these plans are subject to change.

### Current version: 0.0


## Release Features

### 0.1
#### Repositories, Ontology, and Database Generation Executable
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



### 0.3
#### Web Library
This will be a set of generic web functions, not specific to any domain, and used across all APIs.


### 0.4
#### Angular 2 Gulp Packaging
A set of gulp tasks which will aid in building, testing, and minifying angular components.


### 0.5
#### Git Server
A set of components which may be used for hosting a git server.


### 0.6 Package Manager Server
A set of components for hosting npm, nuget, and chocolately packages


### 0.7
#### Build server
A set of components which may be used for hosting a build server.


### 0.8
#### Deployment server
A set of components which may be used for a deployment server.


### 0.9
#### Semantic Versioning API
An executable used to version code in accordance to SemVer 2.0.


### 0.10
#### Agile Management Application
A set of components for managing sprints.


### 0.11
#### Change Management Application
A set of components to track changes made to software.


### 0.12
#### Environment Management Application
A set of components for distrubuting and virtualizing environments


### 0.13
#### Portfolio
A set of components which highlight the work done.