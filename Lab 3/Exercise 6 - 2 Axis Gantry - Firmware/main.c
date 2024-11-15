#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/Com.c"
#include "../MotorLib/DC.c"
#include "../MotorLib/Stepper.c"
#include "../MotorLib/Encoder.c"

// Lab 3 - Exercise 6
// Felix Wilton & Lazar Stanojevic
// Nov 01 2024

// ~~~~~ This Firmware Uses ~~~~~
// DC Motor:
//      P3.6 - CW control
//      P3.7 - CCW control
//      P2.1 - PWM speed control
//      Timer B2
// Stepper Motor:
//      P1.4 - Controls coil A2
//      P1.5 - Controls coil A1
//      P3.4 - Controls coil B2
//      P3.5 - Controls coil B1
//      Timer B0 - Controls coils A1 and A2
//      Timer B1 - Controls coils B1 and B2
// Encoder:
//      P1.1 - CCW step pulse
//      P1.2 - CW step pulse
//      Timer A0 - CW counter
//      Timer A1 - CCW counter
// Debug:
//      P1.6 - Heartbeat LED
#define HEARTBEAT_LED BIT6
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

MessagePacket IncomingPacket = EMPTY_MESSAGE_PACKET;
volatile PACKET_FRAGMENT NextRead = START_BYTE;

volatile bool isGantryRunning = true;
volatile bool isPCAwaitingResponse = false;

// Global target position. This comes in from the C# and is the macro path
volatile int DCM_globalTarget = 0;      // measured in counts
volatile int STP_globalTarget = 0;      // measured in steps
// Local target position. The global target is reached through small deltas, turned into local targets. This synchronizes the two motors.
volatile int DCM_localTarget = 0;       // measured in counts
volatile int STP_localTarget = 0;       // measured in steps
// Actual position. The stepper updates this after commanding a step, and the DC motor accumulates this from the encoder.
volatile int DCM_actualPosition = 0;    // measured in counts
volatile int STP_actualPosition = 0;    // measured in steps
// Speed values. Max DCM PWM, and minimum delay for a STP step.
unsigned int DCM_MaxPWM = 32000;
volatile unsigned int STP_SET_DELAY = 10;   // Hecto-microseconds

// DC Motor Kp parameters
#define MAX_ERROR 327
#define MIN_ERROR 1
#define Kp 500
#define DCM_MinPWM 9000

// Stepper motion data.
STEPPER_STATE stepper_state = A1_xx;
volatile unsigned int STP_delay = 100;       // Hecto-microseconds










// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ Stepper Motor Logic ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// Has a delay variable to regulate speed. If enough cycles have passed, increments a half step and resets the delay
void ControlGantry_STP()
{
    if (STP_delay == 0) {
        STP_delay = STP_SET_DELAY;

        int STP_error = STP_localTarget - STP_actualPosition;

        if (STP_error > 0)
        {
            IncrementHalfStep(&stepper_state, true);
            STP_actualPosition++;
        }
        else if (STP_error < 0)
        {
            IncrementHalfStep(&stepper_state, false);
            STP_actualPosition--;
        }

    } else {
        STP_delay--;
    }
}


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ DC Motor Logic ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


// Updates the current DCM position from the encoder counts
void DCM_UpdateCurrentPosition()
{
    if (ENCODER_GetNetSteps_CW() > 0) {
        DCM_actualPosition += ENCODER_GetNetSteps_CW();
    } else {
        DCM_actualPosition -= ENCODER_GetNetSteps_CCW();
    }
    ENCODER_ClearEncoderCounts();
}

// Absolute value of an error
unsigned int ErrorAbs(int error)
{
    unsigned int eAbs = 0;
    if (error >= 0)
    {
        eAbs = ((unsigned int)error);
    } else {
        eAbs = ((unsigned int)-error);
    }

    return eAbs;
}

// Proportional controller for DC motor based on error difference
void ControlGantry_DCM()
{
    DCM_UpdateCurrentPosition();

    int DCM_error = DCM_localTarget - DCM_actualPosition;
    unsigned int DCM_error_abs = ErrorAbs(DCM_error);
    if (DCM_error_abs > MAX_ERROR)
    {
        DCM_error_abs = MAX_ERROR;
    }

    if (DCM_error_abs < MIN_ERROR)
    {
        DC_Spin(0, CLOCKWISE);
        DC_Brake();
        return;
    }
    // Set up and perform 32-bit multiplication using the hardware multiplier
    MPY = Kp;               // Set operand 1 (lower 16 bits)
    MPY32CTL0 = MPYSAT;     // Turn on Saturation mode
    OP2 = DCM_error_abs;    // Set operand 2
    unsigned int DC_PWM = RESLO;         // Get the result of the multiplication.
    // Force 16 bit saturation mode
    if (RESHI > 0)
    {
        DC_PWM = DCM_MaxPWM;
    }

    // Clamp PWM between bounds
    if (DC_PWM < DCM_MinPWM)
    {
        DC_PWM = DCM_MinPWM;
    }
    if (DC_PWM > DCM_MaxPWM)
    {
        DC_PWM = DCM_MaxPWM;
    }

    // Spin direction based on error
    if (DCM_error > 0) {
        DC_Spin(DC_PWM, CLOCKWISE);
    } else {
        DC_Spin(DC_PWM, COUNTERCLOCKWISE);
    }
}


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ Overall Control Logic ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


