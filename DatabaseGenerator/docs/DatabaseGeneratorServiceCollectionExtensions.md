## Goals
Register a set of components to be used for database generator.

## Requirements
Provide a method which:
1. Registers the DatabaseCreator class
2. Registers the DatabaseMigrator class
3. Registers the ScriptRunner class
4. Throws an ArgumentNullException when the given service collection is null