## Goals
To provide a way to detect which database a connection string is trying to connect to.

## Requirements
Must provide a method, taking a connection string, which:
1. If the connection string is null, then it returns SQLite
2. If the connection string is MsSql, then it returns MsSql
3. If the connection string is Oracle, then it returns Oracle
4. If the connection string cannot be matched, then it returns Unsupported

Must provide a method, taking an nhibernate configuration, which:
1. If the registered database is SQLite, then it returns SQLite
2. If the registered database is MsSql, then it returns MsSql
3. If the registered database is Oracle, then it returns Oracle
4. Otherwise, it returns unsupported