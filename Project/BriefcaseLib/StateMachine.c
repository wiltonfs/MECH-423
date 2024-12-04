#include <msp430.h> 
// Standard functions for our MECH 423 project's state machine
// Felix Wilton & Lazar Stanojevic
// Dec 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef STATEMACHINE_INCLUDED
#define STATEMACHINE_INCLUDED

#define STATE_MACHINE_BUTTONS (MACHINE_BUTTON_1 | MACHINE_BUTTON_2 | MACHINE_BUTTON_3 | MACHINE_BUTTON_4)
#define MACHINE_BUTTON_1 (BIT2)
#define MACHINE_BUTTON_2 (BIT3)
#define MACHINE_BUTTON_3 (BIT4)

// When buttons are down, they return 0.
#define BUTTON_REGISTER (P2IN)
#define ALL_BUTTONS (STATE_MACHINE_BUTTONS | FIRE_BUTTON)
#define NON_BUTTONS_TO_1 (BUTTON_REGISTER | ~(ALL_BUTTONS))
#define SOME_BUTTON_DOWN ((~(NON_BUTTONS_TO_1)) > 0)
#define NO_BUTTON_DOWN (!SOME_BUTTON_DOWN)

void SetupStateMachineAndLaunchButton()
{
    // Configure buttons as a digital input       (M) pg. 74, 76, 77, 78
	P2DIR  &= ~(ALL_BUTTONS);
    P2SEL1 &= ~(ALL_BUTTONS);
    P2SEL0 &= ~(ALL_BUTTONS);

    // Buttons will bridge to GND, so we need to pullup.
    P2OUT |= (ALL_BUTTONS);   // Pullup               (L) pg. 314
    P2REN |= (ALL_BUTTONS);   // Enable resistors     (L) pg. 315
    
    // Interrupt from a falling edge (user presses the button)
    P2IES |= (ALL_BUTTONS); // Falling edge detection (L) pg. 316
}

// The state machine maxes out at 5 binary values, or 31 in decimal
void ClampStateMachine(volatile unsigned char* MachineState)
{
    if (*MachineState > 31)
    {
        *MachineState = 31;
    }
}

void IncrementStateMachine(volatile unsigned char* MachineState, unsigned char Button)
{
    ClampStateMachine(MachineState); // Clamp just in case

    // Stand in switching functionality

    if (Button == 1)
    {
        *MachineState += 1;
    }
    else if (Button == 2)
    {
        *MachineState -= 1;
    }
    else if (Button == 3)
    {
        *MachineState += 10;
    }
    else if (Button == 4)
    {
        *MachineState -= 10;
    }

    ClampStateMachine(MachineState); // Clamp just in case
}









// End inclusion guard
#endif // STATEMACHINE_INCLUDED
