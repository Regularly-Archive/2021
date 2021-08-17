# base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CityService.csproj", ""]
RUN dotnet restore "./CityService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CityService.csproj" -c Release -o /app/build

# publish
FROM build AS publish
RUN dotnet publish "CityService.csproj" -c Release -o /app/publish

# wait-for-it
# FROM debian:buster-slim as wait-for-it
# RUN apt-get update && apt-get install -y "wait-for-it"

# final
FROM base AS final
WORKDIR /app
RUN apt-get update && apt-get -y install netcat
# RUN update-alternatives --config nc 
COPY --from=publish /app/publish .
# COPY --from=wait-for-it /usr/bin/wait-for-it .
HEALTHCHECK --interval=5s --timeout=3s \
  CMD curl -fs http://localhost:80/ || exit 1
ENTRYPOINT ["bash", "entrypoint.sh","-d", "redis_server:6379", "-c", "dotnet CityService.dll"]


