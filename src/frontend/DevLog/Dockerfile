FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS base
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
COPY ./key-1a9b98c1-cf82-4cb8-a58a-b233c5a5be27.xml /home/app/.aspnet/DataProtection-Keys/key-1a9b98c1-cf82-4cb8-a58a-b233c5a5be27.xml
COPY ./key-1a9b98c1-cf82-4cb8-a58a-b233c5a5be27.xml /home/app/ASP.NET/DataProtection-Keys/key-1a9b98c1-cf82-4cb8-a58a-b233c5a5be27.xml
ENTRYPOINT ["dotnet", "DevLog.dll"]