## Goals
To provide a way of generating a set of TypeScript models for C# POCOs


## Synopsis
lvl.TypeScriptGenerator --assembly-path=&lt;assembly-path&gt; --output-bin=&lt;output-bin&gt; [--decorator-path=&lt;decorator-path&gt;] [&lt;C# namespace&gt;=&lt;npm package&gt;[, &lt;C# namespace&gt;=&lt;npm package&gt;...]] 


## Example
lvl.TypeScriptGenerator --assembly-path=lvl.TestDomain.dll --output-bin=&lt;src/models&gt; lvl.Ontology=&lt;@lvl/front-end&gt;


## Options
**--assembly-path**
Path to the dll which will have its POCO objects converted to TypeScript objects. Will detect types which implement IEntity.


**--output-bin**
The directory where the generated files will be placed.


**--decorator-path** *(optional)*
The path, relative to the output bin, to where the decorators can be found. If not provided, the default will be @lvl/front-end. This is intended for classes within @lvl/front-end, which cant reference itself.


**C# Namespace** *(multiple allowed)*
Associates a namespace with a npm package path. Since the generated models may depend on models from a different npm package, they need to be able to resolve them.


## Architecture
<img src="docs\typescript-generator-architecture.png" />


## Syntax
<img src="docs\typescript-generator-syntax.png" />
