// Designer half of the TeamEditForm partial class.
namespace FormulaOneManager.App;

partial class TeamEditForm
{
    private System.ComponentModel.IContainer components = null!;

    private Label nameLabel = null!;
    private TextBox nameBox = null!;
    private Label hqLabel = null!;
    private TextBox hqBox = null!;
    private Label countryLabel = null!;
    private TextBox countryBox = null!;
    private Label budgetLabel = null!;
    private NumericUpDown budgetBox = null!;
    private Label foundedLabel = null!;
    private NumericUpDown foundedBox = null!;
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

        nameLabel = new Label { Text = "Team name:", AutoSize = true, Anchor = AnchorStyles.Left };
        nameBox = new TextBox { Width = 240 };

        hqLabel = new Label { Text = "Headquarters:", AutoSize = true, Anchor = AnchorStyles.Left };
        hqBox = new TextBox { Width = 240 };

        countryLabel = new Label { Text = "Country:", AutoSize = true, Anchor = AnchorStyles.Left };
        countryBox = new TextBox { Width = 240 };

        budgetLabel = new Label { Text = "Budget (M$):", AutoSize = true, Anchor = AnchorStyles.Left };
        budgetBox = new NumericUpDown
        {
            Width = 240,
            Minimum = 0,
            Maximum = 5000,
            DecimalPlaces = 1,
            Increment = 5
        };

        foundedLabel = new Label { Text = "Founded:", AutoSize = true, Anchor = AnchorStyles.Left };
        foundedBox = new NumericUpDown
        {
            Width = 240,
            Minimum = 1900,
            Maximum = 2100,
            DecimalPlaces = 0,
            Increment = 1,
            Value = 2000
        };

        layout.Controls.Add(nameLabel, 0, 0);     layout.Controls.Add(nameBox, 1, 0);
        layout.Controls.Add(hqLabel, 0, 1);       layout.Controls.Add(hqBox, 1, 1);
        layout.Controls.Add(countryLabel, 0, 2);  layout.Controls.Add(countryBox, 1, 2);
        layout.Controls.Add(budgetLabel, 0, 3);   layout.Controls.Add(budgetBox, 1, 3);
        layout.Controls.Add(foundedLabel, 0, 4);  layout.Controls.Add(foundedBox, 1, 4);

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

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(420, 280);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        Text = "Team";
        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.Add(buttonPanel);
        Controls.Add(layout);
    }
}
