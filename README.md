# Multitenancy Monolith

This repository contains everything related to the development of my multitenancy monolith based on ASP.NET Core.

The project focuses on the REST API, authentication, validation, business logic, database access layer and also some staging related deployment with docker compose.

If you are developing an ASP.NET Core application yourself and are looking for a different approach, you've come to the right place.

**I said monolith, but the monster is already evolving beyond that. 🚀 Check out the [container diagram](./docs/CONTAINERS.md) and you know why monolith does not fit anymore. I'll try to come up with new name, but for now the current one must do the job.**

## Getting Started

Clone the repository and start the [compose file](./docker/README.md).

## History

### Initial template
  * /1 In the initial commit I only created a single Visual Studio project from the built-in template and the GitHub repository. https://github.com/cschulzsuper/multitenancy-monolith/commit/0a18987ba7518789da227fce88915b55f3635d78
  * /2 I took the application logic from the template and moved it into an application layer. This layer is split into endpoints and transport. Endpoints only maps routes, while transport contains the logic to handle requests. https://github.com/cschulzsuper/multitenancy-monolith/commit/91a308bdb5b3593ac53061d309caf45837e36c45

### Custome authentication
  * /3 Some say that one should not use a custom authentication handling. I did it anyway. It uses a token that is hard-coded for now. https://github.com/cschulzsuper/multitenancy-monolith/commit/a328424d84ea316d9d7f3268dec9af1983d14a80
  * /4 A sign-in endpoint verifies the user credentials and returns a token. The credentials are still hard-coded and I ignore the existence of OIDC for the time being. https://github.com/cschulzsuper/multitenancy-monolith/commit/951e9ee81f93be4c1f3cd2f69d7e01c8f5790d3d
  * /5 Credentials need to be stored in storage. A user manager will give me that option. This manager allows to store generated tokens on every sign-in, but it still contains a hard-coded list of users. https://github.com/cschulzsuper/multitenancy-monolith/commit/24389e6a6a34e627b3ef73dfc70c624146c81054
  * /6 I added endpoint tests for the sign-in, based on WebApplicationFactory and `xUnit`. Additionally, I did some clean up and moved the user class, which is now called identity, into the data layer. https://github.com/cschulzsuper/multitenancy-monolith/commit/5b8d24570f230945a11e282691912b78265a76a7
  * /7 Preliminary definition of routes related to the complete authentication and authorization flow. As usual, the hardest part is the naming, and telling my inner OCD to shut up. 😆https://github.com/cschulzsuper/multitenancy-monolith/commit/d60eef5f527b82346eb3c3fd7ac915623fab1c31
  * /8 The token, which is returned from the sign-in endpoint, needs to be extendable. It is now evolving into some kind of JWT. I make use of an endpoint filter, that serializes claims into a JSON string, that gets encoded. https://github.com/cschulzsuper/multitenancy-monolith/commit/db104b99567b6c5a45d45737a4937820a61d3a42

### Multitenancy
  * /9 First step to make this monolith multitenantable (is that even a word?🤔). A second API call, after the sign-in, allows to take-up a group membership. Only users with a group member claim have access to the business logic. https://github.com/cschulzsuper/multitenancy-monolith/commit/c50d7d900e3cacb2c99c68f7fcc6d7b60939abc5
  * /10 After focusing on the authentication a refactoring was needed. I chose to align my authentication provider to the implementation of the default providers from #dotnet. Using an existing pattern might help others to comprehend what is going on. https://github.com/cschulzsuper/multitenancy-monolith/commit/e6a9601e3552bbd7dda0e6720e6082f7816f8347

### Clean up
  * /11 All endpoints should follow REST for the better part. Simple GET endpoints to query the currently available data have been added. Additionally, the seed data has been moved to the appsettings. https://github.com/cschulzsuper/multitenancy-monolith/commit/b681d3d26aaafd476e06660f51d4d984426a592f

