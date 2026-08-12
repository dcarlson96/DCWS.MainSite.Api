# DCWS.MainSite.Api

`DCWS.MainSite.Api` is the standalone ASP.NET Core Web API for the [DC Web Systems](https://dcwebsystems.com) website. It establishes the backend API layer that the main website can consume as additional portfolio and business features are developed.

The initial implementation provides a small status endpoint and a conventional Web / Domain / Tests architecture designed to feel natural in Visual Studio.

## Technology

- .NET 10
- ASP.NET Core Web API with controllers
- Built-in dependency injection
- OpenAPI/Swagger UI in Development
- xUnit v3 unit tests

## Project structure

```text
DCWS.MainSite.Api/
├── DCWS.MainSite.Api.sln
├── src/
│   ├── DCWS.MainSite.Api.Web/       # Controllers, HTTP concerns, DI, and startup
│   └── DCWS.MainSite.Api.Domain/    # Contracts, services, models, and business logic
└── tests/
    └── DCWS.MainSite.Api.Tests/     # Domain unit tests
```

The dependency flow is intentionally one-way:

```text
HTTP request -> StatusController -> IStatusService -> StatusService -> StatusResponse
```

The Web project references Domain, and the Tests project tests Domain directly. Domain has no dependency on ASP.NET Core MVC.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Restore, build, and test

From the repository root:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

## Run locally

Run the Web project from the repository root:

```powershell
dotnet run --project src/DCWS.MainSite.Api.Web
```

The development launch profile listens on:

- HTTPS: `https://localhost:7194`
- HTTP: `http://localhost:5194`

Swagger UI is available in Development at:

`https://localhost:7194/swagger`

The generated OpenAPI document is available at:

`https://localhost:7194/swagger/v1/swagger.json`

## Status endpoint

Request:

```http
GET /api/status
```

Example response:

```json
{
  "message": "DC Web Systems API is running.",
  "status": "OK",
  "timestampUtc": "2026-08-11T12:00:00+00:00"
}
```

The timestamp is generated in UTC for each request.
