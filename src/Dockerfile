FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
ARG CODE_VERSION
RUN dotnet publish -r linux-x64 -c Release -o out -p:PublishSingleFile=true --self-contained true -p:InformationalVersion=${CODE_VERSION}

FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["./lignator"]