#include <msp430.h> 
#include "../../FSWLib/FSWLib.c"
// Lab Exam 2 - Q3
// Felix Wilton
// Oct 15 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]


/**
 * main.c
 */

volatile unsigned short falling = 1;

volatile unsigned int startTime = 0;
volatile unsigned int endTime = 0;
volatile unsigned int pulseWidth = 0;

volatile unsigned int overflows = 0;


int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);

    StandardUART0Setup_9600_8();

    EnableSwitches(SWITCH_1 | SWITCH_2);
    P4IES |= (SWITCH_1 | SWITCH_2); // Falling edge detection (L) pg. 316
    falling = 1;

    P4IE |=   (SWITCH_1);    // Enable switch interrupts     (L) pg. 316
    __enable_interrupt();       // Enable global interrupts

    // Timer for timing length
    TimerB1Setup_UpCount_125kHz(65534);

    while(1)
    {
        DelayMillis_8Mhz(524);

        // Heart beat to display general program progression
        // If this stops, you are stuck in an interrupt
        ToggleLED(LED1);
        overflows++;
    }

    return 0;
}

// ISR to toggle LED7 and LED8 after receiving rising edges from S1 and S2 respectively
#pragma vector=PORT4_VECTOR
__interrupt void Port_4(void)
{
    TurnOnLED(LED2);
    P4IFG &= ~BIT0;     // Clear interrupt flag for P4.0
    P4IFG &= ~BIT1;     // Clear interrupt flag for P4.1


    unsigned long i = 0;

    if (falling > 0) {
        TurnOnLED(LED3);
        // Just got a falling edge (button pressed)
        P4IES &= ~(SWITCH_1 | SWITCH_2); // Rising edge detection (L) pg. 316
        falling = 0;

        TB1CTL |= TBCLR;            // Clear Timer B            (L) pg.372
        overflows = 0;

    } else {
        TurnOnLED(LED4);
        // Just got a rising edge (button released)
        P4IES |= (SWITCH_1 | SWITCH_2); // Falling edge detection (L) pg. 316
        falling = 1;

        endTime = TB1R;

        volatile unsigned char BROADCAST = 0;

        for (i = 0; i < overflows; i++)
        {
            BROADCAST += 67;
        }

        for (i = 0; i < endTime; i += 1000)
        {
            BROADCAST += 1;
        }




        if (overflows >= 4)
        {
            BROADCAST = 255;
        }

        // Broadcast the number
        UART_TX_Char_BLOCKING(BROADCAST);

    }

}

