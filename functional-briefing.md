# Functional Briefing — Kentico Log Viewer

## Objective
A Windows desktop application for viewing logs stored in a `CMS_EventLog` SQL table of a Kentico Xperience database. The application must allow managing multiple connections, filtering logs, and copying them easily.

---

## 1. Connection Management

### 1.1 Selection
- A dropdown at the top of the screen lists connections **by name**.
- Selecting a connection immediately triggers log loading.

### 1.2 Management (CRUD)
- A "Manage Connections" button opens a dedicated window.
- Available actions for each connection:
  - **Add** — enter a name and connection string + table name (default: `CMS_EventLog`)
  - **Edit** — modify all fields
  - **Rename** — modify only the name
  - **Delete** — with confirmation
- Connections are persisted locally (see technical section).

### 1.3 Configuration per Connection
| Field | Required | Default Value |
|---|---|---|
| Display Name | Yes | — |
| SQL Server Connection String | Yes | — |
| Table Name | Yes | `CMS_EventLog` |
| Number of logs to display | No | `500` |

---

## 2. Log Grid

### 2.1 Default Displayed Columns
| Column | Behavior |
|---|---|
| `EventID` | Integer, ascending sort possible |
| `EventType` | Short value (E / W / I), with color indicator |
| `EventTime` | Formatted datetime |
| `Source` | Text |
| `EventCode` | Text |
| `EventDescription` | Visually **truncated** text if too long |

Hidden columns by default (but filterable): `UserID`, `UserName`, `IPAddress`, `EventUrl`, `EventMachineName`, `EventUserAgent`, `EventUrlReferrer`.

### 2.2 Row Coloring by EventType
| EventType | Background Color |
|---|---|
| `E` (Error) | Light Red |
| `W` (Warning) | Light Orange/Yellow |
| `I` (Information) | White / Neutral |
| Other value | Light Gray |

### 2.3 Loading
- Default: **last 500 logs** (ORDER BY EventID DESC or EventTime DESC).
- The number is configurable per connection.

### 2.4 Double-click on a row
- Opens a **detail popin** (modal window).
- Displays **all fields** of the selected log, with long values untruncated (`EventDescription`, `EventUrl`, `EventUserAgent`, `EventUrlReferrer`).
- The popin must allow **copying** each field individually.

---

## 3. Filters

### 3.1 Column Header Filters (Visible Columns)
- Each visible column has a filter field directly in the DataGrid header.
- Behavior by field type:
  - **EventType**: list of checkboxes (`E`, `W`, `I`)
  - **EventTime**: date range (DateFrom / DateTo)
  - **Other texts**: free text field (LIKE `%value%`)
  - **EventID**: numeric field (equality or min/max bounds)
- Applying a filter **regenerates the SQL query** on the server side (no client-side filtering).

### 3.2 Extended Filter Panel (All Columns)
- An "Advanced Filters" button opens a side panel or expandable area.
- It exposes the same filter types for **all** table columns, including hidden ones.
- Allows combining multiple filters.

### 3.3 Reset
- A "Reset Filters" button clears all active filters and reloads data.

---

## 4. Auto-refresh

- Toggle ON/OFF visible in the toolbar. **Default: OFF.**
- Configurable interval (e.g., 5s / 10s / 30s / 60s) via an adjacent dropdown, visible only if toggle is ON.
- When ON: automatic refresh with active filters.
- A visual indicator (e.g., timestamp "Last update: HH:mm:ss") is displayed.

---

## 5. Copying Rows

- Single or multiple selection (Shift+click, Ctrl+clic) in the grid.
- `Ctrl+C` shortcut or "Copy" button.
- Copied format: **formatted text**, headers included, one row per log, separators `|`.
