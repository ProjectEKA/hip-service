Health Information Provider
===========

>
> Clinical establishments which generate or store customer data in
> digital form. These include hospitals, primary or secondary health
> care centres, nursing homes, diagnostic centres, clinics, medical
> device companies and other such entities as may be identified by
> regulatory authorities from time to time.

Motivation
==========

>
> Sample implementation of HIP which can be referred for future
> implementation of health information provider service.

Build Status
============

[![Build](https://github.com/ProjectEKA/hip-service/workflows/GitHub%20Actions/badge.svg)](https://github.com/ProjectEKA/hip-service/actions)

Code Style
==========

[C# Naming Conventions](https://github.com/ktaranov/naming-convention/blob/master/C%23%20Coding%20Standards%20and%20Naming%20Conventions.md)

Language/Frameworks
===================

- [C#](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/)
- [ASP.NET Core 3.1](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.1)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

Prerequisites
=============
- [dotnet core >=3.1](https://dotnet.microsoft.com/download)
- [docker >= 19.03.5](https://www.docker.com/)

Running
=======

- Change the current directory to hip-service project

```
cd hip-service
```

- Create docker image

```
docker build -t hip-service .
```

- Running the application

```
docker run -d -p 8000:80 hip-service
```

Running the tests
=================

- Change the current directory to hip-service-test project

```
cd hip-service-test
```

- Running the test 
```
dotnet test
```


Features
========

1. Discovery of a patient account
2. Linking of a patient with [Consent Manager](https://github.com/ProjectEKA/hdaf)
3. Consent artefact's acceptance
4. Data transfer

API Contract
===========

Once ran the application, navigate to

```
{HOST}/swagger/index.html
```





