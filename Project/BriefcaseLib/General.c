#include <msp430.h> 
// Standard functions for our MECH 423 project
// Felix Wilton & Lazar Stanojevic
// Dec 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef GENERAL_INCLUDED
#define GENERAL_INCLUDED

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

void TimerB1Setup_UpCount_125kHz(unsigned short upCountTarget);
// [Requires StandardClockSetup_8Mhz_1Mhz()]
// Setup TimerB1 in the "up count" mode at 125kHz
// upCountTarget of 249 =   ~500Hz interrupt
// upCountTarget of 100 = ~1.25kHz interrupt

// --------------------
// -- Misc Functions --
// --------------------

void UART_TX_Char_BLOCKING(unsigned char c);
// Transmits a char through uart. Blocks until complete!

void DelayMillis_8Mhz(unsigned int millis);

void DelaySeconds_8Mhz(unsigned int seconds);

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

void TimerB1Setup_UpCount_125kHz(unsigned short upCountTarget)
{
    // Setup Timer B in the "up count" mode
    TB1CTL |= TBCLR;            // Clear Timer B            (L) pg.372
    TB1CTL |= (BIT4);           // Up mode                  (L) pg. 372
    TB1CTL |= TBSSEL__SMCLK;    // Clock source select      (L) pg. 372
    TB1CTL |= (BIT7 | BIT6);    // 1/8 divider (125 kHz)    (L) pg. 372
    TB1CCR0 = upCountTarget;    // What we count to         (L) pg. 377
    // 65535 = ~1.9 Hz
    // 249 = 496 Hz
    // 247 = 500 Hz
    // 245 = 504 Hz
    // 10  = ~12.5 kHz
}

// --------------------
// -- Misc Functions --
// --------------------

void UART_TX_Char_BLOCKING(unsigned char c)
{
    // Transmits a char through uart. Blocks until complete!
    while(!UART_READY_TO_TX);
    UART_TX_BUFFER = c; // Write to transmit buffer
}

void DelayMillis_8Mhz(unsigned int millis)
{
    // Assumes 8Mhz clock cycle
    while(millis-- > 0)
    {
        __delay_cycles(8000);
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






// End inclusion guard
#endif // GENERAL_INCLUDED
