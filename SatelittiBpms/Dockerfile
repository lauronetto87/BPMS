#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["SatelittiBpms/SatelittiBpms.csproj", "SatelittiBpms/"]
COPY ["SatelittiBpms.VersionNormalization/SatelittiBpms.VersionNormalization.csproj", "SatelittiBpms.VersionNormalization/"]
COPY ["SatelittiBpms.Analyzers/SatelittiBpms.Analyzers/SatelittiBpms.Analyzers.csproj", "SatelittiBpms.Analyzers/SatelittiBpms.Analyzers/"]
COPY ["SatelittiBpms.Services/SatelittiBpms.Services.csproj", "SatelittiBpms.Services/"]
COPY ["SatelittiBpms.Utilities/SatelittiBpms.Utilities.csproj", "SatelittiBpms.Utilities/"]
COPY ["SatelittiBpms.Translate/SatelittiBpms.Translate.csproj", "SatelittiBpms.Translate/"]
COPY ["SatelittiBpms.Models/SatelittiBpms.Models.csproj", "SatelittiBpms.Models/"]
COPY ["SatelittiBpms.Storage/SatelittiBpms.Storage.csproj", "SatelittiBpms.Storage/"]
COPY ["SatelittiBpms.Options/SatelittiBpms.Options.csproj", "SatelittiBpms.Options/"]
COPY ["SatelittiBpms.Workflow/SatelittiBpms.Workflow.csproj", "SatelittiBpms.Workflow/"]
COPY ["SatelittiBpms.Services.Interfaces/SatelittiBpms.Services.Interfaces.csproj", "SatelittiBpms.Services.Interfaces/"]
COPY ["SatelittiBpms.Repository.Interfaces/SatelittiBpms.Repository.Interfaces.csproj", "SatelittiBpms.Repository.Interfaces/"]
COPY ["SatelittiBpms.ApiGatewayManagementApi/SatelittiBpms.ApiGatewayManagementApi.csproj", "SatelittiBpms.ApiGatewayManagementApi/"]
COPY ["SatelittiBpms.ApiGatewayMock/SatelittiBpms.ApiGatewayMock.csproj", "SatelittiBpms.ApiGatewayMock/"]
COPY ["SatelittiBpms.Mail/SatelittiBpms.Mail.csproj", "SatelittiBpms.Mail/"]
COPY ["SatelittiBpms.Repository/SatelittiBpms.Repository.csproj", "SatelittiBpms.Repository/"]
COPY ["SatelittiBpms.Data/SatelittiBpms.Data.csproj", "SatelittiBpms.Data/"]
COPY ["SatelittiBpms.Authentication/SatelittiBpms.Authentication.csproj", "SatelittiBpms.Authentication/"]
RUN dotnet restore "SatelittiBpms/SatelittiBpms.csproj"
COPY . .
WORKDIR "/src/SatelittiBpms"
RUN dotnet build "SatelittiBpms.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SatelittiBpms.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SatelittiBpms.dll"]