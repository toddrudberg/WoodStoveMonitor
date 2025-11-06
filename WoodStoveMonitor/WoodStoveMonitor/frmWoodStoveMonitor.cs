using Pastel;
using System.Runtime.InteropServices;
using WoodStoveLogger;
using System.Threading;

namespace WoodStoveMonitor
{
  public partial class frmWoodStoveMonitor : Form
  {
    private SerialReader? _reader;
    // Win32
    [DllImport("kernel32.dll")] static extern bool AllocConsole();
    [DllImport("kernel32.dll")] static extern bool FreeConsole();
    [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")] static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nW, int nH, bool repaint);
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    const int SW_SHOW = 5;
    public frmWoodStoveMonitor()
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
      StartSerialMonitor();
    }
    private void StartSerialMonitor()
    {
      string portName = "COM3"; // Later we auto-detect this
      int baud = 115200;

      _reader = new SerialReader(portName, baud);
      _reader.MessageReceived += OnSerialMessage;
      _reader.Start();
    }

    private void OnSerialMessage(string rawLine, System.Text.Json.JsonDocument? json)
    {
      // Ensure we update UI on UI thread
      if (InvokeRequired)
      {
        BeginInvoke(new Action(() => OnSerialMessage(rawLine, json)));
        return;
      }

      // For now, just append to a TextBox or Debug window
      Console.WriteLine(rawLine + Environment.NewLine);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      _reader?.Stop();
    }
  }
}
