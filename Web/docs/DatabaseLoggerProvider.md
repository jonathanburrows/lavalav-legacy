## Goals
Provide a way to create database loggers with types from the service provider.

## Requirements
A class, DatabaseLoggerProvider, which implements ILoggerProvider. When it's CreateLogger is called, it returns a DatabaseLogger.

## Technical Consideration
Try to adhere to dependency injection, dont use the service locator, provide types through the constructor.