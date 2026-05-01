// Logic of the main application window.
using System.ComponentModel;
using FormulaOneManager.Library.Domain.Drivers;
using FormulaOneManager.Library.Domain.Teams;
using FormulaOneManager.Library.Events;
using FormulaOneManager.Library.Exceptions;
using FormulaOneManager.Library.Extensions;
using FormulaOneManager.Library.Persistence;
using FormulaOneManager.Library.Services;

namespace FormulaOneManager.App;

// Main window of the Formula 1 Manager application. The MenuStrip provides
// the File / Help menu, four tabs hold the lists of entities and statistics.
public partial class MainForm : Form
{
    // Single shared service exposing the repositories.
    private readonly RacingService _service = new();

    // BindingList wrappers used to refresh the grids automatically.
    private readonly BindingList<Driver> _driversView = new();
    private readonly BindingList<Team> _teamsView = new();
    private readonly BindingList<ContractRow> _contractsView = new();

    // Special string shown in the category filter to mean "no filter".
    private const string AllCategoriesLabel = "(all)";

    // Constructor wires up the form, the seed data and every event handler.
    public MainForm()
    {
        // Initialize controls created in the designer file.
        InitializeComponent();

        // Subscribe to the library events using our custom delegates so we
        // can update the status bar whenever something changes.
        _service.Log = WriteStatus;
        _service.DriverAdded += OnDriverAdded;
        _service.DriverSigned += OnDriverSigned;

        // Bind the data sources to the grids.
        driversGrid.DataSource = _driversView;
        teamsGrid.DataSource = _teamsView;
        contractsGrid.DataSource = _contractsView;

        // Wire up the menu items.
        newMenuItem.Click += (_, _) => NewSeason();
        openMenuItem.Click += (_, _) => LoadFromDisk();
        saveMenuItem.Click += (_, _) => SaveToDisk();
        exitMenuItem.Click += (_, _) => Close();
        aboutMenuItem.Click += (_, _) => ShowAbout();

        // Wire up the Drivers tab.
        addDriverButton.Click += (_, _) => AddDriver();
        editDriverButton.Click += (_, _) => EditDriver();
        removeDriverButton.Click += (_, _) => RemoveDriver();
        signDriverButton.Click += (_, _) => SignContract();
        searchBox.TextChanged += (_, _) => RefreshDriversGrid();
        categoryFilter.SelectedIndexChanged += (_, _) => RefreshDriversGrid();
        onlyAvailableCheck.CheckedChanged += (_, _) => RefreshDriversGrid();
        driversGrid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) EditDriver(); };

        // Wire up the Teams tab.
        addTeamButton.Click += (_, _) => AddTeam();
        editTeamButton.Click += (_, _) => EditTeam();
        removeTeamButton.Click += (_, _) => RemoveTeam();
        teamsGrid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) EditTeam(); };

        // Wire up the Statistics tab.
        refreshStatsButton.Click += (_, _) => RefreshStatistics();

        // Refresh the relevant grid whenever the user switches tabs.
        tabControl.SelectedIndexChanged += (_, _) =>
        {
            if (tabControl.SelectedTab == statisticsTab) RefreshStatistics();
            else if (tabControl.SelectedTab == contractsTab) RefreshContractsGrid();
            else if (tabControl.SelectedTab == teamsTab) RefreshTeamsGrid();
            else if (tabControl.SelectedTab == driversTab) RefreshDriversGrid();
        };

        // Populate category combo box with the "all" option plus distinct categories.
        ResetCategoryFilter();

        // Load demo data on first run.
        SeedData.Populate(_service);

        // Refresh every visible list now that data is in place.
        RefreshAllGrids();
    }

    // ------------------------------------------------------------------
    // Event handlers fired by the service through delegate types.
    // ------------------------------------------------------------------

    // Fired when the service confirms that a driver was added.
    private void OnDriverAdded(object? sender, DriverEventArgs e) => RefreshDriversGrid();

    // Fired when the service confirms a contract signing.
    private void OnDriverSigned(object? sender, ContractEventArgs e)
    {
        RefreshDriversGrid();
        RefreshContractsGrid();
    }

    // ------------------------------------------------------------------
    // Drivers tab actions.
    // ------------------------------------------------------------------

    // Opens the edit dialog with a brand new driver.
    private void AddDriver()
    {
        // Show the editor with no pre-existing entity.
        using DriverEditForm dialog = new DriverEditForm();
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is null)
            return;

        // Use the service so validation runs and events fire.
        TryRun(() => _service.AddDriver(dialog.Result));
        ResetCategoryFilter();
        RefreshDriversGrid();
    }

    // Opens the editor with the currently selected driver.
    private void EditDriver()
    {
        Driver? selected = GetSelectedDriver();
        if (selected is null) return;

        // Pass the selected entity to the editor for in-place editing.
        using DriverEditForm dialog = new DriverEditForm(selected);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is null)
            return;

        TryRun(() => _service.UpdateDriver(dialog.Result));
        ResetCategoryFilter();
        RefreshDriversGrid();
    }

    // Removes the currently selected driver after confirmation.
    private void RemoveDriver()
    {
        Driver? selected = GetSelectedDriver();
        if (selected is null) return;

        // Warn the user when removing a driver that has contract records.
        bool hasContracts = _service.Contracts.GetForDriver(selected.Id).Any();
        string warning = hasContracts
            ? $"'{selected.DisplayName}' has contract records.\nDeleting will orphan those records.\n\nContinue anyway?"
            : $"Remove '{selected.DisplayName}' from the system?";

        DialogResult confirm = MessageBox.Show(this,
            warning,
            "Confirm removal",
            MessageBoxButtons.YesNo,
            hasContracts ? MessageBoxIcon.Warning : MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        TryRun(() => _service.RemoveDriver(selected.Id));
        ResetCategoryFilter();
        RefreshDriversGrid();
        RefreshContractsGrid();
    }

    // Opens the contract signing dialog for the currently selected driver.
    private void SignContract()
    {
        Driver? selected = GetSelectedDriver();
        if (selected is null) return;

        // Reject signing attempts at the GUI level for nicer feedback. The
        // detailed reason comes straight from the domain model so the user
        // sees exactly why the driver is unavailable (suspended, injured,
        // under-age for F1, over-age for karting, already signed, ...).
        string? issue = selected.GetAvailabilityIssue();
        if (issue != null)
        {
            MessageBox.Show(this,
                $"Cannot sign {selected.DisplayName}.\n\n{issue}",
                "Contract refused",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        // Make sure we have at least one team on file.
        if (_service.Teams.Count == 0)
        {
            MessageBox.Show(this,
                "Please add a team first (Teams tab).",
                "Contract requires team",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            tabControl.SelectedTab = teamsTab;
            return;
        }

        // Open the dedicated contract dialog.
        using ContractForm dialog = new ContractForm(_service.Teams.GetAll(), selected);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedTeam is null)
            return;

        TryRun(() => _service.SignContract(
            selected.Id,
            dialog.SelectedTeam.Id,
            dialog.DurationSeasons,
            dialog.ValueMillions,
            dialog.Notes));
        RefreshDriversGrid();
        RefreshContractsGrid();
    }

    // Returns the driver currently selected in the grid, or null.
    private Driver? GetSelectedDriver()
    {
        if (driversGrid.CurrentRow?.DataBoundItem is Driver selected)
            return selected;

        MessageBox.Show(this, "Please select a driver first.", "No selection",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        return null;
    }

    // ------------------------------------------------------------------
    // Teams tab actions.
    // ------------------------------------------------------------------

    // Opens the editor with a new team.
    private void AddTeam()
    {
        using TeamEditForm dialog = new TeamEditForm();
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is null)
            return;

        TryRun(() => _service.AddTeam(dialog.Result));
        RefreshTeamsGrid();
    }

    // Opens the editor with the selected team.
    private void EditTeam()
    {
        Team? selected = GetSelectedTeam();
        if (selected is null) return;

        using TeamEditForm dialog = new TeamEditForm(selected);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is null)
            return;

        TryRun(() => _service.UpdateTeam(dialog.Result));
        RefreshTeamsGrid();
        RefreshContractsGrid();
    }

    // Removes the currently selected team after confirmation.
    private void RemoveTeam()
    {
        Team? selected = GetSelectedTeam();
        if (selected is null) return;

        bool hasContracts = _service.Contracts.GetAll()
            .Any(c => c.TeamId == selected.Id);
        string warning = hasContracts
            ? $"Team '{selected.DisplayName}' has contract records.\nDeleting will orphan those records.\n\nContinue anyway?"
            : $"Remove team '{selected.DisplayName}'?";

        DialogResult confirm = MessageBox.Show(this,
            warning,
            "Confirm removal",
            MessageBoxButtons.YesNo,
            hasContracts ? MessageBoxIcon.Warning : MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        TryRun(() => _service.RemoveTeam(selected.Id));
        RefreshTeamsGrid();
        RefreshContractsGrid();
    }

    // Returns the currently selected team, or null.
    private Team? GetSelectedTeam()
    {
        if (teamsGrid.CurrentRow?.DataBoundItem is Team selected)
            return selected;

        MessageBox.Show(this, "Please select a team first.", "No selection",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        return null;
    }

    // ------------------------------------------------------------------
    // File operations using the persistence abstraction.
    // ------------------------------------------------------------------

    // Resets every repository so the user can start a new season.
    private void NewSeason()
    {
        DialogResult confirm = MessageBox.Show(this,
            "Clear every driver, team and contract record?",
            "Confirm new season",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        _service.Drivers.Clear();
        _service.Teams.Clear();
        _service.Contracts.Clear();
        ResetCategoryFilter();
        RefreshAllGrids();
        WriteStatus("Season cleared.");
    }

    // Asks the user for a file and loads it using the matching storage type.
    private void LoadFromDisk()
    {
        using OpenFileDialog dialog = new()
        {
            Title = "Load racing data",
            Filter = "JSON snapshot (*.json)|*.json|XML snapshot (*.xml)|*.xml"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        // Pick the storage implementation based on the file extension.
        IRacingStorage storage = SelectStorageByExtension(dialog.FileName);

        TryRun(() =>
        {
            RacingSnapshot snapshot = storage.Load(dialog.FileName);
            _service.LoadSnapshot(snapshot.Drivers, snapshot.Teams, snapshot.Contracts);
            ResetCategoryFilter();
            RefreshAllGrids();
            WriteStatus($"Loaded {snapshot.Drivers.Count} drivers from '{dialog.FileName}'.");
        });
    }

    // Asks the user for a file and saves the current state.
    private void SaveToDisk()
    {
        using SaveFileDialog dialog = new()
        {
            Title = "Save racing data",
            Filter = "JSON snapshot (*.json)|*.json|XML snapshot (*.xml)|*.xml",
            FileName = "racing.json"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        IRacingStorage storage = SelectStorageByExtension(dialog.FileName);

        // Build the snapshot from the current repositories using LINQ.
        RacingSnapshot snapshot = new()
        {
            Drivers = _service.Drivers.GetAll().ToList(),
            Teams = _service.Teams.GetAll().ToList(),
            Contracts = _service.Contracts.GetAll().ToList()
        };

        TryRun(() =>
        {
            storage.Save(dialog.FileName, snapshot);
            WriteStatus($"Saved {snapshot.Drivers.Count} drivers to '{dialog.FileName}'.");
        });
    }

    // Returns the storage implementation matching the given extension.
    private static IRacingStorage SelectStorageByExtension(string path) =>
        Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".xml" => new XmlRacingStorage(),
            _ => new JsonRacingStorage()
        };

    // Shows a small "About" dialog.
    private void ShowAbout()
    {
        MessageBox.Show(this,
            "Formula 1 Manager\n" +
            "Advanced Programming - lab 8/9\n" +
            "Demonstrates abstraction layers, generics, custom exceptions,\n" +
            "delegates, LINQ and JSON/XML serialization in WinForms.\n",
            "About",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    // ------------------------------------------------------------------
    // Refresh helpers.
    // ------------------------------------------------------------------

    // Refreshes every grid at once. Used after big data changes.
    private void RefreshAllGrids()
    {
        RefreshDriversGrid();
        RefreshTeamsGrid();
        RefreshContractsGrid();
        RefreshStatistics();
    }

    // Refreshes the Drivers grid applying the current filters.
    private void RefreshDriversGrid()
    {
        // Keep the currently selected driver id so we can reselect it.
        Guid selectedId = (driversGrid.CurrentRow?.DataBoundItem as Driver)?.Id ?? Guid.Empty;

        // Combine LINQ queries: search, category filter, only available check.
        IEnumerable<Driver> query = _service.SearchDrivers(searchBox.Text);

        // Apply the category filter when something other than "(all)" is selected.
        string? categoryValue = categoryFilter.SelectedItem as string;
        if (!string.IsNullOrWhiteSpace(categoryValue) &&
            categoryValue != AllCategoriesLabel)
        {
            query = query.ByCategory(categoryValue);
        }

        // Apply the "only available" filter when checked.
        if (onlyAvailableCheck.Checked)
            query = query.AvailableOnly();

        // Sort by category then by last name with a chained LINQ query.
        List<Driver> ordered = query
            .OrderBy(d => d.Category)
            .ThenBy(d => d.LastName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _driversView.RaiseListChangedEvents = false;
        _driversView.Clear();
        foreach (Driver driver in ordered)
            _driversView.Add(driver);
        _driversView.RaiseListChangedEvents = true;
        _driversView.ResetBindings();

        // Try to restore the previous selection.
        if (selectedId != Guid.Empty)
        {
            for (int i = 0; i < _driversView.Count; i++)
            {
                if (_driversView[i].Id == selectedId)
                {
                    driversGrid.ClearSelection();
                    driversGrid.Rows[i].Selected = true;
                    break;
                }
            }
        }
    }

    // Refreshes the Teams grid.
    private void RefreshTeamsGrid()
    {
        Guid selectedId = (teamsGrid.CurrentRow?.DataBoundItem as Team)?.Id ?? Guid.Empty;

        List<Team> ordered = _service.Teams.GetAll().OrderByName().ToList();

        _teamsView.RaiseListChangedEvents = false;
        _teamsView.Clear();
        foreach (Team team in ordered)
            _teamsView.Add(team);
        _teamsView.RaiseListChangedEvents = true;
        _teamsView.ResetBindings();

        if (selectedId != Guid.Empty)
        {
            for (int i = 0; i < _teamsView.Count; i++)
            {
                if (_teamsView[i].Id == selectedId)
                {
                    teamsGrid.ClearSelection();
                    teamsGrid.Rows[i].Selected = true;
                    break;
                }
            }
        }
    }

    // Refreshes the Contracts grid by joining contracts with drivers/teams.
    private void RefreshContractsGrid()
    {
        // LINQ join across repositories to build display rows.
        IEnumerable<ContractRow> rows =
            from contract in _service.Contracts.GetAll()
            join driver in _service.Drivers.GetAll() on contract.DriverId equals driver.Id into driverGroup
            from driver in driverGroup.DefaultIfEmpty()
            join team in _service.Teams.GetAll() on contract.TeamId equals team.Id into teamGroup
            from team in teamGroup.DefaultIfEmpty()
            orderby contract.SignedDate descending
            select new ContractRow
            {
                Date = contract.SignedDate.ToString("yyyy-MM-dd"),
                DriverName = driver != null ? $"{driver.FirstName} {driver.LastName}" : "(deleted)",
                TeamName = team?.TeamName ?? "(deleted)",
                Duration = contract.DurationSeasons,
                Value = contract.ValueMillions,
                Notes = contract.Notes
            };

        _contractsView.RaiseListChangedEvents = false;
        _contractsView.Clear();
        foreach (ContractRow row in rows)
            _contractsView.Add(row);
        _contractsView.RaiseListChangedEvents = true;
        _contractsView.ResetBindings();
    }

    // Renders the statistics report.
    private void RefreshStatistics() =>
        statisticsBox.Text = _service.BuildStatisticsReport();

    // Re-populates the category combo box. Called whenever the data changes.
    private void ResetCategoryFilter()
    {
        // Save current selection so we can restore it after rebuilding.
        string previous = categoryFilter.SelectedItem as string ?? AllCategoriesLabel;

        // LINQ DISTINCT to gather every category currently in the repository.
        List<string> categories = _service.Drivers.GetAll()
            .Select(d => d.Category)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Build the combo entries: "(all)" + categories.
        categoryFilter.BeginUpdate();
        categoryFilter.Items.Clear();
        categoryFilter.Items.Add(AllCategoriesLabel);
        foreach (string c in categories)
            categoryFilter.Items.Add(c);
        // Restore the previous selection if still present, otherwise default to "(all)".
        int index = categoryFilter.Items.IndexOf(previous);
        categoryFilter.SelectedIndex = index >= 0 ? index : 0;
        categoryFilter.EndUpdate();
    }

    // ------------------------------------------------------------------
    // Utility helpers.
    // ------------------------------------------------------------------

    // Runs the supplied action and converts library exceptions into friendly
    // dialogs. This single helper keeps every event handler short.
    private void TryRun(Action action)
    {
        try
        {
            action();
        }
        catch (RacingException ex)
        {
            MessageBox.Show(this, ex.Message, "Racing error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            // Unknown errors get a more technical message but never crash the app.
            MessageBox.Show(this, ex.ToString(), "Unexpected error",
                MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }

    // Updates the status bar; subscribed to RacingService.Log delegate.
    private void WriteStatus(string message)
    {
        // Marshal back to the UI thread when needed.
        if (statusStrip.InvokeRequired)
        {
            statusStrip.BeginInvoke((Action<string>)WriteStatus, message);
            return;
        }
        statusLabel.Text = message;
    }

    // Tiny DTO used by the Contracts grid; keeping it nested keeps the API surface clean.
    private sealed class ContractRow
    {
        public string Date { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public double Value { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
