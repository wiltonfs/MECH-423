#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
// Exercise 4
// Felix Wilton
// Sep 26 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// SOME TAKEAWAYS:
// 16 bit registers are often split into two 8 bit registers
// These are called HIGH and LOW
// LOW is bits 0-7, and HIGH is bits 8-15
// I can't find a pattern for LOW and HIGH with the _1 and _0
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();
	SetupLEDPins(ALL_LEDs);
	TurnOffLED(ALL_LEDs);
	StandardUART0Setup_9600_8();
	UCA0IE |= UCRXIE;           // Enable RX interrupt
	__enable_interrupt();       // Enable global interrupts

    // Inside the while loop, send 'a' periodically
    while (1) {
        DelayMillis_8Mhz(500);
        UART_TX_Char_BLOCKING('a');

        // Heart beat to display general program progression
        // If this stops, you are stuck in an interrupt
        ToggleLED(LED2);
    }

	return 0;
}

#pragma vector = USCI_A0_VECTOR
__interrupt void uart_ISR(void) {
    if (UCA0IV == USCI_UART_UCRXIFG)    // Receive buffer full. (L) pg. 504
    {
        // -------------------------------------------
        // --- Handle RX interrupt (data received) ---
        // -------------------------------------------
        // Toggle LED8 for debug visual
        ToggleLED(LED8);

        volatile unsigned char RxByte = UCA0RXBUF; // Read from the receive buffer

        UART_TX_Char_BLOCKING(RxByte);
        UART_TX_Char_BLOCKING(RxByte + 1);

        if (RxByte == 'j')
        {
            // Turn on LED1
            TurnOnLED(LED1);
        } else if (RxByte == 'k')
        {
            // Turn off LED1
            TurnOffLED(LED1);
        }

        return;

    }
    else if (UCA0IV == USCI_UART_UCTXIFG || UCA0IV == USCI_UART_UCTXCPTIFG)     // Transmit buffer empty OR Transmit complete. (L) pg. 504
    {
        // ---------------------------------------------------
        // --- Handle TX interrupt (transmission complete) ---
        // ---------------------------------------------------
        // Turn on LED7 for debug visual
        TurnOnLED(LED7);

        return;
    }
    else if (UCA0IV == USCI_UART_UCSTTIFG)  // Start bit received. (L) pg. 504
    {
        return;
    }
}
