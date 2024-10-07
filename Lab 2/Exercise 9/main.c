#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
#include "../FSWLib/FSWCharBuffer.c"
#include "../FSWLib/FSWFancyUART.c"
// Exercise 9
// Felix Wilton
// Oct 6 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

const unsigned char POP_VALUE = 13;                             // Value to watch for (carriage return) to pop values back through UART
const unsigned char EMPTY_BUFFER_ERROR[] = "Empty buffer!";     // Error message for pop on a empty buffer
const unsigned char FULL_BUFFER_ERROR[] = "Full buffer!";       // Error message for receiving chars on a full buffer

CircularCharBuffer receivedChars = { .head = 0, .tail = 0, .count = 0 }; // Circular buffer of received chars

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

    while(1)
        {
            int whileCounter = 0;

            for (whileCounter = 100; whileCounter > 0; whileCounter--)
            {
                DelayMillis_8Mhz(3);
                // If the uart tx buffer still has chars, try to send one along
                UART_AttemptTransmission();
            }

            // Heart beat to display general program progression
            // If this stops, you are stuck in an interrupt
            ToggleLED(LED1);
            // Debug display of the uart tx status
            BoolToLED(isEmpty(&UARTtransmissionBuffer), LED2);
            BoolToLED(UART_READY_TO_TX, LED3);



        }

	return 0;
}

#pragma vector = USCI_A0_VECTOR
__interrupt void uart_ISR(void) {
    // (L) pg. 504
    if (UCA0IV == 2)
    {
        // -------------------------------------------
        // --- Handle RX interrupt (data received) ---
        // -------------------------------------------
        // Toggle LED8 for debug visual
        ToggleLED(LED8);

        volatile unsigned char RxByte = 0;
        RxByte = UCA0RXBUF; // Read from the receive buffer
        //transmitChar(RxByte); // Echo received data

        if (RxByte == POP_VALUE)
        {
            // Asking for a return value
            // Pop a value from received chars
            CharBufferValue pop = '?';
            if (!popValueSAFE(&receivedChars, &pop)){
                // Pop failed
                TurnOnLED(LED6);    // Turn on LED6 for debug visual
                UART_TX_QueueString(EMPTY_BUFFER_ERROR);
            } else {
                // Transmit popped value
                UART_TX_QueueChar(pop);
            }

        } else {
            // Add character to received chars
            if (!addValue(&receivedChars, RxByte)){
                // Add failed
                TurnOnLED(LED5);    // Turn on LED5 for debug visual
                UART_TX_QueueString(FULL_BUFFER_ERROR);
            }
        }

        return;

    }
    else if (UCA0IV == 4 || UCA0IV == 8)
    {
        // ---------------------------------------------------
        // --- Handle TX interrupt (transmission complete) ---
        // ---------------------------------------------------
        // Turn on LED7 for debug visual
        TurnOnLED(LED7);

        UART_AttemptTransmission();

        return;
    }
}
