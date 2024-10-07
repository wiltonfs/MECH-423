#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
#include "../FSWLib/FSWCharBuffer.c"
#include "../FSWLib/FSWFancyUART.c"
// Exercise 10
// Felix Wilton
// Oct 7 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

typedef enum NEXT_VALUE {
    START_BYTE,
    COM_BYTE,
    D1_BYTE,
    D2_BYTE,
    ESCP_BYTE
} NEXT_VALUE;

typedef struct {
    volatile unsigned char comm;
    volatile unsigned char d1;
    volatile unsigned char d2;
    volatile unsigned char esc;
    volatile unsigned int combined;
} MessagePacket;

MessagePacket IncomingPacket = { .comm = 0, .d1 = 0, .d2 = 0, .esc = 0, .combined = 0};
volatile NEXT_VALUE ExpectedRead = START_BYTE;

void ProcessCompletePacket() {
    // Handle the escape byte
    if (IncomingPacket.esc & BIT0)
        IncomingPacket.d2 = 255;
    if (IncomingPacket.esc & BIT1)
        IncomingPacket.d1 = 255;

    // Combine the data bytes
    IncomingPacket.combined = (IncomingPacket.d1 << 8) | IncomingPacket.d2;

    // Handle the command byte
    if (IncomingPacket.comm == 1) {
        // Modify the square wave duty cycle
        TimerB1_PWM(1, IncomingPacket.combined);
    } else if (IncomingPacket.comm == 2) {
        // Turn on LED2
        TurnOnLED(LED2);
    } else if (IncomingPacket.comm == 3) {
        // Turn off LED2
        TurnOffLED(LED2);
    }
}

void ReceiveStateMachine(volatile unsigned char RxByte) {

    if (RxByte == 255)
    {
        ExpectedRead = COM_BYTE;
        return;
    }

    switch(ExpectedRead) {
    case START_BYTE:
        break;
    case COM_BYTE:
        IncomingPacket.comm = RxByte;
        break;
    case D1_BYTE:
        IncomingPacket.d1 = RxByte;
        break;
    case D2_BYTE:
        IncomingPacket.d2 = RxByte;
        break;
    case ESCP_BYTE:
        IncomingPacket.esc = RxByte;
        ProcessCompletePacket();
        break;
    default:
        break;
    }

    // Increment next read
    if (ExpectedRead >= ESCP_BYTE)
        ExpectedRead = START_BYTE;
    else
        ExpectedRead++;
}

/**
 * main.c
 */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);

    TimerB1Setup_UpCount_125kHz(65535);
    TimerB1_PWM(1, 32767);         // 50% duty cycle, P3.4 output

    StandardUART0Setup_9600_8();
    UCA0IE |= UCRXIE;           // Enable RX interrupt
    //UCA0IE |= UCTXIE;         // Enable TX interrupt
    __enable_interrupt();       // Enable global interrupts

    while(1)
        {
            DelayMillis_8Mhz(375);

            // Heart beat to display general program progression
            // If this stops, you are stuck in an interrupt
            ToggleLED(LED1);

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
        // Turn on LED8 for debug visual
        TurnOnLED(LED8);

        volatile unsigned char RxByte = UCA0RXBUF; // Read from the receive buffer

        ReceiveStateMachine(RxByte);

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
