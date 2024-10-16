#include <msp430.h> 
#include "../../FSWLib/FSWLib.c"
// Lab Exam 2 - Q5
// Felix Wilton
// Oct 15 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]


/**
 * main.c
 */

typedef enum PACKET_PART {
    START_BYTE,
    COM_BYTE,
    DATA_BYTE,
} PACKET_PART;

typedef struct {
    volatile unsigned char comm;
    volatile unsigned char data;
} MessagePacket;

MessagePacket IncomingPacket = { .comm = 0, .data = 0};
volatile PACKET_PART ExpectedRead = START_BYTE;


volatile unsigned int maxCycle = 100;
volatile unsigned int currentCycle = 0;
volatile signed char upCount = 1;
volatile signed char period = 10;

volatile signed char sawtooth = 1;

void ProcessCompletePacket() {
    //TurnOnLED(LED8);
    period = IncomingPacket.data;
    maxCycle = 100;

    if (IncomingPacket.comm == 4)
    {
        // Orb goes dark
        maxCycle = 0;
        currentCycle = 0;
        upCount = 1;
        TimerB1_PWM(1, currentCycle);
    } else if (IncomingPacket.comm == 2)
    {
        // Orb turns bright to dark
        sawtooth = 0;
        currentCycle = maxCycle;
        upCount = -1;
        TimerB1_PWM(1, currentCycle);
    } else if (IncomingPacket.comm == 1)
    {
        // Orb turns dark to bright
        sawtooth = 1;
        currentCycle = 0;
        upCount = 1;
        TimerB1_PWM(1, currentCycle);
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
    case DATA_BYTE:
        IncomingPacket.data = RxByte;
        ProcessCompletePacket();
        break;
    default:
        break;
    }

    // Increment next read
    if (ExpectedRead >= DATA_BYTE)
        ExpectedRead = START_BYTE;
    else
        ExpectedRead++;
}

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);

    TimerB1Setup_UpCount_125kHz(100);
    TimerB1_PWM(1, 50);         // 50% duty cycle, P3.4 output

    StandardUART0Setup_9600_8();
    UCA0IE |= UCRXIE;           // Enable RX interrupt
    //UCA0IE |= UCTXIE;         // Enable TX interrupt
    __enable_interrupt();       // Enable global interrupts

    while(1)
        {
            DelayMillis_8Mhz(period);

            if (maxCycle != 0)
            {




                if (sawtooth > 0)
                {
                    upCount = 1;
                    if (currentCycle == maxCycle)
                    {
                        currentCycle = 0;
                    }


                } else
                {
                    upCount = -1;
                    if (currentCycle == 0)
                    {
                        currentCycle = maxCycle;
                    }

                }
                currentCycle += upCount;
                TimerB1_PWM(1, currentCycle);
            }
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
        //TurnOnLED(LED6);

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
