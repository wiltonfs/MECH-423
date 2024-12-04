#include <msp430.h> 
// Definitions for the messaging protocol we developed in MECH 423 for our project
// Felix Wilton & Lazar Stanojevic
// Nov 2024
// (L) = MSP430 Fammily User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

// Inclusion guard
#ifndef COM_INCLUDED
#define COM_INCLUDED

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Communication Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// --------------------------------
// -- Message Packet Definitions --
// --------------------------------

typedef enum PACKET_FRAGMENT {
    START_BYTE,
    COM_BYTE,
    D1_BYTE,
    D2_BYTE,
    ESCP_BYTE
} PACKET_FRAGMENT;

typedef struct {
    volatile unsigned char comm;
    volatile unsigned char d1;
    volatile unsigned char d2;
    volatile unsigned char esc;
    volatile unsigned int combined;
} MessagePacket;

#define EMPTY_MESSAGE_PACKET { .comm = 0, .d1 = 0, .d2 = 0, .esc = 0, .combined = 0}

// ------------------------------
// -- Command Byte Definitions --
// ------------------------------

#define DEBUG_0 0
#define DEBUG_1 (DEBUG_0 + 1)
#define DEBUG_2 (DEBUG_0 + 2)
#define DEBUG_3 (DEBUG_0 + 3)
#define DEBUG_4 (DEBUG_0 + 4)
#define DEBUG_5 (DEBUG_0 + 5)
#define DEBUG_6 (DEBUG_0 + 6)
#define DEBUG_7 (DEBUG_0 + 7)

#define OUT_0 8             // Encoder clockwise
#define OUT_1 (OUT_0 + 1)   // Encoder counterclockwise
#define OUT_2 (OUT_0 + 2)   // Analog sliders 1 & 2
#define OUT_3 (OUT_0 + 3)   // All binary switches, discrete dial states, and state machine.
#define OUT_4 (OUT_0 + 4)   // Launch button pressed
#define ROT_CW      OUT_0
#define ROT_CCW     OUT_1
#define SLIDERS     OUT_2
#define BIN_INS     OUT_3
#define LAUNCH      OUT_4

#define INP_0 32            // Game Over message
#define INP_1 (INP_0 + 1)   // Game Start message
#define INP_2 (INP_0 + 2)   // Enemy Destroyed message
#define INP_3 (INP_0 + 3)   // Enemy Strike message
#define GM_OVER             INP_0
#define GM_STRT             INP_1
#define MS_DEST             INP_2
#define MS_STRK             INP_3

// ------------------------------
// -- Escape Byte Definitions ---
// ------------------------------

#define ESC_COMMAND BIT0
#define ESC_DATA1   BIT1
#define ESC_DATA2   BIT2

// -------------------------------------
// -- Briefcase Data Mask Definitions --
// -------------------------------------
// Data 1
//TODO
// Data 2
#define MODULE_3_ENABLE_MASK (BIT0)
#define STATE_MACHINE_MASK (BIT1 | BIT2 | BIT3 | BIT4 | BIT5 | BIT6)

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Receiving ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

bool COM_MessagePacketAssembly_StateMachine(MessagePacket *MP, volatile PACKET_FRAGMENT *ExpectedNextReadTracker, volatile unsigned char RxByte);
// Requires the address of a target message packet and the address of a PACKET_FRAGMENT that tracks the next expected fragment to read
// Puts the new byte into the right fragment of the message packet, and increments the expected next read
// Returns true if the Message Packet is now complete

void COM_ApplyEscapeByte(MessagePacket *MP);
// Requires the address of a target message packet
// Processes the escape byte to set COMM, D1, and D2 to 255 if needed

void COM_CombineDataBytes(MessagePacket *MP);
// Requires the address of a target message packet
// Combines D1 (byte) and D2 (byte) into combined (uint)

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Transmitting ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void COM_UART_MakeAndTransmitMessagePacket_BLOCKING(unsigned char comm, unsigned char d1, unsigned char d2);

void COM_UART_TransmitMessagePacket_BLOCKING(MessagePacket *MP);
// Requires the address of a target message packet
// Transmits a complete message packet, blocks until complete

bool COM_UART_TransmitMessagePacketFragment(MessagePacket *MP, volatile PACKET_FRAGMENT *ExpectedNextTransmit);
// Requires the address of a target message packet and the address of a PACKET_FRAGMENT that tracks the next expected fragment to transmit
// Returns true if just transmitted the start byte (255)

void COM_CalculateEscapeByte(MessagePacket *MP);
// Requires the address of a target message packet
// Processes the COMM, D1, and D2 to 254 and sets the escape byte if any are 255

