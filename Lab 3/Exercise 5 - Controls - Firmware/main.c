#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/Com.c"
#include "../MotorLib/DC.c"
#include "../MotorLib/Encoder.c"

// Lab 3 - Exercise 5
// Felix Wilton & Lazar Stanojevic
// Nov 01 2024

// ~~~~~ This Firmware Uses ~~~~~
// DC Motor:
//      P3.6 - CW control
//      P3.7 - CCW control
//      P2.1 - PWM speed control
//      Timer B2
// Encoder:
//      P1.1 - CCW step pulse
//      P1.2 - CW step pulse
//      Timer A0 - CW counter
//      Timer A1 - CCW counter
//      Timer B1 - High accuracy TX timer
// Debug:
//      P1.6 - Heartbeat LED
//      P1.7 - Velocity interrupt LED
#define HEARTBEAT_LED BIT6
#define VEL_IE_LED BIT7
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


MessagePacket txPacket = EMPTY_MESSAGE_PACKET;
PACKET_FRAGMENT nextTx = START_BYTE;

MessagePacket IncomingPacket = EMPTY_MESSAGE_PACKET;
volatile PACKET_FRAGMENT NextRead = START_BYTE;

long DC_targetPosition = 0;     // measured in steps
long DC_currentPosition = 0;    // measured in steps
const unsigned int Kp = 200;              // PWM/step
const unsigned int maxError = 327;        // measured in steps
const unsigned int minError = 2;
const unsigned int minPWM = 9000;

volatile unsigned int NetStepsCW = 0;
volatile unsigned int NetStepsCCW = 0;

void ZeroGantry_DC()
{
    DC_Brake();
    DC_targetPosition = 0;
    DC_currentPosition = 0;
}

unsigned int ErrorAbs(long error)
{
    unsigned long eAbs = 0;
    if (error >= 0)
    {
        eAbs = ((unsigned long)error);
    } else {
        eAbs = ((unsigned long)-error);
    }

    if (eAbs > maxError)
    {
        eAbs = maxError;
    }

    return (unsigned int)eAbs;
}

void ControlGantry_DC()
{

    // Update current DC motor position
    if (ENCODER_GetNetSteps_CW() > 0) {
        DC_currentPosition += ENCODER_GetNetSteps_CW();
        NetStepsCW += ENCODER_GetNetSteps_CW();
    } else {
        DC_currentPosition -= ENCODER_GetNetSteps_CCW();
        NetStepsCCW += ENCODER_GetNetSteps_CCW();
    }
    // Clear the DC steps counter
    ENCODER_ClearEncoderCounts();

    long DC_error = DC_targetPosition - DC_currentPosition;

    unsigned int error_Abs = ErrorAbs(DC_error);

    // Set up and perform 32-bit multiplication using the hardware multiplier
    MPY = Kp;               // Set operand 1 (lower 16 bits)
    MPY32CTL0 = MPYSAT;     // Turn on Saturation mode
    OP2 = error_Abs;        // Set operand 2
    unsigned int DC_PWM = RESLO;         // Get the result of the multiplication.

    if (DC_PWM < minPWM && error_Abs > minError)
    {
        DC_PWM = minPWM;
    }

    if (DC_error > minError) {
        DC_Spin(DC_PWM, CLOCKWISE);
    } else if (DC_error < (-minError)) {
        DC_Spin(DC_PWM, COUNTERCLOCKWISE);
    } else {
        DC_Spin(0, CLOCKWISE);
        DC_Brake();
    }
}

void ProcessCompletePacket() {
    if (IncomingPacket.comm == DEBUG_ECHO_REQUEST) {
        COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(DEBUG_ECHO_RESPONSE, IncomingPacket.d1, IncomingPacket.d2);
        return;
    } else if (IncomingPacket.comm == GAN_DELTA_POS_DC) {
        DC_targetPosition += IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_DELTA_NEG_DC) {
        DC_targetPosition -= IncomingPacket.combined;
        return;
    } else if (IncomingPacket.comm == GAN_ZERO_SETPOINT) {
        if (IncomingPacket.d1 > 0)
            ZeroGantry_DC();
        return;
    }


    // Unhandled COMM byte, notify of that
    COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(DEBUG_UNHANDLED_COMM, IncomingPacket.comm, 0);
}

void EncoderVelocityTimerSetup()
{
    // Set up a timer (TB1) to interrupt every 40 ms (25 Hz)
    // The interrupt sends a packet fragment
    // So a full packet is sent every 40*5=200 ms (5 Hz)

    // Setup Timer B in the "up count" mode
    TB1CTL |= TBCLR;            // Clear Timer B            (L) pg.372
    TB1CTL |= (BIT4);           // Up mode                  (L) pg. 372
    TB1CTL |= TBSSEL__SMCLK;    // Clock source select      (L) pg. 372
    TB1CTL |= (BIT7 | BIT6);    // 1/8 divider (125 kHz)    (L) pg. 372
    //upCountTarget = 5000, then 125000/5000= 25 Hz
    TB1CCR0 = 5000;             // What we count to         (L) pg. 377
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

    //DC Motor set-up
    DC_SetupDCMotor();
    //DC_Spin(32000, CLOCKWISE);  // 50% duty cycle, P2.1 output

    //Encoder set-up
    ENCODER_SetupEncoder();
    EncoderVelocityTimerSetup();
    TB1CCTL0 |= CCIE;           // Enable interrupt   (L) pg. 375

    //Zero the system
    ZeroGantry_DC();

    // Set up Debug LEDs    (M) pg. 73
    P1DIR  |=   (HEARTBEAT_LED | VEL_IE_LED);
    P1SEL1 &=~  (HEARTBEAT_LED | VEL_IE_LED);
    P1SEL0 &=~  (HEARTBEAT_LED | VEL_IE_LED);
    // Turn the LEDs off
    P1OUT  &=~  (HEARTBEAT_LED | VEL_IE_LED);

    __enable_interrupt();        // Enable global interrupts

    while(1)
    {
        DelayMillis_8Mhz(1);
        ControlGantry_DC();

        // Heartbeat
        P1OUT ^= HEARTBEAT_LED;
    }

    return 0;
}

#pragma vector = TIMER1_B0_VECTOR
__interrupt void transmission_timer_ISR(void){
    // Toggle ISR visual
    P1OUT ^= VEL_IE_LED;

    if (COM_UART1_TransmitMessagePacketFragment(&txPacket, &nextTx))
    {
        // Just sent 255, so build the next packet
        if (NetStepsCW > NetStepsCCW)
        {
            txPacket.comm = ENC_ROT_DELTA_CW;
            txPacket.combined = NetStepsCW - NetStepsCCW;
        } else {
            txPacket.comm = ENC_ROT_DELTA_CCW;
            txPacket.combined = NetStepsCCW - NetStepsCW;
        }

        // Clear the steps counter
        NetStepsCW = 0;
        NetStepsCCW = 0;

        // Split "combined" into d1 and d2, calculate the escape byte
        COM_SeperateDataBytes(&txPacket);
        COM_CalculateEscapeByte(&txPacket);
    }
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
