# Clients

## Frontend Clients

Frontend clients are applications or parts of an application that access backend clients.

### `swagger-ui`

  The `swagger-ui` client must provide this value during authentication. 

  * The `swagger-ui` client has the `swagger-json` `scope` to read the `swagger.json`.
    * The `swagger-ui` loads the `swagger.json` files.
  * The `swagger-ui` client has the `endpoints` `scope` to access all `endpoints`.
    * This is required to access the REST API.

### `swagger-ui-host`

  The `swagger-ui-host` client must provide this value during authentication. 

  * The `swagger-ui-host` client has the `swagger-json` `scope` to read the `swagger.json`.
    * The `swagger-ui-host` tests the availability of `swagger.json` files.
  * The `swagger-ui-host` client has the `endpoints` `scope` to access all `endpoints`.
    * This is **currently** required for token verification.

### `portal`

  The `portal` client must provide this value during authentication. 

  * The `portal` client has the `pages` `scope` to access all `portal` `pages`.
    * This is **currently** required for the temporary `portal` index page.
  * The `portal` client has the `endpoints` `scope` to access all `server` `endpoints`.
    * This is **currently** required for token verification.

### `dev-diary`

  The `dev-diary` client must provide this value during authentication. 

  * The `dev-diary` client has the `pages` `scope` to access all `dev-diary` `pages`.
    * This is required for the `dev-diary` pages.
  * The `dev-diary` client has the `endpoints` `scope` to access all `server` `endpoints`.
    * This is **currently** required for token verification.

## Test Clients

Test clients are only relevant in backend tests. Four different client names are used in different scenarios. 

* `endpoint-tests`
* `integration-tests`
* `multitenancy-tests`
* `security-tests`