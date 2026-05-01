// Modal dialog used to sign a contract for a driver with a team.
using FormulaOneManager.Library.Domain.Drivers;
using FormulaOneManager.Library.Domain.Teams;

namespace FormulaOneManager.App;

// Lets the user pick a team and contract terms.
public partial class ContractForm : Form
{
    // Selected team (null until OK is clicked).
    public Team? SelectedTeam { get; private set; }

    // Number of contract seasons.
    public int DurationSeasons { get; private set; } = 1;

    // Total value of the contract in millions of USD.
    public double ValueMillions { get; private set; }

    // Optional textual notes from the team principal.
    public string Notes { get; private set; } = string.Empty;

    // Constructor populates the team combo with the supplied teams.
    public ContractForm(IEnumerable<Team> teams, Driver driver)
    {
        InitializeComponent();

        // Display the driver name as a static label inside the dialog.
        driverValueLabel.Text = $"{driver.Category}: {driver.FirstName} {driver.LastName}";

        // Keep the underlying Team objects so we can return one on OK.
        foreach (Team team in teams)
            teamCombo.Items.Add(team);

        // Show the readable team display name in the combo.
        teamCombo.Format += (sender, args) =>
        {
            if (args.ListItem is Team team)
                args.Value = team.DisplayName;
        };

        // Default selection to first team.
        if (teamCombo.Items.Count > 0)
            teamCombo.SelectedIndex = 0;

        // Validate selection before closing.
        okButton.Click += OnOkClicked;
    }

    // Captures the chosen values and validates the dialog.
    private void OnOkClicked(object? sender, EventArgs e)
    {
        if (teamCombo.SelectedItem is not Team selected)
        {
            MessageBox.Show(this, "Please pick a team.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
            return;
        }

        SelectedTeam = selected;
        DurationSeasons = (int)durationBox.Value;
        ValueMillions = (double)valueBox.Value;
        Notes = notesBox.Text?.Trim() ?? string.Empty;
    }
}
