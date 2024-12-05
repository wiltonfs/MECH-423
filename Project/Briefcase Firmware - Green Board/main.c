#include <msp430.h> 
#include "../BriefcaseLib/Green.c"
#include "../BriefcaseLib/General.c"
#include "../BriefcaseLib/Com.c"
#include "../BriefcaseLib/Encoder.c"
#include "../BriefcaseLib/Analog.c"
#include "../BriefcaseLib/StateMachine.c"
#include "../BriefcaseLib/Switches.c"
#include "../BriefcaseLib/OutputLEDs.c"

// Project Firmware - Green Board Version
// Felix Wilton & Lazar Stanojevic
// Dec 3 2024

// ~~~~~ This Firmware Uses ~~~~~
// General:
//      Timer B1 - UART TX timer
// UART:
//      P2.5 - UART TX
//      P2.6 - UART RX - Piped directly to printer
// Encoder:
//      P3.6 - CCW step pulse
//      P3.7 - CW step pulse
// Sliders:
//      P3.0 (A12) - Slider 1
//      P3.1 (A13) - Slider 2
// Buttons (Use Interrupts):
//      P2.2 - State machine button 1
//      P2.3 - State machine button 2
//      P2.4 - State machine button 3
//      P2.1 - State machine button 4
//      P2.0 - Launch button
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

// UART Transmission
typedef enum TXSequence {
    ROTATION_1,
    ANALOG_SLIDERS,
    ROTATION_2,
    BINARIES
} TXSequence;
volatile TXSequence NextUpdate = ROTATION_1;
volatile PACKET_FRAGMENT NextTxFragment = START_BYTE;
volatile MessagePacket TxPacket = EMPTY_MESSAGE_PACKET;

// ADC Reading
volatile bool readingSlider1 = true;

// Briefcase Data
volatile unsigned char Slider1 = 0;
volatile unsigned char Slider2 = 0;
volatile unsigned char StateMachine_State = 15;

// Buttons de-bouncing
#define DEBOUNCE_RESET 10
volatile unsigned char debounceTimer = 0; // * 5 millis

// Encoder de-bouncing
#define ENCODER_DEBOUNCE_RESET 2
volatile unsigned char encoderDebounceTimer = 0; // * 5 millis


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ UART Transmission Logic ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


void BuildNextPacket()
{

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

    // Increment next update
    if (NextUpdate >= BINARIES) {
        NextUpdate = ROTATION_1;
    } else {
        NextUpdate++;
    }
}

