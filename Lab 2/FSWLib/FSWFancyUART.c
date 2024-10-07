// More complex UART transmission for MECH 423
// Felix Wilton
// Oct 2024

#include "FSWCharBuffer.c"

// Inclusion guard
#ifndef FSWFANCYUART_INCLUDED
#define FSWFANCYUART_INCLUDED

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// Circular buffer of chars to transmit
CircularCharBuffer UARTtransmissionBuffer = { .head = 0, .tail = 0, .count = 0 }; 

void UART_TX_QueueChar(unsigned char c);
// Queues a char for uart transmission

void UART_TX_QueueString(const unsigned char *str);
// Queues a string for uart transmission

void UART_AttemptTransmission();
// If UART is ready, transmits the oldest value in the TX queue

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void UART_TX_QueueChar(unsigned char c) {
    addValue(&UARTtransmissionBuffer, c);
}
void UART_TX_QueueString(const unsigned char *str) {
    while (*str != '\0') { // Continue until we reach the null terminator
        UART_TX_QueueChar(*str);
        str++; // Move to the next character in the string
    }
}
void UART_AttemptTransmission() {
    // If transmit buffer has values, pop and transmit
    if (!isEmpty(&UARTtransmissionBuffer) && UART_READY_TO_TX)
    {
        UCA0TXBUF = popValue(&UARTtransmissionBuffer); // Write to transmit buffer
    }
}














// Inclusion guard
#endif  // FSWFANCYUART_INCLUDED