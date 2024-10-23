#include <msp430.h> 
#include "../../Lab 2/FSWLib/FSWLib.c"
#include "../../Lab 2/FSWLib/FSWCharBuffer.c"
#include "../../Lab 2/FSWLib/FSWFancyUART.c"
// Lab 3 - Exercise 2
// Felix Wilton
// Oct 22 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	return 0;
}
