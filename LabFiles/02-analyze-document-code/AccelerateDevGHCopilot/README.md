# Library App

## Description

Library App is a .NET console application for managing library operations such as patron search, loan tracking, membership renewals, and loan status updates. The application follows a layered architecture with separate domain, infrastructure, and console presentation components, and uses JSON files as its data store.

## Project Structure

- `AccelerateDevGHCopilot/`
  - `src/`
    - `Library.ApplicationCore/`
      - `Entities/` - Domain models such as `Author`, `Book`, `BookItem`, `Loan`, and `Patron`.
      - `Enums/` - Status enums used across the application.
      - `Interfaces/` - Contracts for repositories and services.
      - `Services/` - Business logic for library workflows.
      - `Library.ApplicationCore.csproj` - Project file for the application core layer.
    - `Library.Console/`
      - `Program.cs` - Application startup and dependency injection wiring.
      - `ConsoleApp.cs` - Main console flow and interaction logic.
      - `CommonActions.cs` - Shared action options for the console UI.
      - `ConsoleState.cs` - State management for the console app.
      - `appSettings.json` - Runtime configuration.
      - `Json/` - JSON seed data used by the sample app.
      - `Library.Console.csproj` - Project file for the console application.
    - `Library.Infrastructure/`
      - `Data/` - JSON-backed repository implementations.
      - `Library.Infrastructure.csproj` - Project file for the infrastructure layer.
  - `tests/`
    - `UnitTests/` - Unit tests that validate application behavior.
    - `UnitTests.csproj` - Test project file.

## Key Classes and Interfaces

- `Patron` - Represents a library patron and membership information.
- `Loan` - Represents a loan transaction and associated dates.
- `Book` - Represents a book in the library catalog.
- `BookItem` - Represents an individual physical copy of a book.
- `Author` - Represents a book author.
- `IPatronRepository` - Defines patron data access operations.
- `ILoanRepository` - Defines loan data access operations.
- `IPatronService` - Defines patron business services.
- `ILoanService` - Defines loan business services.
- `PatronService` - Implements patron-related logic.
- `LoanService` - Implements loan-related logic.
- `JsonData` - Loads and saves JSON data and builds populated object graphs.
- `JsonPatronRepository` - Accesses patron records from JSON-backed storage.
- `JsonLoanRepository` - Accesses loan records from JSON-backed storage.
- `ConsoleApp` - Controls the main console user experience.

## Usage

1. Open the solution in Visual Studio Code.
2. Restore dependencies and build the solution:
   ```bash
   dotnet build
   ```
3. Run the console application:
   ```bash
   dotnet run --project src/Library.Console/Library.Console.csproj
   ```
4. Follow the interactive prompts to search for patrons, inspect loans, renew memberships, extend loans, and return books.
5. Run the unit tests:
   ```bash
   dotnet test tests/UnitTests/UnitTests.csproj
   ```

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.