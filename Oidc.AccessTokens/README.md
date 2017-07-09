# Oidc.AccessTokens
## Goals
Provide a way for resource servers to authenticate tokens.



## Config
```
{
	oidc: {
		resource-server: {
			// Address to the authorization server which can validate tokens.
			Authority: string,

			// Will reject http requests if true.
			RequireHttpsMetadata: boolean,

			// The name that this server is listed as on the authorization server.
			ApiName: string,

			// The secret authenticator for this server on the authorization server.
			ApiSecret: string
		}
	}
}
```



## Remarks
This is a simple opinionated wrapper around other providers access token libraries.