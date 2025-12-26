FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BookishAPI.csproj", "./"]
RUN dotnet restore "BookishAPI/BookishAPI.csproj"
COPY . .
WORKDIR "/src/BookishAPI"
RUN dotnet build "BookishAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookishAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookishAPI.dll"]