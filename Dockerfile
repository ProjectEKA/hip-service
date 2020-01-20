FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY HipServiceSrc.sln ./
COPY src/In.ProjectEKA.DefaultHip/*.csproj ./src/In.ProjectEKA.DefaultHip/
COPY src/In.ProjectEKA.HipService/*.csproj ./src/In.ProjectEKA.HipService/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /app/src/In.ProjectEKA.DefaultHip
RUN dotnet build -c Release -o /app

WORKDIR /app/src/In.ProjectEKA.HipService
RUN dotnet publish -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app .
ENTRYPOINT ["dotnet", "In.ProjectEKA.HipService.dll"]
EXPOSE 80