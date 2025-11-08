#include <PMS.h>
#include "PcComm.h"

PcComm pc(Serial);
PMS pms(Serial1);
PMS::DATA pmsData;

const int LED = LED_BUILTIN;
bool led = false;

unsigned long lastBeat = 0;

void setup() {
  pinMode(LED, OUTPUT);

  // --- PC serial link ---
  pc.begin();

  // --- PMS5003 setup ---
  Serial1.begin(9600);
  pms.wakeUp();
  delay(1500);        // let the fan and laser stabilize
  pms.activeMode();
  pc.emitError("PMS5003 active");
}
// int idx = 0;
// uint8_t buf[32];
void loop() {
  unsigned long now = millis();

  // --- 1 Hz heartbeat ---
  if (now - lastBeat >= 1000) {
    lastBeat = now;
    pc.emitHeartbeat();
    led = !led;
    digitalWrite(LED, led);
  }

  // --- Forward PMS5003 frames ---
  if (pms.read(pmsData)) {
    pc.emitPms(
      pmsData.PM_SP_UG_1_0,
      pmsData.PM_SP_UG_2_5,
      pmsData.PM_SP_UG_10_0,
      pmsData.PM_AE_UG_1_0,
      pmsData.PM_AE_UG_2_5,
      pmsData.PM_AE_UG_10_0
    );
  }
  // while (Serial1.available()) 
  // {
  //     uint8_t b = Serial1.read();

  //     if (idx == 0 && b != 0x42) continue;        // sync to 0x42 0x4D
  //     if (idx == 0) { buf[idx++] = b; continue; }
  //     if (idx == 1 && b != 0x4D) { idx = 0; continue; }
  //     if (idx == 1) { buf[idx++] = b; continue; }

  //     buf[idx++] = b;
  //     if (idx >= 32) {
  //       // Frame header (0x42,0x4D), length=buf[2]<<8|buf[3] (usually 28)
  //       uint16_t pm25_cf1 = (buf[12] << 8) | buf[13]; // CF=1 (standard particles)
  //       uint16_t pm25_ae  = (buf[22] << 8) | buf[23]; // Atmospheric environment

  //       Serial.print("BM ");
  //       Serial.print("pm2.5_cf1=");
  //       Serial.print(pm25_cf1);
  //       Serial.print("  pm2.5_ae=");
  //       Serial.println(pm25_ae);

  //       idx = 0;
  //     }
  //   }
  delay(10); // pacing
}
