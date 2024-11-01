#include <msp430.h> 
#include "../MotorLib/General.c"
#include "../MotorLib/DC.c"

int main(void)
{
	WDTCTL = WDTPW | WDTHOLD;	// stop watchdog timer
	StandardClockSetup_8Mhz_1Mhz();

	DC_SetupDCMotor();

	DC_Spin(65000, CLOCKWISE);

	DelaySeconds_8Mhz(5);
	DC_Brake();
	DelaySeconds_8Mhz(5);
	DC_Spin(16000, COUNTERCLOCKWISE);
	DelaySeconds_8Mhz(5);
	DC_Brake();

    while(1)
    {}

	return 0;
}
