FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS base
USER app
WORKDIR /app
EXPOSE 5108
EXPOSE 7272

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
WORKDIR /src/frontend/Swagger
RUN dotnet restore "Swagger.csproj"
RUN dotnet build "Swagger.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Swagger.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Swagger.dll"]