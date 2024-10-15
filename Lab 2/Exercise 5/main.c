#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
// Exercise 5
// Felix Wilton
// Oct 3 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);

    TimerB1Setup_UpCount_125kHz(249);   // now 500 Hz               (L) pg. 377
    TimerB1_PWM(1, 125);
    TimerB1_PWM(2, 62);

	while (1)
	{

	}

	return 0;
}
