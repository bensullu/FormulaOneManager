// Auto-style partial class with the layout of the main window.
namespace FormulaOneManager.App;

// Designer half of the MainForm class. The logic lives in MainForm.cs.
partial class MainForm
{
    // Container holding components that need disposing.
    private System.ComponentModel.IContainer components = null!;

    // Top main menu strip.
    private MenuStrip menuStrip = null!;
    private ToolStripMenuItem fileMenu = null!;
    private ToolStripMenuItem newMenuItem = null!;
    private ToolStripMenuItem openMenuItem = null!;
    private ToolStripMenuItem saveMenuItem = null!;
    private ToolStripMenuItem exitMenuItem = null!;
    private ToolStripMenuItem helpMenu = null!;
    private ToolStripMenuItem aboutMenuItem = null!;

    // Main tab control hosting the four sections.
    private TabControl tabControl = null!;
    private TabPage driversTab = null!;
    private TabPage teamsTab = null!;
    private TabPage contractsTab = null!;
    private TabPage statisticsTab = null!;

    // Drivers tab controls.
    private DataGridView driversGrid = null!;
    private Button addDriverButton = null!;
    private Button editDriverButton = null!;
    private Button removeDriverButton = null!;
    private Button signDriverButton = null!;
    private TextBox searchBox = null!;
    private Label searchLabel = null!;
    private ComboBox categoryFilter = null!;
    private Label categoryFilterLabel = null!;
    private CheckBox onlyAvailableCheck = null!;

    // Teams tab controls.
    private DataGridView teamsGrid = null!;
    private Button addTeamButton = null!;
    private Button editTeamButton = null!;
    private Button removeTeamButton = null!;

    // Contracts tab controls.
    private DataGridView contractsGrid = null!;

    // Statistics tab controls.
    private TextBox statisticsBox = null!;
    private Button refreshStatsButton = null!;

    // Status strip at the bottom of the window.
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel statusLabel = null!;

