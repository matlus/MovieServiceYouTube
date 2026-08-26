# MovieServiceYouTube

A modern ASP.NET Core web service for managing and retrieving movie information from two sources: an **AWS S3-hosted IMDb dataset** and a **local SQL Server database**.

## Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Technology Stack](#technology-stack)
4. [Solution Structure](#solution-structure)
5. [Architecture](#architecture)
6. [Key Components](#key-components)
7. [Database Schema](#database-schema)
8. [Testing Strategy](#testing-strategy)
9. [Configuration](#configuration)
10. [CI/CD Pipeline](#cicd-pipeline)
11. [Getting Started](#getting-started)

---

## Overview

MovieServiceYouTube is built with ASP.NET Core (.NET 6+) and demonstrates a **layered facade pattern**. The application aggregates movie data from two independent sources simultaneously:

- **IMDb data (AWS S3)** — JSON files stored in an S3 bucket at `https://matluscnd1.s3.amazonaws.com/movies/`. Three files (`WithCategories.json`, `WithImageUrls.json`, `WithYears.json`) are fetched concurrently and joined in memory.
- **Local SQL Server** — A `MovieDb` database deployed via dacpac, storing movies, genres, and their associations.

On every read request the service fires concurrent tasks against both sources and merges the results before returning a response, giving callers a unified view of all movies regardless of where they are stored.

---

## Features

- **Movie Management**: Create, retrieve, and manage movie information
- **Dual-Source Data**: Merges results from the IMDb S3 gateway and local SQL Server concurrently
- **Genre Filtering**: Browse and filter movies by genre (Action, Drama, Sci-Fi, Thriller, Comedy, etc.)
- **RESTful API**: Four HTTP endpoints for programmatic access
- **Web Interface**: Razor Pages UI for browsing movies
- **Layered Facade Architecture**: Clean separation of concerns (Presentation → Domain → Data)
- **Comprehensive Testing**: Four-level test strategy with shared utilities
- **Structured Exception Hierarchy**: Custom exception types with middleware translation to HTTP status codes

---

## Technology Stack

| Concern | Technology |
|---|---|
| Web framework | ASP.NET Core (C#, .NET 6+) |
| API style | RESTful JSON API (`[ApiController]`) |
| Frontend | Razor Pages, Bootstrap CSS |
| Database | SQL Server / LocalDB |
| Database deployment | SQL Server Data Tools (SSDT) dacpac |
| Data access | ADO.NET (`SqlClient`, stored procedures) |
| External data | AWS S3 (HTTP/JSON via `HttpClient`) |
| Dependency injection | ASP.NET Core built-in DI |
| Testing framework | MSTest (`[TestClass]` / `[TestMethod]`) |
| CI/CD | Azure Pipelines (windows-latest agent) |

---

## Solution Structure

The solution (`MovieServiceYouTube.sln`) contains eight projects:

| Project | Type | Role |
|---|---|---|
| `MovieServiceYouTube` | ASP.NET Core web app | HTTP entry point — controllers, middleware, Razor Pages, resource models |
| `DomainLayer` | Class library | Business logic — `DomainFacade`, `MovieManager`, `DataFacade`, `ImdbServiceGateway`, `MovieDataManager`, models, exceptions |
| `MovieDb` | SQL Server Database Project | Database schema and stored procedures; builds a `.dacpac` for deployment |
| `AcceptanceTests` | MSTest project | High-level tests that exercise `DomainFacade` end-to-end against a real LocalDB instance |
| `ClassTests` | MSTest project | Unit tests for individual classes (`ConfigurationProvider`, `ExceptionToHttpTranslator`, `MovieAssertions`) |
| `ControllerTests` | MSTest project | Controller-level tests using a `MovieControllerForTest` subclass to substitute dependencies |
| `EndToEndIntegrationTests` | MSTest project | Full HTTP tests against a running instance of the web application |
| `Testing.Shared` | Class library | Shared test utilities — `AssertEx`, `MovieAssertions`, `RandomMovieGenerator`, `MovieEqualityComparer` |

---

## Architecture

### Layered Facade Pattern

```
HTTP Request
    │
    ▼
MoviesController          (MovieServiceYouTube project)
    │  delegates all calls via
    ▼
DomainFacade              (DomainLayer – public API of the domain)
    │  owns
    ▼
MovieManager              (DomainLayer – orchestrates concurrent tasks)
    │
    ├──► ImdbServiceGateway  ──► AWS S3 (3 concurrent HTTP calls, joined in memory)
    │
    └──► DataFacade
              │  owns
              ▼
         MovieDataManager  ──► SQL Server (ADO.NET stored procedures)
```

### Data Flow — `GET /api/movies`

1. `MoviesController.GetMovies()` calls `DomainFacade.GetAllMovies()`.
2. `DomainFacade` delegates to `MovieManager.GetAllMovies()`.
3. `MovieManager` starts **two concurrent tasks**:
   - `ImdbServiceGateway.GetAllMovies()` — fires three concurrent HTTP GET requests to S3, deserialises and joins the responses.
   - `DataFacade.GetAllMovies()` — calls the `GetAllMovies` stored procedure via ADO.NET.
4. `Task.WhenAll` waits for both tasks to complete.
5. Results are combined into a single `ImmutableList<Movie>` and returned up the call stack.
6. `ModelToResourceMapper` converts domain `Movie` models to `MovieResource` DTOs before the response is serialised as JSON.

### Exception Handling

Custom exceptions flow up the call stack and are intercepted by `ExceptionHandlingMiddleware`, which delegates to `ExceptionToHttpTranslator` to produce the correct HTTP status code and response body.

```
MovieServiceBaseException
    ├── MovieServiceBusinessBaseException
    │       ├── DuplicateMovieException          → 409 Conflict
    │       ├── InvalidMovieException            → 400 Bad Request
    │       └── InvalidGenreException            → 400 Bad Request
    ├── MovieServiceNotFoundBaseException
    │       └── MovieWithSpecifiedIdNotFoundException → 404 Not Found
    └── MovieServiceTechnicalBaseException
            ├── ConfigurationSettingMissingException
            └── ConfigurationSettingValueEmptyException
```

---

## Key Components

### `MoviesController`

Located in `MovieServiceYouTube/Controller/MoviesController.cs`. Registered at `api/movies`.

| Method | Route | Description |
|---|---|---|
| `GET` | `api/movies` | Returns all movies from both S3 and SQL Server |
| `GET` | `api/movies/genre/{genre}` | Returns movies matching the specified genre |
| `GET` | `api/movies/id/{id}` | Returns a single movie by its database ID |
| `POST` | `api/movies` | Creates a new movie in the SQL Server database |

The controller delegates every call through `DomainFacade`. Protected virtual overrides (`GetAllMovies`, `GetMovieById`, etc.) allow `MovieControllerForTest` to substitute responses in controller-level tests without a live database.

### `MovieManager`

Located in `DomainLayer/Managers/MovieManager.cs`. The central orchestrator:

- Uses `Task.WhenAll` to run S3 and database calls concurrently for `GetAllMovies` and `GetMoviesByGenre`.
- Lazily instantiates `ImdbServiceGateway` (only created on first use).
- Implements `IDisposable` to clean up the `HttpClient` held by the gateway.
- Throws `MovieWithSpecifiedIdNotFoundException` when no database record matches a requested ID.

### `ImdbServiceGateway`

Located in `DomainLayer/Managers/Services/ImdbService/ImdbServiceGateway.cs`:

- Makes **three concurrent HTTP GET requests** to the configured S3 base URL:
  - `WithCategories.json` — title and genre/category data
  - `WithImageUrls.json` — poster image URLs
  - `WithYears.json` — release year data
- Deserialises each response into `IEnumerable<ImdbMovie>` using `HttpClient.GetFromJsonAsync`.
- Joins all three enumerables by positional index to produce a complete `ImmutableList<Movie>`.
- Throws `ImdbServiceNotFoundException` (404) or `ImdbProxyAuthenticationRequiredException` (407) on HTTP errors.

### `MovieDataManager`

Located in `DomainLayer/Managers/DataLayer/DataManagers/MovieDataManager.cs`:

- Uses raw ADO.NET (`SqlClientFactory`, `DbConnection`, `DbCommand`) — no ORM.
- All database operations are performed through **stored procedures** (see [Database Schema](#database-schema)).
- Write operations (`CreateMovie`, `CreateMovies`) run inside a `Serializable` transaction with explicit rollback on failure.
- Catches `DbException` messages containing `"duplicate key row in object 'dbo.Movie'"` and re-throws as `DuplicateMovieException`.
- `CommandFactoryMovies` builds `DbCommand` objects for each stored procedure, keeping SQL command construction separate from execution logic.

### `DomainFacade`

Located in `DomainLayer/DomainFacade.cs`. The **single public entry point** of the domain layer. The web project and test projects depend only on this class — no other `DomainLayer` types are exposed.

### `DataFacade`

Located in `DomainLayer/Managers/DataLayer/DataFacade.cs`. An internal facade that wraps `MovieDataManager`, keeping the data-access interface stable and hiding ADO.NET details from `MovieManager`.

---

## Database Schema

### Tables

**`Movie`**
| Column | Type | Notes |
|---|---|---|
| `Id` | `INT IDENTITY` | Primary key |
| `Title` | `VARCHAR(50)` | Unique (non-clustered index `IX_Movie`) |
| `Year` | `INT` | Release year |
| `ImageUrl` | `VARCHAR(200)` | Poster image URL |

**`Genre`**
| Column | Type | Notes |
|---|---|---|
| `Id` | `INT IDENTITY` | Primary key |
| `Title` | `VARCHAR(50)` | Unique (non-clustered index `IX_Genre`) |

**`Assoc_MovieGenre`** (many-to-many join table)
| Column | Type | Notes |
|---|---|---|
| `MovieId` | `INT` | FK → `Movie.Id` (CASCADE DELETE/UPDATE) |
| `GenreId` | `INT` | FK → `Genre.Id` |

Composite primary key: `(MovieId, GenreId)`.

### Views

- **`MovieVw`** — flattened view joining `Movie`, `Assoc_MovieGenre`, and `Genre`.

### User-Defined Types

- **`MovieTvp`** — table-valued parameter type used by bulk-insert stored procedures.

### Stored Procedures

| Procedure | Purpose |
|---|---|
| `CreateMovie` | Inserts a single movie with its genre association |
| `CreateMoviesTvpDistinctInsertInto` | Bulk-inserts movies via TVP using `INSERT INTO … SELECT DISTINCT` |
| `CreateMoviesTvpMergeInsertInto` | Bulk-inserts movies via TVP using MERGE + INSERT INTO |
| `CreateMoviesTvpMergeMerge` | Bulk-inserts movies via TVP using a full MERGE statement |
| `CreateMoviesTvpUsingCursor` | Bulk-inserts movies via TVP using a cursor (educational reference) |
| `GetAllMovies` | Returns all movies with genre information |
| `GetMovieById` | Returns a single movie by ID |
| `GetMoviesByGenre` | Returns movies matching a specified genre |
| `GetMoviesByYear` | Returns movies released in a specified year |

The `CreateMoviesTvp*` procedures demonstrate four different SQL techniques for the same bulk-insert operation and serve as educational comparisons.

---

## Testing Strategy

The project uses a **four-level testing approach**, all with MSTest:

| Level | Project | What is tested | Database required |
|---|---|---|---|
| **Class** | `ClassTests` | Individual classes in isolation (no DB, no HTTP) | No |
| **Controller** | `ControllerTests` | `MoviesController` with domain layer substituted | No |
| **Acceptance** | `AcceptanceTests` | `DomainFacade` → `MovieManager` → `MovieDataManager` → real LocalDB | Yes (LocalDB) |
| **End-to-End** | `EndToEndIntegrationTests` | Full HTTP round-trip against a running ASP.NET Core host | Yes (LocalDB) |

### Shared Test Utilities (`Testing.Shared`)

| Class | Purpose |
|---|---|
| `AssertEx` | Extended MSTest assertions (collection comparisons, etc.) |
| `MovieAssertions` | Domain-specific assertions on `Movie` objects |
| `MovieEqualityComparer` | `IEqualityComparer<Movie>` for collection assertions |
| `RandomMovieGenerator` | Generates random `Movie` instances for property-based style tests |
| `RandomStringGenerator` | Generates random strings for test data |

### Test Doubles

- **`HttpMessageHandlerSpy`** (`AcceptanceTests`) — captures outgoing HTTP requests so tests can assert on calls made to the S3 gateway without hitting the real endpoint.
- **`MovieControllerForTest`** (`ControllerTests`) — subclasses `MoviesController` and overrides the protected virtual methods to return predetermined data, isolating the controller from the domain layer.
- **`ServiceLocatorForAcceptanceTesting`** — overrides the default `ServiceLocator` to inject the spy `HttpMessageHandler` into `ImdbServiceGateway`.

---

## Configuration

**`appsettings.json`** (located in the `MovieServiceYouTube` web project):

```json
{
  "AppSettings": {
    "DbConnectionString": "Data Source=(localdb)\\ProjectsV13;Initial Catalog=MovieDb;Integrated Security=True;...",
    "ImdbServiceBaseUrl": "https://matluscnd1.s3.amazonaws.com/movies/"
  }
}
```

| Key | Description |
|---|---|
| `AppSettings:DbConnectionString` | ADO.NET connection string for the `MovieDb` SQL Server / LocalDB instance |
| `AppSettings:ImdbServiceBaseUrl` | Base URL for the AWS S3 bucket that hosts the IMDb JSON files. Must end with `/`. |

`ConfigurationProvider` reads these values at startup and throws `ConfigurationSettingMissingException` or `ConfigurationSettingValueEmptyException` if a required key is absent or empty.

Test projects have their own `appsettings.json` and `appsettings.Development.json` files that can override these values for local development.

---

## CI/CD Pipeline

The `azure-pipelines.yml` file defines an Azure Pipelines build for the `master` branch running on a `windows-latest` agent.

### Pipeline Steps

1. **NuGet restore** — restores all NuGet packages for the solution.
2. **VSBuild** — builds the solution in `Release` configuration and packages the web app as a single-file ZIP artifact.
3. **Create LocalDB instance** — runs `sqllocaldb create ProjectsV13 -s` to create and start the `ProjectsV13` LocalDB instance required by tests.
4. **Deploy dacpac** — deploys `MovieDb.dacpac` using the `MovieDb.publish.xml` publish profile, creating the `MovieDb` database on LocalDB.
5. **Run tests** — executes all `*Tests` projects with `dotnet test`, collecting code coverage and publishing TRX results.
6. **Publish symbols** — (optional) publishes PDB symbol files.
7. **Copy artefacts** — copies the web app binaries and `MovieDb.dacpac` + `MovieDb.Production.publish.xml` to the staging directory.
8. **Publish artefacts** — publishes the staged files as a build artefact for release pipelines.

The `MovieDb.Production.publish.xml` publish profile is provided separately for production deployments and targets a non-LocalDB SQL Server instance.

---

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download) or later
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio)
- Visual Studio 2022+ **or** VS Code with the C# extension

### Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/matlus/MovieServiceYouTube.git
   cd MovieServiceYouTube
   ```

2. **Create the LocalDB instance**

   ```bash
   sqllocaldb create ProjectsV13 -s
   ```

3. **Deploy the database**

   Open `MovieDb/MovieDb.sqlproj` in Visual Studio and click *Publish*, selecting `MovieDb.publish.xml` as the publish profile. Alternatively, use the SSDT command-line tool:

   ```bash
   dotnet build MovieDb/MovieDb.sqlproj -c Debug
   SqlPackage /Action:Publish /SourceFile:"MovieDb/bin/Debug/MovieDb.dacpac" /Profile:"MovieDb/MovieDb.publish.xml"
   ```

4. **Run the web application**

   ```bash
   dotnet run --project MovieServiceYouTube/MovieServiceYouTube.csproj
   ```

   The API will be available at `https://localhost:5001/api/movies`.

5. **Run the tests**

   ```bash
   dotnet test
   ```

### API Quick Reference

| Method | URL | Description |
|---|---|---|
| `GET` | `/api/movies` | All movies (S3 + database) |
| `GET` | `/api/movies/genre/{genre}` | Movies by genre (e.g. `Drama`, `Action`) |
| `GET` | `/api/movies/id/{id}` | Movie by database ID |
| `POST` | `/api/movies` | Create a new movie (JSON body) |

---

## Project Structure
