// Modal dialog used to add or edit a Driver.
using FormulaOneManager.Library.Domain.Drivers;

namespace FormulaOneManager.App;

// The dialog returns the populated Driver instance through the Result property.
public partial class DriverEditForm : Form
{
    // Holds the resulting driver once the user confirms.
    public Driver? Result { get; private set; }

    // Edited driver (null when adding a new one).
    private readonly Driver? _editing;

    // Inputs created on the fly for category-specific fields.
    private NumericUpDown? _titlesBox;
    private NumericUpDown? _polesBox;
    private CheckBox? _contenderCheck;
    private CheckBox? _academyCheck;
    private NumericUpDown? _standingBox;
    private TextBox? _kartingClassBox;
    private NumericUpDown? _seasonWinsBox;
    private NumericUpDown? _testingKmBox;

    // Constructor used when adding a new driver.
    public DriverEditForm() : this(null) { }

    // Constructor used when editing an existing driver.
    public DriverEditForm(Driver? driver)
    {
        InitializeComponent();
        _editing = driver;

        // Populate the static combo boxes.
        foreach (DriverGender gender in Enum.GetValues<DriverGender>())
            genderCombo.Items.Add(gender);
        foreach (FitnessStatus status in Enum.GetValues<FitnessStatus>())
            fitnessCombo.Items.Add(status);

        // React to category changes to swap the category-specific subform.
        categoryCombo.SelectedIndexChanged += (_, _) => RebuildCategoryGroup(_editing);

        // Wire OK to validation.
        okButton.Click += OnOkClicked;

        // Pre-fill controls when editing.
        if (_editing != null)
        {
            categoryCombo.SelectedItem = _editing.Category;
            firstNameBox.Text = _editing.FirstName;
            lastNameBox.Text = _editing.LastName;
            nationalityBox.Text = _editing.Nationality;
            ageBox.Value = Math.Clamp(_editing.Age, (int)ageBox.Minimum, (int)ageBox.Maximum);
            genderCombo.SelectedItem = _editing.Gender;
            fitnessCombo.SelectedItem = _editing.Fitness;
            registrationPicker.Value = _editing.RegistrationDate.Date >= registrationPicker.MinDate
                ? _editing.RegistrationDate
                : DateTime.Today;
            signedCheck.Checked = _editing.HasContract;

            // Disable changing category when editing - the type is immutable.
            categoryCombo.Enabled = false;
            Text = $"Edit {_editing.Category} driver";
        }
        else
        {
            // Sensible defaults for a new entry.
            categoryCombo.SelectedIndex = 0;
            genderCombo.SelectedItem = DriverGender.Unknown;
            fitnessCombo.SelectedItem = FitnessStatus.Fit;
            registrationPicker.Value = DateTime.Today;
            ageBox.Value = 25;
            Text = "Add driver";
        }

        // Build the category-specific subform once with the current selection.
        RebuildCategoryGroup(_editing);
    }

