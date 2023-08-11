**Create a development certifciate**

> `dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\multitenancy-monolith.pfx" -p default`

> `dotnet dev-certs https --trust`

**Execute docker compose**

> `docker compose -p test`