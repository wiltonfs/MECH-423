#include <msp430.h> 
// Green-board specific functionality for our MECH 423 project
// Felix Wilton & Lazar Stanojevic
// Nov 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef GREEN_INCLUDED
#define GREEN_INCLUDED

// ---------------------------------------------
// -- Green Specific Input Button Definitions --
// ---------------------------------------------

#define FIRE_BUTTON (BIT0)
#define MACHINE_BUTTON_4 (BIT1)

// -----------------------------------
// -- Red Specific UART Definitions --
// -----------------------------------

#define UART_READY_TO_TX (UCA1IFG & UCTXIFG)
#define UART_TX_BUFFER UCA1TXBUF

void StandardUARTSetup_9600_8()
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



// End inclusion guard
#endif // GREEN_INCLUDED
