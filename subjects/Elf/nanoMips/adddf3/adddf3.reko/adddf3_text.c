// adddf3_text.c
// Generated by decompiling adddf3.o
// using Reko decompiler version 0.12.1.0.

#include "adddf3.h"

// 0804D000: void __adddf3(Register Eq_n r4, Register Eq_n r5, Register uint32 r6, Register Eq_n r7)
void __adddf3(Eq_n r4, Eq_n r5, uint32 r6, Eq_n r7)
{
	Eq_n r10_n = __ext<word32,word32>(r5, 0x00, 0x14);
	Eq_n r9_n = __ext<word32,word32>(r7, 0x00, 0x14);
	Eq_n r13_n = __ext<word32,word32>(r5, 0x04, 11);
	Eq_n r10_n = __ext<word32,word32>(r7, 0x04, 11);
	Eq_n r8_n = r4 >> 0x1D | r10_n << 0x03;
	Eq_n r3_n = r7 >> 0x1F;
	Eq_n r5_n = r5 >> 0x1F;
	Eq_n r12_n = r4 << 0x03;
	Eq_n r9_n = r6 >> 0x1D | r9_n << 0x03;
	Eq_n r11_n = r13_n - r10_n;
	if (r5 >> 0x1F == r7 >> 0x1F)
	{
		r3_n = r11_n;
		if (r11_n > 0x00)
		{
			Eq_n r7_n;
			if (r10_n == 0x00)
			{
				if ((r9_n | r6 << 0x03) == 0x00)
					goto l0804D052;
				r7_n = (word32) r11_n.u0 - 1;
				if (r11_n == 0x01)
				{
					Eq_n r6_n = (r4 << 0x03) + (r6 << 0x03);
					r10_n.u0 = 0x01;
					r8_n = r8_n + r9_n + (word32) (r6_n < r4 << 0x03);
					r12_n = r6_n;
					goto l0804D0CC;
				}
				if (r11_n == 0x07FF)
				{
l0804D0FE:
					if ((r8_n | r4 << 0x03) == 0x00)
					{
						r8_n.u0 = 0x00;
						r12_n.u0 = 0x00;
						r10_n = r11_n;
						goto l0804D064;
					}
					goto l0804D2D2;
				}
l0804D12C:
				uint32 r7_n;
				ui32 r6_n;
				if (r7_n < 0x39)
				{
					ui32 r4_n;
					if (r7_n < 0x20)
					{
						r7_n = r9_n >> r7_n;
						r6_n = (word32) (r6 << 0x03 << 0x20 - r7_n > 0x00) | r4_n;
						goto l0804D156;
					}
					uint32 r4_n = r9_n >> r7_n;
					ui32 r10_n = 0x00;
					if (r7_n != 0x20)
					{
						uint64 r10_r4_n = SEQ(r9_n, r6 << 0x03) >> r7_n;
						r4_n = (word32) r10_r4_n;
						r10_n = SLICE(r10_r4_n, word32, 32);
					}
					r6_n = (word32) ((r10_n | r6 << 0x03) > 0x00) | r4_n;
				}
				else
					r6_n = (word32) ((r9_n | r6 << 0x03) > 0x00);
				r7_n = 0x00;
l0804D156:
				Eq_n r6_n = r6_n + (r4 << 0x03);
				r10_n = r13_n;
				r8_n = (word32) (r6_n < r4 << 0x03) + ((word32) r8_n.u1 + r7_n);
				r12_n = r6_n;
				goto l0804D0CC;
			}
			if (r13_n != 0x07FF)
			{
				r9_n |= 0x08000000;
				r7_n = r11_n;
				goto l0804D12C;
			}
l0804D112:
			if ((r8_n | r4 << 0x03) != 0x00)
				goto l0804D2D2;
			r8_n.u0 = 0x00;
			r12_n.u0 = 0x00;
			r10_n = r13_n;
l0804D064:
			if (!__bit<word32,word32>(r8_n, 0x17))
			{
				r10_n = (word32) r10_n.u0 + 1;
				r8_n = __ins<word32,word32>(r8_n, 0x00, 0x07, 0x01);
				if (r10_n == 0x07FF)
				{
					r8_n.u0 = 0x00;
					r12_n.u0 = 0x00;
				}
			}
			ui32 r7_n = r12_n >> 0x03 | r8_n << 0x1D;
			Eq_n r8_n = r8_n >> 0x03;
			if (r10_n == 0x07FF)
			{
				if ((r7_n | r8_n) != 0x00)
					r8_n |= 0x00800000;
				else
					r8_n.u0 = 0x00;
			}
			__ins<word32,word32>(__ins<word32,word32>(__ins<word32,word32>(0x00, r8_n, 0x00, 0x01), r10_n, 0x04, 0x01), r5_n, 0x0F, 0x01);
			return;
		}
		if (r11_n != 0x00)
		{
			int32 r11_n;
			uint32 r8_n;
			if (r13_n == 0x00)
			{
				if ((r8_n | r4 << 0x03) == 0x00)
				{
					if (r10_n != 0x07FF)
						goto l0804D1AE;
					goto l0804D1A4;
				}
				r11_n = ~r11_n;
				if (r11_n == 0x00)
				{
					r12_n = (r4 << 0x03) + (r6 << 0x03);
					r8_n = r8_n + r9_n;
l0804D1C2:
					r8_n = r8_n + (word32) (r12_n < r6 << 0x03);
l0804D0CC:
					if (!__bit<word32,word32>(r8_n, 0x17))
					{
						r10_n = (word32) r10_n.u0 + 1;
						if (r10_n != 0x07FF)
						{
							Eq_n r6_n = __ins<word32,word32>(r8_n, 0x00, 0x07, 0x01);
							r12_n = r6_n << 0x1F | (r12_n >> 0x01 | r12_n & 0x01);
							r8_n = r6_n >> 0x01;
							goto l0804D27A;
						}
						goto l0804D060;
					}
					goto l0804D27A;
				}
				if (r10_n == 0x07FF)
				{
l0804D1A4:
					r12_n = r9_n | r6 << 0x03;
					r8_n.u0 = 0x00;
					if (r12_n == 0x00)
						goto l0804D064;
l0804D1AE:
					r8_n = r9_n;
					r12_n = r6 << 0x03;
					goto l0804D27A;
				}
			}
			else
			{
				if (r10_n == 0x07FF)
					goto l0804D1A4;
				r11_n = -r11_n;
				r8_n |= 0x08000000;
			}
			uint32 r11_n;
			ui32 r12_n;
			if (r11_n < 0x39)
			{
				ui32 r4_n;
				if (r11_n < 0x20)
				{
					r11_n = r8_n >> r11_n;
					r12_n = r4_n | (word32) ((r4 << 0x03) << 0x20 - r11_n > 0x00);
					goto l0804D1F8;
				}
				uint32 r4_n = r8_n >> r11_n;
				ui32 r7_n = 0x00;
				if (r11_n != 0x20)
				{
					uint64 r7_r4_n = SEQ(r8_n, r4 << 0x03) >> r11_n;
					r4_n = (word32) r7_r4_n;
					r7_n = SLICE(r7_r4_n, word32, 32);
				}
				r12_n = r4_n | (word32) ((r7_n | r4 << 0x03) > 0x00);
			}
			else
				r12_n = (word32) ((r8_n | r4 << 0x03) > 0x00);
			r11_n = 0x00;
l0804D1F8:
			r12_n = r12_n + (r6 << 0x03);
			r8_n = (word32) r9_n.u1 + r11_n;
			goto l0804D1C2;
		}
		r10_n = (word32) r13_n.u0 + 1;
		if (((word32) r13_n.u0 + 1 & 0x07FF) >= 0x02)
		{
			if (r13_n == 0x07FE)
			{
l0804D060:
				r8_n.u0 = 0x00;
				r12_n.u0 = 0x00;
				goto l0804D064;
			}
			uint32 r6_n = (r4 << 0x03) + (r6 << 0x03);
			uint32 r2_n = r8_n + r9_n + (word32) (r6_n < r4 << 0x03);
			r12_n = r2_n << 0x1F | r6_n >> 0x01;
			r8_n = r2_n >> 0x01;
l0804D27A:
			if ((r12_n & 0x07) != 0x00 && (r12_n & 0x0F) != 0x04)
			{
				Eq_n r6_n = (word32) r12_n.u1 + 4;
				r12_n = r6_n;
				r8_n = (word32) r8_n.u1 + (word32) (r6_n < r12_n);
			}
			goto l0804D064;
		}
		ui32 r7_n = r8_n | r4 << 0x03;
		if (r13_n == 0x00)
		{
			if (r7_n != 0x00)
			{
				r10_n.u0 = 0x00;
				if ((r9_n | r6 << 0x03) != 0x00)
				{
					Eq_n r6_n = (r4 << 0x03) + (r6 << 0x03);
					r12_n = r6_n;
					r8_n = r8_n + r9_n + (word32) (r6_n < r4 << 0x03);
					if (!__bit<word32,word32>(r8_n, 0x17))
					{
						r8_n = __ins<word32,word32>(r8_n, 0x00, 0x07, 0x01);
						r10_n.u0 = 0x01;
					}
				}
				goto l0804D27A;
			}
			r8_n = r9_n;
			r12_n = r6 << 0x03;
l0804D4D2:
			r10_n.u0 = 0x00;
			goto l0804D27A;
		}
		if (r7_n != 0x00)
		{
			r10_n.u0 = 0x07FF;
			if ((r9_n | r6 << 0x03) != 0x00)
			{
				uint32 r10_n = r8_n >> 0x03;
				uint32 r7_n;
				if (__bit<word32,word32>(r9_n >> 0x03 | r8_n >> 0x03, 0x13))
				{
					r7_n = r8_n << 0x1D | __ext<word32,word32>(r4, 0x00, 0x1D);
					r3_n = r5 >> 0x1F;
				}
				else
				{
					r10_n = 0x000FFFFF;
					r7_n = ~0x00;
				}
				r8_n = r7_n >> 0x1D | r10_n << 0x03;
				r12_n = r7_n << 0x03;
				goto l0804D2D0;
			}
			goto l0804D27A;
		}
		r8_n = r9_n;
		r12_n = r6 << 0x03;
l0804D2D2:
		r10_n.u0 = 0x07FF;
		goto l0804D27A;
	}
	Eq_n r14_n = r11_n;
	if (r11_n > 0x00)
	{
		Eq_n r7_n;
		if (r10_n == 0x00)
		{
			if ((r9_n | r6 << 0x03) == 0x00)
			{
l0804D052:
				r10_n = r11_n;
				if (r11_n == 0x07FF && (r8_n | r4 << 0x03) == 0x00)
					goto l0804D060;
				goto l0804D27A;
			}
			r7_n = (word32) r11_n.u0 - 1;
			if (r11_n == 0x01)
			{
				Eq_n r6_n = (r4 << 0x03) - (r6 << 0x03);
				r10_n.u0 = 0x01;
				r8_n = r8_n - r9_n - (word32) (r4 << 0x03 < r6_n);
				r12_n = r6_n;
				goto l0804D332;
			}
			if (r11_n == 0x07FF)
				goto l0804D0FE;
		}
		else
		{
			if (r13_n == 0x07FF)
				goto l0804D112;
			r9_n |= 0x08000000;
			r7_n = r11_n;
		}
		uint32 r7_n;
		ui32 r6_n;
		if (r7_n < 0x39)
		{
			ui32 r4_n;
			if (r7_n < 0x20)
			{
				r7_n = r9_n >> r7_n;
				r6_n = (word32) (r6 << 0x03 << 0x20 - r7_n > 0x00) | r4_n;
				goto l0804D36E;
			}
			uint32 r4_n = r9_n >> r7_n;
			ui32 r10_n = 0x00;
			if (r7_n != 0x20)
			{
				uint64 r10_r4_n = SEQ(r9_n, r6 << 0x03) >> r7_n;
				r4_n = (word32) r10_r4_n;
				r10_n = SLICE(r10_r4_n, word32, 32);
			}
			r6_n = (word32) ((r10_n | r6 << 0x03) > 0x00) | r4_n;
		}
		else
			r6_n = (word32) ((r9_n | r6 << 0x03) > 0x00);
		r7_n = 0x00;
l0804D36E:
		Eq_n r6_n = (r4 << 0x03) - r6_n;
		r10_n = r13_n;
		r8_n = r8_n - r7_n - (word32) (r4 << 0x03 < r6_n);
		r12_n = r6_n;
		goto l0804D332;
	}
	Eq_n r4_n;
	Eq_n r11_n;
	if (r11_n != 0x00)
	{
		int32 r11_n;
		uint32 r8_n;
		if (r13_n == 0x00)
		{
			if ((r8_n | r4 << 0x03) == 0x00)
			{
				if (r10_n != 0x07FF)
					goto l0804D3D8;
				goto l0804D3D0;
			}
			r11_n = ~r11_n;
			if (r11_n == 0x00)
			{
				r12_n = (r6 << 0x03) - (r4 << 0x03);
				r8_n = r9_n - r8_n;
				goto l0804D3EE;
			}
			if (r10_n == 0x07FF)
			{
l0804D3D0:
				r12_n = r9_n | r6 << 0x03;
				if (r12_n == 0x00)
				{
					r8_n.u0 = 0x00;
					r5_n = r7 >> 0x1F;
					goto l0804D064;
				}
				goto l0804D3D8;
			}
		}
		else
		{
			if (r10_n == 0x07FF)
				goto l0804D3D0;
			r11_n = -r11_n;
			r8_n |= 0x08000000;
		}
		uint32 r11_n;
		ui32 r12_n;
		if (r11_n < 0x39)
		{
			ui32 r5_n;
			if (r11_n < 0x20)
			{
				r11_n = r8_n >> r11_n;
				r12_n = r5_n | (word32) ((r4 << 0x03) << 0x20 - r11_n > 0x00);
				goto l0804D426;
			}
			uint32 r5_n = r8_n >> r11_n;
			ui32 r7_n = 0x00;
			if (r11_n != 0x20)
			{
				uint64 r7_r5_n = SEQ(r8_n, r4 << 0x03) >> r11_n;
				r5_n = (word32) r7_r5_n;
				r7_n = SLICE(r7_r5_n, word32, 32);
			}
			r12_n = r5_n | (word32) ((r7_n | r4 << 0x03) > 0x00);
		}
		else
			r12_n = (word32) ((r8_n | r4 << 0x03) > 0x00);
		r11_n = 0x00;
l0804D426:
		r12_n = (r6 << 0x03) - r12_n;
		r8_n = r9_n - r11_n;
l0804D3EE:
		r5_n = r7 >> 0x1F;
		r8_n = r8_n - (word32) (r6 << 0x03 < r12_n);
l0804D332:
		if (__bit<word32,word32>(r8_n, 0x17))
			goto l0804D27A;
		r4_n = __ext<word32,word32>(r8_n, 0x00, 0x17);
		r11_n = r12_n;
		r13_n = r10_n;
l0804D548:
		word32 r7_n = __count_leading_zeros<word32>(r4_n);
		if (r4_n == 0x00)
			r7_n = __count_leading_zeros<word32>(r11_n) + 0x20;
		Eq_n r10_n = r7_n + ~0x07;
		Eq_n r8_n;
		if (r10_n < 0x20)
		{
			r12_n = r11_n << r10_n;
			r8_n = r11_n >> -r10_n | r4_n << r10_n;
		}
		else
		{
			r12_n.u0 = 0x00;
			r8_n = r11_n << r7_n + ~0x27;
		}
		if (r10_n < r13_n)
		{
			r10_n = r13_n - r10_n;
			r8_n = __ins<word32,word32>(r8_n, 0x00, 0x07, 0x01);
			goto l0804D27A;
		}
		int32 r10_n = r10_n - r13_n;
		if (r10_n < 0x1F)
		{
			word32 r7_n = 0x20 - (r10_n + 0x01);
			r12_n = r8_n << r7_n | r12_n >> r10_n + 0x01 | (word32) (r12_n << r7_n > 0x00);
			r8_n = r8_n >> r10_n + 0x01;
		}
		else
		{
			ui32 r7_n = 0x00;
			uint32 r10_n = r8_n >> r10_n + ~0x1E;
			if (r10_n != 0x1F)
				r7_n = r8_n << -(r10_n + 0x01);
			r12_n = r10_n | (word32) ((r12_n | r7_n) > 0x00);
			r8_n.u0 = 0x00;
		}
		goto l0804D4D2;
	}
	if (((word32) r13_n.u0 + 1 & 0x07FF) < 0x02)
	{
		r10_n = r8_n | r4 << 0x03;
		ui32 r7_n = r9_n | r6 << 0x03;
		if (r13_n != 0x00)
		{
			if (r10_n == 0x00)
			{
				if (r7_n == 0x00)
				{
					r5_n.u0 = 0x00;
					r8_n.u0 = 0x007FFFFF;
					r12_n.u0 = ~0x07;
					r10_n.u0 = 0x07FF;
					goto l0804D064;
				}
				r8_n = r9_n;
				r12_n = r6 << 0x03;
l0804D2D0:
				r5_n = r3_n;
				goto l0804D2D2;
			}
			r10_n.u0 = 0x07FF;
			if (r7_n == 0x00)
				goto l0804D27A;
			uint32 r7_n = r8_n >> 0x03;
			uint32 r4_n;
			if (__bit<word32,word32>(r9_n >> 0x03 | r8_n >> 0x03, 0x13))
			{
				r4_n = r8_n << 0x1D | __ext<word32,word32>(r4, 0x00, 0x1D);
				r14_n = r5 >> 0x1F;
			}
			else
			{
				r7_n = 0x000FFFFF;
				r4_n = ~0x00;
			}
			r8_n = r4_n >> 0x1D | r7_n << 0x03;
			r12_n = r4_n << 0x03;
			r5_n = r14_n;
			goto l0804D2D2;
		}
		if (r10_n == 0x00)
		{
			if (r7_n != 0x00)
			{
l0804D3D8:
				r8_n = r9_n;
				r12_n = r6 << 0x03;
				goto l0804D4C2;
			}
			r12_n.u0 = 0x00;
l0804D492:
			r8_n.u0 = 0x00;
			r10_n.u0 = 0x00;
			r5_n.u0 = 0x00;
			goto l0804D064;
		}
		if (r7_n == 0x00)
			goto l0804D4D2;
		Eq_n r4_n = (r4 << 0x03) - (r6 << 0x03);
		Eq_n r7_n = r8_n - r9_n - (word32) (r4 << 0x03 < r4_n);
		if (!__bit<word32,word32>(r7_n, 0x17))
		{
			r12_n = (r6 << 0x03) - (r4 << 0x03);
			r10_n.u0 = 0x00;
			r8_n = r9_n - r8_n - (word32) (r6 << 0x03 < r12_n);
l0804D4C2:
			r5_n = r7 >> 0x1F;
			goto l0804D27A;
		}
		r12_n = r4_n | r7_n;
		if (r12_n != 0x00)
		{
			r8_n = r7_n;
			r12_n = r4_n;
			goto l0804D4D2;
		}
	}
	else
	{
		r11_n = (r4 << 0x03) - (r6 << 0x03);
		r4_n = r8_n - r9_n - (word32) (r4 << 0x03 < r11_n);
		if (!__bit<word32,word32>(r4_n, 0x17))
		{
			r11_n = (r6 << 0x03) - (r4 << 0x03);
			r5_n = r7 >> 0x1F;
			r4_n = r9_n - r8_n - (word32) (r6 << 0x03 < r11_n);
			goto l0804D548;
		}
		r12_n = r11_n | r4_n;
		if (r12_n != 0x00)
			goto l0804D548;
	}
	goto l0804D492;
}

