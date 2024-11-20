#include <msp430.h> 
#include "../BriefcaseLib/Green.c"
#include "../BriefcaseLib/Com.c"
#include "../BriefcaseLib/Encoder.c"

// Project Firmware - Green Board Version
// Felix Wilton & Lazar Stanojevic
// Nov 18 2024

// ~~~~~ This Firmware Uses ~~~~~
// Encoder:
//      P1.1 - CCW step pulse
//      P1.2 - CW step pulse
//      Timer A0 - CW counter
//      Timer A1 - CCW counter
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// UART Receive
MessagePacket IncomingPacket = EMPTY_MESSAGE_PACKET;
volatile PACKET_FRAGMENT NextRead = START_BYTE;

// UART Transmission
typedef enum TXSequence {
    ROTATION_1,
    ANALOG_SLIDERS,
    ROTATION_2,
    BINARIES
} TXSequence;
volatile TXSequence NextUpdate = ROTATION_1;

// Briefcase Data
volatile unsigned char Slider1 = 13;
volatile unsigned char Slider2 = 17;
volatile unsigned char StateMachine_State = 0;


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ UART Transmission Logic ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void TxBriefcaseState()
{
    // Build a packet
    MessagePacket TxPacket = EMPTY_MESSAGE_PACKET;

    // Put in values depending on NextUpdate
    if (NextUpdate == ROTATION_1 || NextUpdate == ROTATION_2)
    {
        unsigned int counts = 0;
        // Send rotation counts
        if (ENCODER_GetNetSteps_CW() > 0) {
            counts = ENCODER_GetNetSteps_CW();
            TxPacket.comm = ROT_CW;
        } else {
            counts = ENCODER_GetNetSteps_CCW();
            TxPacket.comm = ROT_CCW;
        }
        TxPacket.combined = counts;
        COM_SeperateDataBytes(&TxPacket);
        ENCODER_ClearEncoderCounts();
    }

    // Send the packet
    COM_CalculateEscapeByte(&TxPacket);
    COM_UART1_TransmitMessagePacket_BLOCKING(&TxPacket);

    // Increment next update
    if (NextUpdate >= BINARIES) {
        NextUpdate = ROTATION_1;
    } else {
        NextUpdate++;
    }
}



// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ UART Receive Logic ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void ProcessCompletePacket() {
    if (IncomingPacket.comm == GM_OVER) {
        // Game Over message
    } else if (IncomingPacket.comm == GM_STRT) {
        // Game Start message
    } else if (IncomingPacket.comm == MS_DEST) {
        // Enemy Destroyed message
    } else if (IncomingPacket.comm == MS_STRK) {
        // Enemy Strike message
    }
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

    //Encoder set-up
    ENCODER_SetupEncoder();

    __enable_interrupt();        // Enable global interrupts

    while(1)
    {
        DelayMillis_8Mhz(100);

        TxBriefcaseState();
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
