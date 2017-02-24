## Goals
To provide a way to convert a set of string arguments into options for TypeScript generation.


## Requirements
If given a null argument, an ArgumentNullException is thrown.

If given arguments with no --assembly-path key, an ArgumentException is thrown.

If given arguments with multiple --assembly-path keys, an ArgumentException is thrown.

If given an --assembly-path which does not exist, a FileNotFoundException is thrown.

If given arguments with multiple --output-bin keys, an ArgumentException is thrown.

If given arguments with no --output-bin key, an ArgumentException is thrown.

If given arguments with multiple --decorator-path keys, an ArgumentException is thrown.

If given arguments with the same C# namespace keys, an ArgumentException is thrown.

, When the first, second, or last argument is the assembly path, that assembly path will be returned.

When the first, second, or last argument is the output bin, that output bin will be returned.

When the first, second, or last argument is the decorator path, that decorator path will be returned.

When the first, second, or last argument is a C# Namespace mapping, then it will be returned in the package map.

When there are two C# Namespace mappings, then the package map will have 2 entries.