# OrdoMill - Architecture & Code Review Report

**Review Date:** 2026-03-08  
**Last Updated:** 2026-03-08  
**Stack:** WPF (.NET Framework 4.8), MVVM Light, Entity Framework 6, MahApps.Metro  
**Project Type:** Pharmacy Prescription Management Desktop Application

---

## 1. Executive Summary

OrdoMill is a WPF desktop application for managing pharmacy prescriptions, patients, doctors, and invoicing. The codebase follows a basic MVVM architecture using MVVM Light, but exhibits several architectural weaknesses that impact maintainability, testability, and long-term evolution.

### Key Findings:
- **Architecture Pattern:** MVVM with Repository-like base class, but lacks proper separation of concerns
- **Overall Health:** Functional but needs refactoring for maintainability
- **Risk Level:** Medium - Technical debt accumulating
- **Estimated Refactoring Effort:** 2-4 weeks for critical issues

### ✅ Fixes Applied:
1. ✅ Removed `[ImplementPropertyChanged]` from `DbCon` (EF compatibility fix)
2. ✅ Implemented password hashing using PBKDF2 (Security fix)
3. ✅ Fixed duplicate AutoMapper configurations
4. ✅ Fixed empty catch blocks with proper logging
5. ✅ Implemented Unit of Work pattern
6. ✅ Created proper DTO classes without inheritance

---

## 2. Critical Issues

### 2.1 DbContext Instance Per ViewModel (HIGH RISK)
**Location:** `Repository.cs:33`, multiple ViewModels

```csharp
protected Repository()
{
    Context = new DbCon(Settings.Default.ConnectionString);
    // ...
}
```

**Problem:** Each ViewModel creates its own `DbContext` instance that lives for the entire lifetime of the ViewModel. This causes:
- Memory leaks (DbContext holds entity references)
- Stale data when multiple ViewModels modify the same entities
- Connection pool exhaustion
- Inconsistent entity states across ViewModels

**Impact:** Data corruption risk, memory issues, concurrency bugs

**Recommendation:** Implement Unit of Work pattern with short-lived DbContext per operation, or use DI container to manage DbContext lifecycle.

---

### 2.2 Entity Framework Entities with INotifyPropertyChanged via Fody
**Location:** `OrdoMill.Data\Model\*.cs`, `DbCon.cs:7-8`

```csharp
[ImplementPropertyChanged]
public sealed class DbCon : DbContext
```

```csharp
[ImplementPropertyChanged]
public class Patient : EntityBase
```

**Problem:** 
- DbContext with `ImplementPropertyChanged` is non-standard and can cause issues with EF change tracking
- PropertyChanged on entities can trigger unexpected database queries (lazy loading in bindings)
- Mixing EF entities with UI concerns violates separation of concerns

**Impact:** Debugging complexity, N+1 query problems, unpredictable behavior

**Recommendation:** Use DTOs for UI binding, keep EF entities pure. Remove `[ImplementPropertyChanged]` from `DbCon`.

---

### 2.3 Synchronous Database Calls Mixed with Async
**Location:** `Repository.cs`, `OrdonancesViewModel.cs`, `PatientsViewModel.cs`

```csharp
public async Task DeleteEx(int id = 0)
{
    // ...
    Context.Set<TEntity>().Remove(entity);
    await Context.SaveChangesAsync(); // Async
    await SearchEx(); // But SearchEx creates new queries
}
```

**Problem:** Inconsistent async/await usage. Some operations block the UI thread, others don't. Mixed patterns make the codebase unpredictable.

**Impact:** UI freezing, poor user experience, potential deadlocks

---

### 2.4 Password Storage in Plain Text
**Location:** `OrdoMill.Data\Model\User.cs`

```csharp
public string Password { get; set; }
```

**Problem:** Passwords appear to be stored in plain text in the database.

**Impact:** Security vulnerability

**Recommendation:** Implement password hashing (bcrypt, Argon2, or at minimum salted SHA256).

---

## 3. Medium Issues

### 3.1 Overly Generic Repository Base Class
**Location:** `Repository.cs`

```csharp
public abstract class Repository<TEntity, THomeView, TDetailsView> : NavigableViewModel, IRepository<TEntity>
```

**Problem:** 
- Base class handles: CRUD, pagination, navigation, dialog display, search, export
- Violates Single Responsibility Principle
- Difficult to override behavior for specific entities
- Forces all ViewModels to have the same structure

**Impact:** Maintenance burden, rigid architecture, code bloat

**Recommendation:** Split into:
- `CrudService<TEntity>`
- `PaginationService`
- `NavigationService`
- Let ViewModels compose what they need

---

### 3.2 DTO Classes Inherit from Entity Classes
**Location:** `Dto\Dto.cs`

