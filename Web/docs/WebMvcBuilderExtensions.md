## Goals
To build the middleware pipeline for web requests.

## Requirements
Must provide a method, UseWeb, which
1. Calls Microsoft's UseMvc
2. Sets up default routing
3. Can have the routes overridden by a parameter value

Must allow CORs options to be configured, with the behaviour:
1. The cors response containing any headers that are allowed, and supplied in the header
2. The cors response containing any non-standard methods that are allowed, and supplied in the header
3. The cors response containing any origins that are allowed, and supplied in the header
4. If no origin is given, a 404 is returned
5. If an unallowed origin is given, no headers are returned
6. If no method is given, a 404 is returned
7. If an unallowed method is given, no headers are returned
8. If an unallowed header is given, no headers are returned
9. When performing a normal request, any non-standard exposed headers are returned in the header

## Technical Considerations
It is critical that there are no dependencies on a specific domain.

Consider using the Microsoft.Extensions.DependencyInjection for consistency with Microsoft's pattern.

Unit tests might not be able to be written, as the Request Delegates are only privately accessible.