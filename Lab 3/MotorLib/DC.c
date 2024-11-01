#include <msp430.h> 
// Functions for controlling the DC motor we developed in MECH 423, Lab 3
// Felix Wilton & Lazar Stanojevic
// Oct 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef DC_INCLUDED
#define DC_INCLUDED

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ DC Motor Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

#define CLOCKWISE 1
#define COUNTERCLOCKWISE 0

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void DC_SetupDCMotor();
// Requires StandardClockSetup_8Mhz_1Mhz();
// Sets up the pins and timers for the DC motor.
// P3.6 - CW control, P3.7 CCW control. P2.1 - PWM speed control.
// Uses Timer B2

void DC_Brake();
// Brakes the DC Motor by writing LOW to P3.6 and P3.7

void DC_Spin(unsigned int PWM, char direction);
// Spins the DC Motor in a given direction at a given PWM controlled speed.
// Max PWM is 65535, direction is either CLOCKWISE or COUNTERCLOCKWISE

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void DC_SetupDCMotor()
{
    // P3.6 - CW
    // P3.7 - CCW control
    // Use TIMER B2, CCR1, which outputs to pin 2.1
    // Since ACLK is at 8M Hz, overflow at ~122 Hz

    // Set up P3.6 and 3.7 as outputs
    P3DIR  |=   (BIT6 | BIT7);
    P3SEL1 &=~  (BIT6 | BIT7);
    P3SEL0 &=~  (BIT6 | BIT7);
    P3OUT  &=~  (BIT6 | BIT7);

    // Setup TIMER B2 in continuous mode
    TB2CTL |= TBCLR;            // Clear Timer B            (L) pg.372
    TB2CTL |= (BIT5);           // Continuous mode          (L) pg. 372
    TB2CTL |= TBSSEL__ACLK;     // Clock source select      (L) pg. 372

    // Setup PWM for Timer B2's CCR1
    TB2CCTL1 |= (BIT7 | BIT6 | BIT5);   // Reset/Set mode   (L) pg. 365, 366, and 375
    TB2CCR1 = 0;
    // Output TB2.1 on P2.1         (M) pg. 74
    P2DIR  |=  (BIT1);
    P2SEL1 &= ~(BIT1);
    P2SEL0 |=  (BIT1);
}

void DC_Brake()
{
    // Write P3.6 and P3.7 to LOW
    P3OUT  &=~  (BIT6 | BIT7);
}

void DC_Spin(unsigned int PWM, char direction)
{
    // Write the PWM to TIMER B2's CCR1
    TB2CCR1 = PWM;

    if (direction == CLOCKWISE)
    {
        // P3.7 off, P3.6 on
        P3OUT &=~ BIT7;
        P3OUT |=  BIT6;
        return;
    }
    // P3.6 off, P3.7 on
    P3OUT &=~ BIT6;
    P3OUT |=  BIT7;
    return;
}


// End inclusion guard
#endif // DC_INCLUDED
