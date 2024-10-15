#include <msp430.h> 
#include "../../FSWLib/FSWLib.c"
// Lab Exam 2 - Q5
// Felix Wilton
// Oct 15 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]


/**
 * main.c
 */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);

    while(1)
    {
        DelayMillis_8Mhz(375);

        // Heart beat to display general program progression
        // If this stops, you are stuck in an interrupt
        ToggleLED(LED1);
    }

    return 0;
}
