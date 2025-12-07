# Blockchain API

## Overview
A .NET 10 Web API application that fetches and stores blockchain data from BlockCypher APIs for ETH, BTC, DASH, and LTC.

## Architecture
- **Clean Architecture** with CQRS pattern
- **API Gateway Pattern** using Ocelot for routing, rate limiting, and QoS
- **Design Patterns**: Repository, Unit of Work, CQRS, Gateway
- **SOLID Principles** throughout
- **Proper Layer Dependencies**: Domain ← Application ← Infrastructure ← API
- **Pure Domain Entities**: No data annotations - configured via Fluent API

### Clean Architecture Compliance
✅ **Domain Layer**: No dependencies - pure POCOs without infrastructure concerns

✅ **Application Layer**: Depends only on Domain - contains interfaces and business logic

✅ **Infrastructure Layer**: Depends on Application & Domain - implements interfaces and configurations

✅ **API Layer**: Depends on all layers - composition root

✅ **API Gateway**: Single entry point with routing, rate limiting, and circuit breaker

**Key Architectural Decisions**:
1. `BlockchainApiDto` lives in Application layer (not Infrastructure) to maintain proper dependency flow
2. Domain entities are pure POCOs - all EF Core configuration is in `Infrastructure/Configurations` using Fluent API
3. Infrastructure implements `IBlockCypherClient` interface that returns Application DTOs
4. API Gateway (Ocelot) provides centralized routing, rate limiting, and quality of service

## Features
✅ Multiple blockchain data synchronization

✅ Historical data tracking with timestamps

✅ RESTful API with Swagger documentation

✅ **API Gateway with Ocelot** - routing, rate limiting, circuit breaker

✅ Health checks and CORS support

✅ Comprehensive logging

✅ Entity Framework Core with SQLite

✅ Docker support with multi-container setup

✅ Complete test coverage

## Technology Stack
- .NET 10.0
- Entity Framework Core
- SQLite
- MediatR (CQRS)
- AutoMapper
- xUnit
- FluentAssertions

## Getting Started

### Prerequisites
- .NET 10 SDK
- Docker (optional)

### Running Locally

```bash
# Clone the repository
git clone <repository-url>

cd BlockchainAPI

# Restore dependencies
dotnet restore BlockchainAPI.slnx

# Run the API
dotnet run --project src/API

# Run the API Gateway (in separate terminal)
dotnet run --project src/Gateway.API
```

**Access Points:**
- API Gateway: \`http://localhost:5066\`
- Direct API: \`http://localhost:5075\`
- Swagger UI: \`http://localhost:5075/swagger\`

### Running with Docker

```bash
docker-compose up --build
```

**Access Points (Docker):**
- API Gateway: \`https://localhost:5003\`
- Direct API: \`http://localhost:5000\`

## API Endpoints

### Through API Gateway (Recommended)

**Base URL: \`http://localhost:5000\`**

#### Blockchain Data
- \`POST /blockchain/sync/{blockchainType}\` - Sync blockchain data
  - Rate Limit: 30 requests per minute
- \`POST /blockchain/sync-all\` - Sync all blockchains
  - Rate Limit: 5 requests per 5 minutes
- \`GET /blockchain/{blockchainType}/history\` - Get blockchain history
  - Rate Limit: 100 requests per minute
  - Circuit Breaker: Breaks after 3 failures for 30 seconds
- \`GET /blockchain/all-history\` - Get all blockchain history
  - Rate Limit: 100 requests per minute

#### Health Check
- \`GET /health\` - Application health status

### Direct API Access

**Base URL: \`http://localhost:5000\`**

All endpoints available with \`/api\` prefix:
- \`POST /api/blockchain/sync/{blockchainType}\`
- \`POST /api/blockchain/sync-all\`
- \`GET /api/blockchain/{blockchainType}/history\`
- \`GET /api/blockchain/all-history\`
- \`GET /health\`

## API Gateway Features

### Rate Limiting
- **Sync endpoints**: Limited to prevent API abuse
- **Query endpoints**: Higher limits for read operations
- **Customizable**: Configure per route in ocelot.json

### Quality of Service (QoS)
- **Circuit Breaker**: Automatically stops calling failing services
- **Timeout**: 10 second timeout on requests
- **Retry Logic**: Breaks after 3 consecutive failures
- **Recovery**: Circuit closes after 30 seconds

### Benefits
- ✅ Single entry point for all API requests
- ✅ Centralized rate limiting and throttling
- ✅ Protection against downstream service failures
- ✅ Load balancing support (future enhancement)
- ✅ Request/Response logging
- ✅ Easy to add authentication/authorization

## Configuration

Update \`appsettings.json\`:

```json
{
  "BlockCypherApi": {
    "BaseUrl": "https://api.blockcypher.com/v1",
    "Timeout": 30
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=blockchain.db"
  }
}
```

## Testing

```bash
# Run all tests
dotnet test BlockchainAPI.slnx

# Run specific test project
dotnet test tests/UnitTests
dotnet test tests/IntegrationTests
dotnet test tests/FunctionalTests
```

### Test Projects

1. **UnitTests** - Isolated component testing
   - Service logic tests
   - Mock-based testing with Moq
   - Fast execution, no external dependencies

2. **IntegrationTests** - Component integration testing
   - Controller tests with TestServer
   - Database integration with InMemory provider
   - Service layer integration
   - Tests internal API contracts

3. **FunctionalTests** - End-to-end scenarios
   - Complete workflow testing
   - Real HTTP requests via WebApplicationFactory
   - Multi-step user scenarios
   - Pagination and ordering verification

## Project Structure

```
BlockchainAPI/
├── src/
│   ├── API/                    # Web API layer
│   ├── Gateway.API/             # Ocelot API Gateway
│   ├── Application/            # Business logic & CQRS
│   ├── Domain/                 # Pure domain entities
│   └── Infrastructure/         # Data access & external services
└── tests/
    ├── UnitTests/
    ├── IntegrationTests/
    └── FunctionalTests/
```

## Design Patterns

1. **Repository Pattern** - Data access abstraction
2. **Unit of Work** - Transaction management
3. **CQRS** - Command Query Responsibility Segregation using MediatR
4. **API Gateway** - Single entry point with Ocelot for routing and cross-cutting concerns

## Gateway Configuration

The API Gateway uses Ocelot with the following features:

### Rate Limiting Configuration
```json
{
  "RateLimitOptions": {
    "EnableRateLimiting": true,
    "Period": "1m",
    "PeriodTimespan": 60,
    "Limit": 30
  }
}
```

### Circuit Breaker (QoS)
```json
{
  "QoSOptions": {
    "ExceptionsAllowedBeforeBreaking": 3,
    "DurationOfBreak": 30000,
    "TimeoutValue": 10000
  }
}
```
