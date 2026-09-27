# POS System

A C# / WPF student project for practicing a convenience-store point-of-sale workflow with SQL Server LocalDB.

## Implemented Features

- Product lookup by barcode/product code
- Shopping-cart quantity and total calculation
- Cash and cashless payment flows
- Cash/change calculation
- Transaction records stored in SQL Server
- Product registration and stock-related screens
- Transaction history view
- Receipt display and text-file export
- Age-confirmation dialog for age-restricted products
- Staff-ID based session access
- Cash-drawer denomination tracking

## Tech Stack

- C#
- .NET 8
- WPF / XAML
- SQL Server LocalDB
- ADO.NET (`System.Data.SqlClient`)

## Project Structure

- `Views/CheckoutView.xaml.cs` — checkout, cart, payment, receipt, and drawer workflow
- `Views/ProductView.xaml.cs` — product-management UI
- `Views/HistoryView.xaml.cs` — transaction history
- `Views/ReceiptWindow.xaml.cs` — receipt display
- `Data/DatabaseHelper.cs` — LocalDB connection helper
- `Models/` — product, transaction, payment, staff, and drawer models

## Local Setup

The application expects a LocalDB database file at:

`Data/POSDATABASE.mdf`

relative to the application output directory. The database file and schema are not included in this portfolio repository, so a fresh clone requires local database setup before the runtime flows can be tested.

Build with Visual Studio or the .NET 8 SDK on Windows.

## Project Status

This is a learning project rather than a production POS system. The current implementation demonstrates the main retail workflow, but much of the UI, database access, and business logic remains coupled in WPF code-behind.

Possible improvements include:

- Move database and payment logic into dedicated services.
- Use typed SQL parameters instead of `AddWithValue`.
- Add password-based staff authentication and authorization.
- Add automated tests.
- Add database setup/migration scripts for reproducible local setup.