### Cached authentication state
  * /12 Creating a wrapper around the user of the current request helps. It gives me the ability to inject the user into my request handler. Depending on the claims of the user I can control filtering, which results in perfect multitenancy queryability. https://github.com/cschulzsuper/multitenancy-monolith/commit/32360974747c31d694c90ca64afe4797cfe88be9
  * /13 A simple byte cache in the data layer is used to store an authentication verification value. During authentication this value is validated against the value in the token. Additionally, some renaming was necessary. https://github.com/cschulzsuper/multitenancy-monolith/commit/25a49abad32edbf7d989edb8884e4243e79a3013
  * /14 More authentication endpoint tests. In this example I want to write them from the beginning. We all know that it will pay off. Additional, I moved to `IClaimsTransformation` after meeting it on David Fowler's timeline.😱 https://github.com/cschulzsuper/multitenancy-monolith/commit/324c0d2e1b5200b245e5059359f841e1e2918be7

### Repository pattern
  * /15 Paving the way to a real data layer. But first I needed to modify the relationship between groups, members and identities. A membership allows for an identity to be bound to multiple members of different groups. One authentication, multiple accounts. https://github.com/cschulzsuper/multitenancy-monolith/commit/5571bb6a567f8475c83cac151c433f98dbe1a964
  * /16 Repository abstraction is now serving the data for the managers. Everything is still in-memory, but it will help me when I introduce an ORM. https://github.com/cschulzsuper/multitenancy-monolith/commit/ef6e486ec6d0a58833df14d9e057f7c0316b619a
  * /17 Setup of the initial repository data is now served via an insertion, instead of an injected dependency. The repository hides data store from the seed data, which allows to switch the data store to an ORM in the future. https://github.com/cschulzsuper/multitenancy-monolith/commit/45fc5cdc1ced6c0201f9e6bad6074a8ca7ffde57

### Snowflakes
  * /18 It's Christmas season, that's why I implemented some snowflakes with the help of Rob Janssen's IdGen package. Snowflake IDs will become the primary keys in the database. https://github.com/cschulzsuper/multitenancy-monolith/commit/25bf6ab3e0505d85a4479e2912111e1f74349ff6

### Validation
  * /19 Up until now no validation had been implemented. I added an endpoint that allows to create new members. The validation takes place in the management layer, right before calling the repository insert. https://github.com/cschulzsuper/multitenancy-monolith/commit/73d94c044b70ec210486bcfb671e77b6daf05383
  * /20 Validation is such an essential element that it makes sense to think about it a bit more. I implemented a shared validation component that does not depend on an external library. I want full control over the validation. https://github.com/cschulzsuper/multitenancy-monolith/commit/b9febca05bbbe399e0203809a9a90d3abc9b04d6

