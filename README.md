# lavalav
A collection of libraries and DevOps products. These products aim to reduce the time it takes to code, maintain, and release other products.



## Config
When setting up a C# application, the combined microservices will have the following configurations:
```
{
	domain: {
		// Connection String to be used by all the application.
		ConnectionString: string
	},
	logging: {
		LogLevel: Trace | Debug | Information | Warning | Error | Critical | None
	},
	cors: {
		AllowHeaders: string[],
		AllowMethods: string[],
		AllowOrigins: string[],
		ExposedHeaders: string[]
	},
	oidc: {
		authorization-server: {
			// If true, will populate data used in development and unit tests.
			SeedTestData: boolean,

			//If true, will populate data required by the application.
			SeedManditoryData: boolean (default true),

			// Will assign each new user the given roles.
			NewUserRoles: string[],

			// List of active directory servers which can be authenticated against.
			WindowsProviders: [{
				// Title which will be shown in GUIs.
				DisplayName: string,

				// The unique name of the provider.
				AuthenticationScheme: string
			}],

			// Information needed to use facebook as an Independent Service Provider.
			Facebook: {
				Id: string,
				Secret: string
			},

			// Information needed to use microsoft as an Independent Service Provider.
			Microsoft: {
				Id: string,
				Secret: string
			},

			// Information needed to use microsoft as an Independent Service Provider.
			Google: {
				Id: string,
				Secret: string
			}
		}
	}
}
```



## Project Architecture
<img src="docs/api-architecture.png" />

For details on an project's architecture, see its specific README.md