# Setup

### Create a development certifciate

``` bash
dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\multitenancy-monolith.pfx" -p default
dotnet dev-certs https --trust
```

``` bash
openssl pkcs12 -in ./multitenancy-monolith.pfx -clcerts -nokeys -out multitenancy-monolith.crt
openssl pkcs12 -in ./multitenancy-monolith.pfx -nocerts -nodes -out multitenancy-monolith.rsa
```

### Execute docker compose

``` bash
docker compose -p multitenancy-monolith up
```

# Access

### DevLog

* Browser: `https://localhost:7190`
* Username: `demo`
* Password: `default`


### Swagger

* Browser: `https://localhost:7272`
* Username: `demo`
* Password: `default`