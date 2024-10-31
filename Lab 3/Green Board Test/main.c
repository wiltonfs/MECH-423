#include <msp430.h> 
#include "../MotorLib/General.c"


/**
 * main.c
 */
unsigned char counter = 0;
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	StandardClockSetup_8Mhz_1Mhz();
    StandardUART1Setup_9600_8();

    // Set up P3.6, P3.7, and P1.6 as outputs
    P3DIR |= (BIT6 | BIT7);
    P3SEL1 &= ~(BIT6 | BIT7);
    P3SEL0 &= ~(BIT6 | BIT7);
    P1DIR |= BIT6;
    P1SEL1 &= ~BIT6;
    P1SEL0 &= ~BIT6;

    // Turn the LEDs off
    P3OUT &= ~(BIT6 | BIT7);
    P1OUT &= ~BIT6;

    while(1)
    {
        DelayMillis_8Mhz(100);

        P3OUT ^= (BIT6 | BIT7);
        P1OUT ^= BIT6;

        UART_TX_Char_BLOCKING(counter);
        counter++;
    }

	return 0;
}
