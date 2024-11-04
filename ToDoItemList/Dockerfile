# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 17370
EXPOSE 17371


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["WebApplicationForToDoList/WebApplicationForToDoList.csproj", "WebApplicationForToDoList/"]
RUN dotnet restore "./WebApplicationForToDoList/WebApplicationForToDoList.csproj"
COPY . .
WORKDIR "/src/WebApplicationForToDoList"
RUN dotnet build "./WebApplicationForToDoList.csproj" -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./WebApplicationForToDoList.csproj" -o /app/publish /p:UseAppHost=true

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplicationForToDoList.dll", "--urls", "http://*:17370", "--environment", "Development"]
