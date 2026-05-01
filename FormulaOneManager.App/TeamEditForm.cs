// Modal dialog used to add or edit a Team.
using FormulaOneManager.Library.Domain.Teams;

namespace FormulaOneManager.App;

// Returns the populated Team via the Result property.
public partial class TeamEditForm : Form
{
    // The resulting team once the user confirms with OK.
    public Team? Result { get; private set; }

    // Reference to the team being edited (null when adding).
    private readonly Team? _editing;

    // Constructor used when adding a new team.
    public TeamEditForm() : this(null) { }

    // Constructor used when editing an existing team.
    public TeamEditForm(Team? team)
    {
        InitializeComponent();
        _editing = team;

        // OK handler builds the result and validates.
        okButton.Click += OnOkClicked;

        // Pre-fill the controls when editing.
        if (_editing != null)
        {
            nameBox.Text = _editing.TeamName;
            hqBox.Text = _editing.Headquarters;
            countryBox.Text = _editing.Country;
            budgetBox.Value = (decimal)Math.Clamp(_editing.BudgetMillions, (double)budgetBox.Minimum, (double)budgetBox.Maximum);
            foundedBox.Value = Math.Clamp(_editing.FoundedYear, (int)foundedBox.Minimum, (int)foundedBox.Maximum);
            Text = "Edit team";
        }
        else
        {
            // Default values for a new team.
            foundedBox.Value = DateTime.Today.Year;
            Text = "Add team";
        }
    }

    // Validates and produces the result Team instance.
    private void OnOkClicked(object? sender, EventArgs e)
    {
        // Light validation - service performs the rest.
        if (string.IsNullOrWhiteSpace(nameBox.Text))
        {
            MessageBox.Show(this, "Team name cannot be empty.",
                "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
            return;
        }

        Team result = _editing ?? new Team();
        result.TeamName = nameBox.Text.Trim();
        result.Headquarters = hqBox.Text.Trim();
        result.Country = countryBox.Text.Trim();
        result.BudgetMillions = (double)budgetBox.Value;
        result.FoundedYear = (int)foundedBox.Value;

        Result = result;
    }
}
