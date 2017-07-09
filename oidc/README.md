# oidc

## Goals
To provide a set of components and servicecs to log in and manage users.



## Config
@lvl/oidc has the following options:
```
{
	// Server which issues tokens.
	authorizationServerUrl: string,

	// Identifier of the client registered on the authentication server.
	clientId: string,

	// The secret to validate the client identity.
	clientSecret: string,

	// The required scopes and permissions needed by the app to function.
	scopes: string[],

	// Roles which may be assigned by the administrator to users.
	roles: string[]
}
```