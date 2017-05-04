## Goals
To provide utility to run all scripts from a bin folder, in order, against a registered database.

## Requirements
1. Each .sql script in the given directory is run against the registered database
2. The scripts are ordered by string value (ie: *a.sql* is run before *b.sql*)
3. If the directory does not exist, a DirectoryNotFoundException

## Technical Considerations
For easier rollbacks of deployments, consider using a transaction for all scripts.