using System;
using System.IO;
using System.Text;

namespace WoodStoveMonitor
{
  public sealed class LogData
  {
    private static readonly Lazy<LogData> _lazy = new(() => new LogData());
    public static LogData Instance => _lazy.Value;

    private StreamWriter? _writer;
    private DateTime _sessionStart;
    private bool _isActive = false;

    public LogData() { }

    /// <summary>
    /// Starts a new CSV log in ./Logs/
    /// </summary>
    public void Start()
    {
      if (_isActive)
        return;

      string folder = Path.Combine(@"C:\WoodStoveLogs");
      Directory.CreateDirectory(folder);

      string filename = Path.Combine(folder, $"stoveLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
      _writer = new StreamWriter(filename, false, Encoding.UTF8) { AutoFlush = true };
      _sessionStart = DateTime.Now;
      _isActive = true;

      _writer.WriteLine("pc_time_iso,rel_s,arduino_ts_ms,pm25_ae,event");
      Console.WriteLine($"[LOG] Started logging to {filename}");
    }

    public bool IsActive
    {
      get { return _isActive; }
    }

    public DateTime SessionStart
      { get { return _sessionStart; } }

    /// <summary>
    /// Stops and closes the log
    /// </summary>
    public void Stop()
    {
      if (!_isActive)
        return;

      _writer?.Dispose();
      _writer = null;
      _isActive = false;

      Console.WriteLine("[LOG] Logging stopped.");
    }

    /// <summary>
    /// Writes a PM2.5 sample line
    /// </summary>
    public void LogPm(double pm25, long tsMs)
    {
      if (!_isActive || _writer == null)
        return;

      DateTime now = DateTime.Now;
      double relSec = (now - _sessionStart).TotalSeconds;
      _writer.WriteLine($"{now:O},{relSec:0.000},{tsMs},{pm25:0.##},");
    }

    /// <summary>
    /// Writes a labeled event (e.g., "AB ON", "AB OFF", "Door Open")
    /// </summary>
    public void LogEvent(string label)
    {
      if (!_isActive || _writer == null)
        return;

      DateTime now = DateTime.Now;
      double relSec = (now - _sessionStart).TotalSeconds;
      _writer.WriteLine($"{now:O},{relSec:0.000},,,{label}");
    }
  }
}
