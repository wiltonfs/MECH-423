#include <msp430.h> 
#include "General.c"
// Definitions for the encoder reading we developed in MECH 423, Lab 3
// Felix Wilton & Lazar Stanojevic
// Oct 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef ENCODER_INCLUDED
#define ENCODER_INCLUDED

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Encoder Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

#define CW_STEPS_SOURCE TA1R
#define CCW_STEPS_SOURCE TA0R

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void ENCODER_SetupEncoder();
// Sets up the pins and timers for the encoder.
// P1.1 - CW pulses, P1.2 - CCW pulses.
// Uses Timer A0 and Timer A1

void ENCODER_ClearEncoderCounts();
// Clears the encoder counts, stored in TA0R and TA1R

unsigned int ENCODER_GetNetSteps_CW();
// Returns the net number of steps in the clockwise direction, or zero if the net direction is counterclockwise

unsigned int ENCODER_GetNetSteps_CCW();
// Returns the net number of steps in the counterclockwise direction, or zero if the net direction is clockwise

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void ENCODER_SetupEncoder()
{
    // Set P1.1 & P1.2 as the clock signal for TA1 & TA0
    // Set them to TAxCLK mode, (M) pg. 70
    P1DIR  &=~ (BIT1 + BIT2);
    P1SEL1 |=  (BIT1 + BIT2);
    P1SEL0 &=~ (BIT1 + BIT2);

    // Setup Timer A0 in the "continuous count" mode
    TA0CTL |= TACLR;            // Clear Timer A0               (L) pg. 349
    TA0CTL |= MC__CONTINUOUS;   // Continuous mode              (L) pg. 349
    TA0CTL |= TASSEL__TACLK;    // Clock source: TAxCLK         (L) pg. 349

    // Setup Timer A1 in the "continuous count" mode
    TA1CTL |= TACLR;            // Clear Timer A1               (L) pg. 349
    TA1CTL |= MC__CONTINUOUS;   // Continous mode               (L) pg. 349
    TA1CTL |= TASSEL__TACLK;    // Clock source: TAxCLK         (L) pg. 349

    ENCODER_ClearEncoderCounts();
}

void ENCODER_ClearEncoderCounts()
{
    TA0CTL |= TACLR;            // Clear Timer A0       (L) pg. 349
    TA1CTL |= TACLR;            // Clear Timer A1       (L) pg. 349
}

unsigned int ENCODER_GetNetSteps_CW()
{
    if (CW_STEPS_SOURCE > CCW_STEPS_SOURCE)
        return CW_STEPS_SOURCE - CCW_STEPS_SOURCE;
    else
        return 0;
}

unsigned int ENCODER_GetNetSteps_CCW()
{
    if (CCW_STEPS_SOURCE > CW_STEPS_SOURCE)
        return CCW_STEPS_SOURCE - CW_STEPS_SOURCE;
    else
        return 0;
}



// End inclusion guard
#endif // ENCODER_INCLUDED
