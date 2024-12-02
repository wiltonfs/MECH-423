#include <msp430.h> 
// Standard functions for our MECH 423 project's output LEDs
// Felix Wilton & Lazar Stanojevic
// Dec 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef OUTPUT_LEDs_INCLUDED
#define OUTPUT_LEDs_INCLUDED

#define FLASHING_LEDs (BIT0)
#define STATE_MACHINE_LED_OUTS (STATE_MACHINE_LED_OUTS_0 | STATE_MACHINE_LED_OUTS_1 | STATE_MACHINE_LED_OUTS_2 | STATE_MACHINE_LED_OUTS_3 | STATE_MACHINE_LED_OUTS_4)
#define STATE_MACHINE_LED_OUTS_0 (BIT3)
#define STATE_MACHINE_LED_OUTS_1 (BIT4)
#define STATE_MACHINE_LED_OUTS_2 (BIT5)
#define STATE_MACHINE_LED_OUTS_3 (BIT6)
#define STATE_MACHINE_LED_OUTS_4 (BIT7)

void SetupOutputLEDs()
{
    // Configure switches as a digital output       (M) pg. 70, 72, 73
	P1DIR  |=  (STATE_MACHINE_LED_OUTS | FLASHING_LEDs);
    P1SEL1 &= ~(STATE_MACHINE_LED_OUTS | FLASHING_LEDs);
    P1SEL0 &= ~(STATE_MACHINE_LED_OUTS | FLASHING_LEDs);
}

void WriteStateToLEDs(volatile unsigned char state)
{
    P1OUT = (P1OUT & ~STATE_MACHINE_LED_OUTS) | ((state << 3) & STATE_MACHINE_LED_OUTS);
}



// End inclusion guard
#endif // OUTPUT_LEDs_INCLUDED
