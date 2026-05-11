# Technical Briefing — Kentico Log Viewer

## Stack

| Component | Choice | Justification |
|---|---|---|
| Framework | **.NET 10 (Windows)** | Explicit target, LTS |
| UI | **WPF** | Rich controls, native DataGrid, MVVM support |
| Data Access | **Dapper 2.x** | Lightweight, manual SQL queries (needed for dynamic filter generation) |
| MVVM | **CommunityToolkit.Mvvm** | Source generators, `ObservableObject`, `RelayCommand` |
| Config Serialization | **System.Text.Json** | Native .NET, no extra dependency |
| Encryption | **System.Security.Cryptography.ProtectedData** (DPAPI) | Windows account-based encryption, no key to manage |

---

## Project Structure

```
KenticoLogViewer/
├── KenticoLogViewer.sln
└── src/
    └── KenticoLogViewer/                  # Main WPF Project
        ├── KenticoLogViewer.csproj
        ├── App.xaml / App.xaml.cs
        ├── Models/
        │   ├── ConnectionConfig.cs        # Name, ConnectionString (encrypted), TableName, MaxRows
        │   ├── EventLogEntry.cs           # Dapper → POCO mapping
        │   └── LogFilter.cs              # Active filter criteria
        ├── Services/
        │   ├── IConnectionStore.cs
        │   ├── ConnectionStore.cs         # JSON read/write + DPAPI
        │   ├── ILogRepository.cs
        │   └── LogRepository.cs           # SQL queries via Dapper
        ├── ViewModels/
        │   ├── MainViewModel.cs
        │   ├── ConnectionManagerViewModel.cs
        │   ├── LogDetailViewModel.cs
        │   └── FilterPanelViewModel.cs
        ├── Views/
        │   ├── MainWindow.xaml / .cs
        │   ├── ConnectionManagerWindow.xaml / .cs
        │   ├── LogDetailWindow.xaml / .cs
        │   └── FilterPanelControl.xaml / .cs    # Advanced filters panel UserControl
        ├── Converters/
        │   ├── EventTypeToColorConverter.cs
        │   └── TruncateTextConverter.cs
        └── Helpers/
            ├── DpapiHelper.cs             # Encrypt/Decrypt string via ProtectedData
            └── ClipboardFormatter.cs      # Text formatting for copy
```

---

## NuGet Packages

```xml
<PackageReference Include="Dapper" Version="2.*" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="6.*" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
```

> `System.Security.Cryptography.ProtectedData` is included in the Windows SDK via the `System.Security.Cryptography.ProtectedData` package (to be explicitly referenced in .NET 10).

---

## Connection Persistence

### Location
```
%AppData%\KenticoLogViewer\connections.json
```

### JSON Structure (example)
```json
[
  {
    "Id": "3f2a...",
    "Name": "Production RMB",
    "EncryptedConnectionString": "AQAAANCMnd8B...",
    "TableName": "CMS_EventLog",
    "MaxRows": 500
  }
]
```

### DPAPI Encryption
- The plain connection string is encrypted with `ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser)` before serialization.
- It is decrypted on the fly only to open the SQL connection.
- The connection string is never kept in memory longer than necessary.

---

## Data Layer — LogRepository

### Query Generation Principle
Filters are built dynamically but **always parameterized** (no value concatenation in SQL) to avoid SQL injection.

```csharp
// Pseudo-code
var where = new List<string>();
```
