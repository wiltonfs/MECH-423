#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/Com.c"
#include "../MotorLib/DC.c"
#include "../MotorLib/Encoder.c"

// Lab 3 - Exercise 5.6 - Used to characterize motor to a positional command
// Felix Wilton & Lazar Stanojevic
// Nov 08 2024

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
//      Timer B1 - High accuracy measurement and TX timer
// Debug:
//      P1.6 - Heartbeat LED
//      P1.7 - Measurement interrupt LED
#define HEARTBEAT_LED BIT6
#define MEASURE_IE_LED BIT7
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// Record position at very small time intervals, and then tx that after collecting the data

// Data collection
#define DATA_AMMOUNT 200
unsigned int DC_positions[DATA_AMMOUNT];
unsigned char nextDataPoint = 0;

MessagePacket IncomingPacket = EMPTY_MESSAGE_PACKET;
volatile PACKET_FRAGMENT NextRead = START_BYTE;
MessagePacket OutgoingPacket = EMPTY_MESSAGE_PACKET;

volatile long DC_currentPosition = 0;    // measured in steps
volatile long DC_targetPosition = 0;
volatile bool isGantryRunning = false;

// Controls parameters
const unsigned int Kp = 1000;              // PWM/step
const unsigned int maxError = 327;        // measured in steps
const unsigned int minError = 2;
const unsigned int minPWM = 9000;

void ZeroGantry_DC()
{
    DC_Brake();
    DC_currentPosition = 0;
    DC_targetPosition = 0;
    ENCODER_ClearEncoderCounts();
}

void UpdatePosition_DC()
{
    // Update current DC motor position
    if (ENCODER_GetNetSteps_CW() > 0) {
        DC_currentPosition += ENCODER_GetNetSteps_CW();
    } else {
        DC_currentPosition -= ENCODER_GetNetSteps_CCW();
    }
    // Clear the DC steps counter
    ENCODER_ClearEncoderCounts();
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

    UpdatePosition_DC();

    long DC_error = DC_targetPosition - DC_currentPosition;

    unsigned int error_Abs = ErrorAbs(DC_error);

    if (error_Abs <= minError)
    {
        DC_Spin(0, CLOCKWISE);
        DC_Brake();
        return;
    }

    // Set up and perform 32-bit multiplication using the hardware multiplier
    MPY = Kp;               // Set operand 1 (lower 16 bits)
    MPY32CTL0 = MPYSAT;     // Turn on Saturation mode
    OP2 = error_Abs;        // Set operand 2
    unsigned int DC_PWM = RESLO;         // Get the result of the multiplication.

    // Fake saturation mode
    if (RESHI > 0)
    {
        DC_PWM = 65535;
    }

    if (DC_PWM < minPWM)
    {
        DC_PWM = minPWM;
    }

    if (DC_error > minError) {
        DC_Spin(DC_PWM, CLOCKWISE);
    } else {
        DC_Spin(DC_PWM, COUNTERCLOCKWISE);
    }
}

void ProcessCompletePacket() {
    if (IncomingPacket.comm == DEBUG_ECHO_REQUEST) {
        COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(DEBUG_ECHO_RESPONSE, IncomingPacket.d1, IncomingPacket.d2);
        return;
    } else if (IncomingPacket.comm == MES_RESET) {
        isGantryRunning = false;
        ZeroGantry_DC();
        nextDataPoint = 0;
        return;
    } else if (IncomingPacket.comm == MES_REQ_POSITION) {
        ZeroGantry_DC();
        nextDataPoint = 0;
        // Clear measurement timer
        TB1CTL |= TBCLR;
        // Enable measurement timer interrupt
        TB1CCTL0 |= CCIE;
        // Start controls
        DC_targetPosition = IncomingPacket.combined;
        isGantryRunning = true;
        return;
    }


    // Unhandled COMM byte, notify of that
    COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(DEBUG_UNHANDLED_COMM, IncomingPacket.comm, 0);
}

void MeasurementTimerSetup()
{
    // Set up a timer (TB1) to interrupt every 40 ms (25 Hz)
    // The interrupt sends a packet fragment
    // So a full packet is sent every 40*5=200 ms (5 Hz)

    // Setup Timer B in the "up count" mode
    TB1CTL |= TBCLR;            // Clear Timer B            (L) pg.372
    TB1CTL |= (BIT4);           // Up mode                  (L) pg. 372
    TB1CTL |= TBSSEL__SMCLK;    // Clock source select      (L) pg. 372
    TB1CTL |= (BIT7 | BIT6);    // 1/8 divider (125 kHz)    (L) pg. 372
    //upCountTarget = 125, then 125000/125 = 1000 Hz
    TB1CCR0 = 625;             // What we count to         (L) pg. 377
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

    //Encoder set-up
    ENCODER_SetupEncoder();

    //Zero the system
    ZeroGantry_DC();

    MeasurementTimerSetup();
    //TB1CCTL0 |= CCIE;           // Enable interrupt   (L) pg. 375



    // Set up Debug LEDs    (M) pg. 73
    P1DIR  |=   (HEARTBEAT_LED | MEASURE_IE_LED);
    P1SEL1 &=~  (HEARTBEAT_LED | MEASURE_IE_LED);
    P1SEL0 &=~  (HEARTBEAT_LED | MEASURE_IE_LED);
    // Turn the LEDs off
    P1OUT  &=~  (HEARTBEAT_LED | MEASURE_IE_LED);

    __enable_interrupt();        // Enable global interrupts

    while(1)
    {
        DelayMillis_8Mhz(1);
        if (isGantryRunning) {
            ControlGantry_DC();
        }
        // Heartbeat
        P1OUT ^= HEARTBEAT_LED;
    }

    return 0;
}

#pragma vector = TIMER1_B0_VECTOR
__interrupt void data_timer_ISR(void){
    // Toggle ISR visual
    P1OUT ^= MEASURE_IE_LED;


    if (nextDataPoint < DATA_AMMOUNT)
    {
        // If in recording mode, record data
        DC_positions[nextDataPoint] = DC_currentPosition;
        nextDataPoint++;
    }
    else
    {
        unsigned char i = 0;
        // Else, transmit the collected data (BLOCKING)

        // Disable measurement timer interrupt
        TB1CCTL0 &=~ CCIE;
        // Stop motor
        DC_Brake();

        // Transmit the collected data (BLOCKING)
        for (i = 0; i < DATA_AMMOUNT; i++)
        {
            OutgoingPacket.comm = MES_RESPONSE;
            OutgoingPacket.combined = DC_positions[i];
            COM_SeperateDataBytes(&OutgoingPacket);
            COM_CalculateEscapeByte(&OutgoingPacket);
            COM_UART1_TransmitMessagePacket_BLOCKING(&OutgoingPacket);
            DelayMillis_8Mhz(10);
        }
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
