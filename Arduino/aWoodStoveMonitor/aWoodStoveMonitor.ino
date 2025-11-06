// Minimal JSONL heartbeat + dummy sample

unsigned long seq = 0;
unsigned long lastBeat = 0;
unsigned long lastSample = 0;

void setup() {
  Serial.begin(115200);
  while (!Serial) { ; } // Some boards need this
  delay(200);
  emitHeartbeat();
}

void loop() {
  unsigned long now = millis();

  if (now - lastBeat >= 1000) {
    emitHeartbeat();
    lastBeat = now;
  }

  if (now - lastSample >= 1000) {
    emitDummySample();
    lastSample = now;
  }

  // (Later) read sensors and replace dummy data.
}

void emitHeartbeat() {
  seq++;
  // ts_ms is MCU uptime until you sync with PC time
  Serial.print("{\"type\":\"heartbeat\",\"ts_ms\":");
  Serial.print(millis());
  Serial.print(",\"seq\":");
  Serial.print(seq);
  Serial.println(",\"fw\":\"0.0.1\"}");
}

void emitDummySample() {
  seq++;
  float temp_c = 23.5 + (millis() % 1000) * 0.0005; // changing value
  float flue_c = 120.0;
  float o2_pct = 20.9;
  float pm25   = 0.0;

  Serial.print("{\"type\":\"sample\",\"ts_ms\":");
  Serial.print(millis());
  Serial.print(",\"seq\":");
  Serial.print(seq);
  Serial.print(",\"temp_c\":");
  Serial.print(temp_c, 2);
  Serial.print(",\"flue_c\":");
  Serial.print(flue_c, 1);
  Serial.print(",\"o2_pct\":");
  Serial.print(o2_pct, 1);
  Serial.print(",\"pm25_ugm3\":");
  Serial.print(pm25, 1);
  Serial.println("}");
}
