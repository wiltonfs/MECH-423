#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/Com.c"

// Lab 3 - Exercise 4
// Felix Wilton & Lazar Stanojevic
// Oct 31 2024


//unsigned int CWsteps = 0;
//unsigned int CCWsteps = 0;
#define CW_STEPS_SOURCE TA1R
#define CCW_STEPS_SOURCE TA0R

#define CW_SPIN 0
#define CCW_SPIN 1
#define BRAKE 2

MessagePacket txPacket = EMPTY_MESSAGE_PACKET;
PACKET_FRAGMENT nextTx = START_BYTE;

MessagePacket IncomingPacket = EMPTY_MESSAGE_PACKET;
volatile PACKET_FRAGMENT NextRead = START_BYTE;

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

void DCMotor_Setup(){

    //initialize pins 2.1 to primary mode (TB2.1)
    P2SEL0 |= BIT1;
    P2SEL1 &= ~BIT1;
    P2DIR |= BIT1;

    // Setup Timer B in the "up count" mode
    TB2CTL |= TBCLR;             // Clear Timer B            (L) pg.372
    TB2CTL |= (BIT4);            // Up mode                  (L) pg. 372
    TB2CTL |= TBSSEL__ACLK;      // Clock source select      (L) pg. 372
    TB2CTL &= ~(BIT7 | BIT6);    // No divider               (L) pg. 372
    TB2CCR0 = 65535;     // What we count to         (L) pg. 377

    //Set up pins P3.6 and P3.7 to output rotation direction
    P3DIR |= (BIT6 | BIT7);
    P3SEL1 &= ~(BIT6 | BIT7);
    P3SEL0 &= ~(BIT6 | BIT7);

    //Turn both to low
    P3OUT &= ~(BIT6 | BIT7);
}

void DCMotor_PWM(unsigned int dutyCycleCounter){

    TB2CCTL1 = OUTMOD_7;        // Reset/Set mode           (L) pg. 365, 366, and 375
    TB2CCR1 = dutyCycleCounter;
}

void DCMotor_Command(int CMD){

    if(CMD == CW_SPIN){
        // CW spin command to motor driver
        P3OUT |= BIT6;
        P3OUT &= ~BIT7;
    }
    else if(CMD == CCW_SPIN){
        // CCW spin command to motor driver
        P3OUT &= ~BIT6;
        P3OUT |= BIT7;
    }
    else if(CMD == BRAKE){
        // Stop spin command to motor driver
        P3OUT &= ~(BIT6 + BIT7);
    }
}

void ProcessCompletePacket() {

    // Modify the square wave duty cycle
    TimerB1_PWM(1, IncomingPacket.combined);

    // Handle the command byte
    if (IncomingPacket.comm == DCM_CW) {

        DCMotor_Command(CW_SPIN);

    } else if (IncomingPacket.comm == DCM_CCW) {

        DCMotor_Command(CCW_SPIN);

    } else if (IncomingPacket.comm == DCM_BRAKE) {

        DCMotor_Command(BRAKE);
    }
}

void EncoderVelocityTimerSetup(){

    //Uses to TB1.1 to trigger every ~40ms to compile data packet, then every ~200ms a full packet is sent.

    //initialize pins 3.4 to primary mode (TB1.1)
    P3SEL0 |= BIT4;
    P3SEL1 &= ~BIT4;
    P3DIR |= BIT4;

    // Setup Timer B in the "up count" mode
    TB1CTL |= TBCLR;             // Clear Timer B            (L) pg.372
    TB1CTL |= (BIT4);            // Up mode                  (L) pg. 372
    TB1CTL |= TBSSEL__SMCLK;      // Clock source select     (L) pg. 372
    TB1CTL |= (BIT7 | BIT6);    // 1/8 divider (125 kHz)     (L) pg. 372
    TB1CCR0 = 5000;     // What we count to         (L) pg. 377

    //upCountTarget = 5000 -> T = 40.0 ms

    //Enable timer interrupt
    //TB1CCTL1 |= CCIE;

    TB1CCTL1 = OUTMOD_7;        // Reset/Set mode           (L) pg. 365, 366, and 375
    TB1CCR1 = 2500;
}


int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer
    StandardClockSetup_8Mhz_1Mhz();

    StandardUART1Setup_9600_8();
    //UCA1IE |= UCRXIE;           // Enable RX interrupt

    EncoderTimerSetup();

    //DC Motor set-up
    DCMotor_Setup();
    DCMotor_PWM(0);         // 0% duty cycle, P2.1 output

    //Set-up timer for transmitting at 40ms (200ms for full packet)
    EncoderVelocityTimerSetup();

    // Set up P1.6 as LED output
    P1DIR |= BIT6;
    P1SEL1 &= ~BIT6;
    P1SEL0 &= ~BIT6;

    // Turn the LEDs off
    P1OUT &= ~BIT6;

    //__enable_interrupt();        // Enable global interrupts
    _EINT();

    while(1)
    {
        DelayMillis_8Mhz(200);

        P1OUT ^= BIT6;

//        if (COM_UART1_TransmitMessagePacketFragment(&txPacket, &nextTx))
//        {
//            // Build the next packet
//            if (CW_STEPS_SOURCE > CCW_STEPS_SOURCE)
//            {
//                txPacket.comm = ENC_ROT_DELTA_CW;
//                txPacket.combined = CW_STEPS_SOURCE - CCW_STEPS_SOURCE;
//            } else {
//                txPacket.comm = ENC_ROT_DELTA_CCW;
//                txPacket.combined = CCW_STEPS_SOURCE - CW_STEPS_SOURCE;
//            }
//
//            // Clear the steps counter
//            ClearEncoderCounts();
//
//            COM_SeperateDataBytes(&txPacket);
//        }
//        CWsteps += 1;
//        CCWsteps += 55;
    }

    return 0;
}

#pragma vector = TIMER1_B0_VECTOR
__interrupt void transmission_timer_ISR(void){

    //transmit the data packet
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

            COM_SeperateDataBytes(&txPacket);
        }
}


#pragma vector = USCI_A1_VECTOR
__interrupt void uart_ISR(void) {
    if (UCA1IV == USCI_UART_UCRXIFG)    // Receive buffer full. (L) pg. 504
    {
        volatile unsigned char RxByte = UCA1RXBUF; // Read from the receive buffer

        if (COM_MessagePacketAssembly_StateMachine(&IncomingPacket, &NextRead, RxByte))
        {
            // returns True if the packet is now complete
            ProcessCompletePacket();
        }
        return;
    }
    else if (UCA1IV == USCI_UART_UCTXIFG || UCA1IV == USCI_UART_UCTXCPTIFG)     // Transmit buffer empty OR Transmit complete. (L) pg. 504
    {
        return;
    }
    else if (UCA1IV == USCI_UART_UCSTTIFG)  // Start bit received. (L) pg. 504
    {
        return;
    }
}
