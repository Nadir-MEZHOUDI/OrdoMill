# OrdoMill - Agent Guidelines

## Project Overview

OrdoMill is a .NET 10 WPF desktop application for pharmacy management, handling prescriptions (ordonnances), patients, doctors, medications, invoicing, and insurance (CNAS) processing.

## Build Commands

```bash
# Build entire solution
dotnet build OrdoMill.sln

# Build specific project
dotnet build OrdoMill/OrdoMill.csproj
dotnet build OrdoMill.Data/OrdoMill.Data.csproj

# Build in Release mode
dotnet build OrdoMill.sln -c Release

# Clean build artifacts
dotnet clean OrdoMill.sln

# Restore NuGet packages
dotnet restore OrdoMill.sln

# Run the application
dotnet run --project OrdoMill/OrdoMill.csproj
```

## Test Commands

No test projects currently exist. To run tests if added:

```bash
# Run all tests in solution
dotnet test

# Run specific test project
dotnet test OrdoMill.Tests/OrdoMill.Tests.csproj

# Run single test class
dotnet test --filter "FullyQualifiedName~MedicamentsViewModelTests"

# Run single test method
dotnet test --filter "FullyQualifiedName~MedicamentsViewModelTests.SearchEx_ReturnsResults"
```

## Lint/Format Commands

```bash
# Format code (whitespace, style, etc.)
dotnet format

# Format specific project
dotnet format OrdoMill/OrdoMill.csproj

# Verify formatting without changes
dotnet format --verify-no-changes
```

## Architecture

### Solution Structure

```
OrdoMill/
├── OrdoMill/                    # Main WPF application
│   ├── Views/                   # XAML views and ViewModels
│   │   ├── Medicaments/         # Medication management
│   │   ├── Patients/            # Patient management
│   │   ├── Ordonnance/          # Prescription management
│   │   ├── Medecins/            # Doctor management
│   │   ├── Clients/             # Client/Insurance management
│   │   └── ...
│   ├── Services/                # Business logic services
│   ├── ViewModel/               # ViewModel locator, dialogs
│   ├── Interfaces/              # Repository and service interfaces
│   ├── Dto/                     # Data Transfer Objects
│   ├── Converters/              # XAML value converters
│   └── Serializables/           # Serialization models
│
├── OrdoMill.Data/               # Data access layer
│   ├── Model/                   # EF Core entities and DbContext
│   └── Migrations/              # EF migrations
│
└── OrdoMill.Resources/          # Global resources and localization
```

## Code Style Guidelines

### Naming Conventions

- **Classes/Interfaces**: PascalCase (e.g., `MedicamentsViewModel`, `IRepository`)
- **Properties**: PascalCase (e.g., `SelectedItem`, `SearchPattern`)
- **Methods**: PascalCase with `Ex` suffix for async implementations (e.g., `AddEx`, `SearchEx`)
- **Private fields**: camelCase with underscore prefix (e.g., `_context`, `_disposed`)
- **Commands**: PascalCase with `Command` suffix (e.g., `AddCommand`, `SaveCommand`)
- **Events**: PascalCase (e.g., `SearchExpression`)

### File Organization

```csharp
// 1. Using statements (sorted alphabetically, System first)
using System;
using System.Data.Entity;
using OrdoMill.Data.Model;

// 2. Namespace matching folder structure
namespace OrdoMill.Services
{
    // 3. Class with attribute on separate line
    [AddINotifyPropertyChangedInterface]
    public class ExampleClass
    {
        // 4. Constants and static members first
        
        // 5. Private fields
        
        // 6. Public properties
        
        // 7. Constructor(s)
        
        // 8. Public methods
        
        // 9. Protected/Private methods
        
        // 10. IDisposable implementation (if needed)
    }
}
```

### Imports/Usings

```csharp
// Order: System → Microsoft → Third-party → Project namespaces
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using OrdoMill.Data.Model;
using OrdoMill.Interfaces;
```

