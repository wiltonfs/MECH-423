#include <msp430.h> 


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	// Configure P4.0 and P4.1 as a digital input
	//      Configure P4.0 and P4.1 as digital input
	//      Found this in MSP430_Datasheet (M) page 83 and 84
	P4DIR &=  ~(BIT0 | BIT1);
    P4SEL1 &= ~(BIT0 | BIT1);
    P4SEL0 &= ~(BIT0 | BIT1);


	// The switches S1 and S2 are connected to P4.0 and P4.1 on the EXP Board. Enable the internal pull-up resistors for both switches
    //      (L) page 315 has general instructions about PxREN
    //      Nothing says explicitly if its pull up or pull down that I can see. It's either on or off.
    P4REN |= (BIT0 | BIT1);


	// Set P4.0 and P4.1 to get interrupted from a rising edge (i.e. an interrupt occurs when the user lets go of the button). Enable local and global interrupts
    //      (L) page 302 is the P4 table and I saw P4IES for Interrupt Edge Select and P4IE for Interrupt Enable
    //      (L) page 316 tells you what binary value for what type of rising edge. 1 for high-to-low
    //      (L) page 316 tells you how to enable an interrupt
    P4IES &= ~(BIT0 | BIT1); // rising edge detection, write 0
    P4IE |=   (BIT0 | BIT1); // enable interrupts
    //      (L) page 93 tells you that you need to enable GIE to get interrupts at all
    __bis_SR_register(GIE);  // Enable global interrupts


	// Configure P3.6 (LED7) and P3.7 (LED8) as outputs
    //      (M) page 81 and 82
    P3DIR |=   (BIT6 | BIT7);
    P3SEL1 &= ~(BIT6 | BIT7);
    P3SEL0 &= ~(BIT6 | BIT7);

    // Main loop. The MCU will mostly sleep and respond to interrupts.
    while(1) {}

	return 0;
}

// Write an interrupt service routine to toggle LED7 and LED8 after receiving rising edges from S1 and S2 respectively
#pragma vector=PORT4_VECTOR
__interrupt void Port_4(void)
{
    if (P4IFG & BIT0)  // Check if interrupt is from P4.0 (S1)
    {
        P3OUT ^= BIT6; // Toggle LED7 (P3.6)
        P4IFG &= ~BIT0; // Clear interrupt flag for P4.0
    }

    if (P4IFG & BIT1)  // Check if interrupt is from P4.1 (S2)
    {
        P3OUT ^= BIT7; // Toggle LED8 (P3.7)
        P4IFG &= ~BIT1; // Clear interrupt flag for P4.1
    }
}
