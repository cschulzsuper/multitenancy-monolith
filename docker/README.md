# Setup

### Create a development certifciate

#### Used by ASP.NET Core
``` bash
dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\multitenancy-monolith.pfx" -p default
dotnet dev-certs https --trust
```

#### Used in `caddy`
``` bash
openssl pkcs12 -in ./multitenancy-monolith.pfx -clcerts -nokeys -out multitenancy-monolith.crt
openssl pkcs12 -in ./multitenancy-monolith.pfx -nocerts -nodes -out multitenancy-monolith.rsa
```

### Execute docker compose

``` bash
docker compose -p multitenancy-monolith up
```

# Access

* DevLog: `https://localhost/dev-log`
* Swagger: `https://localhost/swagger`
