# API Integration Project

## Table of Contents
- [API Integration Project](#api-integration-project)
  - [Table of Contents](#table-of-contents)
  - [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Clone Repository](#clone-repository)
    - [Setup API Key](#setup-api-key)
    - [API Configuration DTO](#api-configuration-dto)
    - [Run the Server](#run-the-server)
  - [Dataset Information](#dataset-information)
    - [Total Electricity Consumption in Finnish Distribution Networks](#total-electricity-consumption-in-finnish-distribution-networks)
  - [API Reference](#api-reference)
    - [Authentication](#authentication)
    - [Key Endpoints](#key-endpoints)
      - [Get Dataset Data](#get-dataset-data)
      - [Get Multiple Datasets](#get-multiple-datasets)
      - [Get Latest Data](#get-latest-data)
    - [Supported Formats](#supported-formats)
    - [Common Parameters](#common-parameters)
    - [Example Response Structure](#example-response-structure)

## Getting Started

### Prerequisites
- .NET 10.0 SDK
- Git

### Clone Repository
```bash
git clone https://github.com/BasilOmsha/api-integration.git
cd api-integration
```

### Setup API Key
This project uses .NET User Secrets to store the Fingrid API key securely.

1. **Get your Fingrid API Key:**
   - Visit [Fingrid Open Data](https://data.fingrid.fi/en/)
   - Register for an account and obtain your API key

2. **Set the User Secret:**
   ```bash
   cd api-integration.Presenter.API
   dotnet user-secrets set "connStr:fingrid" "YOUR_FINGRID_API_KEY_HERE"
   ```

3. **Verify the secret is set:**
   ```bash
   dotnet user-secrets list
   ```

### API Configuration DTO
The application uses a strongly-typed configuration DTO pattern with runtime validation:

**Configuration DTO:**
```csharp
public class FingridApiConfigurations : IFingridApiConfiguration
{
    [Required][Url]
    public string? BaseUrl { get; set; }
    
    [Required]
    public string? ApiKey { get; set; }
    
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
    
    [Range(0, 10)]
    public int MaxRetryAttempts { get; set; } = 2;
}
```

**Dependency Injection Setup:**
The configuration is registered in `DependencyInjections.cs` with:
- **Binding from appsettings.json** using `configuration.GetSection().Bind()`
- **API key override** from User Secrets (`ConnStr:fingrid`)
- **Runtime validation** with `ValidateDataAnnotations()` and `ValidateOnStart()`
- **HttpClient integration** with automatic configuration injection

**Benefits:**
- Type-safe configuration access (no magic strings)
- Fail-fast validation at startup
- Secure secret management via User Secrets
- Environment-specific overrides via appsettings files
- Runtime null checks in HttpClient setup

**Configuration Files (appsettings / appsettings.Developement):**
```json
{
  "FingridApi": {
    "BaseUrl": "https://data.fingrid.fi/api",
    "TimeoutSeconds": 30,
    "MaxRetryAttempts": 2,
    "RetryDelayMs": 1000
  }
}
```

### Run the Server
```bash
# Navigate to the API project directory
cd api-integration.Presenter.API

# Run the application
dotnet watch
```

The server will start and be available at:
- **HTTP:** `http://localhost:5143`

**Available Endpoints:**
- `/fingrid` - Get electricity consumption data from Fingrid API

## Dataset Information

### Total Electricity Consumption in Finnish Distribution Networks

**Source:** [Fingrid Open Data - Dataset 363](https://data.fingrid.fi/en/datasets/363)

**Dataset Metadata:**
```json
{
    "id": 363,
    "modifiedAtUtc": "2024-10-30T07:25:28.000Z",
    "type": "timeseries",
    "status": "active",
    "organization": "Datahub",
    "nameFi": "Sähkönkulutus Suomen jakeluverkoissa yhteensä",
    "nameEn": "Total electricity consumption in Finnish distribution networks",
    "descriptionFi": "Suomessa sijaitsevien sähkön kulutuskäyttöpaikkojen mittausten yhteenlasketut summatiedot tunneittain jakeluverkoissa. Tiedot ovat peräisin suomalaisten jakeluverkonhaltijoiden datahubiin toimittamista mittaustiedoista. Tiedot toimitetaan päivittäin ja laskennan aikaväli on 1 kuukausi  ja tuorein tieto on -4 päivän takaista. Sähkönkulutus perustuu raportin muodostumishetken lukuihin ja se voi sisältää osittain arvioitua mittaustietoa. Arvioitu mittaustieto päivittyy pääsääntöisesti 11 päivän sisällä. Value-attribuutti sisältää sähkönkulutuksen yksikössä kilowattitunti (kWh) ja Count-attribuutti sisältää käyttöpaikkojen lukumäärän. Tiedot on julkaistu 1.8.2023 alkaen.",
    "descriptionEn": "Aggregate hourly metering data for electricity accounting points in Finnish distribution networks. The data is derived from metering data provided to Datahub by Finnish distribution system operators. The data is submitted daily, the calculation interval is 1 month and the latest data is -4 days. Electricity consumption reflects the figures available at the time the report was generated and may include partially estimated metering data. Estimated metering data is generally corrected within 11 days. The Value attribute contains electricity consumption measured in kilowatt-hours (kWh), and the Count attribute contains the number of accounting points. The data is available starting from 1.8.2023.",
    "dataPeriodFi": "1 h",
    "dataPeriodEn": "1 h",
    "updateCadenceFi": null,
    "updateCadenceEn": null,
    "unitFi": "kWh",
    "unitEn": "kWh",
    "contactPersons": "Datahub asiakaspalvelu: https://support.datahub.fi/fingrid?id=csm_public_form",
    "license": {
        "name": "Creative Commons Attribution",
        "termsLink": "https://creativecommons.org/licenses/by/4.0/"
    },
    "keyWordsFi": [
        "Sähkönkulutus",
        "Datahub"
    ],
    "keyWordsEn": [
        "Electricity consumption",
        "Datahub"
    ],
    "contentGroupsFi": [
        "Kulutus ja tuotanto"
    ],
    "contentGroupsEn": [
        "Consumption and production"
    ],
    "availableFormats": [
        "json",
        "csv",
        "xlsx",
        "xml"
    ],
    "dataAvailableFromUtc": null
}
```

## API Reference

**Base URL:** `https://data.fingrid.fi/api`

### Authentication
The API supports authentication via API key in header or query parameter:
- Header: `x-api-key: YOUR_API_KEY`
- Query: `?x-api-key=YOUR_API_KEY`

### Key Endpoints

#### Get Dataset Data
```
GET /datasets/{datasetId}/data
```
**Example for Dataset 363:**
```
GET /datasets/363/data?startTime=2025-10-30T00:00:00Z&endTime=2025-10-31T00:00:00Z&format=json
```

#### Get Multiple Datasets
```
GET /data?datasets=363&startTime=2025-10-30T00:00:00Z&endTime=2025-10-31T00:00:00Z
```

#### Get Latest Data
```
GET /datasets/363/data/latest
```

### Supported Formats
- JSON (default)
- CSV
- XML
- XLSX

### Common Parameters
- `startTime`: ISO 8601 datetime (UTC assumed if timezone not provided)
- `endTime`: ISO 8601 datetime (UTC assumed if timezone not provided)
- `format`: Response format (json, csv, xml)
- `page`: Page number (default: 1)
- `pageSize`: Results per page (1-20000, default: 10)
- `sortOrder`: asc or desc (default: asc)

### Example Response Structure
```json
{
  "data": [
    {
      "datasetId": 363,
      "startTime": "2025-10-30T23:00:00.000Z",
      "endTime": "2025-10-31T00:00:00.000Z",
      "value": 4558022.284,
      "additionalJson": {
        "TimeSeriesType": "CTT_SUM_CONS_ACP",
        "Res": "PT1H",
        "Uom": "KWH",
        "ReadTS": "2025-10-30T23:00:00Z",
        "Value": "4558022.284",
        "Count": "3162179"
      }
    }
  ],
  "pagination": {
    "total": 1,
    "lastPage": 1,
    "prevPage": null,
    "nextPage": null,
    "perPage": 10,
    "currentPage": 1,
    "from": 0,
    "to": 1
  }
}
```
