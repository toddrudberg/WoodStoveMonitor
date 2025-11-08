#include "PMS5003.h"

StovePMS5003::StovePMS5003(Stream& serial) : pms(serial) {}

void StovePMS5003::begin() {
    pms.wakeUp();
    delay(1500);         // spin-up
    pms.activeMode();
}

bool StovePMS5003::read() {
    return pms.read(data);
}

void StovePMS5003::wake()  { pms.wakeUp(); }
void StovePMS5003::sleep() { pms.sleep();  }
