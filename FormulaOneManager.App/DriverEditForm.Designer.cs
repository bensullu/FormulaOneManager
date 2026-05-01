// Designer half of the DriverEditForm partial class.
namespace FormulaOneManager.App;

partial class DriverEditForm
{
    // Container for components requiring disposal.
    private System.ComponentModel.IContainer components = null!;

    // Common driver fields.
    private Label categoryLabel = null!;
    private ComboBox categoryCombo = null!;
    private Label firstNameLabel = null!;
    private TextBox firstNameBox = null!;
    private Label lastNameLabel = null!;
    private TextBox lastNameBox = null!;
    private Label nationalityLabel = null!;
    private TextBox nationalityBox = null!;
    private Label ageLabel = null!;
    private NumericUpDown ageBox = null!;
    private Label genderLabel = null!;
    private ComboBox genderCombo = null!;
    private Label fitnessLabel = null!;
    private ComboBox fitnessCombo = null!;
    private Label registrationLabel = null!;
    private DateTimePicker registrationPicker = null!;
    private CheckBox signedCheck = null!;

    // Category specific group box (rebuilt when category changes).
    private GroupBox categoryGroup = null!;

    // Action buttons.
    private Button okButton = null!;
    private Button cancelButton = null!;

    // Releases resources held by the form.
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    // Lays out every control. Hand-written for clarity.
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        // Top section uses a TableLayoutPanel for clean alignment.
        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 9,
            Padding = new Padding(12),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        // Category controls.
        categoryLabel = new Label { Text = "Category:", AutoSize = true, Anchor = AnchorStyles.Left };
        categoryCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 240 };
        categoryCombo.Items.AddRange(new object[] { "F1", "F2", "Karting", "Rookie" });

        // First name controls.
        firstNameLabel = new Label { Text = "First name:", AutoSize = true, Anchor = AnchorStyles.Left };
        firstNameBox = new TextBox { Width = 240 };

        // Last name controls.
        lastNameLabel = new Label { Text = "Last name:", AutoSize = true, Anchor = AnchorStyles.Left };
        lastNameBox = new TextBox { Width = 240 };

        // Nationality.
        nationalityLabel = new Label { Text = "Nationality:", AutoSize = true, Anchor = AnchorStyles.Left };
        nationalityBox = new TextBox { Width = 240 };

        // Age controls.
        ageLabel = new Label { Text = "Age (years):", AutoSize = true, Anchor = AnchorStyles.Left };
        ageBox = new NumericUpDown
        {
            Width = 240,
            Minimum = 0,
            Maximum = 80,
            DecimalPlaces = 0,
            Increment = 1
        };

        // Gender controls.
        genderLabel = new Label { Text = "Gender:", AutoSize = true, Anchor = AnchorStyles.Left };
        genderCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 240 };

        // Fitness controls.
        fitnessLabel = new Label { Text = "Fitness:", AutoSize = true, Anchor = AnchorStyles.Left };
        fitnessCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 240 };

        // Registration controls.
        registrationLabel = new Label { Text = "Registered on:", AutoSize = true, Anchor = AnchorStyles.Left };
        registrationPicker = new DateTimePicker { Width = 240, Format = DateTimePickerFormat.Short };

        // Already signed flag (read-only marker; usually managed via Sign button).
        signedCheck = new CheckBox { Text = "Already signed", AutoSize = true };

        // Add every control to the table layout.
        layout.Controls.Add(categoryLabel, 0, 0);     layout.Controls.Add(categoryCombo, 1, 0);
        layout.Controls.Add(firstNameLabel, 0, 1);    layout.Controls.Add(firstNameBox, 1, 1);
        layout.Controls.Add(lastNameLabel, 0, 2);     layout.Controls.Add(lastNameBox, 1, 2);
        layout.Controls.Add(nationalityLabel, 0, 3);  layout.Controls.Add(nationalityBox, 1, 3);
        layout.Controls.Add(ageLabel, 0, 4);          layout.Controls.Add(ageBox, 1, 4);
        layout.Controls.Add(genderLabel, 0, 5);       layout.Controls.Add(genderCombo, 1, 5);
        layout.Controls.Add(fitnessLabel, 0, 6);      layout.Controls.Add(fitnessCombo, 1, 6);
        layout.Controls.Add(registrationLabel, 0, 7); layout.Controls.Add(registrationPicker, 1, 7);
        layout.Controls.Add(new Label(), 0, 8);       layout.Controls.Add(signedCheck, 1, 8);

        // Category specific section.
        categoryGroup = new GroupBox
        {
            Text = "Category specific",
            Dock = DockStyle.Top,
            Height = 130,
            Padding = new Padding(12)
        };

        // Action buttons.
        okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };

        FlowLayoutPanel buttonPanel = new()
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(12),
            Height = 56
        };
        buttonPanel.Controls.AddRange(new Control[] { cancelButton, okButton });

        // Compose the form.
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(450, 540);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        Text = "Driver";
        AcceptButton = okButton;
        CancelButton = cancelButton;
        // Add controls bottom-up so Dock layout processes them correctly.
        Controls.Add(buttonPanel);
        Controls.Add(categoryGroup);
        Controls.Add(layout);
    }
}
