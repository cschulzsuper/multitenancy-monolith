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
dotnet run --project ../src/tools/PreBuild -c Release -- --source-directory="../src/frontend/DevLog" --output-directory="../src/frontend/DevLog" --output-filename="appsettings.{0}.json"
dotnet run --project ../src/tools/PreBuild -c Release -- --source-directory="../src/frontend/Portal" --output-directory="../src/frontend/Portal" --output-filename="appsettings.{0}.json"
dotnet run --project ../src/tools/PreBuild -c Release -- --source-directory="../src/frontend/Swagger" --output-directory="../src/frontend/Swagger" --output-filename="appsettings.{0}.json"
dotnet run --project ../src/tools/PreBuild -c Release -- --source-directory="../src/server/Server" --output-directory="../src/server/Server" --output-filename="appsettings.{0}.json"

docker compose -p multitenancy-monolith build
docker compose -p multitenancy-monolith up -d
```

# Access

* DevLog: `https://localhost/dev-log`
* Swagger: `https://localhost/swagger`
