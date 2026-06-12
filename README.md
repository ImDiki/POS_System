# POS System

This is a Point of Sale (POS) system application developed for the Convenience Store. The project is designed to manage day-to-day retail operations efficiently, including inventory management, secure staff access, and sales processing.

## 🚀 Key Features

* Sales Processing:Real-time barcode scanning, item checkout, and change calculation.
* Inventory Management: Easy registration of new products and stock updates.
* Transaction History: Record and view past sales transactions.
* Drawer Security:Secure access to cash drawer data with staff authentication.
* Receipt Generation: Automated generation of digital receipts for customers.

## 🛠 Tech Stack

* **Language:** C#
* **Framework:** WPF (Windows Presentation Foundation)
* **Database:** SQL Server (via ADO.NET)
* **IDE:** Visual Studio

## 📂 Project Structure

| File/Folder | Purpose |
| :--- | :--- |
| `CheckoutView.xaml.cs` | Main logic for scanning, cart management, and payment processing. |
| `ProductView.xaml.cs` | Logic for product registration and inventory updates. |
| `ReceiptWindow.xaml.cs` | Handles receipt display and printing logic. |
| `DatabaseHelper.cs` | Manages SQL Server connections and queries. |
| `Models/` | Data structures for `TransactionDetails` and products. |

## 💡 How to Run

1. Clone this repository.
2. Ensure you have SQL Server installed and configured.
3. Update the connection string in `DatabaseHelper.cs` to match your local database.
4. Build and run the project using Visual Studio.

---
