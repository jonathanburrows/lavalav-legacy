## Goals
To provide an executable which can seed oidc data during a deployment.



### Syntax
lvl.Oidc.AuthorizationServer.Seed --connection-string 'connection-string' [--seed-menditory] [-seed-test]



### Options
_--connection-string_: the database which will be populated with data.


_--seed-manditory (optional)_: If marked, will add data which is critical for the application to run.


_--seed-test (optional)_: If marked, will add data which can be used for tests and development.



### Example
lvl.Oidc.AuthorizationServer.Seed --connection-string "Server=.;Database=Test;Trusted_Connection=True;" --seed-manditory --seed-test



## Remarks
This is only a wrapper around classes in lvl.Oidc.AuthorizationServer, for unit tests, please check there.
