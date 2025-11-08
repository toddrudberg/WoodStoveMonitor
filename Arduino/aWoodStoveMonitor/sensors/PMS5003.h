#pragma once
#include <Arduino.h>
#include <PMS.h>

class StovePMS5003 {
public:
    StovePMS5003(Stream& serial);
    void begin();
    bool read();
    void wake();
    void sleep();

    // accessors
    uint16_t pm1_0() const { return data.PM_AE_UG_1_0; }
    uint16_t pm2_5() const { return data.PM_AE_UG_2_5; }
    uint16_t pm10()  const { return data.PM_AE_UG_10_0; }

private:
    PMS pms;
    PMS::DATA data;
};
