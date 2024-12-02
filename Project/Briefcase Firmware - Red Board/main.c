#include <msp430.h> 
#include "../BriefcaseLib/Red.c"
#include "../BriefcaseLib/Com.c"
#include "../BriefcaseLib/Encoder.c"
#include "../BriefcaseLib/Analog.c"
#include "../BriefcaseLib/StateMachine.c"
#include "../BriefcaseLib/Switches.c"
#include "../BriefcaseLib/OutputLEDs.c"

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
// Sliders:
//      P3.0 (A12) - Slider 1
//      P3.1 (A13) - Slider 2
// Buttons (Use Interrupts):
//      P2.2 - State machine button 1
//      P2.3 - State machine button 2
//      P2.4 - State machine button 3
//      P2.5 - State machine button 4
//      P2.6 - Launch button
// Switches:
//      PJ.0 - Battery selection 1 switch
//      PJ.1 - Battery selection 2 switch
//      PJ.2 - Battery selection 3 switch
//      PJ.3 - Cruise missile switch
//      P3.2 - Fine adjustment switch
//      P3.3 - Module 2 enable switch
//      P3.4 - Thermal correction bit 0 switch
//      P3.5 - Thermal correction bit 1 switch
//      P3.6 - Module 3 enable switch
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

// ADC Reading
volatile bool readingSlider1 = true;

// Briefcase Data
volatile unsigned char Slider1 = 0;
volatile unsigned char Slider2 = 0;
volatile unsigned char StateMachine_State = 15;

// Buttons de-bouncing
#define DEBOUNCE_RESET 10
volatile unsigned char debounceTimer = 0;


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
        TxPacket.d1 = 0;
        TxPacket.d2 = 0;

        ReadFromBinaryInputSwitches(&TxPacket.d1, &TxPacket.d2);

        TxPacket.d2 |= (StateMachine_State << 1) & STATE_MACHINE_MASK;
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
// Setting up SYSTEMS
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer
    StandardClockSetup_8Mhz_1Mhz();
    StandardUARTSetup_9600_8();
    UCA1IE |= UCRXIE;           // Enable RX interrupt

    // Set up Timer B1 for UART TX every 10 ms
    TimerB1Setup_UpCount_125kHz(1250); // 125000 / x = 100 Hz
    TB1CCTL0 |= CCIE;            // Enable interrupt                (L) pg. 375

// Setting up INPUTS
    // Encoder set-up
    ENCODER_SetupEncoder();

    // Analog set-up
    ADCSetup();
    ADC10IE |= ADC10IE0;        // Enable interrupt when ADC done   (L) pg. 458
    ADC_ReadSlider(readingSlider1);

    // Input buttons set-up
    SetupStateMachineAndLaunchButton();
    P2IE |=   (STATE_MACHINE_BUTTONS | FIRE_BUTTON);    // Enable button interrupts     (L) pg. 316

    // Input switches set-up
    SetupBinaryInputSwitches();

// Setting up OUTPUTS
    // Output LEDs set-up (State machine LEDs, flashed LED)
    SetupOutputLEDs();


    __enable_interrupt();        // Enable global interrupts

    while(1)
    {
        DelayMillis_8Mhz(10);

        // Simple debouncing timer decrement
        if (debounceTimer > 0)
            debounceTimer--;
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


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ ADC Complete ISR ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


// When the ADC has finished converting, save the value and begin the next one
#pragma vector = ADC10_VECTOR
__interrupt void ADC_ISR(void)
{
    unsigned char result = ADC10MEM0 >> 2;  // Read and shift 10-bit result to 8 bits

    if (readingSlider1)
    {
        Slider1 = result;
        readingSlider1 = false;
    } else {
        Slider2 = result;
        readingSlider1 = true;
    }

    ADC_ReadSlider(readingSlider1);
}


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ Button Inputs ISR ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


// State machine buttons or launch button
#pragma vector=PORT2_VECTOR
__interrupt void Port_2(void)
{
    // Simple debouncing timer check
    if (debounceTimer > 0) {
        // Clear all interrupt flags
        P2IFG &= 0;
        return;
    }



    if (P2IFG & FIRE_BUTTON)// Check if interrupt is from P2.7 (Launch button) (L) pg. 317
    {
        // Fire a missile!
        COM_UART1_MakeAndTransmitMessagePacket_BLOCKING(LAUNCH, 0, 0);
        P2IFG &= ~FIRE_BUTTON;  // Clear interrupt flag
    }

    if (P2IFG & MACHINE_BUTTON_1)
    {
        IncrementStateMachine(&StateMachine_State, 1);
        P2IFG &= ~MACHINE_BUTTON_1;     // Clear interrupt flag
    }
    if (P2IFG & MACHINE_BUTTON_2)
    {
        IncrementStateMachine(&StateMachine_State, 2);
        P2IFG &= ~MACHINE_BUTTON_2;     // Clear interrupt flag
    }
    if (P2IFG & MACHINE_BUTTON_3)
    {
        IncrementStateMachine(&StateMachine_State, 3);
        P2IFG &= ~MACHINE_BUTTON_3;     // Clear interrupt flag
    }
    if (P2IFG & MACHINE_BUTTON_4)
    {
        IncrementStateMachine(&StateMachine_State, 4);
        P2IFG &= ~MACHINE_BUTTON_4;     // Clear interrupt flag
    }

    WriteStateToLEDs(StateMachine_State);

    // Simple debouncing timer reset back up
    debounceTimer = DEBOUNCE_RESET;
}
