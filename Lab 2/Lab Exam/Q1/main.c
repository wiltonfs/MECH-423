#include <msp430.h> 
#include "../../FSWLib/FSWLib.c"
// Lab Exam 2 - Q1
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
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);

    TimerB1Setup_UpCount_125kHz(198);
    TB1CTL &= ~(BIT7 | BIT6);    // 1 divider (1 MHz)    (L) pg. 372
    TimerB1_PWM(1, 20); // P3.4

    while(1)
    {
        DelayMillis_8Mhz(500);

        // Heart beat to display general program progression
        // If this stops, you are stuck in an interrupt
        ToggleLED(LED1);
    }

	return 0;
}