    // Cleans up resources used by the form.
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    // Initializes every UI control. Hand-written for clarity.
    private void InitializeComponent()
    {
        // Component container for tooltips etc.
        components = new System.ComponentModel.Container();

        // Build the top menu strip.
        menuStrip = new MenuStrip();
        fileMenu = new ToolStripMenuItem("File");
        newMenuItem = new ToolStripMenuItem("New (clear data)") { ShortcutKeys = Keys.Control | Keys.N };
        openMenuItem = new ToolStripMenuItem("Open...") { ShortcutKeys = Keys.Control | Keys.O };
        saveMenuItem = new ToolStripMenuItem("Save as...") { ShortcutKeys = Keys.Control | Keys.S };
        exitMenuItem = new ToolStripMenuItem("Exit") { ShortcutKeys = Keys.Alt | Keys.F4 };
        fileMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            newMenuItem,
            openMenuItem,
            saveMenuItem,
            new ToolStripSeparator(),
            exitMenuItem
        });
        helpMenu = new ToolStripMenuItem("Help");
        aboutMenuItem = new ToolStripMenuItem("About...");
        helpMenu.DropDownItems.Add(aboutMenuItem);
        menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, helpMenu });
        menuStrip.Dock = DockStyle.Top;

        // Build the tab control.
        tabControl = new TabControl { Dock = DockStyle.Fill };
        driversTab = new TabPage("Drivers");
        teamsTab = new TabPage("Teams");
        contractsTab = new TabPage("Contracts");
        statisticsTab = new TabPage("Statistics");
        tabControl.TabPages.AddRange(new[] { driversTab, teamsTab, contractsTab, statisticsTab });

        // Build the Drivers tab.
        BuildDriversTab();

        // Build the Teams tab.
        BuildTeamsTab();

        // Build the Contracts tab.
        BuildContractsTab();

        // Build the Statistics tab.
        BuildStatisticsTab();

        // Build the bottom status strip.
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel("Ready.");
        statusStrip.Items.Add(statusLabel);

        // Compose the form layout.
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 660);
        MinimumSize = new Size(900, 520);
        Text = "Formula 1 Manager";
        Controls.Add(tabControl);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
    }

    // Lays out the controls inside the Drivers tab.
    private void BuildDriversTab()
    {
        // Top toolbar panel that hosts the search and filter controls.
        FlowLayoutPanel toolbar = new()
        {
            Dock = DockStyle.Top,
            Height = 40,
            Padding = new Padding(8, 6, 8, 6),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        searchLabel = new Label { Text = "Search:", AutoSize = true, Padding = new Padding(0, 6, 4, 0) };
        searchBox = new TextBox { Width = 180 };
        categoryFilterLabel = new Label { Text = "Category:", AutoSize = true, Padding = new Padding(12, 6, 4, 0) };
        categoryFilter = new ComboBox { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
        onlyAvailableCheck = new CheckBox { Text = "Only available", AutoSize = true, Padding = new Padding(12, 4, 0, 0) };
        toolbar.Controls.AddRange(new Control[] { searchLabel, searchBox, categoryFilterLabel, categoryFilter, onlyAvailableCheck });

        // Bottom button bar.
        FlowLayoutPanel buttonBar = new()
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            Padding = new Padding(8, 6, 8, 6),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        addDriverButton = new Button { Text = "Add driver", Width = 110 };
        editDriverButton = new Button { Text = "Edit", Width = 80 };
        removeDriverButton = new Button { Text = "Remove", Width = 80 };
        signDriverButton = new Button { Text = "Sign contract", Width = 130 };
        buttonBar.Controls.AddRange(new Control[] { addDriverButton, editDriverButton, removeDriverButton, signDriverButton });

        // Central data grid view filling the rest of the tab.
        driversGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        // Custom date column without time component.
        DataGridViewTextBoxColumn registeredColumn = new DataGridViewTextBoxColumn
        {
            HeaderText = "Registered",
            DataPropertyName = "RegistrationDate",
            FillWeight = 70,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" }
        };

        // Hook the columns up to Driver properties.
        driversGrid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "Category",    DataPropertyName = "Category",    FillWeight = 50 },
            new DataGridViewTextBoxColumn { HeaderText = "First name",  DataPropertyName = "FirstName",   FillWeight = 70 },
            new DataGridViewTextBoxColumn { HeaderText = "Last name",   DataPropertyName = "LastName",    FillWeight = 70 },
            new DataGridViewTextBoxColumn { HeaderText = "Nationality", DataPropertyName = "Nationality", FillWeight = 80 },
            new DataGridViewTextBoxColumn { HeaderText = "Age",         DataPropertyName = "Age",         FillWeight = 30 },
            new DataGridViewTextBoxColumn { HeaderText = "Fitness",     DataPropertyName = "Fitness",     FillWeight = 60 },
            new DataGridViewCheckBoxColumn { HeaderText = "Signed",     DataPropertyName = "HasContract", FillWeight = 40 },
            registeredColumn);

        driversTab.Controls.Add(driversGrid);
        driversTab.Controls.Add(buttonBar);
        driversTab.Controls.Add(toolbar);
    }

    // Lays out the Teams tab controls.
    private void BuildTeamsTab()
    {
        FlowLayoutPanel buttonBar = new()
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            Padding = new Padding(8, 6, 8, 6),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        addTeamButton = new Button { Text = "Add team", Width = 110 };
        editTeamButton = new Button { Text = "Edit", Width = 80 };
        removeTeamButton = new Button { Text = "Remove", Width = 80 };
        buttonBar.Controls.AddRange(new Control[] { addTeamButton, editTeamButton, removeTeamButton });

        teamsGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        teamsGrid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "Team",         DataPropertyName = "TeamName",     FillWeight = 110 },
            new DataGridViewTextBoxColumn { HeaderText = "Headquarters", DataPropertyName = "Headquarters", FillWeight = 80 },
            new DataGridViewTextBoxColumn { HeaderText = "Country",      DataPropertyName = "Country",      FillWeight = 80 },
            new DataGridViewTextBoxColumn { HeaderText = "Budget (M$)",  DataPropertyName = "BudgetMillions", FillWeight = 60 },
            new DataGridViewTextBoxColumn { HeaderText = "Founded",      DataPropertyName = "FoundedYear",  FillWeight = 50 });

        teamsTab.Controls.Add(teamsGrid);
        teamsTab.Controls.Add(buttonBar);
    }

    // Lays out the Contracts tab controls.
    private void BuildContractsTab()
    {
        contractsGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        // The columns use friendly names produced by the ContractRow DTO below.
        contractsGrid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "Signed",       DataPropertyName = "Date",       FillWeight = 60 },
            new DataGridViewTextBoxColumn { HeaderText = "Driver",       DataPropertyName = "DriverName", FillWeight = 90 },
            new DataGridViewTextBoxColumn { HeaderText = "Team",         DataPropertyName = "TeamName",   FillWeight = 110 },
            new DataGridViewTextBoxColumn { HeaderText = "Seasons",      DataPropertyName = "Duration",   FillWeight = 40 },
            new DataGridViewTextBoxColumn { HeaderText = "Value (M$)",   DataPropertyName = "Value",      FillWeight = 50 },
            new DataGridViewTextBoxColumn { HeaderText = "Notes",        DataPropertyName = "Notes",      FillWeight = 110 });

        contractsTab.Controls.Add(contractsGrid);
    }

    // Lays out the Statistics tab controls.
    private void BuildStatisticsTab()
    {
        statisticsBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font(FontFamily.GenericMonospace, 10f)
        };

        refreshStatsButton = new Button
        {
            Text = "Refresh statistics",
            Dock = DockStyle.Bottom,
            Height = 36
        };

        statisticsTab.Controls.Add(statisticsBox);
        statisticsTab.Controls.Add(refreshStatsButton);
    }
}
