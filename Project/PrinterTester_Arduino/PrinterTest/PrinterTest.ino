#include <SoftwareSerial.h>
SoftwareSerial mySerial(10, 11); // RX, TX
// For MVP:
// Connect Printer GND to Arduino GND
// Connect Printer RX to Arduino 11
void setup()
{
  mySerial.begin(9600);
}
void loop() 
{
  mySerial.println("Hello Mr. Stanojevic!");
  mySerial.println("Hello Mr. Wilton!");
  mySerial.println("Hello Mr. Finn!");
  mySerial.println("Hello Mr. Allen!");
  mySerial.println("Hello Mr. Mussato!");
  mySerial.println("Hello Mr. Sichani!");
  mySerial.println();
  mySerial.println();
  mySerial.println();
  delay(5000);
}