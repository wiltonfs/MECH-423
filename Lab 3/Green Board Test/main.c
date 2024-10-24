#include <msp430.h> 
#include "../MotorLib/Com.c"
#include "../../Lab 2/FSWLib/FSWLib.c"


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	StandardClockSetup_8Mhz_1Mhz();
	
	// For the green board:
    // Green board LED4 = Port 17 = PJ.2 = Red board LED3
    // Green board LED5 = Port 18 = PJ.3 = Red board LED4
    // Green board LED6 = Port 19 = P2.5 = ?
	SetupLEDPins(LED3 | LED4);
    TurnOffLED(LED3 | LED4);
    TurnOnLED(LED4);



    while(1)
    {
        DelayMillis_8Mhz(500);

        // Heart beat to display general program progression
        // If this stops, you are stuck in an interrupt
        ToggleLED(LED3 | LED4);

    }

	return 0;
}
