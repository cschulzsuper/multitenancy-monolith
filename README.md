# Multitenancy Monolith

This repository contains everything related to the development of my multitenancy monolith based on ASP.NET Core.

The project focuses on the REST API, authentication, validation, business logic, database access layer and also some staging related deployment with docker compose.

If you are developing an ASP.NET Core application yourself and are looking for a different approach, you've come to the right place.

**I said monolith, but the monster is already evolving beyond that. 🚀 Check out the [container diagram](./docs/CONTAINERS.md) and you know why monolith does not fit anymore. I'll try to come up with new name, but for now the current one must do the job.**

## Getting Started

Clone the repository and start the [compose file](./docker/README.md).

## History

A [dev-log](https://mmdevlogapp.azurewebsites.net/dev-log) is hosted on [Azure](https://azure.microsoft.com/). It runs in `staging` mode, has guest authentication, should be up-to-date, **but is disabled for now**. 

# Backlog

* Blob Storage: Blob storage in data layer
* Blob Storage: Entity images
* ORM: Entity Framework In-Memroy provider
* ORM: Entity Framework SQL Server provider 
* SignalR: Entity notifications