# TH2.Products API

A minimal .NET 10 Web API project for managing products and categories with SQLite database.

## Quick Start

### Prerequisites

- .NET 10 SDK installed

### Running the Application
1. Navigate to the solution directory:
   ```powershell
   cd c:\Dev\TH2.Products
   ```

2. Build the solution:
   ```powershell
   dotnet build
   ```

3. Run the API:
   ```powershell
   cd TH2.Products.API
   dotnet run
   ```

4. The API will run on `http://localhost:5279`

### Database Initialization

The application automatically:
- Creates the SQLite database (`products.db`) on first run
- Creates all necessary tables with proper indexes and foreign keys
- Seeds initial data with 5 categories and 20 products

## Architecture Overview

### Overall Architecture Approach

Separation of concerns with Domain, Infrastructure, and API layers. Ensuring that business logic is isolated from data access and presentation layers.

Repository pattern is implemented to abstract data access logic. I generally do not like to use DbContext directly without a repository in the event that the underlying
database changes in the future. For the database schema I am using the code first approach.

As requested by the requirements, I have implemented a soft delete on products and dependency injection via the built-in .NET DI container.

DTOs are used to separate internal domain models from API contracts.

Database calls are using async/await to prevent blocking threads on these operations.

## Project Structure

The solution consists of three projects:

- **TH2.Products.API** - ASP.NET Core Web API application. This is a thin API layer that handles incoming HTTP requests and returning appropriate responses (or error codes in the event of failures).
- **TH2.Products.Domain** - Domain entities and business logic lives here. This layer contains the core models (Product, Category) and the interfaces for repositories.
- **TH2.Products.Infrastructure** - Data access layer with Entity Framework Core and SQLite. This layer implements the repository interfaces defined in the Domain project and handles all database interactions.

## Technologies Used

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQLite Database
- Repository Pattern

## Database Schema

### Tables

**Category**
- Id (Primary Key, Auto-increment)
- Name (Required, Max 200 chars)
- Description (Max 1000 chars)
- IsActive (Boolean)

**Product**
- Id (Primary Key, Auto-increment)
- Name (Required, Max 200 chars)
- Description (Max 1000 chars)
- Price (Decimal 18,2)
- CategoryId (Foreign Key to Category)
- StockQuantity (Integer)
- CreatedDate (DateTime)
- IsActive (Boolean)

**Relationships:**
- Foreign Key: Product.CategoryId → Category.Id (Restrict Delete) 
	- Important to ensure there is integrity between the two tables. This will prevent deletion of a category that has products and products must always reference a valid category.
- Index on Product.CategoryId 
	- Required for efficient joins between Products and Categories
- Indexes on IsActive fields for both tables
	- Optimizes queries that are filtering on the IsActive property


## Technology Choices
Outside of the requirements provided, the following technologies were chosen for this project:
**.NET 10**: The latest stable version of .NET at the time of development, providing improved performance and features.
**SQLite**: I chose SQLite over PostgreSQL as it was a simpler implementation for the project. For production I would likely have chosen PostgreSQL as it is more robust and scalable for larger applications.

I chose to not implement AutoMapper and just do model conversions directly in the services. This is a small project and I felt adding AutoMapper would add unnecessary complexity.

## Design Decisions
### How you applied Single Responsibility and Dependency Inversion
I have applied the Single Responsibility principle by ensuring that classes and methods have a single responsibility. With the projects split into API/Domain/Infrastructure layers, each layer has a clear responsibility:
- The API layer handles HTTP requests and responses
- The Domain layer contains business logic and entities
- The Infrastructure layer manages data access.

Dependency Inversion has been applied via the use of interfaces and the DI container in .NET. 
Interfaces are used where appropriate and injected at runtime.

### EF Core approach and query optimization
I have implemented EF Core with a code-first approach. The entities and relationships are defined in the Domain layer and an auto-migration is used to create the database schema.

I have implemented query optimization with the following techniques:
- AsNoTracking - Applied to all read-only queries to avoid EF's change tracking overhead where not needed
- Usage of Include - Used to load Category relationships with Products where needed. This will prevent N+1 query problems requiring multiple calls into the database.
- Database-Level Pagination - Skip() and Take() prevent unnecessary data loading by only fetching the required page of results
- EF.Functions.Like - Leverages database-native LIKE operations for text searching to return only matching records
- Indexing - Comprehensive index strategy covering the filterable and sortable fields (see Index Strategy section)

The main idea behind these implementations is to ensure that queries only retrieve the necessary data and we don't need to apply additional filtering in the API code.

### Complex endpoint choice and rationale
The `/api/products/search` endpoint was chosen as the complex endpoint.

The endpoint supports multiple optional parameters and so the main concern for this endpoint was to build dynamic efficient queries based on the provided filters. 
This requires additional indexes on the filterable fields to ensure performance.

**Multiple Filter Combinations:**
- Text search across Name and Description fields (with multi-word support)
- Category filtering
- Price range filtering (min/max)
- Stock availability filtering
- Combines multiple filters with AND logic

**Sorting Capabilities:**
- Sort by Name, Price, CreatedDate, or StockQuantity
- Ascending or descending order
- Default sorting by Name

**Pagination:**
- Configurable page size (1-100 items per page)
- Returns total count and page metadata
- Prevents loading entire result sets into memory

### Repository pattern decision and trade-offs
Trade-offs of using the repository pattern include:
**Pros:**
- Abstraction of data access logic. It is easier to swap out the underlying data store if needed
- Centralized data access code, improving maintainability and re-usability
- Easier to mock repositories for unit testing
- Separation of concerns between business logic and data access
- Improved testability of services

**Cons:**
- Code for repository interfaces and implementations
- Potential performance overhead if not implemented appropriately

### Index Strategy
**List of Indexes Created:**
- `IX_Product_CategoryId` - Optimizes foreign key joins and category filtering
- `IX_Product_IsActive` - Accelerates active product filtering (soft-delete pattern)
- `IX_Product_Price` - Supports price range filtering and price-based sorting
- `IX_Product_CreatedDate` - Optimizes temporal sorting operations
- `IX_Product_StockQuantity` - Enables efficient in-stock filtering and quantity-based sorting
- `IX_Product_Name` - Improves text search performance on product names
- `IX_Category_IsActive` - Accelerates active category filtering
- `IX_Product_Active_Category_Price` - Covers the most common search pattern: filtering active products by category with price range

I have made some assumptions here on the most important indexes for the search endpoint. 
The goal was to cover the most common filtering and sorting parameter combinations.

I believe the most common search scenario will be searching on active products, filtering by category, and applying price ranges.

## What I Would Do With More Time
Given more time, I would start on the Angular frontend for this application.

There are also several backend improvements that could be made for productionizing this API:
- Authentication/Authorization
- Caching
- Improved Test Coverage (It would be good to have better coverage over common search parameter combinations)
- Logging/Health Checks/Monitoring

## Assumptions & Trade-offs
Assumptions made during development:
- No authentication is required for the API
- Automapper would not be required for this small project
- Single-user scenario with low data volume expectations
- Index strategy was based on assumed common search patterns
