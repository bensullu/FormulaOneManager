// Designer half of the ContractForm partial class.
namespace FormulaOneManager.App;

partial class ContractForm
{
    private System.ComponentModel.IContainer components = null!;

    private Label driverLabel = null!;
    private Label driverValueLabel = null!;
    private Label teamLabel = null!;
    private ComboBox teamCombo = null!;
    private Label durationLabel = null!;
    private NumericUpDown durationBox = null!;
    private Label valueLabel = null!;
    private NumericUpDown valueBox = null!;
    private Label notesLabel = null!;
    private TextBox notesBox = null!;
    private Button okButton = null!;
    private Button cancelButton = null!;

    // Releases held resources.
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    // Builds the dialog layout.
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(12),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        driverLabel = new Label { Text = "Driver:", AutoSize = true, Anchor = AnchorStyles.Left };
        driverValueLabel = new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Font = new Font(Font, FontStyle.Bold)
        };

        teamLabel = new Label { Text = "Team:", AutoSize = true, Anchor = AnchorStyles.Left };
        teamCombo = new ComboBox { Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };

        durationLabel = new Label { Text = "Seasons:", AutoSize = true, Anchor = AnchorStyles.Left };
        durationBox = new NumericUpDown
        {
            Width = 280,
            Minimum = 1,
            Maximum = 10,
            Increment = 1,
            Value = 1
        };

        valueLabel = new Label { Text = "Value (M$):", AutoSize = true, Anchor = AnchorStyles.Left };
        valueBox = new NumericUpDown
        {
            Width = 280,
            Minimum = 0,
            Maximum = 1000,
            DecimalPlaces = 1,
            Increment = 0.5m,
            Value = 5m
        };

        notesLabel = new Label { Text = "Notes:", AutoSize = true, Anchor = AnchorStyles.Left };
        notesBox = new TextBox { Width = 280, Multiline = true, Height = 80 };

        layout.Controls.Add(driverLabel, 0, 0);    layout.Controls.Add(driverValueLabel, 1, 0);
        layout.Controls.Add(teamLabel, 0, 1);      layout.Controls.Add(teamCombo, 1, 1);
        layout.Controls.Add(durationLabel, 0, 2);  layout.Controls.Add(durationBox, 1, 2);
        layout.Controls.Add(valueLabel, 0, 3);     layout.Controls.Add(valueBox, 1, 3);
        layout.Controls.Add(notesLabel, 0, 4);     layout.Controls.Add(notesBox, 1, 4);

        okButton = new Button { Text = "Sign", DialogResult = DialogResult.OK, Width = 90 };
        cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };

        FlowLayoutPanel buttonPanel = new()
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(12),
            Height = 56
        };
        buttonPanel.Controls.AddRange(new Control[] { cancelButton, okButton });

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(480, 360);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        Text = "Sign contract";
        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.Add(buttonPanel);
        Controls.Add(layout);
    }
}
