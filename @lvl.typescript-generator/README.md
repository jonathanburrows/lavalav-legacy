## Goals
To provide a way of generating a set of TypeScript models for C# POCOs


## Synopsis
@lvl.typescript-generator --assembly-path='assembly-path' --output-directory='output-directory' ['C# namespace'='npm package'[, 'C# namespace'='npm package'...]] 


## Example
@lvl.typescript-generator --assembly-path=lvl.TestDomain.dll --output-directory='src/models' lvl.Ontology='@lvl/core'


## Architecture
<img src="docs\typescript-generator-architecture.png" />


## Syntax
<img src="docs\typescript-generator-syntax.png" />
