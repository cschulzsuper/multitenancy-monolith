# Clients

* `swagger`

  The `swagger` client must provide this value during authentication. 

  * The `swagger` client will have the `swagger-json` `scope` to read the `swagger.json`.
  * The `swagger` client will have the `endpoints` `scope` to access all `endpoints`.

* `endpoint-tests`

  The `endpoint-tests` must use this value for testing. 

  * The `endpoints-tests` client will have the `endpoints` `scope` to access all `endpoints`.

* `security-tests`

  The `security-tests` must use this value for testing. 

  * The `security-tests` client will have the `endpoints` `scope` to access all `endpoints`.
