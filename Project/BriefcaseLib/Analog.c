#include <msp430.h> 
// Standard functions for analog inputs in our MECH 423 project
// Felix Wilton & Lazar Stanojevic
// Nov 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef ANALOG_INCLUDED
#define ANALOG_INCLUDED

#define SLIDER_1_CHANNEL 12
#define SLIDER_2_CHANNEL 13

void ADCSetup()
{
    // Set up the Analog Digital Converter (ADC)
    ADC10CTL0 &= ~ADC10ENC;     // Disable conversion               (L) pg. 450
    ADC10CTL0 |= ADC10ON;       // ADC on                           (L) pg. 450
    ADC10CTL0 |= ADC10SHT_8;    // 256 samples per conversion       (L) pg. 449
    ADC10CTL1 |= ADC10CONSEQ_0; // 1 channel and 1 conversion       (L) pg. 452
    ADC10CTL1 |= ADC10SHP;      // Software initiated conversion    (L) pg. 451
    ADC10CTL2 |= ADC10RES;      // 10-bit conversion                (L) pg. 453
}

ADC_ReadSlider(bool slider1)
{
    // Need ADC10ENC = 0 so we can change channel           (L) pg. 455
    ADC10CTL0 &= ~ADC10ENC; // Disable conversion           (L) pg. 450

    // Clear the old analog channel selection               (L) pg. 455
    ADC10MCTL0 &= ADC10INCH_0;
    
    // Feed and start the ADC
    if (slider1) {
        ADC10MCTL0 |= SLIDER_1_CHANNEL;  // ADC input ch             (L) pg. 455
    } else {
        ADC10MCTL0 |= SLIDER_2_CHANNEL;  // ADC input ch             (L) pg. 455
    }
    ADC10CTL0 |= ADC10ENC;  // Enable ADC conversion    (L) pg. 450
    ADC10CTL0 |= ADC10SC;   // Start conversion         (L) pg. 450
}

// End inclusion guard
#endif // ANALOG_INCLUDED
