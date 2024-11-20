#include <msp430.h> 
// Standard functions for programming the Red board in our MECH 423 project
// Felix Wilton & Lazar Stanojevic
// Nov 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef RED_INCLUDED
#define RED_INCLUDED

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Handy Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// ----------------------
// -- bool Definitions --
// ----------------------

#define true 1
#define false 0
typedef short bool;

// ------------------------
// -- System Definitions --
// ------------------------

#define UART_READY_TO_TX (UCA0IFG & UCTXIFG)

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// ----------------------------
// -- System Setup Functions --
// ----------------------------

void StandardClockSetup_8Mhz_1Mhz();
// Setup Master Clock (MCLK) to 8Mhz
// Setup Subsystem Master Clock (SMCLK) to 1Mhz
// Uses the DCO

void StandardUARTSetup_9600_8();
// [Requires StandardClockSetup_8Mhz_1Mhz()]
// Setup UART0 at 9800 baud, 8 data bits, no parity, 1 stop bit

// --------------------
// -- Misc Functions --
// --------------------

void UART_TX_Char_BLOCKING(unsigned char c);
// Transmits a char through uart. Blocks until complete!

void UART_TX_String_BLOCKING(const unsigned char *str);
// Transmits a string through uart. Blocks until complete!

void DelayMillis_8Mhz(unsigned int millis);
void DelayMillis_1Mhz(unsigned int millis);

void DelaySeconds_8Mhz(unsigned int seconds);
void DelaySeconds_1Mhz(unsigned int seconds);

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// ----------------------------
// -- System Setup Functions --
// ----------------------------

void StandardClockSetup_8Mhz_1Mhz()
{
    // Setup Subsystem Master Clock (SMCLK) to 1Mhz
    CSCTL0 = CSKEY;                         // Set the clock password.  (L) pg. 80
    CSCTL1 |= DCOFSEL_3;                    // Set DCO Freq to 8 MHz.   (L) pg. 81
    CSCTL2 |= SELM__DCOCLK;                 // Set MCLK to run on DCO.  (L) pg. 82
    CSCTL2 |= SELS__DCOCLK;                 // Set SMCLK to run on DCO. (L) pg. 82
    CSCTL2 |= SELA__DCOCLK;                 // Set ACLK to run on DCO.  (L) pg. 82
    CSCTL3 &= ~(BIT0 | BIT1 | BIT2);        // MCLK divider of 1        (L) pg. 83

    CSCTL3 &= ~(BIT4 | BIT5 | BIT6);        // Clear relevant bits
    CSCTL3 |= DIVS__8;                      // SMCLK divider of 8       (L) pg. 83
}

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

// --------------------
// -- Misc Functions --
// --------------------

void UART_TX_Char_BLOCKING(unsigned char c)
{
    // Transmits a char through uart. Blocks until complete!
    while(!UART_READY_TO_TX);
    UCA0TXBUF = c; // Write to transmit buffer
}

void UART_TX_String_BLOCKING(const unsigned char *str)
{
    // Transmits a string through uart. Blocks until complete!
    while (*str != '\0') { // Continue until we reach the null terminator
        UART_TX_Char_BLOCKING(*str);
        str++; // Move to the next character in the string
    }
}

void DelayMillis_8Mhz(unsigned int millis)
{
    // Assumes 8Mhz clock cycle
    while(millis-- > 0)
    {
        __delay_cycles(8000);
    }
}

void DelayMillis_1Mhz(unsigned int millis)
{
    // Assumes 1Mhz clock cycle
    while(millis-- > 0)
    {
        __delay_cycles(1000);
    }
}

void DelaySeconds_8Mhz(unsigned int seconds)
{
    // Assumes 8Mhz clock cycle
    while(seconds-- > 0)
    {
        __delay_cycles(8000000);
    }
}

void DelaySeconds_1Mhz(unsigned int seconds)
{
    // Assumes 1Mhz clock cycle
    while(seconds-- > 0)
    {
        __delay_cycles(1000000);
    }
}






// End inclusion guard
#endif // RED_INCLUDED
