## Goals
To provide a way of generating a set of TypeScript models for C# POCOs


## Synopsis
@lvl.typescript-generator --assembly-path=&gt;assembly-path&lt; --output-bin=&gt;output-bin&lt; [--decorator-path=&gt;decorator-path&lt;] [&gt;C# namespace&lt;=&gt;npm package&lt;[, &gt;C# namespace&lt;=&gt;npm package&lt;...]] 


## Example
@lvl.typescript-generator --assembly-path=lvl.TestDomain.dll --output-bin=&gt;src/models&lt; lvl.Ontology=&gt;@lvl/core&lt;


## Options
**--assembly-path**
Path to the dll which will have its POCO objects converted to TypeScript objects. Will detect types which implement IEntity.


**--output-bin**
The directory where the generated files will be placed.


**--decorator-path** *(optional)*
The path, relative to the output bin, to where the decorators can be found. If not provided, the default will be @lvl/core. This is intended for classes within @lvl/core, which cant reference itself.


**C# Namespace** *(multiple allowed)*
Associates a namespace with a npm package path. Since the generated models may depend on models from a different npm package, they need to be able to resolve them.


## Architecture
<img src="docs\typescript-generator-architecture.png" />


## Syntax
<img src="docs\typescript-generator-syntax.png" />
