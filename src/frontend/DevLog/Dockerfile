# TODO https://github.com/Humanizr/Humanizer/issues/1126
# TODO https://github.com/dotnet/dotnet-docker/discussions/4821
# FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5213
EXPOSE 7190

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
WORKDIR /src/frontend/DevLog
RUN dotnet restore "DevLog.csproj"
RUN dotnet build "DevLog.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "DevLog.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevLog.dll"]