#include <msp430.h> 
#include "../../Lab 2/FSWLib/FSWLib.c"


/**
 * main.c
 */
unsigned char counter = 0;
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();
	SetupLEDPins(ALL_LEDs);
	TurnOffLED(ALL_LEDs);

	StandardUART0Setup_9600_8();

	while(1)
	{
	    ToggleLED(LED1);
	    DelayMillis_8Mhz(333);
	    UART_TX_Char_BLOCKING(counter);
	    counter++;
	}

	return 0;
}
