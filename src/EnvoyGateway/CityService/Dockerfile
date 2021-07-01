FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CityService.csproj", ""]
RUN dotnet restore "./CityService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CityService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CityService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CityService.dll"]