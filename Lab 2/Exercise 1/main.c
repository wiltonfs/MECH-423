#include <msp430.h> 
// Exercise 1
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
	
	// Set the clock password
	CSCTL0 = CSKEY;

	// Set DCO Frequency
	CSCTL1 |= DCOFSEL_3;

	// Set SMCLK to run on DCO
	CSCTL2 |= SELM__DCOCLK;

	// Set SMCLK to a divider of 32
	CSCTL3 |= DIVS__32;

	// Set P3.4 to be SMCLK
	P3DIR |= BIT4;
	P3SEL1 |= BIT4;
	P3SEL0 |= BIT4;


	// Infinite while
	while (1)
	{
	    _NOP();
	}

	return 0;
}
