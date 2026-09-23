# Procurement and Inventory Management System

Backend system for managing procurement and inventory processes in a small and medium sized business context. The project demonstrates the implementation of a production oriented .NET 10 backend using Clean Architecture, CQRS, domain events, authentication and authorization, automated testing, containerization, observability, and cloud deployment practices.

## Overview

The motivation behind this project came from researching the type of software commonly used in modern business environments, including enterprise platforms and solutions focused on internal processes such as procurement, inventory, logistics, and supplier management.

Based on that research, I chose procurement and inventory management as a representative domain. The project was designed as an environment to apply backend engineering practices, including architecture, security, testing, API design, containerization, and cloud development.

The system is designed around common workflows such as:

- Product and supplier management.
- Purchase orders and receiving management.
- Stock movements and low stock monitoring.

**Note:** The project is focused strictly on backend development. No frontend application is included.

## Features

- Product and category management.
- Supplier management.
- Inventory management and stock movements.
- Minimum stock level configuration and low stock monitoring.
- Purchase order creation and lifecycle management.
- Purchase order receiving and receipt management.
- User authentication using ASP.NET Core Identity and JWT.
- Role based and policy based authorization.
- Auditing and optimistic concurrency.
- Domain events using MediatR.
- Background inventory monitoring.
- Notification abstraction for low stock events.
- Automated unit and integration testing.
- SQL Server integration testing with Testcontainers.
- Docker and Docker Compose support.
- CI/CD with GitHub Actions.
- Azure App Service deployment.
- Azure Key Vault integration.
- Structured logging with Serilog.
- OpenTelemetry tracing.
- Correlation IDs and health checks.
- Swagger / OpenAPI documentation.

## Architecture

The solution follows Clean Architecture principles, keeping the domain and application logic independent from infrastructure and API concerns.

<img src="docs/images/project-structure.png" alt="Project structure" width="30%"/>

### Solution Structure

```text
src/
├── Procurement.Domain
├── Procurement.Application
├── Procurement.Infrastructure
├── Procurement.Api
└── Procurement.Worker

tests/
├── Procurement.UnitTests
└── Procurement.IntegrationTests
```

### Project Responsibilities

- **Procurement.Domain:** Contains the core business model, including entities, business rules, domain state transitions, and domain events.
- **Procurement.Application:** Contains application use cases implemented through CQRS commands and queries, MediatR handlers, DTOs, validators, domain event handlers, and application interfaces.
- **Procurement.Infrastructure:** Contains implementations for persistence and infrastructure concerns, including Entity Framework Core, SQL Server, ASP.NET Core Identity, JWT generation, domain event dispatching, notifications, and database seeding.
- **Procurement.Api:** Exposes the application through HTTP APIs and contains controllers, authentication and authorization configuration, middleware, Swagger, health checks, exception handling, and application startup configuration.
- **Procurement.Worker:** A separate .NET Worker responsible for monitoring inventory and processing low stock notifications in the background.

## Technology Stack

| Technology            | Purpose                                                 |
| --------------------- | ------------------------------------------------------- |
| .NET 10               | Backend platform                                        |
| C#                    | Programming language                                    |
| ASP.NET Core          | Web API                                                 |
| ASP.NET Core Identity | User and role management                                |
| Entity Framework Core | Data access                                             |
| SQL Server            | Relational database                                     |
| MediatR               | CQRS and domain event handling                          |
| FluentValidation      | Request and command validation                          |
| JWT Bearer            | API authentication                                      |
| xUnit                 | Testing framework                                       |
| Moq                   | Mocking                                                 |
| SQLite                | Lightweight relational database for selected unit tests |
| Bogus                 | Test and seed data generation                           |
| Testcontainers        | SQL Server integration testing                          |
| Docker                | Containerization                                        |
| Docker Compose        | Local development environment                           |
| Serilog               | Structured logging                                      |
| OpenTelemetry         | Tracing instrumentation                                 |
| Azure Key Vault       | Secret management                                       |
| Azure App Service     | API hosting                                             |
| Azure SQL Database    | Cloud database                                          |
| GitHub Actions        | CI/CD                                                   |
| Swagger / OpenAPI     | API documentation                                       |

## Authentication and Authorization

The application uses ASP.NET Core Identity for user and role management and JWT bearer authentication for API access.

- **Login Flow (`POST /api/auth/login`):** Requests are verified by Identity. Upon successful authentication, a JWT containing the user's identifier and role claims is generated.
  ```text
  Login request
      ↓
  ASP.NET Core Identity
      ↓
  Password verification
      ↓
  JWT generation
      ↓
  Protected API endpoints
  ```
