#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
// Exercise 9
// Felix Wilton
// Oct 6 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]#include <msp430.h>

// -------------------------------------
// ---------- Circular Buffer ----------
// -------------------------------------
const unsigned char POP_VALUE = 13;
const unsigned char EMPTY_BUFFER_ERROR[] = "Empty buffer!";
const unsigned char FULL_BUFFER_ERROR[] = "Full buffer!";

typedef unsigned char BufferValue;
#define BUFFER_SIZE 50

typedef struct {
    volatile BufferValue buffer[BUFFER_SIZE];
    volatile unsigned int head;  // Points to the next position to add
    volatile unsigned int tail;  // Points to the next position to pop
    volatile unsigned int count; // Keeps track of how many elements are in the buffer
} CircularBuffer;

CircularBuffer receivedChars = { .head = 0, .tail = 0, .count = 0 }; // Circular buffer of received chars

bool addValue(CircularBuffer *cb, BufferValue value) {
    if (cb->count == BUFFER_SIZE) {
        // Buffer is full, cannot add more values
        return false;
    }
    cb->buffer[cb->head] = value;               // Add value to the head
    cb->head = (cb->head + 1) % BUFFER_SIZE;    // Increment head with wrap-around
    cb->count++;                                // Increment count
    return true;
}

bool addStringValue(CircularBuffer *cb, const unsigned char *str) {
    while (*str != '\0') { // Continue until we reach the null terminator
        if (!addValue(cb, *str)) {
            // If the buffer is full, return false
            return false;
        }
        str++; // Move to the next character in the string
    }
    return true; // Successfully added all characters
}

bool popValue(CircularBuffer *cb, BufferValue *valueOut) {
    if (cb->count == 0) {
        // Buffer is empty, cannot pop any values
        return false;
    }
    *valueOut = cb->buffer[cb->tail];   // Get value from the tail
    cb->tail = (cb->tail + 1) % BUFFER_SIZE;    // Increment tail with wrap-around
    cb->count--;                                // Decrement count
    return true;
}

bool isEmpty(CircularBuffer *cb) {
    return cb->count == 0;
}


// -------------------------------------
// ---------- UART Transmission --------
// -------------------------------------
CircularBuffer transmissionBuffer = { .head = 0, .tail = 0, .count = 0 }; // Circular buffer of chars to transmit

void transmitChar(unsigned char c) {
    // If UART is not busy, just send it through
    // Else, put it in the UART queue
    addValue(&transmissionBuffer, c);
    return;
    /*
    if (UART_READY_TO_TX) {
        UCA0TXBUF = c;
    } else {
        addValue(&transmissionBuffer, c);
    }*/
}
void transmitString(const unsigned char *str) {
    while (*str != '\0') { // Continue until we reach the null terminator
        transmitChar(*str);
        str++; // Move to the next character in the string
    }
}


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
    //UCA0IE |= UCTXIE;           // Enable TX interrupt
    __enable_interrupt();       // Enable global interrupts

    while(1)
        {
            // Heart beat to display general program progression
            // If this stops, you are stuck in an interrupt
            DelayMillis_8Mhz(50);
            ToggleLED(LED1);

            // If transmit buffer has values, pop and transmit
            if (!isEmpty(&transmissionBuffer) && UART_READY_TO_TX)
            {
                BufferValue pop;
                popValue(&transmissionBuffer, &pop);
                UCA0TXBUF = pop; // Write to transmit buffer
            }

            if (isEmpty(&transmissionBuffer))
            { //2, 3, 4
                TurnOnLED(LED2);
            } else {
                TurnOffLED(LED2);
            }

            if (UART_READY_TO_TX)
            { //2, 3, 4
                TurnOnLED(LED3);
            } else {
                TurnOffLED(LED3);
            }
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
            BufferValue pop = '?';
            if (!popValue(&receivedChars, &pop)){
                // Pop failed
                TurnOnLED(LED6);    // Turn on LED6 for debug visual
                transmitString(EMPTY_BUFFER_ERROR);
            } else {
                // Transmit popped value
                transmitChar(pop);
            }

        } else {
            // Add character to received chars
            if (!addValue(&receivedChars, RxByte)){
                // Add failed
                TurnOnLED(LED5);    // Turn on LED5 for debug visual
                transmitString(FULL_BUFFER_ERROR);
            }
        }

        return;

    }
    else if (UCA0IV == 4 || UCA0IV == 8)
    {
        // ---------------------------------------------------
        // --- Handle TX interrupt (transmission complete) ---
        // ---------------------------------------------------
        // Toggle LED7 for debug visual
        TurnOnLED(LED7);

        // If transmit buffer has values, pop and transmit
        if (!isEmpty(&transmissionBuffer))
        {
            BufferValue pop;
            popValue(&transmissionBuffer, &pop);
            UCA0TXBUF = pop; // Write to transmit buffer
        }

        return;
    }
}