void COM_SeperateDataBytes(MessagePacket *MP);
// Requires the address of a target message packet
// Seperates the combined field into d1 and d2



// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Receiving ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void COM_ApplyEscapeByte(MessagePacket *MP)
{
    // Use the escape byte to flag values up to 255
    if (MP->esc & ESC_COMMAND)
        MP->comm = 255;
    if (MP->esc & ESC_DATA1)
        MP->d1 = 255;
    if (MP->esc & ESC_DATA2)
        MP->d2 = 255;
}

void COM_CombineDataBytes(MessagePacket *MP)
{
    MP->combined = (MP->d1 << 8) | MP->d2;
}

bool COM_MessagePacketAssembly_StateMachine(MessagePacket *MP, volatile PACKET_FRAGMENT *ExpectedNextReadTracker, volatile unsigned char RxByte)
{
    bool completedPacket = false;

    if (RxByte == 255)
    {
        // Just got the start byte, next up should be the command byte
        *ExpectedNextReadTracker = COM_BYTE;
        return false;
    }

    switch(*ExpectedNextReadTracker) {
    case START_BYTE:
        break;
    case COM_BYTE:
        MP->comm = RxByte;
        break;
    case D1_BYTE:
        MP->d1 = RxByte;
        break;
    case D2_BYTE:
        MP->d2 = RxByte;
        break;
    case ESCP_BYTE:
        MP->esc = RxByte;
        COM_ApplyEscapeByte(MP);
        COM_CombineDataBytes(MP);
        completedPacket = true;
        break;
    default:
        break;
    }

    // Increment the expected next read
    if (*ExpectedNextReadTracker >= ESCP_BYTE)
        (*ExpectedNextReadTracker) = START_BYTE;
    else
        (*ExpectedNextReadTracker)++;

    return completedPacket; // Only true if just received the escape byte
}

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Transmitting ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void COM_UART_MakeAndTransmitMessagePacket_BLOCKING(unsigned char comm, unsigned char d1, unsigned char d2)
{
    MessagePacket MP = EMPTY_MESSAGE_PACKET;
    MP.comm = comm;
    MP.d1 = d1;
    MP.d2 = d2;
    COM_UART_TransmitMessagePacket_BLOCKING(&MP);
}

void COM_UART_TransmitMessagePacket_BLOCKING(MessagePacket *MP)
{
    COM_CalculateEscapeByte(MP);
    UART_TX_Char_BLOCKING(255);
    UART_TX_Char_BLOCKING(MP->comm);
    UART_TX_Char_BLOCKING(MP->d1);
    UART_TX_Char_BLOCKING(MP->d2);
    UART_TX_Char_BLOCKING(MP->esc);
}

bool COM_UART_TransmitMessagePacketFragment(MessagePacket *MP, volatile PACKET_FRAGMENT *ExpectedNextTransmit)
{
    bool newPacket = false;

    switch(*ExpectedNextTransmit) {
    case START_BYTE:
        UART_TX_Char_BLOCKING(255);
        newPacket = true;
        break;
    case COM_BYTE:
        UART_TX_Char_BLOCKING(MP->comm);
        break;
    case D1_BYTE:
        UART_TX_Char_BLOCKING(MP->d1);
        break;
    case D2_BYTE:
        UART_TX_Char_BLOCKING(MP->d2);
        break;
    case ESCP_BYTE:
        UART_TX_Char_BLOCKING(MP->esc);
        break;
    default:
        break;
    }

    // Increment the expected next transmit
    if (*ExpectedNextTransmit >= ESCP_BYTE)
        (*ExpectedNextTransmit) = START_BYTE;
    else
        (*ExpectedNextTransmit)++;

    return newPacket; // Only true if just transmitted the start byte
}

void COM_CalculateEscapeByte(MessagePacket *MP)
{
    MP->esc = 0;
    if (MP->comm == 255)
    {
        MP->comm = 254;
        MP->esc += BIT0;
    }
    if (MP->d1 == 255)
    {
        MP->d1 = 254;
        MP->esc += BIT1;
    }
    if (MP->d2 == 255)
    {
        MP->d2 = 254;
        MP->esc += BIT2;
    }
}

void COM_SeperateDataBytes(MessagePacket *MP)
{
    MP->d1 = (unsigned char)((MP->combined & 0xFF00) >> 8);
    MP->d2 = (unsigned char)(MP->combined & 0xFF);
}



// End inclusion guard
#endif // COM_INCLUDED
