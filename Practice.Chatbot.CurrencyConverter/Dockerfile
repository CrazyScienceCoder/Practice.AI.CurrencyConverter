FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Domain/src/Domain.csproj", "src/Domain/src/"]
COPY ["src/Application/src/Application.csproj", "src/Application/src/"]
COPY ["src/Infrastructure/src/Infrastructure.csproj", "src/Infrastructure/src/"]
COPY ["src/WebApi/src/WebApi.csproj", "src/WebApi/src/"]
RUN dotnet restore "src/WebApi/src/WebApi.csproj"
COPY . .
RUN dotnet publish "src/WebApi/src/WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Practice.Chatbot.CurrencyConverter.WebApi.dll"]
