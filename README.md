# API Integration

A .NET 10 backend that proxies [Fingrid Open Data](https://data.fingrid.fi/en/) with PostgreSQL caching, real-time SignalR viewer count, Polly resilience, and rate limiting.

## Architecture

Clean Architecture with four layers:

| Layer               | Purpose                                            |
|---------------------|----------------------------------------------------|
| **Domain**          | Entities, interfaces, value objects                |
| **Application**     | DTOs, mappers, service interfaces                  |
| **Infrastructure**  | EF Core, PostgreSQL repositories, migrations       |
| **Presenter.API**   | Controllers, services, SignalR hub, DI, middleware |
| **SharedKernel**    | Error record, Result pattern                       |
| **Unit test**       | Unit test cases                                    |

Key features:
- **DB caching** — batched PostgreSQL upsert with background cleanup
- **Polly** — outer timeout (3 min) → retry (3x on 429) → inner timeout (30s per attempt)
- **SignalR** — live viewer count via `/hubs/dashboard`
- **Rate limiting** — fixed window per endpoint
- **Security** — input validation, HSTS, sanitized error responses, Azure Key Vault

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- A [Fingrid API key](https://data.fingrid.fi/en/) (free registration)

## Getting Started

### 1. Clone

```bash
git clone https://github.com/BasilOmsha/api-integration.git
cd api-integration
```

### 2. Configure secrets

Store the API key and database connection string using .NET User Secrets:

```bash
cd api-integration.Presenter.API

dotnet user-secrets set "ConnectionStrings:fingrid" "YOUR_FINGRID_API_KEY"
dotnet user-secrets set "ConnectionStrings:postgres" "Host=localhost;Port=portnumber;Database=api_integration;Username=YOUR_USER;Password=YOUR_PASSWORD"
```

Verify:
```bash
dotnet user-secrets list
```

### 3. Run database migrations

From the solution root ('*\api-integration>'):

```bash
dotnet ef database update --project api-integration.Infrastructure --startup-project api-integration.Presenter.API
```

### 4. Run

```bash
cd api-integration.Presenter.API
dotnet watch
```

The server starts at:
- **HTTP:** http://localhost:5143
- **HTTPS:** https://localhost:7285
- **scalar:** http://localhost:5143/scalar (development only)

## Database Migrations

Create a new migration after changing entities or `AppDbContext`:

```bash
dotnet ef migrations add MigrationName --project api-integration.Infrastructure --startup-project api-integration.Presenter.API
```

Apply migrations:

```bash
dotnet ef database update --project api-integration.Infrastructure --startup-project api-integration.Presenter.API
```

Revert to a specific migration:

```bash
dotnet ef database update PreviousMigrationName --project api-integration.Infrastructure --startup-project api-integration.Presenter.API
```

## API Endpoints

**Base path:** `/api/v1/fingrid-meta-data`

| Method | Route                                         | Description                                                 |
|--------|-----------------------------------------------|-------------------------------------------------------------|
| GET    | `/{datasetId}`                                | Get cached metadata for a Fingrid dataset (1–999)           |
| GET    | `/{datasetId}/data?startTime=...&endTime=...` | Get time-series data (checks cache first, then Fingrid API) |

**Query parameters** for the data endpoint:
- `startTime` — ISO 8601 UTC datetime
- `endTime` — ISO 8601 UTC datetime

**Example:**
```
GET /api/v1/fingrid-meta-data/75/data?startTime=2025-03-01T00:00:00Z&endTime=2025-03-02T00:00:00Z
```

## Configuration

Defaults in `appsettings.json` / `appsettings.Development.json`:

```json
{
  "FingridApi": {
    "BaseUrl": "https://data.fingrid.fi/api/datasets/",
    "TimeoutSeconds": 30,
    "MaxRetryAttempts": 2,
    "RetryDelayMs": 1000
  }
}
```

Azure Key Vault is used in production for secrets. The vault URI is configured in `appsettings.json` under `KeyVault:VaultUri`.

## Data Source

All data comes from [Fingrid Open Data](https://data.fingrid.fi/en/), licensed under [Creative Commons Attribution 4.0](https://creativecommons.org/licenses/by/4.0/).
