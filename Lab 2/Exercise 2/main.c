#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
// Exercise 2
// Felix Wilton
// Sep 25 2024
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

    EnableSwitches(SWITCH_1 | SWITCH_2);
    P4IE |=   (BIT0 | BIT1);    // Enable switch interrupts     (L) pg. 316
    __enable_interrupt();       // Enable global interrupts

    // Set the LEDs 1 to 8 to output 10010011
    unsigned char selectionMask = LED1 | LED4 | LED7 | LED8;
    TurnOnLED(selectionMask);

    while(1)
    {
        DelayMillis_8Mhz(300);
        ToggleLED(~selectionMask);
    }

    return 0;
}
