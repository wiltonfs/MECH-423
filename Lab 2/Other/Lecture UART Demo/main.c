#include <msp430.h> 


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	// Configure clocks
	CSCTL0 = 0xA500;
	CSCTL1 = DCOFSEL0 + DCOFSEL1;
	CSCTL2 = SELM0 + SELM1 + SELA0 +SELA1 + SELS0 + SELS1;

	P2SEL0 &= ~(BIT0 + BIT1);
	P2SEL1 |= BIT0 + BIT1;

    UCA0CTLW0 |= UCSSEL0;
    UCA0MCTLW = UCOS16 + UCBRF0 + 0x4900;
    UCA0BRW = 52;
    UCA0CTLW0 &= ~UCSWRST;
    UCA0IE |= UCRXIE;
    _EINT();

    while (1)
    {

    }
	
	return 0;
}
#pragma vector = USCI_A0_VECTOR
__interrupt void uart_ISR(void)
{
    unsigned char RxByte =0;
    RxByte = UCA0RXBUF;
    while (!(UCA0IFG & UCTXIFG));
    UCA0TXBUF = RxByte;
    while (!(UCA0IFG & UCTXIFG));
    UCA0TXBUF = RxByte + 1;
}