// Local target has been reached, so move the local target closer to the global target
void UpdateLocalTargetTowardsGlobalTarget()
{
    // 245 encoder counts = 1 rev = 400 halfsteps
    // Roughly 2 halfsteps = 1 encoder count
#define MAX_STEPS_PER_SEGMENT   10
#define MAX_COUNTS_PER_SEGMENT  10


    // Stepper errors
    int STP_global_error = STP_globalTarget - STP_actualPosition;

    // DCM errors
    DCM_UpdateCurrentPosition();
    int DCM_global_error = DCM_globalTarget - DCM_actualPosition;
    unsigned int DCM_global_error_abs = ErrorAbs(DCM_global_error);

    // Remainders
    unsigned int DCM_remaining_counts = ErrorAbs(DCM_globalTarget - DCM_localTarget);
    unsigned int STP_remaining_steps  = ErrorAbs(STP_globalTarget - STP_localTarget);


    // ~~~~~ Case 1 ~~~~~
    // We have reached on one axis, so only have to move in the other axis
    if (STP_global_error == 0 || DCM_global_error_abs < MIN_ERROR)
    {
        STP_localTarget = STP_globalTarget;
        DCM_localTarget = DCM_globalTarget;
        return;
    }

    // ~~~~~ Case 2 ~~~~~
    // Local is close enough to global to just jump there next
    if (STP_remaining_steps <= MAX_STEPS_PER_SEGMENT || DCM_remaining_counts <= MAX_COUNTS_PER_SEGMENT)
    {
        STP_localTarget = STP_globalTarget;
        DCM_localTarget = DCM_globalTarget;
        return;
    }

    // ~~~~~ Case 3 ~~~~~
    // We still have a ways to go for both. Split a movement budget between the two axis
    unsigned char DCM_COUNTS = 3;
    unsigned char STP_STEPS = 3;

    // Needed slopes:
    // Distribute up to 21 counts
    //   x/y    =
    //   1/1    = 10/10
    //  -1/2    =  7/14
    //  12/7    = 12/7
    // -14/1    = 14/1
    //  -7/8    =  7/8

    // In units
    //  counts/step
    //  30/50   = 3/5   =  3/5  = 8     = 16
    // -30/100  = 3/10  =  3/10 = 13    = 13
    // 360/350  = 36/35 =  1/1  = 2     = 14
    // 420/50   = 42/5  =  8/1  = 9     = 18
    // 210/400  = 21/40 =  1/2  = 3     = 15

    // Sorted by magnitude
    // 8/1  = 8
    // 1/1  = 1
    // 6/10 = 0.6
    // 5/10 = 0.5
    // 3/10 = 0.3

    // Hardcode the cases
    if (DCM_globalTarget == 0 && STP_globalTarget > 0)
    {
        // P1 -> P2
        DCM_COUNTS = 3; STP_STEPS = 5;
    }
    else if (DCM_globalTarget < 0 && STP_globalTarget < 0)
    {
        // P2 -> P3
        DCM_COUNTS = 3; STP_STEPS = 10;
    }
    else if (DCM_globalTarget > 0 && STP_globalTarget > 0)
    {
        // P3 -> P4
        DCM_COUNTS = 20; STP_STEPS = 19;
    }
    else if (DCM_globalTarget < 0 && STP_globalTarget > 0)
    {
        // P4 -> P5
        DCM_COUNTS = 9; STP_STEPS = 1;
    }
    else if (DCM_globalTarget == 0 && STP_globalTarget < 0)
    {
        // P5 -> P6
        DCM_COUNTS = 10; STP_STEPS = 19;
    }


    // Apply the counts
    if (DCM_localTarget < DCM_globalTarget)
    {
        DCM_localTarget += DCM_COUNTS;
    } else {
        DCM_localTarget -= DCM_COUNTS;
    }
    // Apply the steps
    if (STP_localTarget < STP_globalTarget)
    {
        STP_localTarget += STP_STEPS;
    } else {
        STP_localTarget -= STP_STEPS;
    }

}

