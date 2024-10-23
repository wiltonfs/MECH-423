#include <msp430.h> 
#include "../MotorLib/Com.c"
#include "../../Lab 2/FSWLib/FSWLib.c"

// Lab 3 - Exercise 2
// Felix Wilton & Lazar Stanojevic
// Oct 23 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

MessagePacket IncomingPacket = EMTPY_MESSAGE_PACKET;
volatile PACKET_FRAGMENT NextRead = START_BYTE;

void ProcessCompletePacket() {
    // Handle the command byte
    if (IncomingPacket.comm == DCM_CW) {

        // Modify the square wave duty cycle
        TimerB1_PWM(1, IncomingPacket.combined);

        // Turn on LED3
        TurnOnLED(LED3);
        // Turn off LED2
        TurnOffLED(LED2);

    } else if (IncomingPacket.comm == DCM_CCW) {

        // Modify the square wave duty cycle
        TimerB1_PWM(1, IncomingPacket.combined);

        // Turn on LED2
        TurnOnLED(LED2);
        // Turn off LED3
        TurnOffLED(LED3);

    } else if (IncomingPacket.comm == DCM_BRAKE) {

        // Turn off LED2
        TurnOffLED(LED2);
        // Turn off LED3
        TurnOffLED(LED3);
    }
}

/**
 * main.c
 */
int main(void)
{

    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    //initialize pins 3.4 and 3.5 to primary mode (TB1.1 and TB1.2)
    P3SEL0 |= BIT4;
    P3SEL1 &= ~BIT4;
    P3DIR |= BIT4;

    P3SEL0 |= BIT5;
    P3SEL1 &= ~BIT5;
    P3DIR |= BIT5;

    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);

    StandardClockSetup_8Mhz_1Mhz();

    // Set-up auxillary clock to run at 8 million Hz
    CSCTL0 = CSKEY;                         // Set the clock password.  (L) pg. 80
    CSCTL2 |= SELA__DCOCLK;                 // Set ACLK to run on DCO. (L) pg. 82

    TimerB1Setup_UpCount_125kHz(65535);

    // Adjust Timer B1 to use auxillary clock and have an output switching of ~123 Hz
    TB1CTL &= ~(BIT9 | BIT8);    // Clear clock select bits   (L) pg. 372
    TB1CTL |= TBSSEL__ACLK;      // Clock source select      (L) pg. 372
    TB1CTL &= ~(BIT7 | BIT6);    // 1 divider (8 MHz)    (L) pg. 372

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

        if (COM_MessagePacketAssembly_StateMachine(&IncomingPacket, &NextRead, RxByte))
        {
            // returns True if the packet is now complete
            ProcessCompletePacket();
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
