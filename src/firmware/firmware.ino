#include "cc1101.h"
#include "receiver.h"

// https://github.com/walterbrebels/somfy-rts/blob/master/src/main.cpp
// https://pushstack.wordpress.com/somfy-rtimestamp-protocol/
// elechouse

int pin = PIN2;

#define MAX_PULSES 16
volatile int pulses[MAX_PULSES];
volatile size_t pulseIndex;
volatile int lastTime;

static Frame frame;
static Receiver receiver;

static void getFrame()
{
  if (!receiver.storeFrame(&frame))
  {
    Serial.println("Frame checksum NOK");
    return;
  }

  Serial.print("Frame address=");
  Serial.print(frame.address, HEX);
  Serial.print(", control code=");
  Serial.print(frame.controlCode, HEX);
  Serial.print(", rolling code=");
  Serial.println(frame.rollingCode);
}

void setup() {
  delay(5000);
  pinMode(pin, INPUT);
  Serial.begin(9600);
  if (ELECHOUSE_cc1101.getCC1101()) {
    Serial.println("Connection OK");
  } else {
    Serial.println("Connection Error");
  }
  ELECHOUSE_cc1101.Init();
  ELECHOUSE_cc1101.setGDO0(pin); 
  ELECHOUSE_cc1101.setMHZ(433.42);
  ELECHOUSE_cc1101.SetRx();
  pulseIndex = 0;
  attachInterrupt(digitalPinToInterrupt(pin), change, CHANGE);
}

void loop()
{
  static int pulses2[MAX_PULSES];
  static int pulseIndex2;

  noInterrupts();
  pulseIndex2 = pulseIndex;
  for (size_t i = 0; i < pulseIndex2; i++) pulses2[i] = pulses[i];
  pulseIndex = 0;
  interrupts();

  if (pulseIndex2 == MAX_PULSES)
  {
    Serial.println("ERROR: Overflow detected!");
  }

  for (size_t i = 0; i < pulseIndex2; i++)
  {
    receiver.pulse(pulses2[i]);

    if (receiver.hasFrame())
    {
      getFrame();
      receiver.reset();
    }
  }
}

void change()
{
  int time = micros();
  int duration = time - lastTime;
  lastTime = time;
  if (pulseIndex < MAX_PULSES) pulses[pulseIndex++] = duration;
}
