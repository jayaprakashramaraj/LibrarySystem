# Library API System

Architecture and technical design for the `Library API System` solution.

---

## Overview

`LibrarySystem` is a modular .NET solution that implements Library API System. It is organized in logical layers (API, Service, Application, Infrastructure, Domain) and uses gRPC internally between the API and the service layer. MongoDB is used as the primary persistence for historical loans, books and borrowers.

The main responsibilities:
- `Library.Api` — HTTP REST API exposing endpoints for consumers. Uses a generated gRPC client to call the backend `Library.Service`.
- `Library.Service` — gRPC server that implements domain-specific business logic and aggregates data from the application layer.
- `Library.Application` — Application services implementing use-cases (borrowing patterns, borrower activity, inventory, etc.).
- `Library.Infrastructure` — Repositories and persistence (MongoDB), seed/initialization utilities and implementation details.
- `Library.Domain` — Domain entities and value objects (Book, Borrower, Loan, etc.).

This separation enforces single responsibility, testability and clean layering (ports & adapters style).

---

## Technology stack

- .NET 10 (C# 14)
- ASP.NET Core Web API (REST front-end)
- gRPC (internal service interface)
- Google.Protobuf / grpc-csharp generated classes
- MongoDB (persistence)
- xUnit + Moq for unit tests
- Docker + Docker Compose for local development and integration/system testing

---

## Projects (high level)

- `src/Library.Api` — REST API, controllers, gRPC client wrapper
- `src/Library.Service` — gRPC service implementation and interceptors
- `src/Library.Application` — Application services and DTOs
- `src/Library.Infrastructure` — Repositories, MongoDB integration, seeders
- `src/Library.Domain` — Entities and domain models

Tests:
- `tests/*` — unit, integration and system end-to-end tests (xUnit)

---

## High-level data flow

1. Client sends HTTP request to `Library.Api` (Controller endpoints under `/api/library`).
2. The API controller uses the generated gRPC client (`LibraryService.LibraryServiceClient`) to call `Library.Service`.
3. `Library.Service` executes application use-cases (via `Library.Application`) which read/write from MongoDB through repository implementations in `Library.Infrastructure`.
4. Results are returned over gRPC to the API, which transforms responses to HTTP results.

gRPC -> TCP (internal) and HTTP -> REST (external).

---

## Important design notes

- Controllers map gRPC errors (`RpcException`) to an HTTP 500 with a stable JSON shape: `{ source = "Library service", error = "..." }`.
- Protobuf-generated messages are stored under compiled output (`obj/Debug/net10.0/*.cs`) and are used by both client/server.
- The solution expects MongoDB to be available when running the service or system tests.

---

## Endpoints (HTTP)

The API exposes the following REST endpoints (examples assume the API is hosted at `http://localhost:5001`):

- Get most-borrowed books
  - `GET http://localhost:5001/api/library/most-borrowed?count=50`
  - Returns: JSON array of `Book` items (from `BookList.Books`).

- Get user activity (borrower activity list) for a date range
  - `GET http://localhost:5001/api/library/user-activity?startDate=2000-01-01&endDate=2026-12-31`
  - Date format: ISO 8601. Returns a list of borrower activity DTOs.

- Get "also borrowed" (books commonly borrowed together with the given book)
  - `GET http://localhost:5001/api/library/also-borrowed/{bookId}`
  - Example using Clean Code book id (as provided):
    - `GET http://localhost:5001/api/library/also-borrowed/7896867a-f5ef-495e-b2b4-918f5e7a9246`

Note: When the gRPC service fails with an `RpcException` the API returns HTTP 500 and a small JSON error object that includes a `source` and `error` message.

---

## Running locally with Docker

Prerequisites:
- Docker and Docker Compose installed
- (Optional) Ports `5000`/`5001` and MongoDB port used by the compose file available

From the repository root (`LibrarySystem`), start the main application stack:

- Build and start in detached mode:

  `docker-compose up --build -d`

This will start the API, gRPC service and a MongoDB instance (see `docker-compose.yml` for details). The API will be reachable at `http://localhost:5000` (HTTP) and `https://localhost:5001` (HTTPS). The example endpoints above use `http://localhost:5001`.

To stop and remove containers:

  `docker-compose down`

---

## Running end-to-end system tests (Docker)

There is a separate compose file for tests which sets up test containers and runs the system test runner.

From repository root:

`docker-compose -f docker-compose.test.yml up --build --exit-code-from system.tests --attach system.tests`

This command will:
- Build the environment
- Execute the system tests container
- Return the test run exit code locally

To cleanup after tests:

  `docker-compose -f docker-compose.test.yml down --remove-orphans`

---

## Development & tests (local without Docker)

Run unit tests (xUnit) with your preferred runner (Visual Studio Test Explorer or `dotnet test`):

- `dotnet test ./tests/Library.Api.Tests`
- `dotnet test ./tests/Library.Application.Tests`
- `dotnet test ./tests/Library.Integration.Tests` (integration tests may require a running MongoDB)

---

## Troubleshooting

- If the API returns HTTP 500, inspect logs from the gRPC service container and API container. The Docker Compose logs are available via:
  - `docker-compose logs api` or `docker-compose logs library.service` (service names depend on `docker-compose.yml`).

- If system tests cannot connect to MongoDB, check the compose file for the MongoDB service name and ports and ensure no other MongoDB instance binds to the same ports.

---

## Useful files

- `docker-compose.yml` — main compose configuration
- `docker-compose.test.yml` — compose configuration for system tests
- `src/Library.Api/Controllers/LibraryController.cs` — REST controller mapping to gRPC
- `src/Library.Service` — gRPC server implementation
- `src/Library.Infrastructure/Seed` — DB seed utilities
- `tests` — unit, integration and system test projects

---


