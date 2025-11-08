#include "PcComm.h"

PcComm::PcComm(HardwareSerial& port, unsigned long baud)
: _port(port), _baud(baud) {}

void PcComm::begin()
{
    _port.begin(_baud);

    // On native-USB boards this becomes true once the CDC is up;
    // on others it may never become true, so guard with a timeout.
    unsigned long t0 = millis();
    while (!_port && (millis() - t0 < 1500)) { /* wait up to 1.5s */ }

    delay(200);
    // First line on boot to prove life:
    _port.println("{\"type\":\"boot\",\"msg\":\"pccomm up\"}");
    emitHeartbeat();
}

void PcComm::emitHeartbeat()
{
    _seq++;
    _port.print("{\"type\":\"heartbeat\",\"ts_ms\":");
    _port.print(millis());
    _port.print(",\"seq\":");
    _port.print(_seq);
    _port.println(",\"fw\":\"0.0.1\"}");
}

void PcComm::emitSample(float tempC, float flueC, float o2Pct, float pm25)
{
    _seq++;
    _port.print("{\"type\":\"sample\",\"ts_ms\":");
    _port.print(millis());
    _port.print(",\"seq\":");
    _port.print(_seq);
    _port.print(",\"temp_c\":");
    _port.print(tempC, 2);
    _port.print(",\"flue_c\":");
    _port.print(flueC, 1);
    _port.print(",\"o2_pct\":");
    _port.print(o2Pct, 1);
    _port.print(",\"pm25_ugm3\":");
    _port.print(pm25, 1);
    _port.println("}");
}

void PcComm::emitError(const char* msg)
{
    _seq++;
    _port.print("{\"type\":\"error\",\"ts_ms\":");
    _port.print(millis());
    _port.print(",\"seq\":");
    _port.print(_seq);
    _port.print(",\"msg\":\"");
    _port.print(msg);
    _port.println("\"}");
}

void PcComm::emitPms(uint16_t sp_pm1_0, uint16_t sp_pm2_5, uint16_t sp_pm10,
                     uint16_t ae_pm1_0, uint16_t ae_pm2_5, uint16_t ae_pm10) {
    _seq++;
    _port.print("{\"type\":\"pms\",\"ts_ms\":");
    _port.print(millis());
    _port.print(",\"seq\":");
    _port.print(_seq);
    _port.print(",\"pm\":{\"sp\":{\"pm1_0\":");
    _port.print(sp_pm1_0);
    _port.print(",\"pm2_5\":");
    _port.print(sp_pm2_5);
    _port.print(",\"pm10\":");
    _port.print(sp_pm10);
    _port.print("},\"ae\":{\"pm1_0\":");
    _port.print(ae_pm1_0);
    _port.print(",\"pm2_5\":");
    _port.print(ae_pm2_5);
    _port.print(",\"pm10\":");
    _port.print(ae_pm10);
    _port.println("}}}");
}
