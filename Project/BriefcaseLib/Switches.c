#include <msp430.h> 
// Standard functions for our MECH 423 project's switches (no interrupt binary inputs)
// Felix Wilton & Lazar Stanojevic
// Dec 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef SWITCHES_INCLUDED
#define SWITCHES_INCLUDED

// PJ Switches
#define PJ_SWITCHES (BATT_SEL_1 | BATT_SEL_2 | BATT_SEL_3 | CRUISE_MIS)
#define BATT_SEL_1 (BIT0)
#define BATT_SEL_2 (BIT1)
#define BATT_SEL_3 (BIT2)
#define CRUISE_MIS (BIT3)

// P3 Switches
#define P3_SWITCHES (FINE_ADJST | MOD_2_ENBL | THERM_CR_0 | THERM_CR_1 | MOD_3_ENBL)
#define FINE_ADJST (BIT2)
#define MOD_2_ENBL (BIT3)
#define THERM_CR_0 (BIT4)
#define THERM_CR_1 (BIT5)
#define MOD_3_ENBL (BIT6)


void SetupBinaryInputSwitches()
{
    // Pull down resistors are ~35 kilo Ohms (M) pg. 19

    // Configure switches as a digital input       (M) pg. 86, 
	PJDIR  &= ~(PJ_SWITCHES);
    PJSEL1 &= ~(PJ_SWITCHES);
    PJSEL0 &= ~(PJ_SWITCHES);
    // Configure switches as a digital input       (M) pg. 80, 81
    P3DIR  &= ~(P3_SWITCHES);
    P3SEL1 &= ~(P3_SWITCHES);
    P3SEL0 &= ~(P3_SWITCHES);

    // Switches will bridge to HIGH, so we need to pulldown when disconnected.
    PJOUT &= ~(PJ_SWITCHES);  // Pulldown             (L) pg. 314
    PJREN |= (PJ_SWITCHES);   // Enable resistors     (L) pg. 315
    P3OUT &= ~(P3_SWITCHES);  // Pulldown             (L) pg. 314
    P3REN |= (P3_SWITCHES);   // Enable resistors     (L) pg. 315
}

void ReadFromBinaryInputSwitches(volatile unsigned char* d1, volatile unsigned char* d2)
{
    // D1: first 4 bits are Battery selection 1-3 and Cruise missile switch
    *d1 |= (PJIN & PJ_SWITCHES);
    // D1: next 4 bits are fine adjustment, module 2 enable, and thermal correction bit 0 & 1
    *d1 |= ((P3IN & P3_SWITCHES) << 2);
    // D2: first bit is module 3 enable (bit 6)
    *d2 |= ((P3IN & MOD_3_ENBL) >> 6);
}




// End inclusion guard
#endif // SWITCHES_INCLUDED
