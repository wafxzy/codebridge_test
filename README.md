# Codebridge Dogs API

REST API for managing dog records with sorting, pagination, and validation.

## Tech Stack

- **.NET 9.0** - Web API framework
- **Entity Framework Core** - ORM with Code First migrations
- **SQL Server 2022** - Database
- **Docker & Docker Compose** - Containerization
- **Swagger/OpenAPI** - API documentation
- **FluentValidation** - Request validation

## Architecture

**Clean Architecture** with separated layers:

```
Codebridge.api/          # Presentation Layer (Controllers, Middleware)
codebridge.services/     # Business Logic Layer (BLL)
codebridge.repository/   # Data Access Layer (DAL)
codebridge.common/       # Shared (Models, DTOs, DbContext)
```

## Quick Start

### With Docker Compose

```bash
docker-compose up -d
```

Access Swagger UI: http://localhost:5254/swagger

### Local Development

```bash
cd Codebridge.api
dotnet run
```

## API Endpoints

- `GET /ping` - Health check
- `GET /dogs` - Get paginated list with sorting
- `POST /dog` - Create new dog record

## Features

- **Pagination & Sorting** - Query parameters for data filtering
- **Auto Migrations** - Database migrations applied on startup
- **Rate Limiting** - Request throttling middleware
- **Global Exception Handling** - Centralized error handling
- **CORS Enabled** - Cross-origin support
- **Seed Data** - Initial data (Neo, Jessy)
