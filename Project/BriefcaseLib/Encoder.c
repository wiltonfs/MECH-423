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

#define CW_PIN  (BIT7)
#define CCW_PIN (BIT6)
#define ENCODER_PINS (CW_PIN | CCW_PIN)

#define CW_PIN_IS_HIGH  ((P3IN & CW_PIN)  > 0)
#define CCW_PIN_IS_HIGH ((P3IN & CCW_PIN) > 0)

volatile unsigned int ACCUMULATED_CW_STEPS = 0;  // P3.7
volatile unsigned int ACCUMULATED_CCW_STEPS = 0; // P3.6

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void ENCODER_SetupEncoder();
// Sets up the pins and interrupt for the encoder.

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
    // Set inputs with pull-down resistors

    // Configure both as a digital input       (M) pg. 82
    P3DIR  &= ~(ENCODER_PINS);
    P3SEL1 &= ~(ENCODER_PINS);
    P3SEL0 &= ~(ENCODER_PINS);
    // Pull-down resistors
    P3OUT &= ~(ENCODER_PINS);       // Pulldown             (L) pg. 314
    P3REN |=  (ENCODER_PINS);       // Enable resistors     (L) pg. 315
    

    // Interrupt from a falling edge
    P3IES |=  (CW_PIN);      // Falling edge detection (L) pg. 316
    // Interrupt from a rising edge
    P3IES &= ~(CCW_PIN);     // Rising edge detection (L) pg. 316

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
