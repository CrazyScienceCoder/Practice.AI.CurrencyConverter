FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/WebApi/src/WebApi.csproj", "src/WebApi/src/"]
COPY ["src/Application/src/Application.csproj", "src/Application/src/"]
COPY ["src/Domain/src/Domain.csproj", "src/Domain/src/"]
COPY ["src/Infrastructure/src/Infrastructure.csproj", "src/Infrastructure/src/"]
COPY ["src/Frankfurter.ApiClient/src/Frankfurter.ApiClient.csproj", "src/Frankfurter.ApiClient/src/"]
RUN dotnet restore "./src/WebApi/src/WebApi.csproj"
COPY . .
WORKDIR "/src/src/WebApi/src"
RUN dotnet build "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Practice.Backend.CurrencyConverter.WebApi.dll"]