## Goals
To build the middleware pipeline for web requests.

## Requirements
Must provide a method, UseWeb, which
1. Calls Microsoft's UseMvc

## Technical Considerations
It is critical that there are no dependencies on a specific domain.

Consider using the Microsoft.Extensions.DependencyInjection for consistency with Microsoft's pattern.