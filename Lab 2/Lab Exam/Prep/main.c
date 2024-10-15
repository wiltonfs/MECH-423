#include <msp430.h> 
#include "../../FSWLib/FSWLib.c"
// Lab Exam 2 - Prep
// Felix Wilton
// Oct 15 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);

    StandardUART0Setup_9600_8();
    UCA0IE |= UCRXIE;           // Enable RX interrupt
    //UCA0IE |= UCTXIE;         // Enable TX interrupt
    __enable_interrupt();       // Enable global interrupts





    const unsigned char welcomeString[] = "WELCOME";
    const unsigned char greetingString[] = " HELLO";
    UART_TX_String_BLOCKING(welcomeString);

    while(1)
        {
            DelayMillis_8Mhz(375);

            // Heart beat to display general program progression
            // If this stops, you are stuck in an interrupt
            ToggleLED(LED1);

            UART_TX_String_BLOCKING(greetingString);

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
