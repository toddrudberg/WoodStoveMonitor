using Microsoft.VisualBasic.Logging;
using OpenTK.Graphics.OpenGL;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoodStoveLogger;

namespace WoodStoveMonitor
{
  public partial class frmMain
  {

    private void OnSerialMessage(string rawLine, System.Text.Json.JsonDocument? json)
    {
      // Ensure we update UI on UI thread
      if (InvokeRequired)
      {
        // Copy JSON text now (before the original JsonDocument gets disposed)
        string? jsonText = json?.RootElement.GetRawText();

        BeginInvoke(new Action(() =>
        {
          System.Text.Json.JsonDocument? doc = null;
          try
          {
            if (jsonText is not null)
              doc = System.Text.Json.JsonDocument.Parse(jsonText);

            // Re-enter handler on UI thread with a fresh, safe JsonDocument
            OnSerialMessage(rawLine, doc);
          }
          finally
          {
            doc?.Dispose(); // dispose our UI-thread doc after use
          }
        }));
        return;
      }

      if (json is null) return;
      var root = json.RootElement;
      string messageType = "none";
      if (root.TryGetProperty("type", out var typeEl))
      {
        messageType = typeEl.GetString();
      }


      switch (messageType)
      {
        case "none":
          break;
        case "heartbeat":
          ledHeartBeat.State = ledHeartBeat.State == LedState.On ? LedState.Off : LedState.On;
          break;
        case "pms":
          // Use host receive time as X (simple & robust)
          DateTime now = DateTime.Now;
          double x = now.ToOADate();

          // Extract AE PM2.5
          // {"pm":{"sp":{"pm1_0":...,"pm2_5":...,"pm10":...},"ae":{"pm1_0":...,"pm2_5":...,"pm10":...}}}
          if (root.TryGetProperty("pm", out var pmEl) &&
              pmEl.TryGetProperty("ae", out var aeEl) &&
              aeEl.TryGetProperty("pm2_5", out var pm25El))
          //aeEl.TryGetProperty("pm1_0", out var pm25El))

          {
            long tsMs = -1;
            if (root.TryGetProperty("ts_ms", out var tsEl))
              tsMs = tsEl.GetInt64();

            double pm25 = pm25El.GetDouble();

            PlotData(pm25);

            if (_logData.IsActive)
            {
              _logData.LogPm(pm25, tsMs);
            }
          }
          break;
      }
    }



    private void Connect()
    {
      if (comboPorts.SelectedItem == null)
      {
        MessageBox.Show("No COM port selected.");
        return;
      }

      string portName = comboPorts.SelectedItem.ToString();
      int baud = 115200;
      ledConnection.State = LedState.Blinking;
      try
      {
        _reader = new SerialReader(portName, baud);
        _reader.MessageReceived += OnSerialMessage;
        _reader.Start();

        // Optional: send START to sync timestamps
        System.Threading.Thread.Sleep(250);
        _reader.WriteLine("START");

        UpdateStatus(true);
      }
      catch (Exception ex)
      {
        ledConnection.State = LedState.Warning;
        MessageBox.Show("Failed to connect: " + ex.Message);
        ledConnection.State = LedState.Off;
        UpdateStatus(false);
      }
    }

    private void Disconnect()
    {
      try
      {
        _reader?.Stop();
        _reader?.Dispose();
      }
      catch { }

      _reader = null;
      ledConnection.State = LedState.Off;
      UpdateStatus(false);
    }

    private void UpdateStatus(bool connected)
    {
      _isConnected = connected;

      btnStartLogging.Enabled = connected;

      btnConnect.Enabled = !_logData.IsActive;

      btnConnect.Text = connected ? "Disconnect" : "Connect";
      lblStatus.Text = connected ? "Connected" : "Disconnected";
      ledConnection.State = connected ? LedState.On : LedState.Off;
    }
  }
}
