#include <msp430.h> 
#include "../BriefcaseLib/Red.c"
#include "../BriefcaseLib/Com.c"
#include "../BriefcaseLib/Encoder.c"

// Project Firmware - Red Board Version
// Felix Wilton & Lazar Stanojevic
// Nov 19 2024

// ~~~~~ This Firmware Uses ~~~~~
// General:
//      Timer B1 - UART TX timer
// UART:
//      P2.0 - UART TX
//      P2.1 - UART RX
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
volatile unsigned char Slider1 = 0;
volatile unsigned char Slider2 = 0;
volatile unsigned char StateMachine_State = 15;


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ UART Transmission Logic ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void TxBriefcaseState()
{
    // Back out if can't transmit yet
    if (!UART_READY_TO_TX)
        return;

    // Build a packet
    MessagePacket TxPacket = EMPTY_MESSAGE_PACKET;

    // Put in values depending on NextUpdate
    if (NextUpdate == ROTATION_1 || NextUpdate == ROTATION_2)
    {
        // Send rotation counts
        unsigned int counts = 0;
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
    else if (NextUpdate == ANALOG_SLIDERS)
    {
        // Send analog slider values
        TxPacket.comm = SLIDERS;
        TxPacket.d1 = Slider1;
        TxPacket.d2 = Slider2;
    }
    else if (NextUpdate == BINARIES)
    {
        // All binary switches, discrete dial states, and state machine.
        TxPacket.comm = BIN_INS;
        // TODO: Assemble data1
        TxPacket.d1 = 255;
        TxPacket.d2 = (StateMachine_State << 1) & STATE_MACHINE_MASK;
    }

    // Send the packet
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
    StandardUARTSetup_9600_8();
    UCA1IE |= UCRXIE;           // Enable RX interrupt

    // Encoder set-up
    ENCODER_SetupEncoder();

    // TODO: Setup all the input and output pins
    //          including pulldown/pullup, etc

    // Set up Timer B1 for UART TX every 10 ms
    TimerB1Setup_UpCount_125kHz(1250); // 125000 / x = 100 Hz
    TB1CCTL0 |= CCIE;            // Enable interrupt                (L) pg. 375


    __enable_interrupt();        // Enable global interrupts

    while(1)
    {
        DelayMillis_8Mhz(300);
        Slider1 += 10;
        Slider2++;
    }

    return 0;
}

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ Timer B1 ISR - UART TX ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

#pragma vector = TIMER1_B0_VECTOR
__interrupt void TIMER_B1_ISR(void)
{
    // Transmit next packet fragment
    TxBriefcaseState();
}


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ UART Receive ISR ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


#pragma vector = USCI_A1_VECTOR
__interrupt void uart_RX_ISR(void)
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
