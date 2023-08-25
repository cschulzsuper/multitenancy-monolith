#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS base
USER app
WORKDIR /app
EXPOSE 5108
EXPOSE 7272
EXPOSE 7273

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src
COPY . .
WORKDIR /src/server/Swagger
RUN dotnet restore "Swagger.csproj"
RUN dotnet build "Swagger.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Swagger.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Swagger.dll"]