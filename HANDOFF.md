# Kentico Log Viewer — Handoff Session 1

## What has been done

### Infrastructure
- `global.json` → Fixed .NET 10.0.203 SDK
- `KenticoLogViewer.sln` + `src/KenticoLogViewer/KenticoLogViewer.csproj` (WPF, net10.0-windows)
- NuGet: Dapper 2.x, Microsoft.Data.SqlClient 6.x, CommunityToolkit.Mvvm 8.x, System.Security.Cryptography.ProtectedData

### Models (`src/KenticoLogViewer/Models/`)
- `ConnectionConfig.cs` — Id, Name, EncryptedConnectionString, TableName, MaxRows
- `EventLogEntry.cs` — grid columns + hidden columns (UserID…EventUrlReferrer)
- `LogFilter.cs` — all filter criteria (EventIdMin/Max, EventTypes, DateFrom/DateTo, LIKE on each column)

### Helpers (`src/KenticoLogViewer/Helpers/`)
- `DpapiHelper.cs` — Encrypt/Decrypt via ProtectedData (DPAPI CurrentUser)
- `ClipboardFormatter.cs` — tabular format `|` with headers

### Services (`src/KenticoLogViewer/Services/`)
- `IConnectionStore` + `ConnectionStore` — Load/Save JSON in `%AppData%\KenticoLogViewer\connections.json`
- `ILogRepository` + `LogRepository` — 100% parameterized Dapper queries, table whitelist regex, `GetLogsAsync` (grid columns) + `GetByIdAsync` (all columns), `CancellationToken` everywhere

### ViewModels (`src/KenticoLogViewer/ViewModels/`)
- `FilterPanelViewModel` — all filter properties, 400ms debounce, `BuildFilter()`, `ResetCommand`
- `LogDetailViewModel` — `LoadAsync`, `Fields` (`ObservableCollection<LogField>`), `CopyFieldCommand`
- `ConnectionManagerViewModel` — connection CRUD, `StartAdd/Edit/Save/Cancel/Delete` via RelayCommand, encryption on write
- `MainViewModel` — `Connections`, `Logs`, `SelectedLogs`, `Filter`, auto-refresh `DispatcherTimer`, `RefreshCommand`, `CopySelectedCommand`, `ReloadConnectionsCommand`, `ToggleAdvancedFiltersCommand`, `ResetFiltersCommand`

### Converters (`src/KenticoLogViewer/Converters/`)
- `EventTypeToColorConverter` — E=#FFEBEE, W=#FFF8E1, I=White, other=#F5F5F5
- `TruncateTextConverter` — cuts at 200 chars with `…`

### Views (`src/KenticoLogViewer/Views/`)
- `FilterPanelControl.xaml/.cs` — Advanced filters panel UserControl (all fields)
- `LogDetailWindow.xaml/.cs` — modal, ItemsControl on `Fields`, copy button per field
- `ConnectionManagerWindow.xaml/.cs` — Connection ListBox + inline Add/Edit form
- `MainWindow.xaml/.cs` — ToolBar (connection dropdown, manage, refresh, filters, auto-refresh, copy), virtualized DataGrid + sorting, EventType row styles, double-clic → LogDetailWindow, Ctrl+C, collapsible FilterPanel, StatusBar

### App.xaml / App.xaml.cs
- `StartupUri` removed, manual instantiation in `OnStartup` (dependency injection)

### Build
✅ `dotnet build` — **succeeded** (1 benign NU1510 warning on ProtectedData already included in net10)

---

## Remaining tasks (session 2+)

### UX / Polishing
- [ ] **Connection deletion confirmation** — `MessageBox.Show` in `ConnectionManagerViewModel.Delete` or in `ConnectionManagerWindow` code-behind before calling `DeleteCommand`
- [ ] **Connection form validation** — disable "Save" if fields are empty (CanExecute on `SaveEditCommand`)
- [ ] **Error feedback** in `MainWindow` when `ErrorMessage` is not empty (visible red TextBlock / notification)
- [ ] **Error feedback** in `LogDetailWindow` if loading fails

### Missing functionality
- [ ] **Column header filters in DataGrid** — currently only in the advanced panel. According to the briefing, each visible column must have its own filter control in the header (custom HeaderTemplate with inline TextBox/CheckBox/DatePicker). Complexity ≈ medium.
- [ ] **Grid multi-selection + Ctrl+C** — ApplicationCommands.Copy CommandBinding exists, check that `SelectedLogs` is correctly synchronized via `OnGridSelectionChanged`. To be tested manually.
- [ ] **`EventType` filter by checkboxes in column header** — if inline filters are added.

### Quality / Robustness
- [ ] **Configurable SQL Timeout** — currently SqlClient connection with default timeout (30s). Add a `CommandTimeout` on the `SqlConnection`.
- [ ] **Handling connections without data** — if the table is empty or not found, display a clear message (e.g., `StatusMessage = "No data"`).
- [ ] **Connection test** in `ConnectionManagerWindow` — "Test" button that does a `SELECT TOP 1` to validate connection + table.

### Packaging / Distribution
- [ ] **Publish**: `win-x64` self-contained + single-file profile (`dotnet publish -r win-x64 --self-contained -p:PublishSingleFile=true`)
- [ ] **Application Icon** — add an `.ico` in `.csproj` resources
- [ ] **User README**

---

## Created files

```
src/KenticoLogViewer/
├── global.json
├── KenticoLogViewer.sln
├── App.xaml / App.xaml.cs
├── Models/
│   ├── ConnectionConfig.cs
│   ├── EventLogEntry.cs
│   └── LogFilter.cs
├── Services/
│   ├── IConnectionStore.cs
│   ├── ConnectionStore.cs
│   ├── ILogRepository.cs
│   └── LogRepository.cs
├── ViewModels/
│   ├── FilterPanelViewModel.cs
│   ├── LogDetailViewModel.cs
│   ├── ConnectionManagerViewModel.cs
│   └── MainViewModel.cs
├── Converters/
│   ├── EventTypeToColorConverter.cs
│   └── TruncateTextConverter.cs
├── Helpers/
│   ├── DpapiHelper.cs
│   └── ClipboardFormatter.cs
└── Views/
    ├── FilterPanelControl.xaml / .cs
```
