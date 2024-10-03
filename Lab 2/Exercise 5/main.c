#include <msp430.h> 
#include "../FSWLib/FSWLib.c"


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();

	SetupLEDPins(ALL_LEDs);
	TurnOffLED(ALL_LEDs);
	TurnOnLED(BIT1);


	while (1)
	{
	    DelayMillis_8Mhz(100);
	    ToggleLED(ALL_LEDs);
	}

	return 0;
}
