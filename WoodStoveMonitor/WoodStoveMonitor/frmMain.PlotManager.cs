using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoodStoveMonitor
{
  public partial class frmMain
  {

    private readonly List<double> xs = new();
    private readonly List<double> ys = new();
    private const int MaxPoints = 10_000;                 // cap memory
    private readonly TimeSpan Window = TimeSpan.FromMinutes(10); // rolling window
    private Scatter? _series;

    enum TimeUnit { Seconds, Minutes, Hours }
    private readonly List<double> xSecs = new();   // raw elapsed seconds (never changed)
    private DateTime _sessionStart;
    private TimeUnit _xUnit = TimeUnit.Seconds;

    private void InitPlot()
    {
      _sessionStart = DateTime.Now;
      spPMS2_5.Plot.Title("PM2.5 (AE) over time");
      spPMS2_5.Plot.YLabel("µg/m³");
      spPMS2_5.Plot.Axes.Bottom.Label.Text = "Time (s)"; // starts in seconds
      
      _xUnit = TimeUnit.Seconds;
      xSecs.Clear();
      xs.Clear();
      ys.Clear();
      spPMS2_5.Plot.Clear();

      _series = spPMS2_5.Plot.Add.Scatter(xs, ys);

      // Handy reference lines (optional)
      var h35 = spPMS2_5.Plot.Add.HorizontalLine(35); h35.LinePattern = LinePattern.Dotted; h35.Text = "35 µg/m³";
      var h150 = spPMS2_5.Plot.Add.HorizontalLine(150); h150.LinePattern = LinePattern.Dotted; h150.Text = "150 µg/m³";

      spPMS2_5.Refresh();

    }

    private void PlotData(double pm25)
    {

      if (!_logData.IsActive || _series is null)
        return;

      // elapsed seconds since session start (ground truth)
      double sec = (DateTime.Now - _sessionStart).TotalSeconds;

      // decide unit for display
      var targetUnit = DecideUnit(sec);

      // if unit changed, reproject ALL plotted X values from xSecs -> xs
      if (targetUnit != _xUnit)
      {
        _xUnit = targetUnit;
        xs.Clear();
        xs.Capacity = Math.Max(xs.Capacity, xSecs.Count);
        for (int i = 0; i < xSecs.Count; i++)
          xs.Add(ConvertFromSeconds(xSecs[i], _xUnit));

        ApplyAxisLabel(_xUnit);
      }

      // append point
      xSecs.Add(sec);
      xs.Add(ConvertFromSeconds(sec, _xUnit));
      ys.Add(pm25);

      // ---- TIME-BASED ROLLING WINDOW (fix: use seconds, not OADate) ----
      // keep a 10-minute view by default (you can make WindowSec configurable)
      const double WindowSec = 10 * 60.0;
      double cutoffSec = sec - WindowSec;

      // trim from the front while oldest is outside the window
      while (xSecs.Count > 0 && xSecs[0] < cutoffSec)
      {
        xSecs.RemoveAt(0);
        xs.RemoveAt(0);
        ys.RemoveAt(0);
      }

      // Cap array size as a safeguard
      if (xs.Count > MaxPoints)
      {
        int drop = xs.Count - MaxPoints;
        xSecs.RemoveRange(0, drop);
        xs.RemoveRange(0, drop);
        ys.RemoveRange(0, drop);
      }

      // ---- AXIS BEHAVIOR ----
      // Option A: Let AutoScale do both axes (simple)
      // spPMS2_5.Plot.Axes.AutoScale();

      // Option B (default here): Fixed Y, autoscale X via window
      // Set X limits to current window explicitly, and set Y once per point softly.
      double xLeft = ConvertFromSeconds(Math.Max(0, cutoffSec), _xUnit);
      double xRight = ConvertFromSeconds(sec, _xUnit);
      spPMS2_5.Plot.Axes.SetLimitsX(xLeft, xRight);

      // gentle Y limit (prevents wild rescaling after huge spikes)
      double currentYMax = Math.Max(50, pm25 * 1.2);
      double yLo = 0;
      double yHi = Math.Max(currentYMax, ys.Count > 0 ? Math.Max(currentYMax, 1.2 * MaxOf(ys)) : currentYMax);
      spPMS2_5.Plot.Axes.SetLimitsY(yLo, yHi);

      spPMS2_5.Refresh();
    }

    // quick helper (no Linq allocations in hot path)
    private static double MaxOf(List<double> arr)
    {
      double m = double.NegativeInfinity;
      for (int i = 0; i < arr.Count; i++)
        if (arr[i] > m) m = arr[i];
      return double.IsNegativeInfinity(m) ? 0 : m;
    }


    private TimeUnit DecideUnit(double elapsedSec)
    {
      if (elapsedSec < 120) return TimeUnit.Seconds;  // < 2 minutes
      if (elapsedSec < 2 * 3600) return TimeUnit.Minutes;  // < 2 hours
      return TimeUnit.Hours;
    }

    private double ConvertFromSeconds(double sec, TimeUnit unit) =>
        unit == TimeUnit.Seconds ? sec :
        unit == TimeUnit.Minutes ? sec / 60.0 :
                                   sec / 3600.0;

    private void ApplyAxisLabel(TimeUnit unit)
    {
      spPMS2_5.Plot.Axes.Bottom.Label.Text =
          unit == TimeUnit.Seconds ? "Time (s)" :
          unit == TimeUnit.Minutes ? "Time (min)" :
                                     "Time (h)";
    }
  }
}
