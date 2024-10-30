#include <msp430.h> 
#include "../MotorLib/General.c"

// Lab 3 - Exercise 4
// Felix Wilton & Lazar Stanojevic
// Oct 29 2024
// (L) = MSP430 Family User Guide [576 pages]
// (M) = MSPE430 Datasheet [124 pages]
// (S) = MSP-EXP430FR5739 User Guide [28 pages]

/**
 * main.c
 */

// Variable declarations
volatile unsigned char cwCount = 0;
volatile unsigned char ccwCount = 0;

void EncoderTimerSetup(void);

int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	
	//P2.2 heart beat LED set-up
	P2DIR |=  (BIT0 + BIT1 + BIT2);
	P2SEL1 &= ~(BIT0 + BIT1 + BIT2);
	P2SEL0 &= ~(BIT0 + BIT1 + BIT2);
	P2OUT |= (BIT0 + BIT1 + BIT2);

    //initialize P1.1 & P1.2 &  to primary capture mode (TA0 & TA1)
    P1SEL0 |= (BIT1 + BIT2);
    P1SEL1 &= ~(BIT1 + BIT2);
    P1DIR &= ~(BIT1 + BIT2);

    StandardClockSetup_8Mhz_1Mhz(); //clock initialize

    StandardUART1Setup_9600_8(); //UART set-up
    UCA1IE |= UCRXIE;           // Enable RX interrupt
    //UCA1IE |= UCTXIE;         // Enable TX interrupt

    EncoderTimerSetup(); //clock initialize for stepper interrupts

    //enable timer interrupts
//    TA0CCTL1 |= CCIE;
//    TA1CCTL1 |= CCIE;

    //__enable_interrupt();       // Enable global interrupts

    while(1){


        _NOP();
        DelayMillis_8Mhz(80);
        P2OUT ^= (BIT2);

        cwCount = (char)(TA1R & 0xFF);
        ccwCount = (char)(TA0R & 0xFF);

        UART_TX_Char_BLOCKING(255);
        UART_TX_Char_BLOCKING(24);
        UART_TX_Char_BLOCKING(cwCount);
        UART_TX_Char_BLOCKING(ccwCount);
        UART_TX_Char_BLOCKING(0);

        ccwCount = 0;
        cwCount = 0;

        //TA1CTL |= TACLR;            // Clear Timer A                (L) pg. 349
        //TA0CTL |= TACLR;            // Clear Timer A                (L) pg. 349


        //while(!(UCA0IFG & UCTXIFG));
        //UCA0TXBUF = cwCount;
        //cwCount = 0;
    }


	return 0;
}


//#pragma vector = TIMER0_A1_VECTOR
//__interrupt void countCWinterrupt(void){
//
//    if(TA0CCTL1 & CCI)
//    {
//        P2OUT ^= (BIT0);
//        cwCount++;
//    }
//}
//
//#pragma vector = TIMER1_A1_VECTOR
//__interrupt void countCCWinterrupt(void){
//
//    if(TA1CCTL1 & CCI)
//    {
//        P2OUT ^= (BIT1);
//        ccwCount++;
//    }
//}

void EncoderTimerSetup(void){

    // Setup Timer A0 in the "continous count" mode
    TA0CTL |= TACLR;            // Clear Timer A                (L) pg. 349
    TA0CTL |= MC_2;             // Continous mode               (L) pg. 349
    TA0CTL |= TASSEL__TACLK;    // Clock source select          (L) pg. 349
    //TA0CTL |= (BIT7 | BIT6);  // 1/8 divider (125 kHz)        (L) pg. 349

//    // Setup Timer A0 Compare/Capture Control Register to rising and falling edges
//    TA0CCTL1 |= CM_3;       //Capture mode (rising & falling)   (L) pg. 351
//    TA0CCTL1 |= CCIS_0;     //Input select CCIOA            (L) pg. 351
//    TA0CCTL1 |= SCS;        //Synchronized capture          (L) pg. 351
//    TA0CCTL1 |= CAP;        //Capture mode enable           (L) pg. 351

    // Setup Timer A1 in the "continous count" mode
    TA1CTL |= TACLR;            // Clear Timer A                (L) pg. 349
    TA1CTL |= MC_2;             // Continous mode               (L) pg. 349
    TA1CTL |= TASSEL__TACLK;    // Clock source select          (L) pg. 349
    //TA0CTL |= (BIT7 | BIT6);  // 1/8 divider (125 kHz)        (L) pg. 349

//    // Setup Timer A1 Compare/Capture Control Register to rising and falling edges
//    TA1CCTL1 |= CM_3;       //Capture mode (rising & falling)   (L) pg. 351
//    TA1CCTL1 |= CCIS_0;     //Input select CCIOA            (L) pg. 351
//    TA1CCTL1 |= SCS;        //Synchronized capture          (L) pg. 351
//    TA1CCTL1 |= CAP;        //Capture mode enable           (L) pg. 351
}
