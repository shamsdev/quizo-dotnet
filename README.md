# Quizo — Backend (ASP.NET Core)

Backend for **Quizo**, a real-time multiplayer quiz game. Exposes a Web API and a **Karizma Connection** hub for the Nuxt client. Built on .NET 8.

## Tech stack

- **.NET 8** — ASP.NET Core Web API
- **KarizmaConnection.Server** — Real-time hub and request/response API
- **Swagger / OpenAPI** — API docs (Swashbuckle)
- **Clean architecture** — Domain, Application, Infrastructure, API (QuizoDotnet)

## Solution structure

| Project | Purpose |
|--------|---------|
| **QuizoDotnet** | API host, Program.cs, CORS, Hub mapping, request handlers |
| **QuizoDotnet.Application** | Game logic, services, DTOs, interfaces |
| **QuizoDotnet.Domain** | Domain models and contracts |
| **QuizoDotnet.Infrastructure** | Data access, external services |

## Prerequisites

- **.NET 8 SDK** — [Download](https://dotnet.microsoft.com/download/dotnet/8.0)

## Run locally

From the **solution root** (where `QuizoDotnet.sln` is):

```bash
dotnet run --project QuizoDotnet
```

Or from the API project:

```bash
cd QuizoDotnet
dotnet run
```

- **HTTP:** [http://localhost:5005](http://localhost:5005) (see `launchSettings.json` for your profile)
- **Swagger UI:** [http://localhost:5005/swagger](http://localhost:5005/swagger) (when running with default profile)
- **Karizma Hub:** `http://localhost:5005/Hub` — used by the Nuxt client for real-time gameplay

## Configuration

- **appsettings.json** / **appsettings.Development.json** — Connection strings, CORS, and other settings. Use **appsettings.sample.json** as a template if needed.
- **CORS** — Allowed origins are configured in `Extensions/CorsConfigurator.cs` (development vs production).

Ensure the Nuxt app’s `SERVER_ENDPOINT` points to this backend’s Hub URL (e.g. `http://localhost:5005/Hub`).

## Build & test

```bash
# Restore and build
dotnet restore
dotnet build

# Run tests (if any)
dotnet test
```

## License

Private / as per your project license.
