#include <msp430.h> 
// Red-board specific functionality for our MECH 423 project
// Felix Wilton & Lazar Stanojevic
// Nov 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef RED_INCLUDED
#define RED_INCLUDED

// -------------------------------------------
// -- Red Specific Input Button Definitions --
// -------------------------------------------

#define FIRE_BUTTON (BIT6)
#define MACHINE_BUTTON_4 (BIT5)

// -----------------------------------
// -- Red Specific UART Definitions --
// -----------------------------------

#define UART_READY_TO_TX (UCA0IFG & UCTXIFG)
#define UART_TX_BUFFER UCA0TXBUF

void StandardUARTSetup_9600_8()
{
    // Set the clock source and configure UART
    UCA0CTLW0 |= UCSWRST;        // Put eUSCI in reset  (L) pg. 495
    UCA0CTLW0 |= UCSSEL__SMCLK;  // SMCLK as source     (L) pg. 495
    UCA0CTLW0 &= ~(BIT9 | BITA); // UART mode           (L) pg. 495
    // Put target baud and BRCLK into equations         (L) pg. 487
    // Or check handy table for common settings         (L) pg. 490
    // For 1MHz clock and 9600 baud:
    // UCOS16 = 1
    // UCBRx = 6
    // UCBRFx = 8
    // UCBRSx = 0x20
    UCA0BRW = 6;                // Prescaler control    (L) pg. 516
    UCA0MCTLW = 0x2081;         // Assemble settings    (L) pg. 497
    // Set the other settings. 8 data bits, no parity, 1 stop bit. These are default but set anyway.
    UCA0CTLW0 &= ~UC7BIT;        // 8 data bits         (L) pg. 495
    UCA0CTLW0 &= ~UCPEN;         // Disable Parity      (L) pg. 495
    UCA0CTLW0 &= ~UCSPB;         // 1 Stop bit          (L) pg. 495

    // Configure pins P2.0 and P2.1 for UART as Receive and Transmit
    // P2.0 TXD, P2.1 RXD
    P2SEL0 &= ~(BIT0 | BIT1);       // (M) pg. 74
    P2SEL1 |=  (BIT0 | BIT1);       // (M) pg. 74

    // Start UART
    UCA0CTLW0 &= ~UCSWRST;       // Undo reset on eUSCI (L) pg. 495
}



// End inclusion guard
#endif // RED_INCLUDED
