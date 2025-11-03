FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

EXPOSE 5000

ENV CONFIGURATION="Release"
ENV PROJECT_NAME="LiftLog.Backend.Api"

FROM base AS restore

WORKDIR /src

COPY ./*.sln ./

COPY src/*/*.csproj ./

RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*} && mv $file src/${file%.*}; done

RUN dotnet restore src/${PROJECT_NAME}

COPY . .

FROM restore AS build

RUN dotnet build src/${PROJECT_NAME} -c ${CONFIGURATION}

RUN dotnet publish src/${PROJECT_NAME} --configuration ${CONFIGURATION} --no-build -o /app/www

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/www /app/www

WORKDIR /app/www

ENTRYPOINT ["dotnet", "LiftLog.Backend.Api.dll"]