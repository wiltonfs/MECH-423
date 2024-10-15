#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
// Exercise 3
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

    while(1)
    {
        DelayMillis_8Mhz(100);

        // Heart beat to display general program progression
        // If this stops, you are stuck in an interrupt
        ToggleLED(LED1);
    }

	return 0;
}

// ISR to toggle LED7 and LED8 after receiving rising edges from S1 and S2 respectively
#pragma vector=PORT4_VECTOR
__interrupt void Port_4(void)
{
    if (P4IFG & BIT0)       // Check if interrupt is from P4.0 (S1) (L) pg. 317
    {
        ToggleLED(LED7);    // Toggle LED7 (P3.6)
        P4IFG &= ~BIT0;     // Clear interrupt flag for P4.0
    }

    if (P4IFG & BIT1)       // Check if interrupt is from P4.1 (S2) (L) pg. 317
    {
        ToggleLED(LED8);    // Toggle LED8 (P3.7)
        P4IFG &= ~BIT1;     // Clear interrupt flag for P4.1
    }
}
