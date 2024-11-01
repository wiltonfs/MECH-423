#include <msp430.h> 
#include "General.c"
// Definitions for the messaging protocol we developed in MECH 423, Lab 3
// Felix Wilton & Lazar Stanojevic
// Oct 2024
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
#define DEBUG_ECHO_REQUEST      DEBUG_0
#define DEBUG_ECHO_RESPONSE     DEBUG_1
#define DEBUG_UNHANDLED_COMM    DEBUG_7

#define DCM_0 8             // Clockwise DC motor
#define DCM_1 (DCM_0 + 1)   // Counterclockwise DC motor
#define DCM_2 (DCM_0 + 2)   // Brake the DC motor
#define DCM_3 (DCM_0 + 3)
#define DCM_4 (DCM_0 + 4)
#define DCM_5 (DCM_0 + 5)
#define DCM_6 (DCM_0 + 6)
#define DCM_7 (DCM_0 + 7)
#define DCM_CW      DCM_0
#define DCM_CCW     DCM_1
#define DCM_BRAKE   DCM_2

#define STP_0 16            // Single step, clockwise direction
#define STP_1 (STP_0 + 1)   // Single step, counterclockwise direction
#define STP_2 (STP_0 + 2)   // Continuous stepping, clockwise direction
#define STP_3 (STP_0 + 3)   // Continuous stepping, counterclockwise direction
#define STP_4 (STP_0 + 4)   // Stop stepping
#define STP_5 (STP_0 + 5)
#define STP_6 (STP_0 + 6)
#define STP_7 (STP_0 + 7)
#define STP_SINGLE_CW      STP_0
#define STP_SINGLE_CCW     STP_1
#define STP_CONT_CW        STP_2
#define STP_CONT_CCW       STP_3
#define STP_STOP           STP_4

#define ENC_0 24            // Communicates a CW rotational delta. D1 and D2 combine to be an int of counts.
#define ENC_1 (ENC_0 + 1)   // Communicates a CCW rotational delta. D1 and D2 combine to be an int of counts.
#define ENC_2 (ENC_0 + 2)
#define ENC_3 (ENC_0 + 3)
#define ENC_4 (ENC_0 + 4)
#define ENC_5 (ENC_0 + 5)
#define ENC_6 (ENC_0 + 6)
#define ENC_7 (ENC_0 + 7)
#define ENC_ROT_DELTA_CW        ENC_0
#define ENC_ROT_DELTA_CCW       ENC_1

#define GAN_0 32
#define GAN_1 (GAN_0 + 1)
#define GAN_2 (GAN_0 + 2)
#define GAN_3 (GAN_0 + 3)
#define GAN_4 (GAN_0 + 4)
#define GAN_5 (GAN_0 + 5)
#define GAN_6 (GAN_0 + 6)
#define GAN_7 (GAN_0 + 7)

// ------------------------------
// -- Escape Byte Definitions ---
// ------------------------------

#define ESC_COMMAND BIT0
#define ESC_DATA1   BIT1
#define ESC_DATA2   BIT2

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

void COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(unsigned char comm, unsigned char d1, unsigned char d2);

void COM_UART1_TransmitMessagePacket_BLOCKING(MessagePacket *MP);
// Requires the address of a target message packet
// Transmits a complete message packet, blocks until complete

bool COM_UART1_TransmitMessagePacketFragment(MessagePacket *MP, volatile PACKET_FRAGMENT *ExpectedNextTransmit);
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

void COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(unsigned char comm, unsigned char d1, unsigned char d2)
{
    MessagePacket MP = EMPTY_MESSAGE_PACKET;
    MP.comm = comm;
    MP.d1 = d1;
    MP.d2 = d2;
    COM_UART1_TransmitMessagePacket_BLOCKING(&MP);
}

void COM_UART1_TransmitMessagePacket_BLOCKING(MessagePacket *MP)
{
    COM_CalculateEscapeByte(MP);
    UART_TX_Char_BLOCKING(255);
    UART_TX_Char_BLOCKING(MP->comm);
    UART_TX_Char_BLOCKING(MP->d1);
    UART_TX_Char_BLOCKING(MP->d2);
    UART_TX_Char_BLOCKING(MP->esc);
}

bool COM_UART1_TransmitMessagePacketFragment(MessagePacket *MP, volatile PACKET_FRAGMENT *ExpectedNextTransmit)
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
