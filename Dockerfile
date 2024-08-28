FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
#COPY nuget.config ./

# Remove docker-compose project from sln references
RUN dotnet sln SatelittiBpms.sln remove docker-compose.dcproj

# Restore
RUN dotnet restore --configfile nuget.config

# Copy everything else and build
#COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:80

ENTRYPOINT ["dotnet", "SatelittiBpms.dll"]

