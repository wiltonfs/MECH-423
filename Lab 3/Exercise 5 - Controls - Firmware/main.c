#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/Com.c"
#include "../MotorLib/DC.c"
#include "../MotorLib/Stepper.c"
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
// Debug:
//      P1.6 - Heartbeat LED
//      P1.7 - Velocity interrupt LED
#define HEARTBEAT_LED BIT6
#define VEL_IE_LED BIT7
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

MessagePacket IncomingPacket = EMPTY_MESSAGE_PACKET;
volatile PACKET_FRAGMENT NextRead = START_BYTE;

void ProcessCompletePacket() {
    if (IncomingPacket.comm == DEBUG_ECHO_REQUEST) {
        COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(DEBUG_ECHO_RESPONSE, IncomingPacket.d1, IncomingPacket.d2);
        return;
    } else if (IncomingPacket.comm == DCM_CW) {
        DC_Spin(IncomingPacket.combined, CLOCKWISE);
        return;
    } else if (IncomingPacket.comm == DCM_CCW) {
        DC_Spin(IncomingPacket.combined, COUNTERCLOCKWISE);
        return;
    } else if (IncomingPacket.comm == DCM_BRAKE) {
        DC_Brake();
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

    //DC Motor set-up
    DC_SetupDCMotor();
    DC_Spin(32000, CLOCKWISE);  // 50% duty cycle, P2.1 output

    //Encoder set-up
    ENCODER_SetupEncoder();

    // Set up Debug LEDs    (M) pg. 73
    P1DIR  |=   (HEARTBEAT_LED | VEL_IE_LED);
    P1SEL1 &=~  (HEARTBEAT_LED | VEL_IE_LED);
    P1SEL0 &=~  (HEARTBEAT_LED | VEL_IE_LED);
    // Turn the LEDs off
    P1OUT  &=~  (HEARTBEAT_LED | VEL_IE_LED);

    __enable_interrupt();        // Enable global interrupts

    while(1)
    {
        DelayMillis_8Mhz(200);

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
