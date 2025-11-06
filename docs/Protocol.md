# Protocol (v0)

**Transport:** USB CDC serial (line-delimited UTF-8 text).  
**Baud:** 115200 (can change; keep in sync).

Each line is one JSON object. Use `\n` line endings.

## Message types

### 1) heartbeat
Emitted every 1000 ms.
```json
{"type":"heartbeat","ts_ms":1730822400000,"seq":1,"fw":"0.0.1"}
