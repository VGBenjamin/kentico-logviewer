# Kentico Log Viewer — Conventions

## Stack
- .NET 10 Windows, WPF, MVVM (CommunityToolkit.Mvvm source generators)
- Dapper 2.x + Microsoft.Data.SqlClient 6.x
- System.Text.Json (config persistence), DPAPI (credentials encryption)

## Structure
```
src/KenticoLogViewer/
├── Models/        ConnectionConfig, EventLogEntry, LogFilter
├── Services/      IConnectionStore/ConnectionStore, ILogRepository/LogRepository
├── ViewModels/    MainViewModel, ConnectionManagerViewModel, LogDetailViewModel, FilterPanelViewModel
├── Views/         MainWindow, ConnectionManagerWindow, LogDetailWindow, FilterPanelControl
├── Converters/    EventTypeToColorConverter, TruncateTextConverter
└── Helpers/       DpapiHelper, ClipboardFormatter
```

## Immutable Rules
- **SQL**: 100% parameterized via `DynamicParameters` — never concatenate values
- **Table/Column names**: validated by whitelist before interpolation into SQL
- **Connection strings**: DPAPI encrypted at rest, decrypted on demand, never in static cache
- **MVVM**: `[ObservableProperty]` + `[RelayCommand]` — no code-behind except pure WPF events
- **Config**: `%AppData%\KenticoLogViewer\connections.json`

## CMS_EventLog — Columns
Grid: `EventID, EventType, EventTime, Source, EventCode, EventDescription`
Detail (by EventID): all columns
Hidden but filterable: `UserID, UserName, IPAddress, EventUrl, EventMachineName, EventUserAgent, EventUrlReferrer`
Coloration: E=`#FFEBEE`, W=`#FFF8E1`, I=white

## NuGet
```xml
<PackageReference Include="Dapper" Version="2.*" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="6.*" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
<PackageReference Include="System.Security.Cryptography.ProtectedData" Version="*" />
```
