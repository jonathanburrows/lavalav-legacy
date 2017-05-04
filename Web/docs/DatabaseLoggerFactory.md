## Goals
Provide a way to construct DatabaseLoggers with types resolved from Service Provider.

## Requirements
Create a class, DatabaseLoggerFactory, which inherits from LoggerFactory, which automatically adds a DatabaseLoggerProvider.

Creating a logger from the DatabaseLoggerFactory will default to a DatabaseLogger, if no other providers have been added.