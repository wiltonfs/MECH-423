#include <msp430.h> 
// Standard functions for programming the MSP430 board in MECH 423
// Felix Wilton
// Sept, Oct 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef FSWLIB_INCLUDED
#define FSWLIB_INCLUDED

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

// ---------------------
// -- LED Definitions --
// ---------------------

#define LED1 BIT0
#define LED2 BIT1
#define LED3 BIT2
#define LED4 BIT3
#define LED5 BIT4
#define LED6 BIT5
#define LED7 BIT6
#define LED8 BIT7
#define ALL_LEDs 255

// ------------------------
// -- System Definitions --
// ------------------------

#define UART_READY_TO_TX (UCA0IFG & UCTXIFG)
#define NTC_PIN 4
#define xAccel_PIN 12
#define yAccel_PIN 13
#define zAccel_PIN 14


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


// --------------------------
// -- LED Output Functions --
// --------------------------

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

void BoolToLED(bool value, unsigned char selectionMask);
// [Make sure it has been setup with SetupLEDPins();]
// Ex - Display the value of UART_READY_TO_TX on LED1:  BoolToLED(UART_READY_TO_TX, LED1);

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

void StandardADCSetup();
// Setup the ADC with 10-bit conversion, 256 samples per conversion
// Also sets up the internal reference generator at 1.5V

void TurnOnAccelerometer();
// Power P2.7 to power the accelerometer

void TimerB1Setup_UpCount_125kHz(unsigned short upCountTarget);
// [Requires StandardClockSetup_8Mhz_1Mhz()]
// Setup TimerB1 in the "up count" mode at 125kHz
// upCountTarget of 249 =   ~500Hz interrupt
// upCountTarget of 100 = ~1.25kHz interrupt

void TimerB1_PWM(int channel, unsigned int dutyCycleCounter);
// [Requires StandardClockSetup_8Mhz_1Mhz() and TimerB1Setup_UpCount_125kHz()]
// Setup TB1.channel to produce a square wave, duty cycle = dutyCycleCounter/upCountTarget

// --------------------
// -- Misc Functions --
// --------------------

void UART_TX_Char_BLOCKING(unsigned char c);
// Transmits a char through uart. Blocks until complete!

void UART_TX_String_BLOCKING(const unsigned char *str);
// Transmits a string through uart. Blocks until complete!

void ADC_SampleFromAnalogChannel(int channel);
// [Requires StandardADCSetup()]
// Point the ADC at the given channel and start a conversion

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

void BoolToLED(bool value, unsigned char selectionMask)
{
    if (value) { TurnOnLED(selectionMask);}
    else {TurnOffLED(selectionMask);}
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

void StandardADCSetup()
{
    // Set up the Analog Digital Converter (ADC)
    ADC10CTL0 |= ADC10ON;       // ADC on                           (L) pg. 450
    ADC10CTL0 |= ADC10SHT_8;    // 256 samples per conversion       (L) pg. 449
    ADC10CTL1 |= ADC10CONSEQ_0; // 1 channel and 1 conversion       (L) pg. 452
    ADC10CTL1 |= ADC10SHP;      // Software initiated conversion    (L) pg. 451
    ADC10CTL2 |= ADC10RES;      // 10-bit conversion                (L) pg. 453
    ADC10MCTL0 |= ADC10SREF_1;  // Reference selection              (L) pg. 455

    // Set up internal reference generator
    while(REFCTL0 & REFGENBUSY);// If ref generator busy, WAIT      (L) pg. 431, pg. 429
    REFCTL0 |= REFVSEL_0;       // Select ref = 1.5V                (L) pg. 431
    REFCTL0 |= REFON;           // Turn on internal reference       (L) pg. 431
    __delay_cycles(400);        // Delay for ref to settle
}

void TurnOnAccelerometer()
{
    // Configure P2.7 to output high to power the accelerometer
    P2DIR  |=  BIT7;            // P2.7 in Output mode  (M) pg. 74
    P2SEL1 &= ~BIT7;
    P2SEL0 &= ~BIT7;
    P2OUT  |=  BIT7;            // P2.7 output high
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
void TimerB1_PWM(int channel, unsigned int dutyCycleCounter)
{
    if (channel == 1)
    {
        TB1CCTL1 = OUTMOD_7;        // Reset/Set mode           (L) pg. 365, 366, and 375
        TB1CCR1 = dutyCycleCounter;

        // Output TB1.1 on P3.4         (M) pg. 81
        P3DIR  |=  (BIT4);
        P3SEL1 &= ~(BIT4);
        P3SEL0 |=  (BIT4);
    }
    else
    {
        TB1CCTL2 = OUTMOD_7;        // Reset/Set mode           (L) pg. 365, 366, and 375
        TB1CCR2 = dutyCycleCounter;

        // Output TB1.2 on P3.5         (M) pg. 81
        P3DIR  |=  (BIT5);
        P3SEL1 &= ~(BIT5);
        P3SEL0 |=  (BIT5);
    }
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

void ADC_SampleFromAnalogChannel(int channel)
{
    // Need ADC10ENC = 0 so we can change channel           (L) pg. 455
    ADC10CTL0 &= ~ADC10ENC; // Disable conversion           (L) pg. 450

    // Clear the old analog channel selection               (L) pg. 455
    ADC10MCTL0 &= ADC10INCH_0;
    
    // Feed and start the ADC
    ADC10MCTL0 |= channel;  // ADC input ch             (L) pg. 455
    ADC10CTL0 |= ADC10ENC;  // Enable ADC conversion    (L) pg. 450
    ADC10CTL0 |= ADC10SC;   // Start conversion         (L) pg. 450
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
#endif // FSWLIB_INCLUDED