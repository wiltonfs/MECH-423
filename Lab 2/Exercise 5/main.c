#include <msp430.h> 
#include "../FSWLib/FSWLib.c"


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();

	SetupLEDPins(BIT0 | BIT1 | BIT2 | BIT3);
	TurnOnLED(BIT1);
	TurnOffLED(BIT0 | BIT2 | BIT3);

	while (1)
	{
	    DelayMillis_8Mhz(100);
	    ToggleLED(255);
	}

	return 0;
}
