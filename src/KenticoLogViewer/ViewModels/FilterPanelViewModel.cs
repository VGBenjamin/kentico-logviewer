using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KenticoLogViewer.Models;

namespace KenticoLogViewer.ViewModels;

public partial class FilterPanelViewModel : ObservableObject
{
    public event EventHandler? FilterChanged;

    private System.Threading.CancellationTokenSource? _debounceCts;
    private const int DebounceMs = 400;

    // --- EventID ---
    [ObservableProperty] private string _eventIdMinFilter = string.Empty;
    [ObservableProperty] private string _eventIdMaxFilter = string.Empty;

    // --- EventType checkboxes ---
    [ObservableProperty] private bool _filterTypeE = false;
    [ObservableProperty] private bool _filterTypeW = false;
    [ObservableProperty] private bool _filterTypeI = false;

    // --- EventTime range ---
    [ObservableProperty] private DateTime? _dateFrom;
    [ObservableProperty] private string _dateFromTimeStr = "00:00:00";
    [ObservableProperty] private DateTime? _dateTo;
    [ObservableProperty] private string _dateToTimeStr = "23:59:59";

    // --- Grid columns ---
    [ObservableProperty] private string _sourceFilter = string.Empty;
    [ObservableProperty] private string _eventCodeFilter = string.Empty;
    [ObservableProperty] private string _eventDescriptionFilter = string.Empty;

    // --- Advanced (hidden columns) ---
    [ObservableProperty] private string _userIdFilter = string.Empty;
    [ObservableProperty] private string _userNameFilter = string.Empty;
    [ObservableProperty] private string _ipAddressFilter = string.Empty;
    [ObservableProperty] private string _eventUrlFilter = string.Empty;
    [ObservableProperty] private string _eventMachineNameFilter = string.Empty;
    [ObservableProperty] private string _eventUserAgentFilter = string.Empty;
    [ObservableProperty] private string _eventUrlReferrerFilter = string.Empty;

    partial void OnEventIdMinFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnEventIdMaxFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnFilterTypeEChanged(bool value) => ScheduleFilterChanged();
    partial void OnFilterTypeWChanged(bool value) => ScheduleFilterChanged();
    partial void OnFilterTypeIChanged(bool value) => ScheduleFilterChanged();
    partial void OnDateFromChanged(DateTime? value) => ScheduleFilterChanged();
    partial void OnDateFromTimeStrChanged(string value) => ScheduleFilterChanged();
    partial void OnDateToChanged(DateTime? value) => ScheduleFilterChanged();
    partial void OnDateToTimeStrChanged(string value) => ScheduleFilterChanged();
    partial void OnSourceFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnEventCodeFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnEventDescriptionFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnUserIdFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnUserNameFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnIpAddressFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnEventUrlFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnEventMachineNameFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnEventUserAgentFilterChanged(string value) => ScheduleFilterChanged();
    partial void OnEventUrlReferrerFilterChanged(string value) => ScheduleFilterChanged();

    private void ScheduleFilterChanged()
    {
        _debounceCts?.Cancel();
        _debounceCts = new System.Threading.CancellationTokenSource();
        var token = _debounceCts.Token;
        var scheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

        _ = Task.Delay(DebounceMs, token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
                FilterChanged?.Invoke(this, EventArgs.Empty);
        }, scheduler);
    }

    [RelayCommand]
    public void Reset()
    {
        _debounceCts?.Cancel();

        EventIdMinFilter = string.Empty;
        EventIdMaxFilter = string.Empty;
        FilterTypeE = false;
        FilterTypeW = false;
        FilterTypeI = false;
        DateFrom = null;
        DateFromTimeStr = "00:00:00";
        DateTo = null;
        DateToTimeStr = "23:59:59";
        SourceFilter = string.Empty;
        EventCodeFilter = string.Empty;
        EventDescriptionFilter = string.Empty;
        UserIdFilter = string.Empty;
        UserNameFilter = string.Empty;
        IpAddressFilter = string.Empty;
        EventUrlFilter = string.Empty;
        EventMachineNameFilter = string.Empty;
        EventUserAgentFilter = string.Empty;
        EventUrlReferrerFilter = string.Empty;

        FilterChanged?.Invoke(this, EventArgs.Empty);
    }

    public LogFilter BuildFilter()
    {
        var types = new List<string>();
        if (FilterTypeE) types.Add("E");
        if (FilterTypeW) types.Add("W");
        if (FilterTypeI) types.Add("I");

        return new LogFilter
        {
            EventIdMinFilter = EventIdMinFilter,
            EventIdMaxFilter = EventIdMaxFilter,
            EventTypes = types,
            DateFrom = CombineDateTime(DateFrom, DateFromTimeStr),
            DateTo = CombineDateTime(DateTo, DateToTimeStr),
            SourceFilter = SourceFilter,
            EventCodeFilter = EventCodeFilter,
            EventDescriptionFilter = EventDescriptionFilter,
            UserIdFilter = UserIdFilter,
            UserNameFilter = UserNameFilter,
            IpAddressFilter = IpAddressFilter,
            EventUrlFilter = EventUrlFilter,
            EventMachineNameFilter = EventMachineNameFilter,
            EventUserAgentFilter = EventUserAgentFilter,
            EventUrlReferrerFilter = EventUrlReferrerFilter,
        };
    }

    private static DateTime? CombineDateTime(DateTime? date, string timeStr)
    {
        if (date is null) return null;
        return TimeSpan.TryParse(timeStr, out var t)
            ? date.Value.Date.Add(t)
            : date.Value.Date;
    }
}
