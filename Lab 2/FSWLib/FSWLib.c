#include <msp430.h> 
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


// --------------------------
// -- LED Output Functions --
// --------------------------

#define ALL_LEDs 255

void SetupLEDPins(unsigned char selectionMask);
// Ex - Setup LED 1:            SetupLEDPins(0b00000001);
// Ex - Setup LED 2, 4, 6, 8:   SetupLEDPins(0b10101010);

void WriteCharToLED(unsigned char displayedValue);
// Ex - Display 73:             WriteCharToLED(73);
// Equivalent to:               TurnOffLED(ALL_LEDs); TurnOnLED(0b01001001);

void TurnOnLED(unsigned char selectionMask);
// [Make sure it has been setup with SetupLEDPins();]
// Ex - Light LED 1:            TurnOnLED(0b00000001);
// Ex - Light LED 2, 4, 6, 8:   TurnOnLED(0b10101010);

void TurnOffLED(unsigned char selectionMask);
// [Make sure it has been setup with SetupLEDPins();]
// Ex - Extinguish LED 1:           TurnOffLED(0b00000001);
// Ex - Extinguish LED 2, 4, 6, 8:  TurnOffLED(0b10101010);

void ToggleLED(unsigned char selectionMask);
// [Make sure it has been setup with SetupLEDPins();]
// Ex - Toggle LED 1:           ToggleLED(0b00000001);
// Ex - Toggle LED 2, 4, 6, 8:  ToggleLED(0b10101010);

// ----------------------------
// -- System Setup Functions --
// ----------------------------

void StandardClockSetup_8Mhz_1Mhz();
// Setup Master Clock (MCLK) to 8Mhz
// Setup Subsystem Master Clock (SMCLK) to 1Mhz
// Uses the DCO

void StandardUART0Setup_9600_8();
// [Requires StandardClockSetup_8Mhz_1Mhz()]
// Setup UART0 at 9800 baud, 8 data bits, no parity, 1 stop bit

// --------------------
// -- Misc Functions --
// --------------------

void DelayMillis_8Mhz(unsigned int millis);
void DelayMillis_1Mhz(unsigned int millis);

void DelaySeconds_8Mhz(unsigned int seconds);
void DelaySeconds_1Mhz(unsigned int seconds);

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// --------------------------
// -- LED Output Functions --
// --------------------------

void SetupLEDPins(unsigned char selectionMask)
{
    // (S) page 17 to see the PINS to LEDs
    // PJ.0 = LED1, PJ.3 = LED4
    // P3.4 = LED5, P3.7 = LED8

    // Setup LED1 - LED4
    // Configure PJ.0, PJ.1, PJ.2, and PJ.3 as digital outputs
    // (M) page 86
    PJDIR |=   (selectionMask & (BIT0 | BIT1 | BIT2 | BIT3));
    PJSEL1 &= ~(selectionMask & (BIT0 | BIT1 | BIT2 | BIT3));
    PJSEL0 &= ~(selectionMask & (BIT0 | BIT1 | BIT2 | BIT3));

	// Setup LED5 - LED8
    // Configure P3.4, P3.5, P3.6, and P3.7 as digital outputs
	// (M) page 81, 82
	P3DIR |=   (selectionMask & (BIT4 | BIT5 | BIT6 | BIT7));
	P3SEL1 &= ~(selectionMask & (BIT4 | BIT5 | BIT6 | BIT7));
	P3SEL0 &= ~(selectionMask & (BIT4 | BIT5 | BIT6 | BIT7));
}

void WriteCharToLED(unsigned char displayedValue)
{
    TurnOffLED(ALL_LEDs);
    TurnOnLED(displayedValue);
}

void TurnOnLED(unsigned char selectionMask)
{
    PJOUT |= (selectionMask & (BIT0 | BIT1 | BIT2 | BIT3));
    P3OUT |= (selectionMask & (BIT4 | BIT5 | BIT6 | BIT7));
}

void TurnOffLED(unsigned char selectionMask)
{
    PJOUT &= ~(selectionMask & (BIT0 | BIT1 | BIT2 | BIT3));
    P3OUT &= ~(selectionMask & (BIT4 | BIT5 | BIT6 | BIT7));
}

void ToggleLED(unsigned char selectionMask)
{
    PJOUT ^= (selectionMask & (BIT0 | BIT1 | BIT2 | BIT3));
    P3OUT ^= (selectionMask & (BIT4 | BIT5 | BIT6 | BIT7));
}

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
    CSCTL3 &= ~(BIT0 | BIT1 | BIT2);        // MCLK divider of 1        (L) pg. 83

    CSCTL3 &= ~(BIT4 | BIT5 | BIT6);        // Clear relevant bits
    CSCTL3 |= DIVS__8;                      // SMCLK divider of 8       (L) pg. 83
}

void StandardUART0Setup_9600_8()
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

void DelaySeconds_1Mhz(unsigned int seconds)
{
    // Assumes 1Mhz clock cycle
    while(seconds-- > 0)
    {
        __delay_cycles(1000000);
    }
}