```csharp
[ImplementPropertyChanged]
public class Patient : Data.Model.Patient
{
}
```

**Problem:**
- DTOs inherit all EF entity behavior including navigation properties
- Lazy loading can still occur
- Doesn't actually decouple from EF
- Creates confusion about which type to use where

**Impact:** Not achieving the decoupling DTOs are meant to provide

**Recommendation:** Create true POCO DTOs without inheritance from EF entities.

---

### 3.3 Static Service Locator Pattern
**Location:** `ViewModelLocator.cs`, `Statics.cs`, `LoggingService.cs`

```csharp
public static ViewModelLocator Locator 
    => Application.Current.Resources["Locator"] as ViewModelLocator;
```

**Problem:**
- Static access to services makes testing difficult
- Hidden dependencies
- Tight coupling to WPF Application lifecycle

**Impact:** Hard to unit test, hidden dependencies, difficult to mock

---

### 3.4 Exception Swallowing
**Location:** Multiple files

```csharp
catch (Exception ex)
{
    // await ex.AppLoggingAsync();
    // This is a really, really unexpected exception.
}
```

```csharp
catch (Exception ex)
{
    // 
}
```

**Problem:** Empty catch blocks or commented-out logging hide errors and make debugging impossible.

**Impact:** Silent failures, data corruption, debugging nightmares

---

### 3.5 AutoMapper Configuration Issues
**Location:** `ViewModelLocator.cs:132-180`

```csharp
cfg.CreateMap<Assure, Assure>();
cfg.CreateMap<Assure, Assure>(); // Duplicate
```

**Problem:**
- Duplicate mappings
- Self-mappings (Type to same Type) serve no purpose
- No actual DTO-to-Entity or Entity-to-DTO mappings

**Impact:** AutoMapper is not being used effectively

---

### 3.6 Hardcoded Strings for Localization
**Location:** `Statics.cs:61-83`, `Helper.cs:136-160`

```csharp
case "يوم":
    break;
case "يومي":
    SaveBackUp();
```

**Problem:** Hardcoded Arabic strings in switch statements make localization maintenance impossible.

**Impact:** Cannot change languages without code changes

---

## 4. Minor Issues

### 4.1 Inconsistent Naming Conventions
- `MedecinsTypesConvertr` (typo: should be `Converter`)
- `Medecins` vs `Medicaments` (French pluralization inconsistent)
- `BorderauView` vs `Bordereau` (typo)
- `Ordonance` vs `Ordonnance` (inconsistent spelling)
- `ServiDate` / `SoineDate` (French abbreviations unclear)

### 4.2 Commented-Out Code Blocks
**Location:** `Dto.cs` (entire file is 450+ lines of commented code), `POCOS.cs` (same)

These files contain hundreds of lines of commented-out generated code. Should be removed.

### 4.3 Magic Numbers
**Location:** Multiple files

```csharp
PageSize = 50; // Why 50?
if (facture.Ordonnances?.Count >= 50)
if (facture.Ordonnances?.Count <= 25)
```

### 4.4 Unused Code
**Location:** `Repository.cs:30`

```csharp
//TODO Change Row Color To Red If Error I ORdo and Suspended Clients
```

TODO comments indicating incomplete work.

### 4.5 Inconsistent File Organization
- `ViewModels` in both `/ViewModel/` and `/Views/*/`
- `Services` folder contains ViewModels (`NavigableViewModel`, `ViewModelWithDialogs`)
- Converters in two locations (`/Converters/` and `/Resources/Converters/`)

### 4.6 UI Logic in ViewModel
**Location:** `VentViewModel.cs:84-95`

```csharp
public SolidColorBrush NameColor
    => new SolidColorBrush((!Assure?.Suspende ?? false) ? Colors.Green : Colors.Red);
```

Color/brush creation belongs in XAML converters, not ViewModels.

---

## 5. Suggested Refactorings

### 5.1 Implement Proper Unit of Work (Priority: HIGH)

```csharp
// Recommended structure:
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class, IEntity;
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly DbCon _context;
    
    public async Task<int> SaveChangesAsync() 
        => await _context.SaveChangesAsync();
    
    public void Dispose() => _context.Dispose();
}
```

### 5.2 Separate DTOs from Entities

```csharp
// Current (bad):
public class Patient : Data.Model.Patient { }

// Recommended:
public class PatientDto
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    // Only what's needed for the UI
}
```

### 5.3 Extract Navigation Logic

Move navigation from `Repository` base class to a dedicated `INavigationService`:

```csharp
public interface INavigationService
{
    Task NavigateTo<TView>() where TView : UserControl;
    Task GoBack();
    object CurrentView { get; }
}
```

### 5.4 Create Proper Service Layer

