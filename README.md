# Formula 1 Manager

A small Windows Forms desktop application written in C# / .NET 9.0 for the
**Advanced Programming – Lab 8/9** assignment.

The program lets a fictional Formula 1 management company keep track of:

- **Drivers** of four different categories (F1, F2, Karting, Rookie),
- the **Teams** that can sign them, and
- the **Contracts** that bind a driver to a team.

It demonstrates every concept the lecturer asked for in the brief.

## Solution layout

```
FormulaOneManager/
├── FormulaOneManager.sln
├── FormulaOneManager.Library/      (class library with all the logic)
│   ├── Domain/
│   │   ├── Common/                 (RacingEntity, IRacingEntity)
│   │   ├── Drivers/                (Driver, F1Driver, F2Driver, ...)
│   │   ├── Teams/                  (Team)
│   │   └── Contracts/              (Contract)
│   ├── Repositories/               (Repository<T> + concrete repos)
│   ├── Exceptions/                 (RacingException + 4 children)
│   ├── Events/                     (custom delegates and EventArgs)
│   ├── Extensions/                 (LINQ helpers, AddRange<T>, ...)
│   ├── Persistence/                (JSON / XML serialization)
│   └── Services/                   (RacingService facade)
└── FormulaOneManager.App/          (WinForms desktop UI)
    ├── Program.cs
    ├── MainForm.cs / .Designer.cs
    ├── DriverEditForm.cs / .Designer.cs
    ├── TeamEditForm.cs / .Designer.cs
    ├── ContractForm.cs / .Designer.cs
    └── SeedData.cs
```

## How it satisfies the brief

| Requirement | Where it lives |
|-------------|----------------|
| Class library with reusable logic | `FormulaOneManager.Library` |
| WinForms GUI with **MenuStrip** | `MainForm.Designer.cs` |
| **At least two abstraction layers** | `IRacingEntity` → `RacingEntity` → `Driver` → `F1Driver`/`F2Driver`/`KartingDriver`/`RookieDriver`, plus `IRepository<T>` → `Repository<T>` → concrete repos |
| **Polymorphism** | abstract `Driver.Category` / `GetRacingMotto`, overridden in every subclass |
| **Custom exceptions** | `RacingException` base + `EntityNotFoundException`, `DuplicateEntityException`, `InvalidContractException`, `ValidationException` |
| **Delegates & events** | `RacingDelegates.cs` (`DriverAddedHandler`, `DriverSignedHandler`, `RacingLogHandler`) wired in `RacingService` |
| **LINQ queries** | search, statistics, joins, ordering, grouping in `RacingService`, `MainForm`, `RacingExtensions` |
| **Generic types / methods** | `IRepository<T>`, `Repository<T>`, `AddRange<T>`, `OrderByName<T>` |
| **Extension methods** | `RacingExtensions.cs` |
| **Initialization lists** | `SeedData.Populate`, all `new Driver { ... }` literals |
| **Avoid overusing `var`** | explicit types used everywhere |
| **JSON & XML persistence via abstraction** | `IRacingStorage` → `JsonRacingStorage`, `XmlRacingStorage`, exposed through `File → Open / Save…` |
| **Single-line comments on every block** | yes, all source files |
| **English-only code & comments** | yes |
| **Git version control** | this repository |

## Running locally

```powershell
cd FormulaOneManager
dotnet build
dotnet run --project FormulaOneManager.App
```

Use **File → Save as…** to persist the current state to `.json` or `.xml`,
and **File → Open…** to reload it back.

## Author

Süleyman Efe Metik – Erasmus / Advanced Programming, lab 8/9 (dr Łukasz Marchel).
