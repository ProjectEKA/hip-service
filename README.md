## :hospital: Health Information Provider

>
> Clinical establishments which generate or store customer data in
> digital form. These include hospitals, primary or secondary health
> care centres, nursing homes, diagnostic centres, clinics, medical
> device companies and other such entities as may be identified by
> regulatory authorities from time to time.

## :muscle: Motivation

>
> Sample implementation of HIP which can be referred for future
> implementation of health information provider service.

## Build Status

[![Build](https://github.com/ProjectEKA/hip-service/workflows/GitHub%20Actions/badge.svg)](https://github.com/ProjectEKA/hip-service/actions)

## :+1: Code Style


[C# Naming Conventions](https://github.com/ktaranov/naming-convention/blob/master/C%23%20Coding%20Standards%20and%20Naming%20Conventions.md)

## :tada: Language/Frameworks

- [C#](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/)
- [ASP.NET Core 3.1](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.1)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## :checkered_flag: Requirements

- [dotnet core >=3.1](https://dotnet.microsoft.com/download)
- [docker >= 19.03.5](https://www.docker.com/)

## :whale: Running From The Docker Image

Create docker image

```
docker build -t hip-service hip-service/.
```

To run the image

```
docker run -d -p 8000:80 hip-service
```

## :rocket: Running From Source
To run 

```
dotnet run --environment="dev" --project hip-service/hip-service.csproj
```

## Running The Tests

To run the tests 
```
dotnet test hip-service-test
```


## Features

1. Discovery of a patient account
2. Linking of a patient with [Consent Manager](https://github.com/ProjectEKA/hdaf)
3. Consent artefact's acceptance
4. Data transfer

## API Contract

Once ran the application, navigate to

```
{HOST}/swagger/index.html
```





