#include <msp430.h> 
#include "../../FSWLib/FSWLib.c"
// Lab Exam 2 - Q2
// Felix Wilton
// Oct 15 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]


/**
 * main.c
 */
volatile unsigned char xAccel = 'X';

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);
    StandardUART0Setup_9600_8();

    TurnOnAccelerometer();

    StandardADCSetup();
    ADC10IE |= ADC10IE0;        // Enable interrupt when ADC done   (L) pg. 458

    ADC_SampleFromAnalogChannel(12); // Start converting ch A12 (x)

    // Set up a timer interrupt to trigger an interrupt every 10 ms (100Hz)
    // I changed to every 10 ms and just transmit one part of the data packet at at time
    // Exercise instructions say every 40 ms and send all 4 parts of the packet
    TimerB1Setup_UpCount_125kHz(1250); // 125000 / x = 100
    TB1CCTL0 |= CCIE;            // Enable interrupt                (L) pg. 375
    __enable_interrupt();       // Enable global interrupts

    while(1)
    {
        DelayMillis_8Mhz(100);

        // Heart beat to display general program progression
        // If this stops, you are stuck in an interrupt
        ToggleLED(LED1);
    }

    return 0;
}

#pragma vector = TIMER1_B0_VECTOR
__interrupt void TIMER_B1_ISR(void)
{
    ToggleLED(LED5); // Reached this ISR

    UART_TX_Char_BLOCKING(xAccel);
}

// When the ADC has finished converting, save the value and begin the next one
#pragma vector = ADC10_VECTOR
__interrupt void ADC_ISR(void)
{
    unsigned char result = ADC10MEM0 >> 2;  // Read and shift 10-bit result to 8 bits
    xAccel = result;
    // Start reading the next channel. X = A12, Y = A13, Z = A14
    ADC_SampleFromAnalogChannel(12);
}
