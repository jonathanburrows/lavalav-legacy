# Oidc.AuthorizationServer
## Goals
Provide a microservice to issue and authenticate tokens.


## Endpoints
Supplies all endpoints required by openid (discovery, userinfo, ect).


Additionally, for custom business logic, the additional endpoints are provided:

`GET oidc/personal-details` fetches a flattened list of roles for a user.

`PUT oidc/personal-details` updates a users roles from a flat model.

`GET oidc/recover-username/<email>` will send an email to a user with their username.

`GET oidc/reset-password/request/<username>` will send an email to a user with a link to reset their password.

`PUT oidc/reset-password` will change a users password (if valid).


*For security purposes, the domain models are not accessible from the api.*

## Config
```
{
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

