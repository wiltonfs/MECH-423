#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
// Exercise 7
// Felix Wilton
// Oct 4 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

typedef enum NEXT_VALUE {
    START,
    Xc,
    Yc,
    Zc
} NEXT_VALUE;

volatile unsigned char xAccel = 0;
volatile unsigned char yAccel = 0;
volatile unsigned char zAccel = 0;

volatile NEXT_VALUE nextTx  = START;
volatile NEXT_VALUE nextADC = START;

/**
 * main.c
 */
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);
    StandardUART0Setup_9600_8();

    /*

    // Configure P2.7 to output high to power the accelerometer
    P2DIR  |=  BIT7;            // P2.7 in Output mode  (M) pg. 74
    P2SEL1 &= ~BIT7;
    P2SEL0 &= ~BIT7;
    P2OUT  |=  BIT7;            // P2.7 output high

    // Set up the ADC
    // Set up its interrupt to trigger when conversion is complete
    ADC10CTL0 |= ADC10ON;       // ADC on               (L) pg. 449
    ADC10CTL0 &= ~ADC10ENC;     // Disable conversion   (L) pg. 450
    ADC10CTL1 |= ADC10SSEL_3;   // SMCLK source         (L) pg. 452
    ADC10CTL2 |= ADC10RES;      // 10-bit conversion    (L) pg. 453
    ADC10IE |= ADC10IE0;        // Enable interrupt     (L) pg. 458

    // Begin the first ADC conversion
    ADC10MCTL0 |= ADC10INCH_12; // Start with A12 (x)   (L) pg. 455
    ADC10CTL0 |= ADC10ENC;      // Enable conversion    (L) pg. 450
    ADC10CTL0 |= ADC10SC;       // Start ADC conversion (L) pg. 450

    // Set up a timer interrupt to trigger an interrupt every 40 ms (25Hz)
    // I changed to every 10 ms (100Hz) and just transmit one part of the data packet
    TimerB1Setup_UpCount_125kHz(1250); // 125000 / x = 100
    TB1CCTL0 |= CCIE;            // Enable interrupt         (L) pg. 375
    __enable_interrupt();        // Enable global interrupts
    */

    while(1)
    {
        // Heart beat to display general program progression
        DelayMillis_8Mhz(500);
        ToggleLED(BIT0);
    }

	return 0;
}

// Using the timer interrupt, transmit the result using the UART with 255 as the start byte
// The data packet should look like 255, X-axis, Y-axis, Z-axis
#pragma vector = TIMER0_B1_VECTOR
__interrupt void TIMER0_B1_ISR(void)
{
    TurnOnLED(BIT1); // Reached this ISR

    // Send the data packet through the UART
    while (!(UCA0IFG & UCTXIFG)); // Hold until the "finish last transmission" flag is up
    switch (nextTx) {
        case START:
            UCA0TXBUF = 255; // Write to transmit buffer
            nextTx++;
            break;
        case Xc:
            // Handle X case
            UCA0TXBUF = xAccel; // Write to transmit buffer
            nextTx++;
            break;
        case Yc:
            // Handle Y case
            UCA0TXBUF = yAccel; // Write to transmit buffer
            nextTx++;
            break;
        case Zc:
            // Handle Z case
            UCA0TXBUF = zAccel; // Write to transmit buffer
            nextTx = START;
            break;
        default:
            // Handle unexpected case
            break;
    }
}

// When the ADC has finished converting, save the value and begin the next one
#pragma vector = ADC10_VECTOR
__interrupt void ADC_ISR(void)
{
    TurnOnLED(BIT2); // Reached this ISR

    // Bit shift the 10-bit result to an 8-bit value
    unsigned char result = ADC10MEM0 >> 2;  // Read and shift 10-bit result to 8 bits

    // Need ADC10ENC = 0 so we can change channel           (L) pg. 455
    ADC10CTL0 &= ~ADC10ENC; // Disable conversion           (L) pg. 450

    if (nextADC == Xc)
    {
        xAccel = result;
        // Setup to read Y channel
        ADC10MCTL0 = ADC10INCH_13; // Process A13 (y)      (L) pg. 455
        nextADC = Yc;
    }
    else if (nextADC == Yc)
    {
        yAccel = result;
        // Setup to read Z channel
        ADC10MCTL0 = ADC10INCH_14; // Process A14 (z)      (L) pg. 455
        nextADC = Zc;
    }
    else if (nextADC == Zc)
    {
        zAccel = result;
        // Setup to read X channel
        ADC10MCTL0 = ADC10INCH_12; // Process A12 (x)      (L) pg. 455
        nextADC = Xc;
    }
    else
    {
        // Setup to read X channel
        ADC10MCTL0 = ADC10INCH_12; // Process A12 (x)      (L) pg. 455
        nextADC = Xc;
    }

    ADC10IFG  &= ~ADC10IFG0;    // Clear interrupt flag     (L) pg. 459
    ADC10CTL0 |= ADC10ENC;      // Re-enable conversion     (L) pg. 450
    ADC10CTL0 |= ADC10SC;       // Start ADC conversion     (L) pg. 450

}

#pragma vector = PORT1_VECTOR  // Change to the appropriate vector for your device
__interrupt void UNHANDLED_ISR(void)
{
    // Handle unexpected interrupts
    TurnOnLED(BIT7); // Indicate that an unexpected interrupt occurred
    DelayMillis_8Mhz(1); // Don't lose the ISR
}