// Check if we've reached local or global targets yet
void GantryCheckReachedSetpoint()
{
    // Stepper errors
    int STP_global_error = STP_globalTarget - STP_actualPosition;
    int STP_local_error = STP_localTarget - STP_actualPosition;

    // DCM errors
    DCM_UpdateCurrentPosition();
    int DCM_global_error = DCM_globalTarget - DCM_actualPosition;
    int DCM_local_error = DCM_localTarget - DCM_actualPosition;
    unsigned int DCM_global_error_abs = ErrorAbs(DCM_global_error);
    unsigned int DCM_local_error_abs = ErrorAbs(DCM_local_error);


    // ~~~~~ Case 1 ~~~~~
    // If reached global setpoint, transmit success
    if (STP_global_error == 0 && DCM_global_error_abs < MIN_ERROR)
    {
        // Reached global setpoint

        // Align local to global, just in case
        STP_localTarget = STP_globalTarget;
        DCM_localTarget = DCM_globalTarget;
        // Stop DC motor, just in case
        DC_Spin(0, CLOCKWISE);
        DC_Brake();

        if (isPCAwaitingResponse) {
            COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(GAN_REACH_SETPOINT, 0, 0);
            isPCAwaitingResponse = false;
        }

        return;
    }


    // ~~~~~ Case 2 ~~~~~
    // Otherwise, if reached local setpoint, move local goal towards global setpoint
    if (STP_local_error == 0 && DCM_local_error_abs < MIN_ERROR) {

        UpdateLocalTargetTowardsGlobalTarget();

        return;
    }

}

void ZeroGantry()
{
    DC_Spin(0, CLOCKWISE);
    DC_Brake();
    ENCODER_ClearEncoderCounts();
    DCM_actualPosition = 0;
    STP_actualPosition = 0;
    DCM_globalTarget = 0;
    STP_globalTarget = 0;
    DCM_localTarget = 0;
    STP_localTarget = 0;
}


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ UART Logic ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void ProcessCompletePacket() {
    if (IncomingPacket.comm == DEBUG_ECHO_REQUEST) {
        COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(DEBUG_ECHO_RESPONSE, IncomingPacket.d1, IncomingPacket.d2);
        return;
    }
    if (IncomingPacket.comm == GAN_RESUME) {
        isPCAwaitingResponse = true;
        isGantryRunning = true;
        return;
    }
    if (IncomingPacket.comm == GAN_PAUSE) {
        isPCAwaitingResponse = false;
        isGantryRunning = false;
        return;
    }

    // Speed controls
    if (IncomingPacket.comm == GAN_SET_MAX_PWM_DC) {
        DCM_MaxPWM = IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_SET_DELAY_STP) {
        STP_SET_DELAY = IncomingPacket.combined;
        return;
    }

    // Absolute locations
    if (IncomingPacket.comm == GAN_ABS_POS_DC) {
        DCM_localTarget = DCM_actualPosition;
        DCM_globalTarget = IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_ABS_NEG_DC) {
        DCM_localTarget = DCM_actualPosition;
        DCM_globalTarget = -IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_ABS_POS_STP) {
        STP_localTarget = STP_actualPosition;
        STP_globalTarget = IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_ABS_NEG_STP) {
        STP_localTarget = STP_actualPosition;
        STP_globalTarget = -IncomingPacket.combined;
        return;
    }

    // Zero gantry
    if (IncomingPacket.comm == GAN_ZERO_SETPOINT) {
        ZeroGantry();
        return;
    }

    // Un-handled/un-implemented COMM byte, we should notify the PC
    COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(DEBUG_UNHANDLED_COMM, IncomingPacket.comm, 0);
}


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ Main ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer
    StandardClockSetup_8Mhz_1Mhz();
    StandardUART1Setup_9600_8();
    UCA1IE |= UCRXIE;           // Enable RX interrupt

    ZeroGantry();

    //DC Motor set-up
    DC_SetupDCMotor();

    //Stepper Motor set-up
    SetupStepperTimers();

    //Encoder set-up
    ENCODER_SetupEncoder();

    // Set up Debug LEDs    (M) pg. 73
    P1DIR  |=   (HEARTBEAT_LED);
    P1SEL1 &=~  (HEARTBEAT_LED);
    P1SEL0 &=~  (HEARTBEAT_LED);
    // Turn the LEDs off
    P1OUT  &=~  (HEARTBEAT_LED);

    __enable_interrupt();        // Enable global interrupts

    while(1)
    {
        DelayHectoMicros_8Mhz(1); // Delay 1/10 ms
        if (isGantryRunning)
        {
            ControlGantry_STP();
            ControlGantry_DCM();
            GantryCheckReachedSetpoint();
        } else {
            DC_Brake();
        }
        // Heartbeat
        P1OUT ^= HEARTBEAT_LED;
    }

    return 0;
}


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ UART Receive ISR ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


#pragma vector = USCI_A1_VECTOR
__interrupt void uart_ISR(void)
{
    if (UCA1IV == USCI_UART_UCRXIFG)    // Receive buffer full. (L) pg. 504
    {
        volatile unsigned char RxByte = UCA1RXBUF; // Read from the receive buffer

        if (COM_MessagePacketAssembly_StateMachine(&IncomingPacket, &NextRead, RxByte))
        {
            // returns True if the packet is now complete
            ProcessCompletePacket();
        }
    }
}
