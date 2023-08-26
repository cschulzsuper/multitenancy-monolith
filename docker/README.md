# Setup

### Create a development certifciate

``` bash
dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\multitenancy-monolith.pfx" -p default
```

``` bash
dotnet dev-certs https --trust
```

### Execute docker compose

``` bash
docker compose -p multitenancy-monolith up
```

# Access

### DevDiary

* Browser: `https://localhost:7190`
* Username: `admin`
* Password: `default`


### Swagger

* Browser: `https://localhost:7272`
* Username: `admin`
* Password: `default