#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/DC.c"

int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	StandardClockSetup_8Mhz_1Mhz();

	DC_SetupDCMotor();

	DC_Spin(48000, CLOCKWISE);

    while(1)
    {}

	return 0;
}
