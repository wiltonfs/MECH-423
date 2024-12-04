#include <msp430.h> 
// Definitions for the encoder reading we developed in MECH 423 for our project
// Felix Wilton & Lazar Stanojevic
// Dec 2024
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

#define P2_ENCODER_PIN (BIT7)
#define P3_ENCODER_PIN (BIT7)

#define P2_ENCODER_PIN_IS_HIGH ((P2IN & P2_ENCODER_PIN) > 0)
#define P3_ENCODER_PIN_IS_HIGH ((P3IN & P3_ENCODER_PIN) > 0)

volatile unsigned int ACCUMULATED_CW_STEPS = 0;  // P3.7
volatile unsigned int ACCUMULATED_CCW_STEPS = 0; // P2.7

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void ENCODER_SetupEncoder();
// Sets up the pins and interrupt for the encoder.
// P3.7 - CCW pulses, P2.7 - CW pulses.

void ENCODER_ClearEncoderCounts();
// Clears the encoder counts, stored in two unsigned ints.

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
    // Set P2.7 and P3.7 as inputs with pull-down resistors

    // Configure both as a digital input       (M) pg. 78 and pg. 82
	P2DIR  &= ~(P2_ENCODER_PIN);
    P2SEL1 &= ~(P2_ENCODER_PIN);
    P2SEL0 &= ~(P2_ENCODER_PIN);
    P3DIR  &= ~(P3_ENCODER_PIN);
    P3SEL1 &= ~(P3_ENCODER_PIN);
    P3SEL0 &= ~(P3_ENCODER_PIN);
    // Pull-down resistors
    P2OUT &= ~(P2_ENCODER_PIN);         // Pulldown             (L) pg. 314
    P3OUT &= ~(P3_ENCODER_PIN);         // Pulldown             (L) pg. 314
    P2REN |= (P2_ENCODER_PIN);          // Enable resistors     (L) pg. 315
    P3REN |= (P3_ENCODER_PIN);          // Enable resistors     (L) pg. 315
    


    // P2.7 Interrupt from a falling edge
    P2IES |= (P2_ENCODER_PIN);      // Falling edge detection (L) pg. 316
    // P3.7 Interrupt from a rising edge
    P3IES &= ~(P3_ENCODER_PIN);    // Rising edge detection (L) pg. 316

    ENCODER_ClearEncoderCounts();
}

void ENCODER_ClearEncoderCounts()
{
    ACCUMULATED_CW_STEPS = 0;
    ACCUMULATED_CCW_STEPS = 0;
}

unsigned int ENCODER_GetNetSteps_CW()
{
    if (ACCUMULATED_CW_STEPS > ACCUMULATED_CCW_STEPS)
        return ACCUMULATED_CW_STEPS - ACCUMULATED_CCW_STEPS;
    else
        return 0;
}

unsigned int ENCODER_GetNetSteps_CCW()
{
    if (ACCUMULATED_CCW_STEPS > ACCUMULATED_CW_STEPS)
        return ACCUMULATED_CCW_STEPS - ACCUMULATED_CW_STEPS;
    else
        return 0;
}



// End inclusion guard
#endif // ENCODER_INCLUDED
