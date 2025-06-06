// PIC18Legacy_page_n.c
// Generated by decompiling PIC18Legacy.hex
// using Reko decompiler version 0.12.1.0.

#include "PIC18Legacy.h"

// 00000000: void fn00000000(Register cu8 TABLAT)
void fn00000000(cu8 TABLAT)
{
	g_b0001 &= 191;
	Data21[1].t0000.u1 = (ptr32) 330;
	fn00000E(TABLAT, 0x00, 0x00);
}

byte g_b0001 = ~0x10; // 00000001
// 00000E: void fn00000E(Register cu8 TABLAT, Register Eq_n FSR0, Register Eq_n TBLPTR)
// Called from:
//      fn00000000
void fn00000E(cu8 TABLAT, Eq_n FSR0, Eq_n TBLPTR)
{
	__tblrd(TBLPTR, 0x01);
	g_b00C5 = TABLAT;
	__tblrd(TBLPTR, 0x01);
	g_b00C6 = TABLAT;
	byte TBLPTRL_n = 0x06;
	byte TBLPTRH_n = 0x00;
	byte TBLPTRU_n = 0x00;
	bool v20_n = TABLAT != 0x00;
	bool v23_n;
	while (true)
	{
		if (!v20_n && g_b00C5 == 0x00)
			return;
		__tblrd(TBLPTR, 0x01);
		g_b00C0 = TABLAT;
		__tblrd(TBLPTR, 0x01);
		g_b00C1 = TABLAT;
		__tblrd(TBLPTR, 0x01);
		g_b00C2 = TABLAT;
		__tblrd(TBLPTR, 0x01);
		__tblrd(TBLPTR, 0x01);
		__tblrd(TBLPTR, 0x01);
		__tblrd(TBLPTR, 0x01);
		__tblrd(TBLPTR, 0x01);
		__tblrd(TBLPTR, 0x01);
		g_b00C3 = TABLAT;
		__tblrd(TBLPTR, 0x01);
		g_b00C4 = TABLAT;
		__tblrd(TBLPTR, 0x01);
		__tblrd(TBLPTR, 0x01);
		g_b00C7 = TBLPTRL_n;
		g_b00C8 = TBLPTRH_n;
		g_b00C9 = TBLPTRU_n;
		g_b00C3 = g_b00C3;
		v23_n = g_b00C3 != 0x00;
l000080:
		if (v23_n)
			break;
		g_b00C4 = g_b00C4;
		if (g_b00C4 != 0x00)
			break;
		TBLPTRL_n = g_b00C7;
		TBLPTRH_n = g_b00C8;
		TBLPTRU_n = g_b00C9;
		--g_b00C5;
		g_b00C6 -= !(cond(g_b00C5) & 0x01);
		v20_n = g_b00C6 != 0x00;
	}
	while (true)
	{
		__tblrd(TBLPTR, 0x01);
		*FSR0.u1 = TABLAT;
		--g_b00C3;
		FSR0.u0 = FSR0 + &g_b0001;
		v23_n = g_b00C3 != 0x00;
		if (g_b00C3 < 0x00)
			break;
		--g_b00C4;
	}
	goto l000080;
}

cu8 g_b00C0 = ~0x2A; // 000000C0
cu8 g_b00C1 = 110; // 000000C1
cu8 g_b00C2 = 242; // 000000C2
cu8 g_b00C3 = 0x8E; // 000000C3
cu8 g_b00C4 = 0x93; // 000000C4
cu8 g_b00C5 = 0x6A; // 000000C5
cu8 g_b00C6 = 0x01; // 000000C6
byte g_b00C7 = 0x0E; // 000000C7
byte g_b00C8 = ~0x19; // 000000C8
byte g_b00C9 = 110; // 000000C9
byte g_b00CA = 0x02; // 000000CA
// 0000D0: void fn0000D0(Register byte LATB, Register byte FSR2L, Register (ptr16 Eq_n) FSR2, Register (ptr16 Eq_n) FSR1)
void fn0000D0(byte LATB, byte FSR2L, struct Eq_n * FSR2, struct Eq_n * FSR1)
{
	FSR1->b0000 = FSR2L;
	while (FSR2->b00FE != 0x00)
	{
		if ((g_b00CA & 0x01) != 0x00)
		{
			g_b00CA &= ~0x01;
			if ((LATB & 0x01) != 0x00)
				LATB |= 0x80;
			else
				LATB &= 0x7F;
		}
	}
	FSR1->b0001 = FSR1->b0001;
}

// 000128: void fn000128(Register cu8 WREG, Register cu8 FSR0L, Register cu8 FSR0H, Register cu8 PRODL, Register Eq_n FSR0)
void fn000128(cu8 WREG, cu8 FSR0L, cu8 FSR0H, cu8 PRODL, Eq_n FSR0)
{
	while (FSR0H < WREG)
	{
		*FSR0.u1 = 0x00;
		FSR0.u1 = (word32) FSR0 + 1;
	}
	while (FSR0L < PRODL)
	{
		*FSR0.u1 = 0x00;
		FSR0.u1 = (word32) FSR0 + 1;
	}
}

