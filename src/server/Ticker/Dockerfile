#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS base
USER app
WORKDIR /app
EXPOSE 5051
EXPOSE 7206

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src
COPY . .
WORKDIR /src/server/Ticker
RUN dotnet restore "Ticker.csproj"
RUN dotnet build "Ticker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ticker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ticker.dll"]