FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS base
USER app
WORKDIR /app
EXPOSE 5015
EXPOSE 7053

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
WORKDIR /src/frontend/Portal
RUN dotnet restore "Portal.csproj"
RUN dotnet build "Portal.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Portal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Portal.dll"]