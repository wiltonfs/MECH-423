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

// The state machine maxes out at 5 binary values, or 31 in decimal. Also, it can't be zero
void ClampStateMachine(volatile unsigned char* MachineState)
{
    if (*MachineState > 31)
    {
        *MachineState = 31;
    }
    if (*MachineState == 0)
    {
        *MachineState = 31;
    }

    // Illegal entries take you back to 31
    if (*MachineState == 1 || *MachineState == 2 || *MachineState == 4 || *MachineState == 8 || *MachineState == 16)
    {
        *MachineState = 31;
    }
}

void IncrementStateMachine(volatile unsigned char* MachineState, unsigned char Button)
{
    ClampStateMachine(MachineState); // Clamp just in case
    volatile unsigned char inputState = *MachineState;
    volatile unsigned char outputState = *MachineState;

    bool button_X = (Button == 1);
    bool button_Y = (Button == 2);
    bool button_Z = (Button == 3);
    bool button_T = (Button == 4);

    // Entry points to different state groups
    #define A_entry 5
    #define B_entry 6
    #define C_entry 29
    #define D_entry 25
    #define E_entry 14
    #define F_entry 3
    #define G_entry 31

    #define NO_CHANGE outputState

    // A Group - five states, [5, 10, 13, 17, 21]
    if (inputState == 5)
    {
        if (button_Z) {outputState = 10;}
    }
    else if (inputState == 10)
    {
        if (button_X){outputState = 17;}
        if (button_T){outputState = B_entry;}
    }
    else if (inputState == 13)
    {
        if (button_Y){outputState = 21;}
        if (button_T){outputState = 5;}
    }
    else if (inputState == 17)
    {
        if (button_Y){outputState = 5;}
        if (button_Z){outputState = 13;}
    }
    else if (inputState == 21)
    {
        if (button_X){outputState = 17;}
    }

    // B Group - four states, [6, 9, 12, 20]
    if (inputState == 6)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = 9;}
    }
    else if (inputState == 9)
    {
        if (button_X){outputState = 6;}
        if (button_Y){outputState = 12;}
        if (button_Z){outputState = 20;}
        if (button_T){outputState = C_entry;}
    }
    else if (inputState == 12)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = 9;}
    }
    else if (inputState == 20)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = 9;}
    }

    // C Group - three states, [27, 28, 29]
    if (inputState == 27)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = 29;}
    }
    else if (inputState == 28)
    {
        if (button_X){outputState = G_entry;}
        if (button_Y){outputState = 27;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = NO_CHANGE;}
    }
    else if (inputState == 29)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = 28;}
        if (button_T){outputState = NO_CHANGE;}
    }

    // D Group - three states, [23, 24, 25]
    if (inputState == 23)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = 24;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = A_entry;}
    }
    else if (inputState == 24)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = 23;}
        if (button_Z){outputState = 25;}
        if (button_T){outputState = A_entry;}
    }
    else if (inputState == 25)
    {
        if (button_X){outputState = 23;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = 24;}
        if (button_T){outputState = NO_CHANGE;}
    }

    // E Group - five states, [14, 18, 22, 26, 30]
    if (inputState == 14)
    {
        if (button_X){outputState = 22;}
        if (button_Y){outputState = G_entry;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = 26;}
    }
    else if (inputState == 18)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = 14;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = NO_CHANGE;}
    }
    else if (inputState == 22)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = 14;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = 30;}
    }
    else if (inputState == 26)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = 18;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = NO_CHANGE;}
    }
    else if (inputState == 30)
    {
        if (button_X){outputState = NO_CHANGE;}
        if (button_Y){outputState = 18;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = NO_CHANGE;}
    }

    // F Group - five states, [3, 7, 11, 15, 19]
    if (inputState == 3)
    {
        if (button_X){outputState = 11;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = 7;}
    }
    else if (inputState == 7)
    {
        if (button_X){outputState = G_entry;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = 3;}
        if (button_T){outputState = NO_CHANGE;}
    }
    else if (inputState == 11)
    {
        if (button_X){outputState = 15;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = NO_CHANGE;}
    }
    else if (inputState == 15)
    {
        if (button_X){outputState = 19;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = NO_CHANGE;}
    }
    else if (inputState == 19)
    {
        if (button_X){outputState = 3;}
        if (button_Y){outputState = NO_CHANGE;}
        if (button_Z){outputState = NO_CHANGE;}
        if (button_T){outputState = NO_CHANGE;}
    }

    // G Group - one state, [31]
    if (inputState == 31)
    {
        if (button_X){outputState = D_entry;}
        if (button_Y){outputState = C_entry;}
        if (button_Z){outputState = F_entry;}
        if (button_T){outputState = E_entry;}
    }

    *MachineState = outputState;
    ClampStateMachine(MachineState); // Clamp just in case
}



// End inclusion guard
#endif // STATEMACHINE_INCLUDED