```csharp
public interface IPatientService
{
    Task<PatientDto> GetByIdAsync(int id);
    Task<IEnumerable<PatientDto>> SearchAsync(string query);
    Task<int> CreateAsync(CreatePatientRequest request);
    Task UpdateAsync(UpdatePatientRequest request);
    Task DeleteAsync(int id);
}
```

### 5.5 Clean Up ViewModel Base Classes

Current hierarchy:
```
ViewModelBase (MVVM Light)
  -> ViewModelWithDialogs
    -> NavigableViewModel
      -> Repository<TEntity, THomeView, TDetailsView>
```

Recommended:
```
ViewModelBase (MVVM Light)
  -> [Individual ViewModels compose services via DI]
```

---

## 6. Things That Should Remain Unchanged

### 6.1 MVVM Light Framework
Continue using MVVM Light - it's stable and well-understood by the team. Migrating to CommunityToolkit.Mvvm would be a low-priority improvement.

### 6.2 Entity Framework 6
While EF Core would be preferable, the migration effort isn't worth it for this project. Stay on EF6.

### 6.3 MahApps.Metro
The UI framework is appropriate for the project. Keep it.

### 6.4 WPF/.NET Framework 4.8
Given this is a desktop application with no need for cross-platform, staying on .NET Framework is acceptable. Migration to .NET 6+ would be beneficial but not critical.

### 6.5 Project Structure (3-Project Layout)
```
OrdoMill (Main WPF App)
OrdoMill.Data (EF Models)
OrdoMill.Resources (Localization/Resources)
```
This is a reasonable separation.

---

## 7. Priority Order

### Immediate (Week 1-2)
1. **Fix DbContext lifecycle** - Memory leak risk
2. **Remove [ImplementPropertyChanged] from DbCon** - EF compatibility
3. **Implement password hashing** - Security vulnerability
4. **Fix empty catch blocks** - Debugging capability

### Short-term (Week 3-4)
5. **Split Repository base class** - Maintainability
6. **Create proper DTOs** - Decoupling
7. **Fix AutoMapper configuration** - Code clarity
8. **Add proper async/await patterns** - UI responsiveness

### Medium-term (Month 2-3)
9. **Remove commented code** - Code cleanliness
10. **Fix naming inconsistencies** - Maintainability
11. **Extract hardcoded strings** - Localization support
12. **Reorganize file structure** - Discoverability

### Low Priority (Ongoing)
13. **Add unit tests** - Reliability
14. **Documentation** - Knowledge transfer
15. **Performance profiling** - Optimization

---

## 8. Metrics Summary

| Metric | Value | Target |
|--------|-------|--------|
| Files with >500 lines | 3 | 0 |
| Average method length | ~25 lines | <15 lines |
| Cyclomatic complexity (est.) | High | Medium |
| Test coverage | ~0% | >60% |
| DbContext instances per session | Multiple | 1 per operation |
| Async consistency | ~70% | 100% |

---

## 9. Conclusion

OrdoMill is a functional application that serves its business purpose, but has accumulated technical debt that will make future maintenance increasingly difficult. The most critical issues relate to DbContext lifecycle management and the mixing of concerns in the Repository base class.

The recommended approach is to:
1. Address critical issues immediately (security, memory)
2. Incrementally refactor toward cleaner architecture
3. Avoid big-bang rewrites - use the strangler fig pattern

**Estimated effort to reach acceptable state:** 6-8 weeks of focused refactoring

---

## 10. Applied Fixes Summary

The following critical and high-priority issues have been addressed:

### ✅ Fixed Issues:

#### 1. **Removed `[ImplementPropertyChanged]` from DbCon** (CRITICAL)
- **File:** `OrdoMill.Data\Model\DbCon.cs`
- **Change:** Removed `[ImplementPropertyChanged]` attribute and `using PropertyChanged;` directive
- **Reason:** DbContext should not have INotifyPropertyChanged as it interferes with EF change tracking
- **Impact:** Prevents unpredictable behavior and memory leaks

#### 2. **Implemented Secure Password Hashing** (CRITICAL)
- **File:** `OrdoMill\Services\PasswordHasher.cs` (NEW)
- **Algorithm:** PBKDF2 with SHA-256, 10,000 iterations
- **Features:**
  - Salt generation using cryptographic RNG
  - Secure hash verification with timing-safe comparison
  - Legacy password support for backward compatibility
  - Automatic rehashing of old passwords on successful login
- **Files Modified:**
  - `OrdoMill\Views\Home\LoginViewModel.cs` - Uses `PasswordHasher.VerifyPassword()`
  - `OrdoMill\Views\Users\UsersViewModel.cs` - Uses `PasswordHasher.HashPassword()` on save

