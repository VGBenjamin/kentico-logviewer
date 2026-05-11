# Kentico Log Viewer

WPF Application (.NET 10, Windows) to view `CMS_EventLog` logs from Kentico Xperience databases.

## Prerequisites

- Windows 10 / 11
- Network access to a Kentico Xperience SQL Server instance
- .NET 10 Runtime (or use the self-contained executable)

## Launching the application

### Development Mode

```powershell
cd d:\Kentico-Log-Viewer
dotnet run --project src/KenticoLogViewer/KenticoLogViewer.csproj
```

### Publish a self-contained executable (win-x64, single file)

```powershell
cd d:\Kentico-Log-Viewer
dotnet publish src/KenticoLogViewer/KenticoLogViewer.csproj /p:PublishProfile=win-x64
```

The executable is generated in `src/KenticoLogViewer/bin/Publish/win-x64/KenticoLogViewer.exe`.

---

## First connection

1. Launch the application.
2. Click on **Manage…** in the toolbar.
3. Click on **Add**, and fill in the fields:
   - **Name**: label displayed in the dropdown list
   - **Connection string**: SQL Server connection string (e.g., `Server=myserver;Database=mydb;Integrated Security=True;TrustServerCertificate=True;`)
   - **Table name**: default `CMS_EventLog`
   - **Max rows**: maximum number of rows loaded (default 500)
   - **Timeout (s)**: SQL query timeout in seconds (default 30)
4. Click on **Test connection** to validate before saving.
5. Click on **Save**.

Connections are stored encrypted (DPAPI) in `%AppData%\KenticoLogViewer\connections.json`.

---

## Usage

### Toolbar

| Control | Action |
| --- | --- |
| Dropdown list | Select the active connection |
| **Manage…** | Open the connection manager (add / edit / delete) |
| **⟳ Refresh** | Reload logs with active filters |
| **Advanced filters** | Show / hide the advanced filters panel |
| **Reset filters** | Clear all filters |
| **Auto-refresh** | Enable automatic reloading |
| Interval | 5 s / 10 s / 30 s / 60 s |
| **📋 Copy selection** | Copy selected rows to the clipboard |

### Column header filters

Each grid column has a filter field in its header:

- **EventID**: filter on minimum value
- **Type**: check E (Error), W (Warning), I (Information) — multiple selections possible
- **Date/Time**: From / To range
- **Source, Code, Description**: `LIKE` search (partial match)

Each filter modification automatically triggers a new query after 400 ms.

### Advanced filters panel

The side panel (**Advanced filters** button) also exposes hidden columns:
`UserName`, `IPAddress`, `EventUrl`, `EventMachineName`, `EventUserAgent`, `EventUrlReferrer`.

### Event detail

Double-click on a row to open the detail window. Each field has a **📋** button to copy its value.

### Multi-copy

Select multiple rows (Shift+click, Ctrl+clic) then **Ctrl+C** or **📋 Copy selection**.  
Pasted format:

```text
EventID | EventType | EventTime | Source | EventCode | EventDescription
1234    | E         | 2026-05-11 14:00:00 | MyModule | MY_ERROR | Something went wrong
```

### Row coloring

| Color | Type |
| --- | --- |
| Light Red `#FFEBEE` | Error (E) |
| Light Yellow `#FFF8E1` | Warning (W) |
| White | Information (I) |
