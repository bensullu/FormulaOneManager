// Facade exposing the high level operations used by the GUI.
using FormulaOneManager.Library.Domain.Contracts;
using FormulaOneManager.Library.Domain.Drivers;
using FormulaOneManager.Library.Domain.Teams;
using FormulaOneManager.Library.Events;
using FormulaOneManager.Library.Exceptions;
using FormulaOneManager.Library.Extensions;
using FormulaOneManager.Library.Repositories;

namespace FormulaOneManager.Library.Services;

// The service owns the three repositories and centralizes business rules.
// It also exposes the delegate-based events so the GUI can react to them.
public class RacingService
{
    // Repository keeping every driver registered in the system.
    public DriverRepository Drivers { get; } = new();

    // Repository keeping the registered teams.
    public TeamRepository Teams { get; } = new();

    // Repository keeping the contract records.
    public ContractRepository Contracts { get; } = new();

    // Event raised whenever a new driver is added to the system.
    public event DriverAddedHandler? DriverAdded;

    // Event raised whenever a contract is successfully signed.
    public event DriverSignedHandler? DriverSigned;

    // Plain delegate based logger consumers can subscribe to.
    public RacingLogHandler? Log;

    // Adds a driver after running validation and notifies subscribers.
    public void AddDriver(Driver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);

        // Field level validation - throws our custom exception when failing.
        ValidateDriver(driver);

