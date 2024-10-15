#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
// Exercise 4
// Felix Wilton
// Sep 26 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// SOME TAKEAWAYS:
// 16 bit registers are often split into two 8 bit registers
// These are called HIGH and LOW
// LOW is bits 0-7, and HIGH is bits 8-15
// I can't find a pattern for LOW and HIGH with the _1 and _0
int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	// --------------------------
	// ------- UART SETUP -------
	// --------------------------

	// Setup Subsystem Master Clock (SMCLK) to 1Mhz
    CSCTL0 = CSKEY;         // Set the clock password.  (L) pg. 80
    CSCTL1 |= DCOFSEL_3;    // Set DCO Freq to 8 MHz.   (L) pg. 81
    CSCTL2 |= SELM__DCOCLK; // Set MCLK to run on DCO.  (L) pg. 82
    CSCTL2 |= SELS__DCOCLK; // Set SMCLK to run on DCO. (L) pg. 82
    CSCTL3 |= DIVS__8;      // SMCLK divider of 8       (L) pg. 83

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

    // --------------------------
    // ----- END UART SETUP -----
    // --------------------------

    // LECTURE EXAMPLE CODE
    UCA0IE |= UCRXIE; // Enable RX interrupt
    _EINT(); // Global interrupt enable

    // Configure P3.6 (LED7) and P3.7 (LED8) as outputs
    // Using these for debug
    //      (M) page 81 and 82
    P3DIR |=   (BIT6 | BIT7);
    P3SEL1 &= ~(BIT6 | BIT7);
    P3SEL0 &= ~(BIT6 | BIT7);

    // Configure PJ.0 (LED1) as digital output
    //      (M) page 86
    PJDIR |=   (BIT0);
    PJSEL1 &= ~(BIT0);
    PJSEL0 &= ~(BIT0);

    // Turn off LED8 for debug visual
    P3OUT &= ~BIT7;

    // Turn on LED7 for debug visual
    P3OUT |= BIT6;

    // Inside the while loop, send 'a' periodically
    while (1) {
        // UCA0AFG is the register with the interrupt flags
        // UCTXIFG is the bit of the flag of interest
        // Hold until the flag is up
        //while (!(UCA0IFG & UCTXIFG));

        //UCA0TXBUF = 'a'; // Transmit the letter 'a'
        //__delay_cycles(600000);        // Delay before next transmission
    }

	return 0;
}

#pragma vector = USCI_A0_VECTOR
__interrupt void uart_echo_ISR(void)
{
    // Turn on LED8 for debug visual
    P3OUT |= BIT7;

    unsigned char RxByte = 0;
    RxByte = UCA0RXBUF; // Read from the receive buffer
    while (!(UCA0IFG & UCTXIFG)); // Hold until the "finish last transmission" flag is up
    UCA0TXBUF = RxByte; // Write to transmit buffer
    while (!(UCA0IFG & UCTXIFG)); // Hold until the "finish last transmission" flag is up
    UCA0TXBUF = RxByte + 1; // Write to transmit buffer

    if (RxByte == 'j')
    {
        // Turn on LED1
        PJOUT |= BIT0;
    } else if (RxByte == 'k')
    {
        // Turn off LED1
        PJOUT &= ~BIT0;
    }
}
