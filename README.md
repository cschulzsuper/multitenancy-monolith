
# Multitenancy Monolith

This repository contains everything related to the development of my multitenancy monolith based on ASP.NET Core.

The application focuses on the REST API, authentication and the database access layer.

If you are developing an ASP.NET Core application yourself and are looking for a different approach, you've come to the right place.

# History

* Initial template
  * /1 In the initial commit I only created a single Visual Studio project from the built-in template and the GitHub repository. https://github.com/cschulzsuper/multitenancy-monolith/commit/0a18987ba7518789da227fce88915b55f3635d78
  * /2 I took the application logic from the template and moved it into an application layer. This layer is split into endpoints and transport. Endpoints only maps routes, while transport contains the logic to handle requests. https://github.com/cschulzsuper/multitenancy-monolith/commit/91a308bdb5b3593ac53061d309caf45837e36c45

* Custome authentication
  * /3 Some say that one should not use a custom authentication handling. I did it anyway. It uses a token that is hard-coded for now. https://github.com/cschulzsuper/multitenancy-monolith/commit/a328424d84ea316d9d7f3268dec9af1983d14a80
  * /4 A sign-in endpoint verifies the user credentials and returns a token. The credentials are still hard-coded and I ignore the existence of OIDC for the time being. https://github.com/cschulzsuper/multitenancy-monolith/commit/951e9ee81f93be4c1f3cd2f69d7e01c8f5790d3d
  * /5 Credentials need to be stored in storage. A user manager will give me that option. This manager allows to store generated tokens on every sign-in, but it still contains a hard-coded list of users. https://github.com/cschulzsuper/multitenancy-monolith/commit/24389e6a6a34e627b3ef73dfc70c624146c81054
  * /6 I added endpoint tests for the sign-in, based on WebApplicationFactory and `xUnit`. Additionally, I did some clean up and moved the user class, which is now called identity, into the data layer. https://github.com/cschulzsuper/multitenancy-monolith/commit/5b8d24570f230945a11e282691912b78265a76a7
  * /7 Preliminary definition of routes related to the complete authentication and authorization flow. As usual, the hardest part is the naming, and telling my inner OCD to shut up. 😆https://github.com/cschulzsuper/multitenancy-monolith/commit/d60eef5f527b82346eb3c3fd7ac915623fab1c31
  * /8 The token, which is returned from the sign-in endpoint, needs to be extendable. It is now evolving into some kind of JWT. I make use of an endpoint filter, that serializes claims into a JSON string, that gets encoded. https://github.com/cschulzsuper/multitenancy-monolith/commit/db104b99567b6c5a45d45737a4937820a61d3a42

* Multitenancy
  * /9 First step to make this monolith multitenantable (is that even a word?🤔). A second API call, after the sign-in, allows to take-up a group membership. Only users with a group member claim have access to the business logic. https://github.com/cschulzsuper/multitenancy-monolith/commit/c50d7d900e3cacb2c99c68f7fcc6d7b60939abc5
  * /10 After focusing on the authentication a refactoring was needed. I chose to align my authentication provider to the implementation of the default providers from #dotnet. Using an existing pattern might help others to comprehend what is going on. https://github.com/cschulzsuper/multitenancy-monolith/commit/e6a9601e3552bbd7dda0e6720e6082f7816f8347

* Clean up
  * /11 All endpoints should follow REST for the better part. Simple GET endpoints to query the currently available data have been added. Additionally, the seed data has been moved to the appsettings. https://github.com/cschulzsuper/multitenancy-monolith/commit/b681d3d26aaafd476e06660f51d4d984426a592f

* Cached authentication state
  * /12 Creating a wrapper around the user of the current request helps. It gives me the ability to inject the user into my request handler. Depending on the claims of the user I can control filtering, which results in perfect multitenancy queryability. https://github.com/cschulzsuper/multitenancy-monolith/commit/32360974747c31d694c90ca64afe4797cfe88be9
  * /13 A simple byte cache in the data layer is used to store an authentication verification value. During authentication this value is validated against the value in the token. Additionally, some renaming was necessary. https://github.com/cschulzsuper/multitenancy-monolith/commit/25a49abad32edbf7d989edb8884e4243e79a3013
  * /14 More authentication endpoint tests. In this example I want to write them from the beginning. We all know that it will pay off. Additional, I moved to `IClaimsTransformation` after meeting it on David Fowler's timeline.😱 https://github.com/cschulzsuper/multitenancy-monolith/commit/324c0d2e1b5200b245e5059359f841e1e2918be7