#### 3. **Fixed AutoMapper Duplicate Configurations** (MEDIUM)
- **File:** `OrdoMill\ViewModel\ViewModelLocator.cs`
- **Change:** Removed duplicate `CreateMap` calls
- **Added:** Proper Entity-to-DTO and DTO-to-Entity mappings
- **Impact:** Cleaner configuration, better performance

#### 4. **Fixed Empty Catch Blocks** (MEDIUM)
- **Files Modified:**
  - `OrdoMill\Services\Repository.cs`
  - `OrdoMill\Services\Statics.cs`
  - `OrdoMill\Views\Vente\OrdoViewModel.cs`
- **Change:** Replaced empty/commented catch blocks with proper logging
- **Pattern:**
  ```csharp
  catch (OperationCanceledException)
  {
      // Expected when operation is cancelled
  }
  catch (Exception ex)
  {
      await ex.AppLoggingAsync();
  }
  ```

#### 5. **Implemented Unit of Work Pattern** (HIGH)
- **New Files:**
  - `OrdoMill\Interfaces\IUnitOfWork.cs`
  - `OrdoMill\Services\UnitOfWork.cs`
- **Features:**
  - Implements `IDisposable` for proper resource cleanup
  - Short-lived DbContext per unit of work
  - Async and sync save methods
  - All DbSets exposed through interface
- **Usage:** Inject `IUnitOfWork` instead of creating `DbCon` instances
- **Registered:** Added to DI container in `ViewModelLocator`

#### 6. **Created Proper DTO Classes** (MEDIUM)
- **File:** `OrdoMill\Dto\DtoV2.cs` (NEW)
- **Pattern:** True POCO DTOs without inheritance from EF entities
- **Classes:**
  - `AssureDto`, `PatientDto`, `OrdonnanceDto`
  - `FactureDto`, `BordereauDto`, `MedecinDto`
  - `MedicamentDto`, `MedOrdDto`, `FormeDto`
  - `PathologieDto`, `UserDto`, `InfoDto`, `HistoriqueDto`
- **Next Steps:** Migrate ViewModels to use new DTOs (breaking change)

### 📋 Remaining Tasks:

#### High Priority:
- [ ] **Migrate ViewModels to use UnitOfWork** (2-3 days)
  - Replace `new DbCon()` with injected `IUnitOfWork`
  - Ensure proper disposal of UnitOfWork instances
  
- [ ] **Update ViewModels to use new DTOs** (3-5 days)
  - Replace old DTOs in `Dto.cs` with new DTOs in `DtoV2.cs`
  - Update AutoMapper mappings
  - Test all CRUD operations

#### Medium Priority:
- [ ] **Add input validation** (1-2 days)
  - Validate user inputs in ViewModels
  - Add data annotations to DTOs
  
- [ ] **Implement proper async patterns** (2-3 days)
  - Ensure all database operations are async
  - Remove `Task.Delay(1)` anti-patterns

#### Low Priority:
- [ ] **Fix naming inconsistencies** (1 day)
  - `MedecinsTypesConvertr` → `MedecinsTypesConverter`
  - `BorderauView` → `BordereauView`
  - `Ordonance` → `Ordonnance`

- [ ] **Remove commented code** (0.5 day)
  - Clean up `Dto.cs` and `POCOS.cs` files

### Migration Guide for UnitOfWork:

**Before:**
```csharp
public class PatientsViewModel : Repository<Patient, ShowPatientsView, PatientDetailsView>
{
    protected DbCon Context { get; private set; }
    
    public PatientsViewModel()
    {
        Context = new DbCon(Settings.Default.ConnectionString);
    }
}
```

**After:**
```csharp
public class PatientsViewModel : Repository<Patient, ShowPatientsView, PatientDetailsView>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public PatientsViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // Use _unitOfWork.Patients instead of Context.Patients
}
```

### Migration Guide for Passwords:

**Database Migration Required:**
```sql
-- Run this to migrate existing plain-text passwords
-- Warning: This will invalidate all existing passwords
-- Users will need to reset their passwords after migration
UPDATE Users SET Password = ''; -- or generate temporary passwords
```

**Recommended Approach:**
1. Deploy new code with password hashing
2. Legacy passwords will automatically be detected and verified
3. On first successful login, password will be rehashed
4. No immediate database migration needed

---

## 11. Testing Recommendations

Before deploying to production:

1. **Test password authentication** with existing accounts
2. **Verify all CRUD operations** work with new DTOs
3. **Test UnitOfWork disposal** - no memory leaks
4. **Verify logging** - check all exceptions are logged
5. **Performance test** - ensure no regressions

---

*Report generated by architecture review analysis*  
*Fixes applied on: 2026-03-08*
