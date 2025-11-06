using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WoodStoveMonitor
{
  public enum LedState { Off, On, Warning, Blinking }

  public sealed class LedIndicator : Control
  {
    private LedState _state = LedState.Off;
    private Color _onColor = Color.LimeGreen;
    private Color _offColor = Color.Firebrick;
    private Color _warnColor = Color.Gold;
    private Color _borderColor = Color.FromArgb(60, 60, 60);
    private bool _showText = false;

    // Blink support
    private readonly Timer _blinkTimer;
    private bool _blinkVisible = true;
    private int _blinkInterval = 400;

    public LedIndicator()
    {
      SetStyle(ControlStyles.AllPaintingInWmPaint |
               ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.UserPaint |
               ControlStyles.ResizeRedraw, true);
      DoubleBuffered = true;
      Size = new Size(16, 16);
      TabStop = false;

      _blinkTimer = new Timer { Interval = _blinkInterval };
      _blinkTimer.Tick += (_, __) => { _blinkVisible = !_blinkVisible; Invalidate(); };
      UpdateBlinkTimer();
    }

    [Category("Behavior")]
    public LedState State
    {
      get => _state;
      set { _state = value; UpdateBlinkTimer(); Invalidate(); }
    }

    [Category("Appearance")]
    public Color OnColor
    {
      get => _onColor;
      set { _onColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    public Color OffColor
    {
      get => _offColor;
      set { _offColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    public Color WarningColor
    {
      get => _warnColor;
      set { _warnColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    public Color BorderColor
    {
      get => _borderColor;
      set { _borderColor = value; Invalidate(); }
    }

    [Category("Behavior")]
    public int BlinkInterval
    {
      get => _blinkInterval;
      set
      {
        _blinkInterval = Math.Max(50, value);
        _blinkTimer.Interval = _blinkInterval;
        Invalidate();
      }
    }

    [Category("Appearance")]
    public bool ShowText
    {
      get => _showText;
      set { _showText = value; Invalidate(); }
    }

    // Thread-safe helpers
    public void SetStateThreadSafe(LedState state)
    {
      if (InvokeRequired) { BeginInvoke(new Action(() => State = state)); }
      else { State = state; }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      var g = e.Graphics;
      g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

      // Layout: draw a circle at left; optional text to right
      int diameter = Height - 2;               // keep circle inside bounds
      int x = 1;
      int y = 1;

      // Choose color
      Color fill = _offColor;
      switch (_state)
      {
        case LedState.On: fill = _onColor; break;
        case LedState.Warning: fill = _warnColor; break;
        case LedState.Blinking: fill = _blinkVisible ? _onColor : _offColor; break;
        case LedState.Off: fill = _offColor; break;
      }

      // Draw glow ring (subtle)
      using (var shadow = new SolidBrush(Color.FromArgb(40, fill)))
        g.FillEllipse(shadow, x - 1, y - 1, diameter + 2, diameter + 2);

      // Main fill
      using (var br = new SolidBrush(fill))
        g.FillEllipse(br, x, y, diameter, diameter);

      // Simple highlight
      using (var hi = new SolidBrush(Color.FromArgb(100, Color.White)))
        g.FillEllipse(hi, x + diameter / 4, y + diameter / 6, diameter / 2, diameter / 2);

      // Border
      using (var pen = new Pen(_borderColor))
        g.DrawEllipse(pen, x, y, diameter, diameter);

      // Optional text (“Connected/Disconnected/Warning…”)
      if (_showText)
      {
        string label = _state switch
        {
          LedState.On => "Connected",
          LedState.Off => "Disconnected",
          LedState.Warning => "Warning",
          LedState.Blinking => "Connecting…",
          _ => ""
        };

        var rect = new Rectangle(x + diameter + 6, 0, Width - (diameter + 8), Height);
        TextRenderer.DrawText(g, label, Font, rect, ForeColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
      }
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      Invalidate();
    }

    private void UpdateBlinkTimer()
    {
      if (_state == LedState.Blinking) { if (!_blinkTimer.Enabled) _blinkTimer.Start(); }
      else { if (_blinkTimer.Enabled) _blinkTimer.Stop(); _blinkVisible = true; }
    }
  }
}
