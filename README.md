# MovieServiceYouTube

A modern ASP.NET Core web service for managing and retrieving movie information from multiple sources including IMDb and a local SQL Server database.

## Overview

MovieServiceYouTube is a full-stack web application built with ASP.NET Core (.NET 6+) that demonstrates clean architecture principles with a layered design. The application aggregates movie data from external APIs (IMDb) and persists data to a SQL Server database, providing both API and web-based interfaces for movie management and discovery.

## Features

- **Movie Management**: Create, retrieve, and manage movie information
- **Multi-Source Data**: Combines data from IMDb service gateway and local database
- **Genre Filtering**: Browse and filter movies by genre (Action, Drama, Sci-Fi, Thriller, Comedy, etc.)
- **RESTful API**: Comprehensive API endpoints for programmatic access
- **Web Interface**: Razor Pages UI for browsing movies
- **Clean Architecture**: Separation of concerns with Domain, Data, and Presentation layers
- **Comprehensive Testing**: Extensive unit test coverage with shared testing utilities
- **Exception Handling**: Custom middleware for centralized error handling

## Technology Stack

- **Framework**: ASP.NET Core (C#)
- **Database**: SQL Server with SDK-style SQL projects (dacpac deployment)
- **Architecture**: Layered architecture with Domain-Driven Design principles
- **Testing**: MSTest framework with custom assertion helpers
- **Frontend**: Razor Pages with Bootstrap CSS framework

## Related articles

- [Always Use the "as" Operator? No Thank You!](https://matlus.com/writing/always-use-as-operator-no-thank-you/) uses examples from this project to discuss casts.
- [Extension Methods? No Thank You!](https://matlus.com/writing/extension-methods-no-thank-you/) uses examples from this project to discuss extension methods.
- [Separate State from Behavior? Yes Please!](https://matlus.com/writing/separate-state-from-behavior/) uses this project to illustrate the separation.
