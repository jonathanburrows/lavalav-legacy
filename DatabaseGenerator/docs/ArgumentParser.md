## Goals
To move the logic and validation of parsing command line arguments into a class which can be unit tested.

## Requirements
Provide a class with a single function, Parse, which will:
1. When a connection string is specified first in the argument list, populates the connection string
2. When a connection string is speficied last in the argument list, populates the connection string
3. When a connection string is specified in the middle of an argument list, populates the connection string
4. Throws an ArgumentException if the connection string switch is missing from the argument list
5. Throws an ArgumentException if the connection string switch is present without a value in the argument list
6. When a assembly path is specified first in the argument list, populates the assembly path
7. When a assembly path is speficied last in the argument list, populates the assembly path
8. When a assembly path is specified in the middle of an argument list, populates the assembly path
9. Throws an ArgumentException if the assembly path switch is missing from the argument list
10. Throws an ArgumentException if the assembly path switch is present without a value in the argument list
11. When a pre-generation script bin path is specified first in the argument list, populates the pre-generation script bin path
12. When a pre-generation script bin path is speficied last in the argument list, populates the pre-generation script bin path
13. When a pre-generation script bin path is specified in the middle of an argument list, populates the pre-generation script bin path
14. Throws an ArgumentException if the pre-generation script bin path switch is present without a value in the argument
15. Throws an ArgumentException if the assembly path switch is present without a value in the argument list
16. When a post-generation script bin path is specified first in the argument list, populates the post-generation script bin path
17. When a post-generation script bin path is speficied last in the argument list, populates the post-generation script bin path
18. When a post-generation script bin path is specified in the middle of an argument list, populates the post-generation script bin path
19. Throws an ArgumentException if the post-generation script bin path switch is present without a value in the argument

## Technical Considerations
To make this unit testable, consider not performing any validation checks which involve the file system, that may be handled in a high layer.