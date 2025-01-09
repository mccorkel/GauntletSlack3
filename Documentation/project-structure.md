# ChatGenius Project Structure

This document outlines the organization and structure of the ChatGenius project files.

## Monorepo Approach

ChatGenius will follow a monorepo approach, where all codebases (frontend, backend, shared components) reside within a single Git repository. This promotes code sharing, simplifies dependency management, and facilitates collaboration among developers.

## Directory Structure

chatgenius/
├── client/                  # Blazor WebAssembly frontend
│   ├──  wwwroot/            # Static files (HTML, CSS, JavaScript)
│   ├──  Pages/              # Blazor pages and components
│   ├──  Shared/             # Shared UI components
│   ├──  Services/           # Client-side services
│   ├──  Program.cs          # Application entry point
│   └──  ...
├── server/                  # ASP.NET Core Web API backend
│   ├──  Controllers/        # API controllers
│   ├──  Models/             # Data models and DTOs
│   ├──  Services/           # Server-side services
│   ├──  Hubs/               # SignalR hubs for real-time communication
│   ├──  Startup.cs          # Application configuration
│   └──  ...
├── shared/                  # Shared code (models, utilities)
│   ├──  ...
├──  .github/                # GitHub Actions workflows
│   ├──  ...
├──  docs/                   # Project documentation
│   ├──  ...
├──  tests/                  # Unit and integration tests
│   ├──  ...
├──  README.md              # Project description
├──  LICENSE                 # License information
└──  ...


## Key Directories

*   **client:** Contains the Blazor WebAssembly frontend code.
    *   **wwwroot:** Houses static files like HTML, CSS, and JavaScript.
    *   **Pages:** Contains Blazor pages and components for the user interface.
    *   **Shared:** Holds shared UI components used across different pages.
    *   **Services:** Contains client-side services for data fetching and API interaction.
*   **server:** Contains the ASP.NET Core Web API backend code.
    *   **Controllers:** Defines API controllers for handling requests.
    *   **Models:** Contains data models and DTOs (Data Transfer Objects) for API communication.
    *   **Services:** Contains server-side services for business logic and data access.
    *   **Hubs:** Contains SignalR hubs for real-time communication features.
*   **shared:** Contains code shared between the frontend and backend, such as data models and utility functions.
*   **.github:** Contains GitHub Actions workflow files for CI/CD automation.
*   **docs:** Contains project documentation files (like this one).
*   **tests:** Contains unit and integration tests for the frontend and backend code.

## Benefits of Monorepo

*   **Simplified dependency management:** Easier to manage dependencies between frontend and backend code.
*   **Code sharing:** Promotes code reuse and consistency across the project.
*   **Improved collaboration:** Facilitates collaboration among developers working on different parts of the application.
*   **Atomic changes:** Allows for atomic commits and changes across the entire project.

This `project-structure.md` file provides a clear overview of how the ChatGenius project is organized. By following this structure, developers can easily navigate the codebase and contribute to the project effectively.