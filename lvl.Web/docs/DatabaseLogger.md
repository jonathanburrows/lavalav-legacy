## Goals
Provide a way to log information into a database using nhibernate.

## Requirements
Provide a method, Log, which adds a log entry into a repository:
1. After logging a message, it is stored in the repository
2. If the log level is lower than what is enabled, no log entry is added

Provide a method, IsEnabled, which determines if a Log Level can be set:
If the configured log level is greater or less than the given log level, true
Otherwise, false

## Technical Considerations
This class will be instantiated by a logger factory, the constructor parameters may or may not be registered.