- **Identity Configuration:** Enforces 12-character minimum passwords (requiring at least one digit, uppercase, and lowercase letter). Accounts are locked for 15 minutes after five failed attempts.
- **Swagger:** Includes built in Bearer authentication support for testing protected API endpoints.

* **Roles and Policies:**

  | Role          | Description                                                   | Policy             |
  | ------------- | ------------------------------------------------------------- | ------------------ |
  | **SuperUser** | Full access. Bypasses Purchaser and Warehouse restrictions.   | `RequireSuperUser` |
  | **Purchaser** | Manages products, categories, suppliers, and purchase orders. | `RequirePurchaser` |
  | **Warehouse** | Manages inventory and receiving operations.                   | `RequireWarehouse` |

* **Demo Users:** Development and staging environments are seeded with the following accounts (these are strictly excluded from Production):

  | Username    | Role      | Password        |
  | ----------- | --------- | --------------- |
  | `superuser` | SuperUser | `SuperUser123!` |
  | `purchaser` | Purchaser | `Purchaser123!` |
  | `warehouse` | Warehouse | `Warehouse123!` |

## Validation

Application commands and requests are validated using FluentValidation.
Validators are integrated into the application request pipeline and execute before command handlers process requests.
This keeps validation rules separate from application handlers and provides consistent validation across the API.

## Domain Events

The application uses domain events to separate important business state changes from the actions that may need to happen afterward.

Entities collect domain events while their state changes. The events are dispatched through MediatR after successful persistence.

### Purchase Order Events

Lifecycle events processed at the application level for logging:

- PurchaseOrderCreatedEvent
- PurchaseOrderApprovedEvent
- PurchaseOrderRejectedEvent
- PurchaseOrderCancelledEvent
- PurchaseOrderPartiallyReceivedEvent
- PurchaseOrderCompletedEvent

### Low Stock Events

Triggered when: `CurrentQuantity <= MinimumStockLevel`

- **State transition:** Detects the initial drop below minimum stock to prevent duplicate events.
- **Execution context:** Handlers may execute before outer database transactions commit.
- **Future scaling:** Adding external side effects (e.g., notifications) will require implementing an Outbox pattern.

## Notifications

The application defines a provider independent `INotificationService` abstraction for notifications generated by application events.

The current implementation logs notifications instead of sending them through an external email, SMS, or messaging provider.

This keeps the application independent from a specific notification provider while leaving the integration point available for a future implementation.

## Background Processing

The project includes a separate .NET Worker that monitors inventory in the background.
The InventoryMonitoringWorker:

- Runs periodically every five minutes.
- Checks products against their minimum stock level.
- Processes low stock notifications.
- Uses the notification service abstraction.
- Keeps track of products already notified during the current Worker execution.

This prevents repeated notifications while a product remains below its minimum stock level.
If inventory recovers and later becomes low again, the product can enter a new notification cycle.

## Database

The application uses SQL Server with Entity Framework Core.
The database contains the main procurement and inventory entities as well as the ASP.NET Core Identity tables.

### Migrations

- **Coverage:** EF Core migrations include initial database creation, user integration, inventory concurrency, and ASP.NET Core Identity.
- **Execution:** Applied automatically at application startup in SQL Server environments (except `Testing`).

### Concurrency

- **Mechanism:** Optimistic concurrency using SQL Server `rowversion` on inventory records.
- **Conflict Handling:** Modifications using an outdated row version are rejected and return a `409 Conflict` HTTP status.

### Auditing

- **Tracked Fields:** Relevant entities include `CreatedBy`, `UpdatedBy`, and `UpdatedAt`.
- **Data Source:** Audit information is automatically populated using the authenticated user's context.

### Data Seeding

- **Development and Staging:** Seeds users and sample business data for testing and local development.
- **Production:** Creates only required roles, omits development users and sample data.

## Testing

The solution contains separate unit and integration test projects:

```text
tests/
├── Procurement.UnitTests
└── Procurement.IntegrationTests
```

The current test suite contains 118 tests total (73 unit tests, 45 integration tests).

