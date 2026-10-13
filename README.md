[![Build and Test](https://github.com/akos2003jan02/IT-Asset-Manager/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/akos2003jan02/IT-Asset-Manager/actions/workflows/build-and-test.yml)
# 💻 IT Asset Manager

[![Build and Test](https://github.com/akos2003jan02/IT-Asset-Manager/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/akos2003jan02/IT-Asset-Manager/actions/workflows/build-and-test.yml)
![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/UI-WPF-007ACC?logo=windows&logoColor=white)
![Database](https://img.shields.io/badge/Database-SQLite%20%7C%20EF%20Core-003B57?logo=sqlite&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)

A modern, desktop-based IT hardware inventory and lifecycle management system built with **.NET 10**, **WPF**, and **Entity Framework Core (SQLite)**. 

Engineered with clean architecture principles, real-time KPI dashboards, automated audit trailing, robust edge-case validation, and an automated continuous integration (CI) pipeline.

---

## 📸 Key Features

- **📊 Modern Dark-Themed Dashboard:**
  - Real-time KPI summary cards (*Total Devices*, *In Stock*, *Issued*, *In Service*, *Attention Required*).
  - Categorized attention panel highlighting expired warranties, unassigned units, and pending service tickets.
  - Custom color-coded status badges for quick visual identification.
- **🛡️ Data Integrity & Edge-Case Protection:**
  - Database-enforced and pre-validated unique serial number constraints.
  - Automated sequential inventory ID generation (`InventoryIdService`).
  - Validation guards against empty inputs, duplicate entries, and non-existent record mutations.
- **📜 Automated Audit Trail:**
  - Complete change history logging (`AssetHistories`) tracking property alterations (manufacturer, model, serial, status, user reassignment).
- **⚡ Power-User Ergonomics:**
  - Global keyboard shortcuts (`Ctrl+F` search, `Ctrl+N` new device, `Ctrl+Shift+C` serial copy, `Enter` details, `Delete` device).
  - Multi-condition instant search across manufacturer, model, serial, department, and assigned user.
- **📂 Data Import / Export:**
  - Support for bulk device imports and report generation via Excel.

---

## 🛠️ Tech Stack & Architecture

- **Platform:** .NET 10 (Windows Presentation Foundation - WPF)
- **Language:** C# 13
- **Data Persistence:** Entity Framework Core with SQLite
- **Architecture Pattern:** MVVM (Model - View - ViewModel) + Service Layer pattern
- **Testing:** xUnit, Microsoft.Data.Sqlite in-memory database testing
- **CI/CD:** GitHub Actions running on `windows-latest`

```text
IT-Asset-Manager/
├── .github/
│   └── workflows/
│       └── build-and-test.yml      # Automated CI workflow
├── IT.Asset.Manager/               # Main WPF Application
│   ├── Commands/                   # RelayCommand implementations
│   ├── Converters/                 # XAML Value Converters (Status badges)
│   ├── Data/                       # DbContext and SQLite migrations
│   ├── Models/                     # Entity definitions (Asset, AssetHistory, etc.)
│   ├── Services/                   # Business logic (AssetService, Inventory, QR, Export)
│   ├── ViewModels/                 # MVVM ViewModels
│   └── Views/                      # Windows and UI templates
└── IT.Asset.Manager.Tests/         # Automated Unit & Edge-Case Test Suite
    ├── AssetTests.cs               # 11 xUnit tests covering CRUD & boundary cases
    └── TestDbContext.cs           # Isolated in-memory SQLite context
