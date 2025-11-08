#pragma once
#include <Arduino.h>

class PcComm {
public:
    PcComm(HardwareSerial& port, unsigned long baud = 115200);

    void begin();
    void emitHeartbeat();
    void emitSample(float tempC, float flueC, float o2Pct, float pm25);
    void emitError(const char* msg);

    void emitPms(uint16_t sp_pm1_0, uint16_t sp_pm2_5, uint16_t sp_pm10,
                 uint16_t ae_pm1_0, uint16_t ae_pm2_5, uint16_t ae_pm10);

private:
    HardwareSerial& _port;
    unsigned long   _baud;
    unsigned long   _seq = 0;
};
