#include <msp430.h> 
#include "../MotorLib/General.c"

// Lab 3 - Exercise 4
// Felix Wilton & Lazar Stanojevic
// Oct 29 2024
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
