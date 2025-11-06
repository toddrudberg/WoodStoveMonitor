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
        BeginInvoke(new Action(() => OnSerialMessage(rawLine, json)));
        return;
      }

      // For now, just append to a TextBox or Debug window
      Console.WriteLine(rawLine + Environment.NewLine);
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

      btnConnect.Text = connected ? "Disconnect" : "Connect";
      lblStatus.Text = connected ? "Connected" : "Disconnected";
      ledConnection.State = connected ? LedState.On : LedState.Off;
    }
  }
}
