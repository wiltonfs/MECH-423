#include <msp430.h> 
#include "../FSWLib/FSWLib.c"
// Exercise 6
// Felix Wilton
// Oct 3 2024

// Timer A is running 8x faster than timer B (No divider)
// Connect P3.4 to P1.0 and expect: 125*8=1000
// Connect P3.5 to P1.0 and expect: 62*8=496

volatile unsigned int risingEdgeTime = 0;
volatile unsigned int fallingEdgeTime = 0;
volatile unsigned int pulseWidth = 0;


/**
 * main.c
 */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    // -----------------------------------------
    // ---------- PWM From Exercise 5 ----------
    // -----------------------------------------

    StandardClockSetup_8Mhz_1Mhz();
    SetupLEDPins(ALL_LEDs);
    TurnOffLED(ALL_LEDs);

    TimerB1Setup_UpCount_500Hz();
    TimerB1_PWM(1, 125);         // 50% duty cycle
    TimerB1_PWM(2, 62);          // 25% duty cycle

    // Output TB1.1 on P3.4 and TB1.2 on P3.5                 (M) pg. 81
    P3DIR  |=  (BIT4 | BIT5);
    P3SEL1 &= ~(BIT4 | BIT5);
    P3SEL0 |=  (BIT4 | BIT5);


    // --------------------------------
    // ---------- Exercise 6 ----------
    // --------------------------------

    // [Set up timer A to measure the length of time of a pulse from a rising edge to a falling edge]
    // Find a pin that can be a CC Input for Timer A:
    //      (S) page 14 I see P1.0 can be TA0.1
    //      (M) page 70 tells me bits to connect these systems
    P1DIR  &= ~BIT0;             // Set P1.0 to TA0.CCI1A function   (M) pg. 70
    P1SEL1 &= ~BIT0;
    P1SEL0 |=  BIT0;

    // Set up Timer A0 [Using SMCLK so 1Mhz. Might need a divider if the pulse is long]
    TA0CTL |= TACLR;            // Clear Timer A            (L) pg. 349
    TA0CTL |= MC_2;             // Continuous mode          (L) pg. 349
    TA0CTL |= TASSEL__SMCLK;    // Clock source select      (L) pg. 349

    // Set Capture settings for Timer A0.1
    TA0CCTL1 |= CM_3;            // Capture on both edges    (L) pg. 351
    TA0CCTL1 |= CCIS_0;          // Select CCI0A as input    (L) pg. 351
    TA0CCTL1 |= SCS;             // Synchronize capture      (L) pg. 351, explained on (L) pg. 340
    TA0CCTL1 |= CAP;             // Turn on capture mode     (L) pg. 351
    TA0CCTL1 |= CCIE;            // Enable interrupt         (L) pg. 351

    __enable_interrupt();        // Enable global interrupts

    while (1)
    {
        // Display the measured 16-bit value

        // Heart beat to display general program progression
        DelayMillis_8Mhz(800);
        ToggleLED(BIT0); // Breakpoint here and look at pulseWidth

    }

    return 0;
}


#pragma vector = TIMER0_A1_VECTOR
__interrupt void TIMER0_A1_ISR(void)
{
    // Timer_Ax Interrupt Vector Register                   (L) pg. 353
    if (TA0IV == TA0IV_TACCR1)  // Check if interrupt is caused by capture on CCR1
    {
        // Timer_Ax Capture/Compare Control 1 Register      (L) pg. 351
        if (TA0CCTL1 & CCI)     // Check the actual input line. If 1, we just got the rising edge
        {
            // When a capture is performed, TAR was copied into TA0CCR1 (L) pg. 353
            risingEdgeTime = TA0CCR1;  // Store that time as the rising edge
        }
        else                    // Falling edge detected
        {
            // When a capture is performed, TAR was copied into TA0CCR1 (L) pg. 353
            fallingEdgeTime = TA0CCR1; // Store the time as the falling edge

            // I believe there could be an overflow issue here if the pulse bridges the top of Timer A. So I check:
            if (fallingEdgeTime > risingEdgeTime)
            {
                pulseWidth = fallingEdgeTime - risingEdgeTime;  // Calculate the pulse width
            }
            else if (fallingEdgeTime < risingEdgeTime)
            {
                pulseWidth = 65535 + fallingEdgeTime - risingEdgeTime;  // Calculate the pulse width
            }
        }
    }
}
