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

// ------------------------------
// -- Escape Byte Definitions ---
// ------------------------------

#define ESC_COMMAND BIT0
#define ESC_DATA1   BIT1
#define ESC_DATA2   BIT2

// -------------------------------------
// -- Briefcase Data Mask Definitions --
// -------------------------------------
#define MODULE_3_ENABLE_MASK (BIT0)
#define STATE_MACHINE_MASK (BIT1 | BIT2 | BIT3 | BIT4 | BIT5 | BIT6)

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Declarations ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


void COM_UART_MakeAndTransmitMessagePacket_BLOCKING(unsigned char comm, unsigned char d1, unsigned char d2);

void COM_CalculateEscapeByte(volatile MessagePacket *MP);
// Requires the address of a target message packet
// Processes the COMM, D1, and D2 to 254 and sets the escape byte if any are 255

void COM_SeperateDataBytes(volatile MessagePacket *MP);
// Requires the address of a target message packet
// Seperates the combined field into d1 and d2


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~ Function Definitions ~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void COM_UART_MakeAndTransmitMessagePacket_BLOCKING(unsigned char comm, unsigned char d1, unsigned char d2)
{
    volatile MessagePacket MP = EMPTY_MESSAGE_PACKET;
    MP.comm = comm;
    MP.d1 = d1;
    MP.d2 = d2;

    COM_CalculateEscapeByte(&MP);
    UART_TX_Char_BLOCKING(255);
    UART_TX_Char_BLOCKING(MP.comm);
    UART_TX_Char_BLOCKING(MP.d1);
    UART_TX_Char_BLOCKING(MP.d2);
    UART_TX_Char_BLOCKING(MP.esc);
}

void COM_CalculateEscapeByte(volatile MessagePacket *MP)
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

void COM_SeperateDataBytes(volatile MessagePacket *MP)
{
    MP->d1 = (unsigned char)((MP->combined & 0xFF00) >> 8);
    MP->d2 = (unsigned char)(MP->combined & 0xFF);
}



// End inclusion guard
#endif // COM_INCLUDED