### Repository requirements
  * /21 Slowly extending the repository. Asynchronicity is key when talk to a database. With the static dictionary currently used this may not be exactly true, but it will become important once I switch to an ORM. https://github.com/cschulzsuper/multitenancy-monolith/commit/75fa043dbccc3bde11ac7271db026ca3b2d318e4
  * /22 Bulk deletions in the repository can be tricky, when the operation needs to be atomic. In the current static dictionary implementation I solve this requirement by using [SemaphoreSlim](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Threading/SemaphoreSlim.cs). https://github.com/cschulzsuper/multitenancy-monolith/commit/e6dcdbbbb0be44f4d647250f1e206c71ce7d4a77
  * /23 For the update method in the repository, I use an `Action` which will modify the entity. I will not support real bulk updates yet. After [#29654](https://github.com/dotnet/efcore/issues/29654) is implemented, I will have another look for an additional update method. https://github.com/cschulzsuper/multitenancy-monolith/commit/6e4bae6652d618885efa30f4b91a27928a9d6a52

### Error handling
  * /24 Simple adjustments add some generic error handling. It uses [problem details](https://www.rfc-editor.org/rfc/rfc7807) and supports swagger. Standardized support for error messages allows clients to use a generic error handling as well. https://github.com/cschulzsuper/multitenancy-monolith/commit/3b3dbb323d5be9b0ba19ee88a853dddbf46bc3fa
  * /25 Endpoint error messages help to increase the usefulness of the problem details. Additionally, it turned out that I forgot that the temporary static dictionary data provider does not cope well with update actions. I needed to make all entities `ICloneable`. https://github.com/cschulzsuper/multitenancy-monolith/commit/ac414583372631693cf59c33b839f79a8e94cf2d

### Role based authorization
  * /26 [Role requirements](./docs/ROLES.md) restrict endpoint access for authenticated identities and members. Security tests as part of endpoint tests ensure this permanently. https://github.com/cschulzsuper/multitenancy-monolith/commit/6a9c4801f9bf2ada9aae78be7143c2af5c5ecd53 

### Authorized swagger files
  * /27 [Client requirement](./docs/CLIENTS.md) in authentication and authorization request will later allow a restricted usage of badges. https://github.com/cschulzsuper/multitenancy-monolith/commit/03edcb32647649ea23f4beca53e4adb73924808a
  * /28 Only clients that authenticate as `swagger` client can access the `swagger.json` in production. In development this restriction is not applied. A security tests verifies the behavior. https://github.com/cschulzsuper/multitenancy-monolith/commit/a5e53c0c9fa505d76a551a5466e79e9040f645e0
  * /29 A nice gem is to make the swagger ui available in staging environments with the necessity of providing an access token for it. https://github.com/cschulzsuper/multitenancy-monolith/commit/c262d72a03ed0a7cb784a390acb3ba5501e352a9

### Domain specific swagger files
  * /30 Maybe it is nice for clients to have separated swagger files. I grouped endpoints by domains (`authentication`, `administration`, `business`, `foundation`) and provide a swagger file for each of them. https://github.com/cschulzsuper/multitenancy-monolith/commit/e6356978f1e1f9865900870d7053bd4935bac251

### Custom properties
  * /31 How about the ability to add custom properties to REST resources. I have defined management API for this feature, and regrouped the endpoints once again. https://github.com/cschulzsuper/multitenancy-monolith/commit/fa9e2b77d06613f4f417ab8c15c064587ab546a5 
  * /32 Major commit. The complete draft to manage custom properties including all endpoint tests and some improvements to the validation logic. An example business object endpoint is also implemented. https://github.com/cschulzsuper/multitenancy-monolith/commit/6afd43a64242e838a797f96d2b5b1cad648d0858
  * /33 Completion of custom properties for the business objects example entity. Refactoring of the repository service factories and extension of the entity constrains assertion in the repository. https://github.com/cschulzsuper/multitenancy-monolith/commit/816de3717c29827787ada5a235b6bfc3a178ae66

### Status code assertions
  * /34 Draft implementation to assert the error messages in the endpoint tests. Static classes are used to define the error messages. https://github.com/cschulzsuper/multitenancy-monolith/commit/f09e0b99e132bbd81ab806ab22965fc142b61d7e
  * /35 Extending the exception handling to allow better status codes in problem details. The status codes are asserted in the endpoint tests.https://github.com/cschulzsuper/multitenancy-monolith/commit/d1c48acea69f646447510391c48a7eb2f4d7c3b7

### Ticker 
  * /36 A hard test on my architecture. At some point the monolith will be separated. A ticket service the runs as a secondary service. 🤔 That was the easy part, the hard part was restructuring authentication and all endpoint tests again. https://github.com/cschulzsuper/multitenancy-monolith/commit/b27ca630f7627030521f3abddbd74c486183e3bb
  * /37 The separation of services caused hard coded urls as part of `Cors` and `Swagger`. Introducing components that provide configurations to the rest of the application will help to keep it flexibil. https://github.com/cschulzsuper/multitenancy-monolith/commit/8809d4cefdb494ce347f402ee03aaf2c2e84ba6d
  * /38 A confirm flow makes it possible to authenticate once as a ticker user before it is necessary to confirm the ticker user. An integration tests ensures the flow always succeeds. https://github.com/cschulzsuper/multitenancy-monolith/commit/22e3b96bfb1f6dff30540e4678726c2eaa6b0431
  * /39 A special `/post` endpoint allows a user to create a new ticker message. It is automatically created for his ticker user. https://github.com/cschulzsuper/multitenancy-monolith/commit/68bef72c842832e9d8332ba7e05ab5262d67d6ae

### Events
  * /40 An event storage abstraction allows publishing events from different places. The first implementation logs the events. Usage of `ITestOutputHelper` makes it possible to see the logs in the tests. https://github.com/cschulzsuper/multitenancy-monolith/commit/a454630ffc358c9fb5dbdbb2ef218fd7df231297
  * /41 Lightweight events via `System.Threading.Channels` and migration to `net8.0`. This event implementation should be enough for the moment. I'm kind of glad that I focused on tests early on. Green on all `683`. https://github.com/cschulzsuper/multitenancy-monolith/commit/a151383bf8b58ddf31676a9f3d6bcf1391f4b428
  * /42 Multitenancy events. This kind of implemenation is not my cup of tea, as some asynchronouse task handling is going on, but all tests give me the confidence that the implementaion is sufficient for now. https://github.com/cschulzsuper/multitenancy-monolith/commit/047a3597fa48fc715063d9ba5cd2c0ed8cef5d50
  * /43 Event publication interception for testability of event publication. Not a perfect solution for now, but it will work until I come up with something better. https://github.com/cschulzsuper/multitenancy-monolith/commit/9c15676cd6d1c49c612dc108abfd9a5f3baf4706
  
### Ticker Orchestration
  * /44 Handling the `ticker-message-inserted` to automatically create a bookmark for the current ticker user. Testing this event handler in an event tests, which is similar to the endpoint tests. https://github.com/cschulzsuper/multitenancy-monolith/commit/4e0691f61a4b52b6e33374759b2c1d1f30c12252
  * /45 Managing bookmarks for ticker messages is possible through simple endpoints. Nothing fancy in this commit. https://github.com/cschulzsuper/multitenancy-monolith/commit/c4d869658f43c915a46f0266b6f75f75ae1f51ca
  * /46 Managing ticker users via endpoints was not necessary until now. Again, nothing fancy in this commit as well. https://github.com/cschulzsuper/multitenancy-monolith/commit/ca6cfa169b5554d55bde43124a3e338dedaf2ae6
  * /47 Resetting the secret of a ticker user was an easy implementation. Mainly, because of the foundation that was implemented in all other commits. https://github.com/cschulzsuper/multitenancy-monolith/commit/15b57f89e5fb6219fe68eef0536ef41cfd54de17
  * /48 Productive day. The last bit in the ticker orchestration is the flagging of the ticker bookmark updated flag after the ticker message has been changed or deleted. https://github.com/cschulzsuper/multitenancy-monolith/commit/35016419362ad121c35f09757f6ba9f044af0738

### Authentictaion and authorization refactoring
  * /49 Introduction of prefixes for groups, members and identities. The new resources are `account-groups`, `account-members` and `authentication-identities`. Renaming of all related classes. https://github.com/cschulzsuper/multitenancy-monolith/commit/f1d7fd2e269451b1dd20c3cb8a7441e65f15b4a9
  * /50 The missing `account-groups` endpoint is implemented. With tests and some additional renaming that was forgotten in the previous commit. https://github.com/cschulzsuper/multitenancy-monolith/commit/e94cb99731e3eb28e73450deaa23d49d933ab876
  * /51 `HTTP HEAD` requests check for used `account-groups` and `authentication-identities`. Additionally, registration object definition for  `authentication-identities` and `account-groups`. They will be used in registration flows. https://github.com/cschulzsuper/multitenancy-monolith/commit/65aa4a787c9889f8cb8ef1891150ebb52ef5e415
  * /52 Registration endpoints for `authentication-identities` and `account-groups`. Not much more this time, apart from smaller code improvements. Seal classes whenever you can! https://github.com/cschulzsuper/multitenancy-monolith/commit/773663d77fe7745830561e5f84357244b2e0cdf6
  * /53 Registration flow components continues! This time confirmation and approval. https://github.com/cschulzsuper/multitenancy-monolith/commit/c6953dc7d51785499bce0ae68e70aca6afef2df1
  * /54 Implemented resource endpoints to manage `account-registration` and `authentication-registration`. Also renamed the `administration` domain to `extension`. https://github.com/cschulzsuper/multitenancy-monolith/commit/645dc09ae8da8ea64ac6ac1a25239e397eb52d84

### Back in action
  * /55 Fixes to produce a testable state. Refactored `Repository` to be able to trigger `ticker-bookmark` events. Evaluation of `ticker-bookmark` events in `ticker` integration tests. Changed and fixed JSON serialization of `IDictionary<string, object>` when serialized in asynchronous context. https://github.com/cschulzsuper/multitenancy-monolith/commit/6baa877e7ae3f30a28919dbbd7b047f85d4a9439
  * /56 Added `confirm` endpoint, logic and related tests to unmark `ticker-bookmark`. Changed event handler project name from `Events` to `Subscriptions`. https://github.com/cschulzsuper/multitenancy-monolith/commit/7bafe4651a8e8ff8eff73fa8c8e140d1b61278ce

### Background Jobs
  * /57 First draft of a very simple background job component, that exeutes jobs sequentially in a `HostedService`. For now, only simple logging heartbeat is implemented. https://github.com/cschulzsuper/multitenancy-monolith/commit/0c127587c5b1227f5dde1930f8618bb9d4c54fe7
  * /58 An HTTP PUT endpoint allows to change the schedule of a background job. Additionally, a changes were necessary to fix the swagger client and the authentication.https://github.com/cschulzsuper/multitenancy-monolith/commit/b065dfffa7235f78bbb8c37e7c3e72996672096d
  * /59 Entity and management to store the job schedule in the data layer. Additionally, changes in the validation. https://github.com/cschulzsuper/multitenancy-monolith/commit/08a50726bd84ae8ca0cc31474d4331ac326b7be6
  * /60 Renamed `job` to `planned-job`, implemented endpoints and tests related to background jobs. https://github.com/cschulzsuper/multitenancy-monolith/commit/868e25cc147011d1db746d8d1310b5f01c9e583f

### Bearer Token Refactoring
  * /61 Migration to the new [bearer token authentication](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.bearertokenextensions.addbearertoken?view=aspnetcore-8.0) of .NET 8 started, but this first step is only a build-up. I moved the `Claim` mapping from my custom authentication to the `ClaimsTransformation`. https://github.com/cschulzsuper/multitenancy-monolith/commit/81ad5b6b2338187ed977b929307b83bc8a06c8c0
  * /62 First draft with the refactored authentication. I was able to remove most of the code from the custom authentication I used before. I will probably iterate over it once more, but the first implementation seems to work. https://github.com/cschulzsuper/multitenancy-monolith/commit/bac94486fae1acbad3b22279997d23ba355c7627
  * /63 Some cleanup after the refactoring. I also removed all hard-coded URLs and fixed usings and namespaces. https://github.com/cschulzsuper/multitenancy-monolith/commit/803d86298bafa58bd6c02e416bcae6eafe6935f8

### Developer Experience
  * /64 Kick-off for the `docker-compose` support. I added a `Dockerfile` to the `Swagger` client and optimized a little bit of code in it. https://github.com/cschulzsuper/multitenancy-monolith/commit/b992958d76b902fdcf5679be6727dde2b60308de
  * /65 `Dockerfile` for `server` and `ticker`, and aditionally a component to add `SwaggerDocs` only if the corresponding `swagger.json` endpoint is available. https://github.com/cschulzsuper/multitenancy-monolith/commit/f3f33add9eb79e729b746077cb5e402ea3a6ee66
  * /66 Puh. A first `docker-compose.yml` is now integrated. Too have easy configuration of ports and host, I had to change some things in the configuration. There will be more `Docker` stuff, and I also have the feeling, that the configuration will change again. https://github.com/cschulzsuper/multitenancy-monolith/commit/b0f897aebd3b0a44413988e79e54fc2248977caa

### Swagger Staging Access
  * /67 It took me a bit, and I'm still amazed that it is actually working. I added the groundwork for a sign-in. For now, this is located in the `Swagger` project, and it is only a simple redirect, with credentials from the configuration. While implementing this, I also refactored quite a bit in the web service layer. https://github.com/cschulzsuper/multitenancy-monolith/commit/01c1db9a0bec0d5314d0b2a602d28a13f3482bba
  * /68 I turned the `Swagger` project into a .NET 8 Blazor SSR. The same redirect code, that was previously in the `sign-in` endpoint is now in a simple `sign-in` page. https://github.com/cschulzsuper/multitenancy-monolith/commit/b36a8f32d3682d16dcd3bb3183d88ab11ad39ad4
  * /69 A very minimalistic Blazor SSR `sign-in` page that still has some issues caused by .NET 8 preview 7, but I'm satisfied with this first implementation. https://github.com/cschulzsuper/multitenancy-monolith/commit/d200a4fe5070305c07fa9197a352d16f76f02a6c
  * /70 Changed some minor things. The biggest change is an additional port mapping for the `Swagger` client. I use this temporarily to make sure that the Blazor SSR does a full reload for the Swagger UI redirect after the sign-in. https://github.com/cschulzsuper/multitenancy-monolith/commit/89a771b7c68d7d1bc0fe309fa43d1c0eff35419b

### Development Log
  * /71 I have not touched the seed data implementation for quite a while, but the moment has arrived to give it a much-needed overhaul. Beyond its existing applications, I plan to integrate it into a development diary. In a first step, I've separated the sign-in functionality from the `Swagger` project and moved it into a new frontend layer. https://github.com/cschulzsuper/multitenancy-monolith/commit/d95d613a86d4abfd2260c2ff3c6d1dfdbf88713f
  * /72 I'm sticking to the wireframe styles, when it comes to the development diary. This was more pain than gain, given my limited interest in UI/UX design. https://github.com/cschulzsuper/multitenancy-monolith/commit/6bbc53f0620b4f077f8daaa69e7a17da4766478c
  * /73 Given that I'm not a thirteen-year-old girl, I renamed the development diary to development log. A new seed data implementation is used for its data. Additionally, the management and the data layer for the log have been implemented. https://github.com/cschulzsuper/multitenancy-monolith/commit/8b05c0e43cd393159cac4eb11b66ea03abd3c7a9
  * /74 The old seed data provider is gone, and I also changed other things related to the configuration, that is provided via `appsettings.json`. I will fill the development log, when I have something deployable. https://github.com/cschulzsuper/multitenancy-monolith/commit/0ce0bb460d3f1d2be2168da1dfab01b60bbafe48

### nginx Reverse Proxy
  * /75 I'm not an expert when it comes to nginx, so this is just some early script kiddy experiment. I added an nginx container with a simple nginx.conf. Adjustments in the API routes and `<base>` tags where necessary. It seems to work. https://github.com/cschulzsuper/multitenancy-monolith/commit/f707807d2f6867618aef48c2889f637033a0a145

### Bearer Token Clean Up
  * /76 After the recent _Bearer Token Refactoring_, I had to evaluate, if the `verification` keys, that are stored in the token, are still needed. The build-in `DataProtection` secures the token for my needs quite well. I decided now, to remove everthing releted to the `verification` and also made the token validation much simpler. 

# Backlog

* Blob Storage: Blob storage in data layer
* Blob Storage: Entity images
* ORM: Entity Framework In-Memroy provider
* ORM: Entity Framework SQL Server provider 
* SignalR: Entity notifications