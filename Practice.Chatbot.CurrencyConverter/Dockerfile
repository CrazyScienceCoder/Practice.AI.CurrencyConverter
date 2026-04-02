FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Chatbot project files
COPY ["Practice.Chatbot.CurrencyConverter/src/Domain/src/Domain.csproj", "Practice.Chatbot.CurrencyConverter/src/Domain/src/"]
COPY ["Practice.Chatbot.CurrencyConverter/src/Application/src/Application.csproj", "Practice.Chatbot.CurrencyConverter/src/Application/src/"]
COPY ["Practice.Chatbot.CurrencyConverter/src/Infrastructure/src/Infrastructure.csproj", "Practice.Chatbot.CurrencyConverter/src/Infrastructure/src/"]
COPY ["Practice.Chatbot.CurrencyConverter/src/WebApi/src/WebApi.csproj", "Practice.Chatbot.CurrencyConverter/src/WebApi/src/"]

RUN dotnet restore "Practice.Chatbot.CurrencyConverter/src/WebApi/src/WebApi.csproj"

COPY Practice.Chatbot.CurrencyConverter/ Practice.Chatbot.CurrencyConverter/

RUN dotnet build "Practice.Chatbot.CurrencyConverter/src/WebApi/src/WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Practice.Chatbot.CurrencyConverter/src/WebApi/src/WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Practice.Chatbot.CurrencyConverter.WebApi.dll"]
