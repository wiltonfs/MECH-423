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

volatile long DC_targetPosition = 100;     // measured in counts
volatile long DC_currentPosition = 0;    // measured in counts
volatile long STP_targetPosition = 400;    // measured in steps
volatile long STP_currentPosition = 0;   // measured in steps

const unsigned int DC_minError = 100;     // measured in steps
unsigned int DC_PWM = 32000;

STEPPER_STATE stepper_state = A1_xx;
volatile int STP_SET_DELAY = 10;   // Hecto-microseconds
volatile int STP_delay = 100;       // Hecto-microseconds

#define RESET_SETPOINT_TIMER 100
volatile int SETPOINT_TIMER = RESET_SETPOINT_TIMER;  // Hecto-microseconds

void GantryCheckReachedSetpoint()
{
    if (SETPOINT_TIMER == 0) {
        // Stable
        COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(GAN_REACH_SETPOINT, 0, 0);
        SETPOINT_TIMER = -1;
    } else {
        SETPOINT_TIMER--;
    }
}

void ZeroGantry()
{
    DC_Brake();
    DC_targetPosition = 0;
    DC_currentPosition = 0;
    STP_targetPosition = 0;
    STP_currentPosition = 0;
}

void ControlGantry_STP()
{
    if (STP_delay <= 0) {

        long STP_error = STP_targetPosition - STP_currentPosition;

        if (STP_error > 0)
        {
            IncrementHalfStep(&stepper_state, true);
            STP_currentPosition++;
            SETPOINT_TIMER = RESET_SETPOINT_TIMER; // If error, not at setpoint
        }
        else if (STP_error < 0)
        {
            IncrementHalfStep(&stepper_state, false);
            STP_currentPosition--;
            SETPOINT_TIMER = RESET_SETPOINT_TIMER; // If error, not at setpoint
        }


        STP_delay = STP_SET_DELAY;


    } else {
        STP_delay--;
    }
}

void ControlGantry_DC()
{
    // Update current DC motor position
    if (ENCODER_GetNetSteps_CW() > 0) {
        DC_currentPosition += ENCODER_GetNetSteps_CW();
    } else {
        DC_currentPosition -= ENCODER_GetNetSteps_CCW();
    }
    // Clear the DC steps counter
    ENCODER_ClearEncoderCounts();

    long DC_error = DC_targetPosition - DC_currentPosition;

    if (DC_error > 3) {
        DC_Spin(DC_PWM, CLOCKWISE);
        SETPOINT_TIMER = RESET_SETPOINT_TIMER; // If error, not at setpoint
        return;
    } else if (DC_error < -3) {
        DC_Spin(DC_PWM, COUNTERCLOCKWISE);
        SETPOINT_TIMER = RESET_SETPOINT_TIMER; // If error, not at setpoint
        return;
    }

    // Otherwise
    DC_Brake();
}

void ProcessCompletePacket() {
    if (IncomingPacket.comm == DEBUG_ECHO_REQUEST) {
        COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(DEBUG_ECHO_RESPONSE, IncomingPacket.d1, IncomingPacket.d2);
        return;
    }

    if (IncomingPacket.comm == GAN_RESUME) {
        isGantryRunning = true;
        return;
    }
    if (IncomingPacket.comm == GAN_PAUSE) {
        isGantryRunning = false;
        return;
    }



    // Relative locations
    if (IncomingPacket.comm == GAN_DELTA_POS_DC) {
        DC_targetPosition += IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_DELTA_NEG_DC) {
        DC_targetPosition -= IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_DELTA_POS_STP) {
        STP_targetPosition += IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_DELTA_NEG_STP) {
        STP_targetPosition -= IncomingPacket.combined;
        return;
    }

    // Speed controls
    if (IncomingPacket.comm == GAN_SET_MAX_PWM_DC) {
        DC_PWM = IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_SET_DELAY_STP) {
        STP_SET_DELAY = IncomingPacket.combined;
        return;
    }



    // Absolute locations
    if (IncomingPacket.comm == GAN_ABS_POS_DC) {
        DC_targetPosition = IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_ABS_NEG_DC) {
        DC_targetPosition = -IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_ABS_POS_STP) {
        STP_targetPosition = IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_ABS_NEG_STP) {
        STP_targetPosition = -IncomingPacket.combined;
        return;
    }


    // Zero gantry
    if (IncomingPacket.comm == GAN_ZERO_SETPOINT) {
        ZeroGantry();
        return;
    }


    // Unhandled COMM byte, notify of that
    COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(DEBUG_UNHANDLED_COMM, IncomingPacket.comm, 0);
}


/**
 * main.c
 */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer
    StandardClockSetup_8Mhz_1Mhz();
    StandardUART1Setup_9600_8();
    UCA1IE |= UCRXIE;           // Enable RX interrupt

    //ZeroGantry();

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
            ControlGantry_DC();
            GantryCheckReachedSetpoint();
        } else {
            DC_Brake();
        }
        // Heartbeat
        P1OUT ^= HEARTBEAT_LED;
    }

    return 0;
}

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
