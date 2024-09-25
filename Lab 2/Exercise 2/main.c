#include <msp430.h> 


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	// Configure PJ.0, PJ.1, PJ.2, PJ.3, P3.4, P3.5, P3.6, and P3.7 as digital outputs
	//      Configure PJ.0, PJ.1, PJ.2, and PJ.3 as digital outputs
	//      Found this in MSP430_Datasheet (M) page 86
	PJDIR |=   (BIT0 | BIT1 | BIT2 | BIT3);
	PJSEL1 &= ~(BIT0 | BIT1 | BIT2 | BIT3);
	PJSEL0 &= ~(BIT0 | BIT1 | BIT2 | BIT3);
	//      Configure P3.4, P3.5, P3.6 as digital outputs
	//      Found this in MSP430_Datasheet (M) page 81
	P3DIR |=   (BIT4 | BIT5 | BIT6);
	P3SEL1 &= ~(BIT4 | BIT5 | BIT6);
	P3SEL0 &= ~(BIT4 | BIT5 | BIT6);
	//      Configure P3.7 as digital output
	//      Found this in MSP430_Datasheet (M) page 82
	P3DIR |=   (BIT7);
    P3SEL1 &= ~(BIT7);
    P3SEL0 &= ~(BIT7);



	// Set the LEDs 1 to 8 to output 10010011
    //      Looked at MSP430_Experimenter_Board_User_Guide (S) page 17 to see the PINS to LEDs
    //      PJ.0 = LED1, PJ.3 = LED4
    //      P3.4 = LED5, P3.7 = LED8
    //      I used LED1 as most significant bit

    PJOUT |= (BIT0 | BIT3);
    P3OUT |= (BIT6 | BIT7);

	// Blink the LEDs that are 0
	while(1)
	{
	    // Delay some amount
	    __delay_cycles(600000);

	    // Toggle the LEDs that are 0
	    PJOUT ^= (BIT1 | BIT2);
	    P3OUT ^= (BIT4 | BIT5);
	}

	return 0;
}
