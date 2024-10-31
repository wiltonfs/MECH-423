#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/Com.c"

/**
 * main.c
 */
unsigned int CWsteps = 0;
unsigned int CCWsteps = 0;
#define CW_STEPS_SOURCE TA1R
#define CCW_STEPS_SOURCE TA0R

MessagePacket txPacket = EMPTY_MESSAGE_PACKET;
PACKET_FRAGMENT nextTx = START_BYTE;

void EncoderTimerSetup(void)
{
    //initialize P1.1 & P1.2 &  to primary capture mode (TA0 & TA1)
    P1SEL0 |= (BIT1 + BIT2);
    P1SEL1 &= ~(BIT1 + BIT2);
    P1DIR &= ~(BIT1 + BIT2);

    // Setup Timer A0 in the "continous count" mode
    TA0CTL |= TACLR;            // Clear Timer A                (L) pg. 349
    TA0CTL |= MC_2;             // Continous mode               (L) pg. 349
    TA0CTL |= TASSEL__TACLK;    // Clock source select          (L) pg. 349

    // Setup Timer A1 in the "continous count" mode
    TA1CTL |= TACLR;            // Clear Timer A                (L) pg. 349
    TA1CTL |= MC_2;             // Continous mode               (L) pg. 349
    TA1CTL |= TASSEL__TACLK;    // Clock source select          (L) pg. 349
}

void ClearEncoderCounts(void)
{
    TA1CTL |= TACLR;            // Clear Timer A1                (L) pg. 349
    TA0CTL |= TACLR;            // Clear Timer A0                (L) pg. 349
}

void DCMotor_TimerB2Setup(){

    int upCountTarget = 65535;

    //initialize pins 2.1 to primary mode (TB2.1)
    P2SEL0 |= BIT1;
    P2SEL1 &= ~BIT1;
    P2DIR |= BIT1;

    // Setup Timer B in the "up count" mode
    TB2CTL |= TBCLR;             // Clear Timer B            (L) pg.372
    TB2CTL |= (BIT4);            // Up mode                  (L) pg. 372
    TB2CTL |= TBSSEL__ACLK;      // Clock source select      (L) pg. 372
    TB2CTL |= ~(BIT7 | BIT6);    // No divider               (L) pg. 372
    TB2CCR0 = upCountTarget;     // What we count to         (L) pg. 377
}

int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	StandardClockSetup_8Mhz_1Mhz();
    StandardUART1Setup_9600_8();
    EncoderTimerSetup();

    // Set up P3.6, P3.7, and P1.6 as outputs
    P3DIR |= (BIT6 | BIT7);
    P3SEL1 &= ~(BIT6 | BIT7);
    P3SEL0 &= ~(BIT6 | BIT7);
    P1DIR |= BIT6;
    P1SEL1 &= ~BIT6;
    P1SEL0 &= ~BIT6;

    // Turn the LEDs off
    P3OUT &= ~(BIT6 | BIT7);
    P1OUT &= ~BIT6;

    //DC Motor set-up
    DCMotor_TimerB2Setup();

    TimerB2_PWM(1, 32767);         // 50% duty cycle, P3.4 output

    while(1)
    {
        DelayMillis_8Mhz(50);

        P3OUT ^= (BIT6 | BIT7);
        P1OUT ^= BIT6;

        if (COM_UART1_TransmitMessagePacketFragment(&txPacket, &nextTx))
        {
            // Build the next packet
            if (CW_STEPS_SOURCE > CCW_STEPS_SOURCE)
            {
                txPacket.comm = ENC_ROT_DELTA_CW;
                txPacket.combined = CW_STEPS_SOURCE - CCW_STEPS_SOURCE;
            } else {
                txPacket.comm = ENC_ROT_DELTA_CCW;
                txPacket.combined = CCW_STEPS_SOURCE - CW_STEPS_SOURCE;
            }

            // Clear the steps counter
            ClearEncoderCounts();
            //CW_STEPS_SOURCE = 0;
            //CCW_STEPS_SOURCE = 0;

            COM_SeperateDataBytes(&txPacket);
        }
        CWsteps += 1;
        CCWsteps += 55;
    }

	return 0;
}
