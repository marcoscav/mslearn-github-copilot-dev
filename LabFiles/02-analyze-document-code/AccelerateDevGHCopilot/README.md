# Library App

## Description

Library App is a .NET console-based library management application. It models core library entities (books, patrons, and loans), applies business rules through application services, and uses JSON-backed repositories for persistence. The solution is organized with a clean separation between domain logic, console UI, infrastructure, and automated tests.

## Project Structure

- src
	- Library.ApplicationCore
		- Entities
		- Enums
		- Interfaces
		- Services
		- Library.ApplicationCore.csproj
	- Library.Console
		- Program.cs
		- ConsoleApp.cs
		- ConsoleState.cs
		- CommonActions.cs
		- appSettings.json
		- Json
		- Library.Console.csproj
	- Library.Infrastructure
		- Data
		- Library.Infrastructure.csproj
- tests
	- UnitTests
		- ApplicationCore
		- LoanFactory.cs
		- PatronFactory.cs
		- UnitTests.csproj
- README.md

## Key Classes and Interfaces

- Entities
	- Author, Book, BookItem, Loan, Patron
- Application interfaces
	- ILoanRepository, ILoanService, IPatronRepository, IPatronService
- Domain services
	- LoanService, PatronService
- Console layer
	- ConsoleApp
- Infrastructure data access
	- JsonData, JsonLoanRepository, JsonPatronRepository

## Usage

1. Prerequisites
	 - .NET SDK installed (recommended current LTS).
2. Restore and build
	 - dotnet build src/Library.Console/Library.Console.csproj
3. Run the application
	 - dotnet run --project src/Library.Console/Library.Console.csproj
4. Run unit tests
	 - dotnet test tests/UnitTests/UnitTests.csproj

## License

This project is licensed under the MIT License. See the repository-level LICENSE file for details.
