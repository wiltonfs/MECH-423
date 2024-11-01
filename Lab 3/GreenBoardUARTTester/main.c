#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/Com.c"


/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();
	StandardUART1Setup_9600_8();
	UCA1IE |= UCRXIE;           // Enable RX interrupt
	__enable_interrupt();       // Enable global interrupts

	return 0;
}

#pragma vector = USCI_A1_VECTOR
__interrupt void uart_ISR(void)
{
    if (UCA1IV == USCI_UART_UCRXIFG)    // Receive buffer full. (L) pg. 504
    {
        volatile unsigned char RxByte = UCA1RXBUF; // Read from the receive buffer

        // Echo + 1
        UART_TX_Char_BLOCKING(RxByte + 1);
    }
}