### MVVM Pattern

**ViewModels**:
- Use `[AddINotifyPropertyChangedInterface]` from PropertyChanged.Fody
- Extend `Repository<TEntity, THomeView, TDetailsView>` for CRUD operations
- Use `RelayCommand` / `RelayCommand<T>` from CommunityToolkit.Mvvm
- Register in `ViewModelLocator` for DI

```csharp
[AddINotifyPropertyChangedInterface]
public class MedicamentsViewModel : Repository<Medicament, ShowMedicamentsView, MedicamentDetailsView>
{
    public ObservableCollection<string> Dcis { get; set; }
    
    public override async Task SearchEx()
    {
        // Implementation
    }
}
```

**Views (XAML code-behind)**:
```csharp
namespace OrdoMill.Views.Medicaments
{
    public partial class MedicamentsView
    {
        public MedicamentsView()
        {
            InitializeComponent();
        }
    }
}
```

### Entity Framework Patterns

**Entities** (in OrdoMill.Data/Model):
```csharp
[Serializable]
[AddINotifyPropertyChangedInterface]
public class Patient : EntityBase
{
    public string Nom { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? DateNaissance { get; set; }
    
    [ForeignKey(nameof(AssureId))]
    public virtual Assure Assure { get; set; }
    
    public virtual ICollection<Ordonnance> Ordonnances { get; set; }
}
```

**DbContext**:
- Connection string from `Settings.Default.ConnectionString`
- Use `MigrateDatabaseToLatestVersion` initializer

### Async/Await Patterns

```csharp
// Use async suffix for async methods (but existing code uses Ex suffix)
public async Task SaveEx()
{
    try
    {
        await Context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        await ex.AppLoggingAsync();
        await ShowErrorMessage("Error message");
    }
}

// Use CancellationToken for cancellable operations
searchCts?.Cancel();
searchCts = new CancellationTokenSource();
var token = searchCts.Token;
```

### Error Handling

```csharp
// Use AppLoggingAsync extension for exception logging
try
{
    // Operation
}
catch (OperationCanceledException)
{
    // Expected cancellation - no logging needed
}
catch (Exception ex)
{
    await ex.AppLoggingAsync();
    await ShowErrorMessage("User-friendly message");
}
```

### Dependency Injection

Register services in `ViewModelLocator`:
```csharp
services.AddTransient<DbCon>(sp => new DbCon(Settings.Default.ConnectionString));
services.AddTransient<IUnitOfWork, UnitOfWork>();
services.AddTransient<MedicamentsViewModel>();
```

### DTOs

Extend entity classes with `[AddINotifyPropertyChangedInterface]`:
```csharp
[AddINotifyPropertyChangedInterface]
public class Medicament : Data.Model.Medicament
{
    // Additional computed properties
}
```

### XAML Conventions

- Use MahApps.Metro controls
- Bind to ViewModel properties via `DataContext`
- Use `ViewModelLocator` as x:Key="Locator" in App.xaml

### Comments

- Avoid redundant comments that restate code
- Use TODO comments for future work: `// TODO: Description`
- Add XML documentation only for public APIs if required

### Formatting

- Indent: 4 spaces (no tabs)
- Braces: Allman style (opening brace on new line)
- Keep lines under 120 characters
- Single statement per line

## Key Technologies

- **.NET 10** with WPF
- **Entity Framework 6** (System.Data.Entity)
- **CommunityToolkit.Mvvm** - MVVM helpers
- **PropertyChanged.Fody** - INotifyPropertyChanged weaving
- **MahApps.Metro** - UI controls
- **AutoMapper** - Object mapping
- **EPPlus** - Excel export
- **Microsoft.Extensions.DependencyInjection** - DI container

## Important Notes

- Nullable reference types are **disabled** (`<Nullable>disable</Nullable>`)
- Implicit usings are **enabled**
- WPF binding errors are logged via `WpfBindingErrors` package
- Application uses LocalDB with connection string in Settings