* Repository pattern
  * /15 Paving the way to a real data layer. But first I needed to modify the relationship between groups, members and identities. A membership allows for an identity to be bound to multiple members of different groups. One authentication, multiple accounts. https://github.com/cschulzsuper/multitenancy-monolith/commit/5571bb6a567f8475c83cac151c433f98dbe1a964
  * /16 Repository abstraction is now serving the data for the managers. Everything is still in-memory, but it will help me when I introduce an ORM. https://github.com/cschulzsuper/multitenancy-monolith/commit/ef6e486ec6d0a58833df14d9e057f7c0316b619a
  * /17 Setup of the initial repository data is now served via an insertion, instead of an injected dependency. The repository hides data store from the seed data, which allows to switch the data store to an ORM in the future. https://github.com/cschulzsuper/multitenancy-monolith/commit/45fc5cdc1ced6c0201f9e6bad6074a8ca7ffde57

* Snowflakes
  * /18 It's Christmas season, that's why I implemented some snowflakes with the help of Rob Janssen's IdGen package. Snowflake IDs will become the primary keys in the database. https://github.com/cschulzsuper/multitenancy-monolith/commit/25bf6ab3e0505d85a4479e2912111e1f74349ff6

* Validation
  * /19 Up until now no validation had been implemented. I added an endpoint that allows to create new members. The validation takes place in the management layer, right before calling the repository insert. https://github.com/cschulzsuper/multitenancy-monolith/commit/73d94c044b70ec210486bcfb671e77b6daf05383
  * /20 Validation is such an essential element that it makes sense to think about it a bit more. I implemented a shared validation component that does not depend on an external library. I want full control over the validation. https://github.com/cschulzsuper/multitenancy-monolith/commit/b9febca05bbbe399e0203809a9a90d3abc9b04d6

* Repository requirements
  * /21 Slowly extending the repository. Asynchronicity is key when talk to a database. With the static dictionary currently used this may not be exactly true, but it will become important once I switch to an ORM. https://github.com/cschulzsuper/multitenancy-monolith/commit/75fa043dbccc3bde11ac7271db026ca3b2d318e4
  * /22 Bulk deletions in the repository can be tricky, when the operation needs to be atomic. In the current static dictionary implementation I solve this requirement by using [SemaphoreSlim](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Threading/SemaphoreSlim.cs). https://github.com/cschulzsuper/multitenancy-monolith/commit/e6dcdbbbb0be44f4d647250f1e206c71ce7d4a77
  * /23 For the update method in the repository, I use an `Action` which will modify the entity. I will not support real bulk updates yet. After [#29654](https://github.com/dotnet/efcore/issues/29654) is implemented, I will have another look for an additional update method. https://github.com/cschulzsuper/multitenancy-monolith/commit/6e4bae6652d618885efa30f4b91a27928a9d6a52

* Error handling
  * /24 Simple adjustments add some generic error handling. It uses [problem details](https://www.rfc-editor.org/rfc/rfc7807) and supports swagger. Standardized support for error messages allows clients to use a generic error handling as well. https://github.com/cschulzsuper/multitenancy-monolith/commit/3b3dbb323d5be9b0ba19ee88a853dddbf46bc3fa
  * /25 Endpoint error messages help to increase the usefulness of the problem details. Additionally, it turned out that I forgot that the temporary static dictionary data provider does not cope well with update actions. I needed to make all entities `ICloneable`. https://github.com/cschulzsuper/multitenancy-monolith/commit/ac414583372631693cf59c33b839f79a8e94cf2d

* Role based authorization
  * /26 [Role requirements](./docs/ROLES.md) restrict endpoint access for authenticated identities and members. Security tests as part of endpoint tests ensure this permanently. https://github.com/cschulzsuper/multitenancy-monolith/commit/6a9c4801f9bf2ada9aae78be7143c2af5c5ecd53 

* Authorized swagger files
  * /27 [Client requirement](./docs/CLIENTS.md) in authentication and authorization request will later allow a restricted usage of badges. https://github.com/cschulzsuper/multitenancy-monolith/commit/03edcb32647649ea23f4beca53e4adb73924808a
  * /28 Only clients that authenticate as `swagger` client can access the `swagger.json` in production. In development this restriction is not applied. A security tests verifies the behavior. https://github.com/cschulzsuper/multitenancy-monolith/commit/a5e53c0c9fa505d76a551a5466e79e9040f645e0
  * /29 A nice gem is to make the swagger ui available in staging environments with the necessity of providing an access token for it. https://github.com/cschulzsuper/multitenancy-monolith/commit/c262d72a03ed0a7cb784a390acb3ba5501e352a9

* Domain specific swagger files
  * /30 Maybe it is nice for clients to have separated swagger files. I grouped endpoints by domains (`authentication`, `administration`, `business`, `foundation`) and provide a swagger file for each of them. 

# Next

* Custom properties: Aggregate extension management
* Custom properties: Distinct aggregate type management
* Custom properties: Index properties in data layer

# Backlog

* ORM: Entity Framework In-Memroy provider
* ORM: Entity Framework SQL Server provider 
* Business: Example endpoint
