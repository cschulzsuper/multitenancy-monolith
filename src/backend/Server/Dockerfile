FROM mcr.microsoft.com/dotnet/nightly/aspnet:9.0-preview-jammy-chiseled AS base
USER app
WORKDIR /app
EXPOSE 5177
EXPOSE 7207

FROM mcr.microsoft.com/dotnet/nightly/sdk:9.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
WORKDIR /src/backend/Server
RUN dotnet restore "Server.csproj"
RUN dotnet build "Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Server.dll"]