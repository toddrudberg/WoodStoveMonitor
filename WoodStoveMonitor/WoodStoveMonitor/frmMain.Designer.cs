namespace WoodStoveMonitor
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      btnConnect = new Button();
      comboPorts = new ComboBox();
      lblStatus = new Label();
      ledConnection = new LedIndicator();
      ledHeartBeat = new LedIndicator();
      lblHeartBeat = new Label();
      spPMS2_5 = new ScottPlot.WinForms.FormsPlot();
      btnStartLogging = new Button();
      SuspendLayout();
      // 
      // btnConnect
      // 
      btnConnect.Location = new Point(12, 12);
      btnConnect.Name = "btnConnect";
      btnConnect.Size = new Size(75, 23);
      btnConnect.TabIndex = 0;
      btnConnect.Text = "Connect";
      btnConnect.UseVisualStyleBackColor = true;
      btnConnect.Click += btnConnect_Click;
      // 
      // comboPorts
      // 
      comboPorts.FormattingEnabled = true;
      comboPorts.Location = new Point(93, 12);
      comboPorts.Name = "comboPorts";
      comboPorts.Size = new Size(78, 23);
      comboPorts.TabIndex = 1;
      // 
      // lblStatus
      // 
      lblStatus.AutoSize = true;
      lblStatus.Location = new Point(200, 16);
      lblStatus.Name = "lblStatus";
      lblStatus.Size = new Size(79, 15);
      lblStatus.TabIndex = 3;
      lblStatus.Text = "Disconnected";
      // 
      // ledConnection
      // 
      ledConnection.BlinkInterval = 400;
      ledConnection.BorderColor = Color.FromArgb(60, 60, 60);
      ledConnection.Location = new Point(178, 15);
      ledConnection.Name = "ledConnection";
      ledConnection.OffColor = Color.Firebrick;
      ledConnection.OnColor = Color.LimeGreen;
      ledConnection.ShowText = false;
      ledConnection.Size = new Size(16, 16);
      ledConnection.State = LedState.Off;
      ledConnection.TabIndex = 4;
      ledConnection.TabStop = false;
      ledConnection.Text = "ledIndicator1";
      ledConnection.WarningColor = Color.Gold;
      // 
      // ledHeartBeat
      // 
      ledHeartBeat.BlinkInterval = 400;
      ledHeartBeat.BorderColor = Color.FromArgb(60, 60, 60);
      ledHeartBeat.Location = new Point(301, 16);
      ledHeartBeat.Name = "ledHeartBeat";
      ledHeartBeat.OffColor = Color.Firebrick;
      ledHeartBeat.OnColor = Color.LimeGreen;
      ledHeartBeat.ShowText = false;
      ledHeartBeat.Size = new Size(16, 16);
      ledHeartBeat.State = LedState.Off;
      ledHeartBeat.TabIndex = 5;
      ledHeartBeat.TabStop = false;
      ledHeartBeat.Text = "ledIndicator1";
      ledHeartBeat.WarningColor = Color.Gold;
      // 
      // lblHeartBeat
      // 
      lblHeartBeat.AutoSize = true;
      lblHeartBeat.Location = new Point(323, 17);
      lblHeartBeat.Name = "lblHeartBeat";
      lblHeartBeat.Size = new Size(69, 15);
      lblHeartBeat.TabIndex = 6;
      lblHeartBeat.Text = "Arduino HB";
      // 
      // spPMS2_5
      // 
      spPMS2_5.DisplayScale = 1F;
      spPMS2_5.Location = new Point(33, 135);
      spPMS2_5.Name = "spPMS2_5";
      spPMS2_5.Size = new Size(732, 314);
      spPMS2_5.TabIndex = 7;
      // 
      // btnStartLogging
      // 
      btnStartLogging.Enabled = false;
      btnStartLogging.Location = new Point(54, 455);
      btnStartLogging.Name = "btnStartLogging";
      btnStartLogging.Size = new Size(117, 23);
      btnStartLogging.TabIndex = 8;
      btnStartLogging.Text = "Start Logging";
      btnStartLogging.UseVisualStyleBackColor = true;
      btnStartLogging.Click += btnStartLogging_Click;
      // 
      // frmMain
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(838, 800);
      Controls.Add(btnStartLogging);
      Controls.Add(spPMS2_5);
      Controls.Add(lblHeartBeat);
      Controls.Add(ledHeartBeat);
      Controls.Add(ledConnection);
      Controls.Add(lblStatus);
      Controls.Add(comboPorts);
      Controls.Add(btnConnect);
      Name = "frmMain";
      Text = "Woodstove Lab";
      FormClosing += frmWoodStoveMonitor_FormClosing;
      Load += frmWoodStoveMonitor_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Button btnConnect;
    private ComboBox comboPorts;
    private Label lblStatus;
    private LedIndicator ledConnection;
    private LedIndicator ledHeartBeat;
    private Label lblHeartBeat;
    private ScottPlot.WinForms.FormsPlot spPMS2_5;
    private Button btnStartLogging;
  }
}
