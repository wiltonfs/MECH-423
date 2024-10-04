#include <msp430.h> 
#include "../FSWLib/FSWLib.c"


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();
	SetupLEDPins(ALL_LEDs);

	// Setup Timer B in the "up count" mode
	TB1CTL |= TBCLR;            // Clear Timer B            (L) pg.372
	TB1CTL |= (BIT4);           // Up mode                  (L) pg. 372
	TB1CTL |= TBSSEL__SMCLK;    // Clock source select      (L) pg. 372
    TB1CTL |= (BIT7 | BIT6);    // 1/8 divider (125 kHz)    (L) pg. 372
    TB1CCR0  = 250;             // now 500 Hz               (L) pg. 377

	// Configure TB1.1 to produce a 500 Hz square wave
    TB1CCTL1 = OUTMOD_7;        // Reset/Set mode           (L) pg. 366 and 375
    TB1CCR1 = 125;              // 50% duty cycle           (L) pg. 366 and

	// Output TB1.1 on P3.4
    P3DIR |= BIT4;
    P3SEL1 &= ~BIT4;            // Select TB1.1 function    (M) pg. 81
    P3SEL0 |=  BIT4;            // Select TB1.1 function    (M) pg. 81

	// Configure TB1.2 to produce a 500 Hz square wave at 25% duty cycle
    TB1CCTL2 = OUTMOD_7;        // Reset/Set mode           (L) pg. 366 and 375
    TB1CCR2 = 62;               // 25% duty cycle

	// Output TB1.2 on P3.5
    P3SEL1 &= ~BIT5;            // Select TB1.2 function    (M) pg. 81
    P3SEL0 |=  BIT5;            // Select TB1.2 function    (M) pg. 81

	while (1)
	{

	}

	return 0;
}