- **Unit Tests:** Unit tests use xUnit and Moq. SQLite is used for tests that require relational database behavior, while Bogus is used to generate realistic test data where appropriate.
- **Integration Tests:** Integration tests run the API through ASP.NET Core's test hosting infrastructure and use Testcontainers to create a temporary SQL Server container. This allows the integration tests to validate the application against the same relational database technology used by the application. The integration environment also applies the application's EF Core migrations when the test database is created.
- **Test Authentication:** Integration tests use a dedicated `TestAuthenticationHandler` to simulate authenticated users with specific roles. This allows authorization behavior to be tested without depending on the production login flow for every test.

## Security and Configuration

Sensitive configuration is kept outside source control, supporting multiple providers (application files, environment variables, .NET User Secrets, Docker Compose, and Azure Key Vault).

- **Secrets Management:** No secret values are stored as insecure defaults. The `.env` file is excluded from Git, while `.env.example` documents expected configuration keys.
- **JWT Validation:** Startup checks enforce signing key presence and minimum length, issuer, audience, and token expiration. Bearer authentication validation includes a one-minute clock tolerance.
- **Azure Key Vault:** Staging and Production environments load secrets dynamically when `KeyVaultUri` is configured. `DefaultAzureCredential` handles passwordless authentication to Azure services.

## Docker and Local Development

The repository includes a multi stage Dockerfile and a Docker Compose configuration for running the API and SQL Server locally.

<img src="docs/images/docker-compose.png" alt="Docker compose configuration" width="95%"/>

- **Docker Compose:** The local environment contains the Procurement API and SQL Server 2022.
  - **API Endpoints:**
    - Swagger: http://localhost:8080/swagger
    - Health Check: http://localhost:8080/health
  - **Database:** SQL Server is exposed at `localhost:1434` (uses standard port `1433` inside the Docker network).
  - **Commands:**
    - Start: `docker compose up --build`
    - Stop: `docker compose down`
- **Local Configuration:** Docker Compose supports configuration through environment variables. Sensitive local values can be provided through a local `.env` file or other local configuration mechanisms. The `.env` file is excluded from Git, while `.env.example` provides an example of the expected configuration values without containing real secrets.

## Observability

The API includes logging, tracing, correlation IDs, and health checks to support application diagnostics.

- **Logging:** Uses Serilog for structured application and request logging. Logs automatically include application context and request correlation information.
- **Correlation IDs:** Accepts client-provided `X-Correlation-ID` headers or generates a new ID per request. The ID is returned in the response and injected into the logging context for tracing.
- **OpenTelemetry:** Configured for ASP.NET Core tracing instrumentation. Currently exports tracing data to the console (no external observability backend is configured).
- **Health Checks:** Exposes a `GET /health` endpoint to verify API status and SQL Server connectivity.

## CI/CD

The project uses GitHub Actions for automated validation and Azure deployment via the `.github/workflows/main-pipeline.yml` workflow. It triggers automatically on pushes to `develop` and `main` (or manually), with Azure access managed through the configured GitHub Actions integration.

<img src="docs/images/github-actions.png" alt="GitHub Actions workflow" width="95%"/>

- **Build and Test:** The pipeline restores the solution, runs automated tests, builds in Release configuration, and stores the published API as a deployment artifact.
- **Staging:** Pushes to `develop` deploy the API to the `procurement-inventory-sf-staging` App Service.
- **Production:** Pushes to `main` deploy the API to the `procurement-inventory-sf` App Service.

## Azure

The project is configured to use Azure for cloud hosting and supporting services. Application configuration is included, but resource provisioning and access are managed outside this repository.

- **Azure App Service:** Hosts the API.
- **Azure SQL Database:** Cloud relational database.
- **Azure Key Vault:** Secure secret storage.
- **Managed Identity:** Passwordless authentication between Azure resources.

<img src="docs/images/azure-resources.png" alt="Azure resources" width="95%"/>

## API Documentation

- **Swagger / OpenAPI:** Available locally at http://localhost:8080/swagger. Includes the configured Bearer authentication scheme for testing protected endpoints.

  <img src="docs/images/swagger-api.png" alt="Swagger API documentation" width="95%"/>

  <img src="docs/images/swagger-products.png" alt="Swagger products endpoints" width="95%"/>

## Future Extensions

- **Worker:** Independent deployment of the Worker.
- **Notifications:** Integration with an external notification provider for low stock alerts.
- **Observability:** External observability infrastructure for application monitoring and diagnostics.
- **Infrastructure:** Infrastructure as Code (IaC) for Azure resources.
- **Workflows:** Additional procurement and inventory workflows.
- **Frontend:** Frontend application development.

## License

This project is provided as a portfolio and demonstration project.
