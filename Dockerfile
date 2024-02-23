FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Install NativeAOT build prerequisites
RUN apt-get update && \
    apt-get install -y --no-install-recommends clang zlib1g-dev

COPY . ./
RUN dotnet restore

RUN dotnet publish -c Release -o out

EXPOSE 3000
ENV ASPNETCORE_URLS=http://+:3000

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "RinhaBackend.dll"]