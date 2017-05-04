## Goals
Provide a script which can build, link, and run dotnet projects from an npm package.


## Syntax
dotnet-runner &gt;project-path&lt; [args]


## Example
dotnet-runner ../TypeScriptGenerator --assembly-path=../lvl.Ontology/bin/Debug/net461/lvl.Ontology.dll --output-bin=./src/models --decorator-path=./src/decorators


## Options
**project-path**

Denotes where the npm/C# project resides.


**args** *(optional)*

Arguments which will be passed to the dotnet executable.


## Requirements
An error is thrown if the project path does not exist.


An error is thrown if the project path does not contain a project.json.


An error is thrown if the project path does not contain a package.json.


An error is thrown if the package.json does not have a name value.


An error is thrown if the package.json does not have a bin folder.


If the project is not compiled, then it is compiled.


If the project has already been compiled, it is not recompiled.


If the package is not linked, then it is linked.


If the package has already been linked, then it is not linked again.


The executable in the bin folder is called.


All arguments given are passed to the executable when called.