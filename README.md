## :hospital: Health Information Provider

> Clinical establishments which generate or store customer data in
> digital form. These include hospitals, primary or secondary health
> care centres, nursing homes, diagnostic centres, clinics, medical
> device companies and other such entities as may be identified by
> regulatory authorities from time to time.

## :muscle: Motivation

> Sample implementation of HIP which can be referred for future
> implementation of health information provider service.

## Build Status

[![Build](https://github.com/ProjectEKA/hip-service/workflows/master/badge.svg)](https://github.com/ProjectEKA/hip-service/actions)

## :+1: Code Style

[C# Naming Conventions](https://github.com/ktaranov/naming-convention/blob/master/C%23%20Coding%20Standards%20and%20Naming%20Conventions.md)

## :tada: Language/Frameworks

-   [C#](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/)
-   [ASP.NET Core 3.1](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.1)
-   [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
-   [Bogus](https://github.com/bchavez/Bogus)

## :checkered_flag: Requirements

-   [dotnet core >=3.1](https://dotnet.microsoft.com/download)
-   [docker >= 19.03.5](https://www.docker.com/)

## :whale: Running From The Docker Image

Create docker image

```
docker build -t hip-service hip-service/.
```

To run the image

```
docker run -d -p 8000:80 hip-service
```

To use docker compose locally

```
docker-compose up -d
```

To use docker compose to run pre-existing image from docker-hub

```
export {ENVIRONMENT_VARIABLE}={ENVIRONMENT_VALUE}
docker-compose -f docker-compose.yml -f docker-compose.{environment}.yml up -d

Example:
export IMAGE_TAG=13f9004
docker-compose -f docker-compose.yml -f docker-compose.development.yml up -d
```

## :rocket: Running From Source
To run 

```
dotnet run --environment="dev" --project src/In.ProjectEKA.HipService/In.ProjectEKA.HipService.csproj
```

## Running The Tests

To run the tests 
```
dotnet test test/In.ProjectEKA.HipServiceTest/In.ProjectEKA.HipServiceTest.csproj
```

## Features

1.  Discovery of a patient account
2.  Linking of a patient with [Consent Manager](https://github.com/ProjectEKA/hdaf)
3.  Consent artefact's acceptance
4.  Data transfer

## API Contract

Once ran the application, navigate to

```alpha
{HOST}/swagger/index.html
```

## Gateway and authentication stubs

In the docker compose file there are two additional containers for development purposes.
'gateway' is for simulating the Gateway service. It logs incoming requests to its console. Hence you can use it to get insight into the requests sent from HIP service to the Gateway service.
'oidc-server-mock' is used to simulate the OpenID Connect authentication provided by the Gateway service.
To make them work, you first need to get a authentication token from the oidc-server-mock by providing the configured clientid, client secret, grant type and scope (see these settings in the client configuration file: /gatewayStubConfigurations/oicdConfigs/clients-config.json). Using this authentication token as an Oath 2.0 token, you can send a request to the HIP service that will be accepted by the authentication.
Please note in the docker compose file the ports that these containers need to use to be able to run. 
You can use the described flow with any tooling, however, we included a sample Postman collection (/gatewayStubConfigurations/postmanCollections/HIPSamplePostmanCollections.json) that you can use for sending the requests, or for reference.
Please follow the following steps with the Postman collection:
1. make sure that the docker stack is running - in case needed, (re)build docker stack (docker-compose -f docker-compose.yml up --build -d)
2. open postman and import the collection
3. if you don't have, create an environment in postman (https://learning.postman.com/docs/sending-requests/managing-environments/)
4. run the Post Token request from the imported collection
5. run the Happy Case request from the imported collection -> you should get a 202 response
6. if you go go to the logs for the gateway stub container, you will see the response posted to gateway


## To view structured (json) logs

```sh
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y \
  -p 7000:80 \
  -p 5341:5341 \
  datalust/seq:latest
```
