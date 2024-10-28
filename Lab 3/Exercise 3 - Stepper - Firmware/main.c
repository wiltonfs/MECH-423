#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/Com.c"
#include "../MotorLib/Stepper.c"

// Lab 3 - Exercise 3
// Felix Wilton & Lazar Stanojevic
// Oct 28 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

MessagePacket IncomingPacket = EMPTY_MESSAGE_PACKET;
volatile PACKET_FRAGMENT NextRead = START_BYTE;

STEPPER_STATE stepper_state = A1_xx;

void ProcessCompletePacket() {
    // Handle the command byte
    if (IncomingPacket.comm == STP_SINGLE_CW) {
        // Single step clockwise
        IncrementHalfStep(&stepper_state, true);

    } else if (IncomingPacket.comm == STP_SINGLE_CCW) {
        // Single step counter-clockwise
        IncrementHalfStep(&stepper_state, false);
    }
}

/**
 * main.c
 */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer
    StandardClockSetup_8Mhz_1Mhz();

    SetupStepperTimers();

    StandardUART1Setup_9600_8();
    UCA1IE |= UCRXIE;           // Enable RX interrupt
    //UCA1IE |= UCTXIE;         // Enable TX interrupt
    __enable_interrupt();       // Enable global interrupts


    while(1)
    {
        DelayMillis_8Mhz(500);
    }

    return 0;
}

#pragma vector = USCI_A1_VECTOR
__interrupt void uart_ISR(void) {
    if (UCA1IV == USCI_UART_UCRXIFG)    // Receive buffer full. (L) pg. 504
    {
        // -------------------------------------------
        // --- Handle RX interrupt (data received) ---
        // -------------------------------------------
        volatile unsigned char RxByte = UCA1RXBUF; // Read from the receive buffer

        if (COM_MessagePacketAssembly_StateMachine(&IncomingPacket, &NextRead, RxByte))
        {
            // returns True if the packet is now complete
            ProcessCompletePacket();
        }

        return;

    }
    else if (UCA1IV == USCI_UART_UCTXIFG || UCA1IV == USCI_UART_UCTXCPTIFG)     // Transmit buffer empty OR Transmit complete. (L) pg. 504
    {
        // ---------------------------------------------------
        // --- Handle TX interrupt (transmission complete) ---
        // ---------------------------------------------------
        return;
    }
    else if (UCA1IV == USCI_UART_UCSTTIFG)  // Start bit received. (L) pg. 504
    {
        return;
    }
}