    // Builds the category-specific input controls.
    private void RebuildCategoryGroup(Driver? source)
    {
        // Clear any previous controls.
        categoryGroup.Controls.Clear();
        _titlesBox = null; _polesBox = null; _contenderCheck = null;
        _academyCheck = null; _standingBox = null;
        _kartingClassBox = null; _seasonWinsBox = null;
        _testingKmBox = null;

        string category = (categoryCombo.SelectedItem as string) ?? "F1";

        // Common helper to create label-input pairs.
        TableLayoutPanel grid = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            AutoSize = true
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        switch (category)
        {
            case "F1":
                _titlesBox = new NumericUpDown { Width = 240, Minimum = 0, Maximum = 20, Value = (source as F1Driver)?.ChampionshipTitles ?? 0 };
                _polesBox = new NumericUpDown { Width = 240, Minimum = 0, Maximum = 200, Value = (source as F1Driver)?.PolePositions ?? 0 };
                _contenderCheck = new CheckBox
                {
                    Text = "Championship contender",
                    AutoSize = true,
                    Checked = (source as F1Driver)?.IsChampionshipContender ?? true
                };
                grid.Controls.Add(new Label { Text = "Titles:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
                grid.Controls.Add(_titlesBox, 1, 0);
                grid.Controls.Add(new Label { Text = "Pole positions:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
                grid.Controls.Add(_polesBox, 1, 1);
                grid.Controls.Add(new Label(), 0, 2);
                grid.Controls.Add(_contenderCheck, 1, 2);
                break;

            case "F2":
                _academyCheck = new CheckBox
                {
                    Text = "Junior academy member",
                    AutoSize = true,
                    Checked = (source as F2Driver)?.IsJuniorAcademyMember ?? true
                };
                _standingBox = new NumericUpDown { Width = 240, Minimum = 1, Maximum = 30, Value = (source as F2Driver)?.CurrentStanding ?? 10 };
                grid.Controls.Add(new Label(), 0, 0);
                grid.Controls.Add(_academyCheck, 1, 0);
                grid.Controls.Add(new Label { Text = "Standing:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
                grid.Controls.Add(_standingBox, 1, 1);
                break;

            case "Karting":
                _kartingClassBox = new TextBox { Width = 240, Text = (source as KartingDriver)?.KartingClass ?? "OK-Junior" };
                _seasonWinsBox = new NumericUpDown { Width = 240, Minimum = 0, Maximum = 100, Value = (source as KartingDriver)?.SeasonWins ?? 0 };
                grid.Controls.Add(new Label { Text = "Karting class:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
                grid.Controls.Add(_kartingClassBox, 1, 0);
                grid.Controls.Add(new Label { Text = "Season wins:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
                grid.Controls.Add(_seasonWinsBox, 1, 1);
                break;

            case "Rookie":
                _testingKmBox = new NumericUpDown
                {
                    Width = 240,
                    Minimum = 0,
                    Maximum = 1000000,
                    DecimalPlaces = 0,
                    Increment = 100,
                    Value = (decimal)((source as RookieDriver)?.TestingKilometers ?? 0)
                };
                grid.Controls.Add(new Label { Text = "Testing km:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
                grid.Controls.Add(_testingKmBox, 1, 0);
                break;
        }

        categoryGroup.Controls.Add(grid);
    }

    // Validates the form and produces the resulting Driver instance.
    private void OnOkClicked(object? sender, EventArgs e)
    {
        // Basic validation - the service performs additional checks too.
        if (string.IsNullOrWhiteSpace(firstNameBox.Text) ||
            string.IsNullOrWhiteSpace(lastNameBox.Text))
        {
            MessageBox.Show(this, "First name and last name cannot be empty.",
                "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
            return;
        }

        // Build the right concrete subclass based on the selected category.
        Driver driver = (categoryCombo.SelectedItem as string) switch
        {
            "F2" => new F2Driver
            {
                IsJuniorAcademyMember = _academyCheck?.Checked ?? true,
                CurrentStanding = (int)(_standingBox?.Value ?? 10)
            },
            "Karting" => new KartingDriver
            {
                KartingClass = _kartingClassBox?.Text ?? "OK-Junior",
                SeasonWins = (int)(_seasonWinsBox?.Value ?? 0)
            },
            "Rookie" => new RookieDriver
            {
                TestingKilometers = (double)(_testingKmBox?.Value ?? 0m)
            },
            _ => new F1Driver
            {
                ChampionshipTitles = (int)(_titlesBox?.Value ?? 0),
                PolePositions = (int)(_polesBox?.Value ?? 0),
                IsChampionshipContender = _contenderCheck?.Checked ?? true
            }
        };

        // Preserve the id when editing so the repository can locate the entry.
        if (_editing != null)
            driver.Id = _editing.Id;

        // Common fields fill last so editor users see them as the source of truth.
        driver.FirstName = firstNameBox.Text.Trim();
        driver.LastName = lastNameBox.Text.Trim();
        driver.Nationality = nationalityBox.Text.Trim();
        driver.Age = (int)ageBox.Value;
        driver.Gender = (DriverGender)(genderCombo.SelectedItem ?? DriverGender.Unknown);
        driver.Fitness = (FitnessStatus)(fitnessCombo.SelectedItem ?? FitnessStatus.Unknown);
        driver.RegistrationDate = registrationPicker.Value.Date;
        driver.HasContract = signedCheck.Checked;

        Result = driver;
    }
}
