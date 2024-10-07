#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
// Exercise 8
// Felix Wilton
// Oct 6 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

volatile int currentTemperature = 100;

#define ROOM_TEMP 169
//#define MAX_TEMP 155
//const int tempStep = (ROOM_TEMP - MAX_TEMP) / 7;
const int tempStep = 2;

/**
 * main.c
 */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);
    TurnOnLED(LED1);
    StandardUART0Setup_9600_8();

    TurnOnAccelerometer();

    StandardADCSetup();
    ADC10IE |= ADC10IE0;        // Enable interrupt when ADC done   (L) pg. 458
    __enable_interrupt();       // Enable global interrupts

    ADC_SampleFromAnalogChannel(NTC_PIN); // Start converting ch A10 (NTC)

    while(1)
    {
        int i;
        int tempRemainder;
        DelayMillis_8Mhz(50);

        // Transmit temperature
        if (UART_READY_TO_TX)
        {
            unsigned char out = currentTemperature > 255 ? 255 : (unsigned char) currentTemperature;
            UCA0TXBUF = out; // Write to transmit buffer
        }

        // Display as digital thermometer
        tempRemainder = currentTemperature - ROOM_TEMP;
        for (i = 1; i < 8; i++)
        {
            if (tempRemainder <= 0)
            {
                TurnOnLED(1 << i);
            } else {
                TurnOffLED(1 << i);
            }
            tempRemainder += tempStep;
        }
    }
    return 0;
}

// When the ADC has finished converting, use the value and begin the next one
#pragma vector = ADC10_VECTOR
__interrupt void ADC_ISR(void)
{
    currentTemperature = ADC10MEM0;         // Read current temperature
    ADC_SampleFromAnalogChannel(NTC_PIN);   // Start reading the temperature again
}
