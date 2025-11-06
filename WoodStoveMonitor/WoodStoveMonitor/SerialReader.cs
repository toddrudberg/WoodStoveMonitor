using System;
using System.IO.Ports;
using System.Text.Json;

namespace WoodStoveLogger
{
  public sealed class SerialReader : IDisposable
  {
    private readonly SerialPort _port;
    public bool IsRunning { get; private set; }
    public event Action<string, JsonDocument?>? MessageReceived;
    public SerialReader(string portName, int baud)
    {
      _port = new SerialPort(portName, baud)
      {
        NewLine = "\n",
        ReadTimeout = 500,
        DtrEnable = true,
        RtsEnable = true
      };
      _port.DataReceived += OnData;
    }

    public void Start()
    {
      if (IsRunning) return;
      _port.Open();
      IsRunning = true;
    }

    public void Stop()
    {
      if (!IsRunning) return;
      IsRunning = false;
      _port.DataReceived -= OnData;
      if (_port.IsOpen) _port.Close();
    }

    private void OnData(object? sender, SerialDataReceivedEventArgs e)
    {
      try
      {
        while (_port.BytesToRead > 0)
        {
          string line = _port.ReadLine().Trim();
          if (string.IsNullOrWhiteSpace(line))
            continue;

          try
          {
            using var doc = JsonDocument.Parse(line);
            MessageReceived?.Invoke(line, doc);
          }
          catch
          {
            // Non-JSON but still meaningful
            MessageReceived?.Invoke(line, null);
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ERROR] {ex.Message}");
      }
    }

    public void Dispose() => Stop();
  }
}
