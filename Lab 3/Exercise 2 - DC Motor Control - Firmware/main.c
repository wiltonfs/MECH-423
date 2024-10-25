#include <msp430.h> 
#include "../MotorLib/Com.c"
#include "../../Lab 2/FSWLib/FSWLib.c"

// Lab 3 - Exercise 2
// Felix Wilton & Lazar Stanojevic
// Oct 23 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// ------------------------------------------
// ----- Messaging Protocol Definitions -----
// ------------------------------------------

// Clockwise rotation direction for DC motor
#define DCM_0 8
#define DCM_CW 8

// Counter-clockwise rotation direction for DC motor
#define DCM_1 9
#define DCM_CCW 9

// Brake command for DC motor
#define DCM_3 10
#define DCM_BRAKE 10

void StandardUART1Setup_9600_8();

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
        IncomingPacket.d1 = 255;
    if (IncomingPacket.esc & BIT1)
        IncomingPacket.d2 = 255;

    // Combine the data bytes
    IncomingPacket.combined = (IncomingPacket.d1 << 8) | IncomingPacket.d2;

    // Modify the square wave duty cycle
    TimerB1_PWM(1, IncomingPacket.combined);

    // Handle the command byte
    if (IncomingPacket.comm == DCM_CW) {

        // CW spin command to motor driver
        P3OUT |= BIT6;
        P3OUT &= ~BIT7;

    } else if (IncomingPacket.comm == DCM_CCW) {

        // CCW spin command to motor driver
        P3OUT &= ~BIT6;
        P3OUT |= BIT7;

    } else if (IncomingPacket.comm == DCM_BRAKE) {

        // Stop spin command to motor driver
        P3OUT &= ~(BIT6 + BIT7);

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

    //initializing relevant pins for  outputs (P3.6 & P3.7) - this is arbitrary assignment. Can be any I/O pins
    P3SEL0 &= ~(BIT6 + BIT7);
    P3SEL1 &= ~(BIT6 + BIT7);
    P3DIR |= BIT6 + BIT7;

    //Stop command to motor driver
    P3OUT &= ~(BIT6 + BIT7);

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

    //StandardUART1Setup_9600_8();

    // Set the clock source and configure UART
    UCA1CTLW0 |= UCSWRST;        // Put eUSCI in reset  (L) pg. 495
    UCA1CTLW0 |= UCSSEL__SMCLK;  // SMCLK as source     (L) pg. 495
    UCA1CTLW0 &= ~(BIT9 | BITA); // UART mode           (L) pg. 495
    // Put target baud and BRCLK into equations         (L) pg. 487
    // Or check handy table for common settings         (L) pg. 490
    // For 1MHz clock and 9600 baud:
    // UCOS16 = 1
    // UCBRx = 6
    // UCBRFx = 8
    // UCBRSx = 0x20
    UCA1BRW = 6;                // Prescaler control    (L) pg. 516
    UCA1MCTLW = 0x2081;         // Assemble settings    (L) pg. 497
    // Set the other settings. 8 data bits, no parity, 1 stop bit. These are default but set anyway.
    UCA1CTLW0 &= ~UC7BIT;        // 8 data bits         (L) pg. 495
    UCA1CTLW0 &= ~UCPEN;         // Disable Parity      (L) pg. 495
    UCA1CTLW0 &= ~UCSPB;         // 1 Stop bit          (L) pg. 495

    // Configure pins P2.5 and P2.6 for UART as Receive and Transmit
    // P2.5 TXD, P2.6 RXD
    P2SEL0 &= ~(BIT5 | BIT6);       // (M) pg. 74
    P2SEL1 |=  (BIT5 | BIT6);       // (M) pg. 74

    // Start UART
    UCA1CTLW0 &= ~UCSWRST;       // Undo reset on eUSCI (L) pg. 495

    UCA1IE |= UCRXIE;           // Enable RX interrupt
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

#pragma vector = USCI_A1_VECTOR
__interrupt void uart_ISR(void) {
    if (UCA1IV == USCI_UART_UCRXIFG)    // Receive buffer full. (L) pg. 504
    {
        // -------------------------------------------
        // --- Handle RX interrupt (data received) ---
        // -------------------------------------------
        // Turn on LED8 for debug visual
        ToggleLED(LED8);

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
        // Turn on LED7 for debug visual
        TurnOnLED(LED7);

        return;
    }
    else if (UCA1IV == USCI_UART_UCSTTIFG)  // Start bit received. (L) pg. 504
    {
        return;
    }
}

void StandardUART1Setup_9600_8()
{
    // Set the clock source and configure UART
    UCA1CTLW0 |= UCSWRST;        // Put eUSCI in reset  (L) pg. 495
    UCA1CTLW0 |= UCSSEL__SMCLK;  // SMCLK as source     (L) pg. 495
    UCA1CTLW0 &= ~(BIT9 | BITA); // UART mode           (L) pg. 495
    // Put target baud and BRCLK into equations         (L) pg. 487
    // Or check handy table for common settings         (L) pg. 490
    // For 1MHz clock and 9600 baud:
    // UCOS16 = 1
    // UCBRx = 6
    // UCBRFx = 8
    // UCBRSx = 0x20
    UCA1BRW = 6;                // Prescaler control    (L) pg. 516
    UCA1MCTLW = 0x2081;         // Assemble settings    (L) pg. 497
    // Set the other settings. 8 data bits, no parity, 1 stop bit. These are default but set anyway.
    UCA1CTLW0 &= ~UC7BIT;        // 8 data bits         (L) pg. 495
    UCA1CTLW0 &= ~UCPEN;         // Disable Parity      (L) pg. 495
    UCA1CTLW0 &= ~UCSPB;         // 1 Stop bit          (L) pg. 495

    // Configure pins P2.5 and P2.6 for UART as Receive and Transmit
    // P2.5 TXD, P2.6 RXD
    P2SEL0 &= ~(BIT5 | BIT6);       // (M) pg. 74
    P2SEL1 |=  (BIT5 | BIT6);       // (M) pg. 74

    // Start UART
    UCA1CTLW0 &= ~UCSWRST;       // Undo reset on eUSCI (L) pg. 495
}
