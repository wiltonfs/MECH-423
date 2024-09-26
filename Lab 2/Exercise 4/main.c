#include <msp430.h> 


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	// Set the clock source and configure UART
    UCA0CTL1 |= UCSWRST;        // Put eUSCI in reset
    UCA0CTL1 |= UCSSEL_2;       // SMCLK as clock source
    UCA0BR0 = 104;              // 9600 baud rate with 1MHz clock (N = 1MHz / 9600)
    UCA0BR1 = 0;
    UCA0MCTLW = 0xD600;         // Set modulation (UCBRSx=0xD6, UCOS16=1)

    // Configure pins P2.0 and P2.1 for UART
    P2SEL0 &= ~(BIT0 | BIT1);
    P2SEL1 |= BIT0 | BIT1;      // Set P2.0 (RXD) and P2.1 (TXD) for UART

    UCA0CTL1 &= ~UCSWRST;       // Initialize eUSCI

    // Inside the while loop, send 'a' periodically
    while (1) {
        while (!(UCA0IFG & UCTXIFG));  // Wait for the transmit buffer to be empty
        UCA0TXBUF = 'a';               // Transmit the letter 'a'
        __delay_cycles(600000);        // Delay before next transmission
    }

	return 0;
}
