# ChatGenius Tech Stack

This document outlines the technology stack chosen for the ChatGenius application and provides justification for each selection.

## Frontend

*   **Blazor WebAssembly:**
    *   **Why:** Blazor WebAssembly allows us to build a rich and interactive user interface with C# instead of JavaScript, leveraging existing .NET skills and sharing code with the backend.
    *   **How it works:** Blazor WebAssembly runs .NET code directly in the browser, enabling client-side rendering and a native-like experience.

## Backend

*   **ASP.NET Core Web API:**
    *   **Why:** ASP.NET Core is a robust and performant framework for building web APIs, providing features like dependency injection, routing, and middleware.
    *   **How it works:** ASP.NET Core handles HTTP requests, processes data, and interacts with the database and other services.

## Version Control

*   **Git (GitHub):**
    *   **Why:** Git is a distributed version control system that enables efficient collaboration and code management. GitHub provides a centralized platform for hosting the repository and collaborating with developers.
    *   **How it works:** Git tracks changes to the codebase, allowing developers to branch, merge, and revert changes. GitHub provides tools for code review, issue tracking, and project management.

## Deployment

*   **Azure Static Web Apps (SWA):**
    *   **Why:** Azure SWA offers a streamlined deployment process for Blazor WebAssembly applications, with built-in CI/CD integration and serverless hosting.
    *   **How it works:** Azure SWA automatically builds and deploys the application from the GitHub repository, providing a scalable and cost-effective hosting solution.

## Infrastructure

*   **Azure Cloud Platform:**
    *   **Why:** Azure provides a comprehensive suite of cloud services, including compute, storage, database, and networking, allowing us to build a scalable and reliable application.
    *   **How it works:** Azure services are interconnected and managed through the Azure portal, providing a unified platform for managing the application infrastructure.

## Database

*   **Azure SQL Database:**
    *   **Why:** Azure SQL Database is a fully managed relational database service that offers scalability, security, and high availability.
    *   **How it works:** Azure SQL Database provides a SQL Server database engine in the cloud, allowing us to store and manage application data.

## ORMs (Object-Relational Mappers)

*   **Entity Framework Core:**
    *   **Why:** Entity Framework Core simplifies database interaction by providing an object-oriented interface to the relational database.
    *   **How it works:** Entity Framework Core maps database tables to C# classes, allowing developers to work with data using familiar object-oriented concepts.

## UI & Styling

*   **Blazor Component Library (e.g., MudBlazor, Radzen Blazor):**
    *   **Why:** A Blazor component library provides pre-built UI components and styling options, accelerating development and ensuring a consistent look and feel.
    *   **How it works:** The component library offers reusable components that can be easily integrated into the Blazor application, providing a rich set of UI elements.

## API Integrations

*   **Azure Pub/Sub:**
    *   **Why:** Azure Pub/Sub enables real-time communication between the backend and frontend, facilitating features like instant messaging and presence updates.
    *   **How it works:** Azure Pub/Sub provides a publish-subscribe messaging system, allowing the backend to publish messages to topics and the frontend to subscribe to those topics.

## Monitoring & Error Tracking

*   **Azure Monitor:**
    *   **Why:** Azure Monitor provides comprehensive monitoring and logging capabilities, allowing us to track application performance, identify errors, and troubleshoot issues.
    *   **How it works:** Azure Monitor collects telemetry data from the application and provides tools for visualizing, analyzing, and alerting on that data.

## Authentication

*   **Azure Entra ID:**
    *   **Why:** Azure Entra ID is a cloud-based identity and access management service that provides secure authentication and authorization for the application.
    *   **How it works:** Azure Entra ID allows users to authenticate with their organizational accounts or social logins, providing secure access to the application.

This `tech-stack.md` document provides a clear and concise overview of the technologies used in ChatGenius and justifies the choices made. It serves as a valuable resource for developers and stakeholders to understand the technical foundation of the application.