## Goals
To provide a way of generating databases for an application dll and a connection string.

## Architecture
<img src="docs\database-generator-architecture.png" />

## Synopsis
lvl.DatabaseGenerator 
	--connection-string 'connection-string'
	--assembly-path 'assembly-path'
	[--post-generation-script-bin 'path']
	[--pre-generation-script-bin 'path']
	[--migrate] 
	[--dry-run]

## Example
lvl.DatabaseGenerator --connection-string "helloworld" --migrate --assembly-path "here" --pre-generation-script-bin 'there'


## Options
_--connection-string_: The database whos schema will be modified.


_--assembly-path_: The assembly path which contains the entities, or references assemblies with entities, which will be used to generate database.


_--post-generation-script-bin_: A diretory which all files ending with sql extension will be run before the database is generated.


_--pre-generation-script-bin_: A directory which all files ending with sql exensions will be run after the database is generated.


_--migrate_: Denotes that the database should not recreated from scratched, but patched.


_--dry-run_: Will denote that no changes will be made, to report on any potential errors.