void TxBriefcaseState()
{
    // Back out if can't transmit yet
    if (!UART_READY_TO_TX) {
        return;
    }

    if (NextTxFragment == COM_BYTE) {
        UART_TX_BUFFER = TxPacket.comm;
    } else if (NextTxFragment == D1_BYTE) {
        UART_TX_BUFFER = TxPacket.d1;
    } else if (NextTxFragment == D2_BYTE) {
        UART_TX_BUFFER = TxPacket.d2;
    } else if (NextTxFragment == ESCP_BYTE) {
        UART_TX_BUFFER = TxPacket.esc;
    } else if (NextTxFragment == START_BYTE) {
        // Transmit the start byte
        UART_TX_BUFFER = 255;

        // Build the next packet
        BuildNextPacket();
    }

    // Increment next packet fragment
    if (NextTxFragment >= ESCP_BYTE) {
        NextTxFragment = START_BYTE;
    } else {
        NextTxFragment++;
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

    // Set up Timer B1 for UART TX every 10 ms
    TimerB1Setup_UpCount_125kHz(1250); // 125000 / x = 100 Hz
    TB1CCTL0 |= CCIE;            // Enable interrupt                (L) pg. 375

// Setting up INPUTS
    // Encoder set-up
    ENCODER_SetupEncoder();
    P3IE |= ENCODER_PINS;   // Re-enable encoder interrupts     (L) pg. 316

    // Analog set-up
    ADCSetup();
    ADC10IE |= ADC10IE0;        // Enable interrupt when ADC done   (L) pg. 458
    ADC_ReadSlider(readingSlider1);

    // Input buttons set-up
    SetupStateMachineAndLaunchButton();
    P2IE |= ALL_BUTTONS;        // Enable button interrupts     (L) pg. 316

    // Input switches set-up
    SetupBinaryInputSwitches();

// Setting up OUTPUTS
    // Output LEDs set-up (State machine LEDs, flashed LED)
    SetupOutputLEDs();


    __enable_interrupt();        // Enable global interrupts

    while(1)
    {
        DelayMillis_8Mhz(5);
        // Display the state on the LEDs
        WriteStateToLEDs(StateMachine_State);

        // Simple debouncing timer decrement. Decrement when no button is down
        if (debounceTimer > 0 && NO_BUTTON_DOWN) {
            debounceTimer--;
        } else {
            P2IE |= ALL_BUTTONS;    // Re-enable button interrupts     (L) pg. 316
        }

        // Simple encoder debouncing timer decrement
        if (encoderDebounceTimer > 0) {
            encoderDebounceTimer--;
        } else {
            P3IE |= ENCODER_PINS;   // Re-enable encoder interrupts     (L) pg. 316
        }
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
    } else {
        Slider2 = result;
    }

    readingSlider1 = !readingSlider1;
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
        // Clear button interrupt flags
        P2IFG &= ~(ALL_BUTTONS);
        return;
    }



    if (P2IFG & FIRE_BUTTON)// Check if interrupt is from P2.7 (Launch button) (L) pg. 317
    {
        // Fire a missile!
        COM_UART_MakeAndTransmitMessagePacket_BLOCKING(LAUNCH, 0, 0);
        P2IFG &= ~FIRE_BUTTON;  // Clear interrupt flag
        debounceTimer = DEBOUNCE_RESET; // Simple debouncing timer reset back up
        P2IE &= ~(ALL_BUTTONS);         // Disable button interrupts     (L) pg. 316
    }

    if (P2IFG & MACHINE_BUTTON_1)
    {
        IncrementStateMachine(&StateMachine_State, 1);
        P2IFG &= ~MACHINE_BUTTON_1;     // Clear interrupt flag
        debounceTimer = DEBOUNCE_RESET; // Simple debouncing timer reset back up
        P2IE &= ~(ALL_BUTTONS);         // Disable button interrupts     (L) pg. 316
    }
    if (P2IFG & MACHINE_BUTTON_2)
    {
        IncrementStateMachine(&StateMachine_State, 2);
        P2IFG &= ~MACHINE_BUTTON_2;     // Clear interrupt flag
        debounceTimer = DEBOUNCE_RESET; // Simple debouncing timer reset back up
        P2IE &= ~(ALL_BUTTONS);         // Disable button interrupts     (L) pg. 316
    }
    if (P2IFG & MACHINE_BUTTON_3)
    {
        IncrementStateMachine(&StateMachine_State, 3);
        P2IFG &= ~MACHINE_BUTTON_3;     // Clear interrupt flag
        debounceTimer = DEBOUNCE_RESET; // Simple debouncing timer reset back up
        P2IE &= ~(ALL_BUTTONS);         // Disable button interrupts     (L) pg. 316
    }
    if (P2IFG & MACHINE_BUTTON_4)
    {
        IncrementStateMachine(&StateMachine_State, 4);
        P2IFG &= ~MACHINE_BUTTON_4;     // Clear interrupt flag
        debounceTimer = DEBOUNCE_RESET; // Simple debouncing timer reset back up
        P2IE &= ~(ALL_BUTTONS);         // Disable button interrupts     (L) pg. 316
    }


}

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~ Encoder ISR ~~~~~~~~~~~~~~~~~~~~
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// Encoder pins (rising edge and falling edge)
#pragma vector=PORT3_VECTOR
__interrupt void Port_3(void)
{
    // Immediately extract the two pin values
    volatile unsigned char cw = (P3IN & CW_PIN);
    volatile unsigned char ccw = (P3IN & CCW_PIN);

    if (P3IFG & CW_PIN)
    {
        // CW Pin, Falling Edge

        // Confirm CW pin is low
        if (cw == 0) {
            // If other guy is still high, I'm leading. Otherwise, I'm lagging.
            if (ccw > 0) {
                // CW is leading, increment the CW
                ACCUMULATED_CW_STEPS++;
            } else {
                // CCW is leading, increment the CCW
                ACCUMULATED_CCW_STEPS++;
            }

            encoderDebounceTimer = ENCODER_DEBOUNCE_RESET; // Simple encoder debouncing timer reset back up
            P3IE &= ~ENCODER_PINS;   // Disable encoder interrupts     (L) pg. 316
        }

        P3IFG &= ~CW_PIN;  // Clear interrupt flag
    }

    if (P3IFG & CCW_PIN)
    {
        // CCW Pin, Rising Edge

        // Confirm CCW pin is high
        if (ccw > 0) {
            // If other guy is already high, I'm lagging. Otherwise, I'm leading.
            if (cw > 0) {
                // CW is leading, increment the CW
                ACCUMULATED_CW_STEPS++;
            } else {
                // CCW is leading, increment the CCW
                ACCUMULATED_CCW_STEPS++;
            }

            encoderDebounceTimer = ENCODER_DEBOUNCE_RESET; // Simple encoder debouncing timer reset back up
            P3IE &= ~ENCODER_PINS;   // Disable encoder interrupts     (L) pg. 316
        }

        P3IFG &= ~CCW_PIN;  // Clear interrupt flag
    }


}