        Drivers.Add(driver);
        Log?.Invoke($"Driver added: {driver.DisplayName}");
        DriverAdded?.Invoke(this, new DriverEventArgs(driver));
    }

    // Updates a driver in place (Id must already exist).
    public void UpdateDriver(Driver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        ValidateDriver(driver);
        Drivers.Update(driver);
        Log?.Invoke($"Driver updated: {driver.DisplayName}");
    }

    // Removes the driver whose Id is given.
    public void RemoveDriver(Guid id)
    {
        Drivers.Remove(id);
        Log?.Invoke($"Driver removed: {id}");
    }

    // Adds a team after light validation.
    public void AddTeam(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);
        ValidateTeam(team);
        Teams.Add(team);
        Log?.Invoke($"Team added: {team.DisplayName}");
    }

    // Updates an existing team.
    public void UpdateTeam(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);
        ValidateTeam(team);
        Teams.Update(team);
        Log?.Invoke($"Team updated: {team.DisplayName}");
    }

    // Removes the team whose Id is given.
    public void RemoveTeam(Guid id)
    {
        Teams.Remove(id);
        Log?.Invoke($"Team removed: {id}");
    }

    // Performs the contract signing transaction enforcing every business rule.
    public Contract SignContract(Guid driverId, Guid teamId, int durationSeasons,
        double valueMillions, string notes = "")
    {
        // Fetch the entities up front so missing ones produce clear errors.
        Driver driver = Drivers.GetById(driverId);
        Team team = Teams.GetById(teamId);

        // Ask the driver for the first business rule that prevents the signing.
        // The returned sentence already explains the cause (suspended, injured,
        // under-age for F1, over-age for karting, already signed, ...).
        string? issue = driver.GetAvailabilityIssue();
        if (issue != null)
            throw new InvalidContractException(
                $"Cannot sign {driver.DisplayName}. {issue}");

        // Validate contract terms.
        if (durationSeasons <= 0)
            throw new ValidationException(nameof(durationSeasons), "Duration must be at least 1 season.");
        if (valueMillions < 0)
            throw new ValidationException(nameof(valueMillions), "Contract value cannot be negative.");

        // Build the contract record using an object initializer list.
        Contract contract = new()
        {
            DriverId = driver.Id,
            TeamId = team.Id,
            SignedDate = DateTime.Today,
            DurationSeasons = durationSeasons,
            ValueMillions = valueMillions,
            Notes = notes ?? string.Empty
        };

        // Mark the driver as signed and persist both updates atomically.
        driver.HasContract = true;
        Drivers.Update(driver);
        Contracts.Add(contract);

        // Notify every subscriber via our custom delegate types.
        Log?.Invoke($"{driver.DisplayName} was signed by {team.DisplayName}.");
        DriverSigned?.Invoke(this, new ContractEventArgs(contract, driver));

        return contract;
    }

    // Builds a textual statistics report based purely on LINQ queries.
    public string BuildStatisticsReport()
    {
        // Snapshot the data once to keep the report consistent.
        IReadOnlyList<Driver> drivers = Drivers.GetAll();
        IReadOnlyList<Team> teams = Teams.GetAll();
        IReadOnlyList<Contract> contracts = Contracts.GetAll();

        // LINQ - count available drivers.
        int availableCount = drivers.Count(d => d.IsAvailable());

        // LINQ - group by category to know how many of each we host.
        string[] byCategory = drivers
            .GroupBy(d => d.Category)
            .OrderByDescending(g => g.Count())
            .Select(g => $"  - {g.Key}: {g.Count()}")
            .ToArray();

        // LINQ - average age across all drivers.
        double averageAge = drivers.Any() ? drivers.Average(d => d.Age) : 0.0;

        // LINQ - country with the most teams.
        string topCountry = teams
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Country) ? "(unknown)" : t.Country)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "(none)";

        // LINQ - most recent contract signing date.
        DateTime? lastContract = contracts.Any()
            ? contracts.Max(c => c.SignedDate)
            : (DateTime?)null;

        // LINQ - total value of all signed contracts.
        double totalContractValue = contracts.Sum(c => c.ValueMillions);

        // Use the extension method to get a friendly summary line.
        string signedSummary = drivers.CountAndDescribe(
            d => d.HasContract, "driver signed", "drivers signed");

        // Compose the final multiline report.
        return string.Join(Environment.NewLine, new[]
        {
            $"Total drivers:      {drivers.Count}",
            $"Available now:      {availableCount}",
            $"Average age:        {averageAge:0.##} years",
            $"Total teams:        {teams.Count}",
            $"Top country:        {topCountry}",
            $"{signedSummary} so far.",
            $"Total contract value: {totalContractValue:0.##}M USD",
            lastContract.HasValue
                ? $"Last contract:      {lastContract.Value:yyyy-MM-dd}"
                : "Last contract:      (no contracts yet)",
            "",
            "Drivers per category:",
            byCategory.Length == 0 ? "  (no drivers)" : string.Join(Environment.NewLine, byCategory)
        });
    }

    // Generic helper used by the GUI's "search" textbox.
    public IEnumerable<Driver> SearchDrivers(string query)
    {
        // An empty query returns everything (avoids special-casing in the UI).
        if (string.IsNullOrWhiteSpace(query))
            return Drivers.GetAll();

        // The trimmed lowercase query is reused inside the LINQ filter.
        string normalized = query.Trim().ToLowerInvariant();

        return Drivers.GetAll().Where(d =>
            d.FirstName.ToLowerInvariant().Contains(normalized) ||
            d.LastName.ToLowerInvariant().Contains(normalized) ||
            d.Nationality.ToLowerInvariant().Contains(normalized) ||
            d.Category.ToLowerInvariant().Contains(normalized));
    }

    // Replaces every repository's content from a previously serialized snapshot.
    public void LoadSnapshot(
        IEnumerable<Driver> drivers,
        IEnumerable<Team> teams,
        IEnumerable<Contract> contracts)
    {
        // Reload uses the bulk operation defined on the base repository.
        Drivers.Reload(drivers);
        Teams.Reload(teams);
        Contracts.Reload(contracts);
        Log?.Invoke("Racing data successfully loaded from disk.");
    }

    // Local validation helper for drivers.
    private static void ValidateDriver(Driver driver)
    {
        // First name is required.
        if (string.IsNullOrWhiteSpace(driver.FirstName))
            throw new ValidationException(nameof(driver.FirstName), "First name cannot be empty.");

        // Last name is required.
        if (string.IsNullOrWhiteSpace(driver.LastName))
            throw new ValidationException(nameof(driver.LastName), "Last name cannot be empty.");

        // Negative ages don't make sense.
        if (driver.Age < 0)
            throw new ValidationException(nameof(driver.Age), "Age cannot be negative.");

        // Future registration dates are rejected.
        if (driver.RegistrationDate > DateTime.Today.AddDays(1))
            throw new ValidationException(nameof(driver.RegistrationDate), "Registration date cannot be in the future.");
    }

    // Local validation helper for teams.
    private static void ValidateTeam(Team team)
    {
        // Team name is required.
        if (string.IsNullOrWhiteSpace(team.TeamName))
            throw new ValidationException(nameof(team.TeamName), "Team name cannot be empty.");

        // Budget cannot be negative.
        if (team.BudgetMillions < 0)
            throw new ValidationException(nameof(team.BudgetMillions), "Budget cannot be negative.");
    }
}
