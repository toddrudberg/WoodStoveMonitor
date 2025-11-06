using Pastel;
using System.Runtime.InteropServices;
using WoodStoveLogger;
using System.Threading;

namespace WoodStoveMonitor
{
  public partial class frmMain : Form
  {
    private SerialReader? _reader;
    private bool _isConnected = false;

    // Win32
    [DllImport("kernel32.dll")] static extern bool AllocConsole();
    [DllImport("kernel32.dll")] static extern bool FreeConsole();
    [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")] static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nW, int nH, bool repaint);
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    const int SW_SHOW = 5;
    public frmMain()
    {
      InitializeComponent();

      // Put form in upper-left half of primary screen
      var wa = Screen.PrimaryScreen.WorkingArea;
      StartPosition = FormStartPosition.Manual;
      Left = wa.Left;
      Top = wa.Top;
      Width = wa.Width / 2;
      Height = wa.Height;

      EnsureConsole();
      PositionConsoleRightHalf();
    }

    private void EnsureConsole()
    {
      if (GetConsoleWindow() == IntPtr.Zero)
      {
        AllocConsole();
        Console.WriteLine("Console logging started...".Pastel(ConsoleColor.Green));
      }
      else
      {
        // If already attached, just make sure it's visible
        ShowWindow(GetConsoleWindow(), SW_SHOW);
      }
    }

    private void PositionConsoleRightHalf()
    {
      var cw = GetConsoleWindow();
      if (cw == IntPtr.Zero) return;

      var wa = Screen.PrimaryScreen.WorkingArea;
      int x = wa.Left + wa.Width / 2;
      int y = wa.Top;
      int w = wa.Width / 2;
      int h = wa.Height;

      MoveWindow(cw, x, y, w, h, true);
    }

    private void frmWoodStoveMonitor_Load(object sender, EventArgs e)
    {
      comboPorts.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
      if (comboPorts.Items.Count > 0)
        comboPorts.SelectedIndex = 0;

      UpdateStatus(false);

    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      _reader?.Stop();
    }

    private void btnConnect_Click(object sender, EventArgs e)
    {
      OnBtnConnect_Click(sender, e);

    }

    private void frmWoodStoveMonitor_FormClosing(object sender, FormClosingEventArgs e)
    {
      Disconnect();
    }
  }
}
