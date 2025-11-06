# WoodStove Logger

Arduino firmware + C# app for USB/Serial logging of wood stove sensor data.

## Quick start

### Firmware
1. Open `firmware/arduino-logger/arduino-logger.ino` in Arduino IDE.
2. Tools → Board/Port → select your device.
3. Upload, open Serial Monitor @ 115200.

### C# App
1. Open `apps/csharp-logger/WoodStoveLogger.sln` in Visual Studio.
2. Update COM port in `SerialReader.cs` if needed.
3. Run.

## Project layout
- `firmware/` – Arduino sketches and embedded libs.
- `apps/` – Host applications (C#, etc.).
- `docs/` – Protocol, wiring, design notes.
- `scripts/` – Build/pack/release helpers.
- `.github/workflows/` – CI config.

## Protocol
See `docs/protocol.md`.
