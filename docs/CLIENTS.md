# Clients

## Frontend Clients

Frontend clients are applications or parts of an application that access backend clients.

### `swagger-ui`

  The `swagger-ui` client must provide this value during authentication. 

  * The `swagger-ui` client has the `openapi-json` `scope` to read the `*.json` files.
    * The `swagger-ui` loads the `*.json` files.
  * The `swagger-ui` client has the `endpoints` `scope` to access all `endpoints`.
    * This is required to access the REST API.

### `swagger-ui-host`

  The `swagger-ui-host` client must provide this value during authentication. 

  * The `swagger-ui-host` client has the `openapi-json` `scope` to read the `*.json` files.
    * The `swagger-ui-host` tests the availability of `*.json` files.
  * The `swagger-ui-host` client has the `endpoints` `scope` to access all `endpoints`.
    * This is **currently** required for token verification.

### `portal`

  The `portal` client must provide this value during authentication. 

  * The `portal` client has the `pages` `scope` to access all `portal` `pages`.
    * This is **currently** required for the temporary `portal` index page.
  * The `portal` client has the `endpoints` `scope` to access all `server` `endpoints`.
    * This is **currently** required for token verification.

### `dev-log`

  The `dev-log` client must provide this value during authentication. 

  * The `dev-log` client has the `pages` `scope` to access all `dev-log` `pages`.
    * This is required for the `dev-log` pages.
  * The `dev-log` client has the `endpoints` `scope` to access all `server` `endpoints`.
    * This is **currently** required for token verification.

## Test Clients

Test clients are only relevant in backend tests. Four different client names are used in different scenarios. 

* `endpoint-tests`
* `integration-tests`
* `multitenancy-tests`
* `security-tests`