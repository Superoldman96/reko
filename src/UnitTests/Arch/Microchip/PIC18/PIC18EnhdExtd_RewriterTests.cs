#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2025 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Libraries.Microchip;
using NUnit.Framework;

namespace Reko.UnitTests.Arch.Microchip.PIC18.Rewriter
{
    using Common;
    using static Common.Sample;

    /// <summary>
    /// PIC18 enhanced("cpu_pic18f_v6")/extended execution mode rewriter tests.
    /// </summary>
    [TestFixture]
    public class PIC18EnhdExtd_RewriterTests : PICRewriterTestsBase
    {
        [OneTimeSetUp]
        public void OneSetup()
        {
            SetPICModel(PIC18EnhancedName, PICExecMode.Extended);
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_ADDULNK()
        {
            ExecTest(Words(0xE8C6),
                "0|T--|000200(2): 4 instructions",
                    "1|L--|FSR2 = FSR2 + 6<8>",
                    "2|L--|STKPTR = STKPTR - 1<8>",
                    "3|L--|TOS = Stack[STKPTR]",
                    "4|R--|return (0,0)"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_ADDFSR()
        {
            ExecTest(Words(0xE812),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|FSR0 = FSR0 + 0x12<8>"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_ADDLW()
        {
            ExecTest(Words(0x0F00),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + 0<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x0F55),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + 0x55<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_ADDWF()
        {
            ExecTest(Words(0x2400),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[FSR2 + 0<8>:byte]",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2401),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[FSR2 + 1<8>:byte]",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x24C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + TRISB",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2500),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[BSR:0<8>:byte]",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2501),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[BSR:1<8>:byte]",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x25C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[BSR:0xC3<8>:byte]",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2601),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = WREG + Data[FSR2 + 1<8>:byte]",
                    "2|L--|CDCZOVN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x26C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = WREG + TRISB",
                    "2|L--|CDCZOVN = cond(TRISB)"
                );
            ExecTest(Words(0x2701),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = WREG + Data[BSR:1<8>:byte]",
                    "2|L--|CDCZOVN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x27C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = WREG + Data[BSR:0xC3<8>:byte]",
                    "2|L--|CDCZOVN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_ADDWFC()
        {
            ExecTest(Words(0x2000),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[FSR2 + 0<8>:byte] + C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2001),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[FSR2 + 1<8>:byte] + C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x20C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + TRISB + C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2100),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[BSR:0<8>:byte] + C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2101),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[BSR:1<8>:byte] + C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x21C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[BSR:0xC3<8>:byte] + C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2212),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 0x12<8>:byte] = WREG + Data[FSR2 + 0x12<8>:byte] + C",
                    "2|L--|CDCZOVN = cond(Data[FSR2 + 0x12<8>:byte])"
                );
            ExecTest(Words(0x22C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = WREG + TRISB + C",
                    "2|L--|CDCZOVN = cond(TRISB)"
                );
            ExecTest(Words(0x233F),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0x3F<8>:byte] = WREG + Data[BSR:0x3F<8>:byte] + C",
                    "2|L--|CDCZOVN = cond(Data[BSR:0x3F<8>:byte])"
                );
            ExecTest(Words(0x23C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = WREG + Data[BSR:0xC3<8>:byte] + C",
                    "2|L--|CDCZOVN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_ANDLW()
        {
            ExecTest(Words(0x0B00),
               "0|L--|000200(2): 2 instructions", "1|L--|WREG = WREG & 0<8>", "2|L--|ZN = cond(WREG)"
               );
            ExecTest(Words(0x0B55),
               "0|L--|000200(2): 2 instructions", "1|L--|WREG = WREG & 0x55<8>", "2|L--|ZN = cond(WREG)"
               );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_ANDWF()
        {
            ExecTest(Words(0x1400),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG & Data[FSR2 + 0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1401),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG & Data[FSR2 + 1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x14C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG & TRISB",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1500),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG & Data[BSR:0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1501),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG & Data[BSR:1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x15C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG & Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1614),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 0x14<8>:byte] = WREG & Data[FSR2 + 0x14<8>:byte]",
                    "2|L--|ZN = cond(Data[FSR2 + 0x14<8>:byte])"
                );
            ExecTest(Words(0x16C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = WREG & TRISB",
                    "2|L--|ZN = cond(TRISB)"
                );
            ExecTest(Words(0x173F),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0x3F<8>:byte] = WREG & Data[BSR:0x3F<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:0x3F<8>:byte])"
                );
            ExecTest(Words(0x17C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = WREG & Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BC()
        {
            ExecTest(Words(0xE200),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(ULT,C)) branch 000202"
                );
            ExecTest(Words(0xE27F),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(ULT,C)) branch 000300"
                );
            ExecTest(Words(0xE2FF),
               "0|T--|000200(2): 1 instructions",
                   "1|T--|if (Test(ULT,C)) branch 000200"
               );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BCF()
        {
            ExecTest(Words(0x9001),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] & 0xFE<8>"
                );
            ExecTest(Words(0x94C4),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|TRISC = TRISC & 0xFB<8>"
                );
            ExecTest(Words(0x9101),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = Data[BSR:1<8>:byte] & 0xFE<8>"
                );
            ExecTest(Words(0x9FC4),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:0xC4<8>:byte] = Data[BSR:0xC4<8>:byte] & 0x7F<8>"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BN()
        {
            ExecTest(Words(0xE600),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(LT,N)) branch 000202"
                );
            ExecTest(Words(0xE67F),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(LT,N)) branch 000300"
                );
            ExecTest(Words(0xE6FF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(LT,N)) branch 000200"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BNC()
        {
            ExecTest(Words(0xE300),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(UGE,C)) branch 000202"
                );
            ExecTest(Words(0xE37F),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(UGE,C)) branch 000300"
                );
            ExecTest(Words(0xE3FF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(UGE,C)) branch 000200"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BNN()
        {
            ExecTest(Words(0xE700),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(GE,N)) branch 000202"
                );
            ExecTest(Words(0xE77F),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(GE,N)) branch 000300"
                );
            ExecTest(Words(0xE7FF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(GE,N)) branch 000200"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BNOV()
        {
            ExecTest(Words(0xE500),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(NO,OV)) branch 000202"
                );
            ExecTest(Words(0xE57F),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(NO,OV)) branch 000300"
                );
            ExecTest(Words(0xE5FF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(NO,OV)) branch 000200"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BNZ()
        {
            ExecTest(Words(0xE100),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(NE,Z)) branch 000202"
                );
            ExecTest(Words(0xE17F),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(NE,Z)) branch 000300"
                );
            ExecTest(Words(0xE1FF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(NE,Z)) branch 000200"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BOV()
        {
            ExecTest(Words(0xE400),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(OV,OV)) branch 000202"
                );
            ExecTest(Words(0xE47F),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(OV,OV)) branch 000300"
                );
            ExecTest(Words(0xE4FF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(OV,OV)) branch 000200"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BRA()
        {
            ExecTest(Words(0xD000),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|goto 000202"
                );
            ExecTest(Words(0xD005),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|goto 00020C"
                );
            ExecTest(Words(0xD3FF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|goto 000A00"
                );
            ExecTest(Words(0xD7FF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|goto 000200"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BSF()
        {
            ExecTest(Words(0x8001),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] | 1<8>"
                );
            ExecTest(Words(0x84C3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|TRISB = TRISB | 4<8>"
                );
            ExecTest(Words(0x8101),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = Data[BSR:1<8>:byte] | 1<8>"
                );
            ExecTest(Words(0x8FC3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = Data[BSR:0xC3<8>:byte] | 0x80<8>"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BTFSC()
        {
            ExecTest(Words(0xB000),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if ((Data[FSR2 + 0<8>:byte] & 1<8>) == 0<8>) branch 000204"
                );
            ExecTest(Words(0xB102),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if ((Data[BSR:2<8>:byte] & 1<8>) == 0<8>) branch 000204"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BTFSS()
        {
            ExecTest(Words(0xA002),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if ((Data[FSR2 + 2<8>:byte] & 1<8>) != 0<8>) branch 000204"
                );
            ExecTest(Words(0xA105),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if ((Data[BSR:5<8>:byte] & 1<8>) != 0<8>) branch 000204"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BTG()
        {
            ExecTest(Words(0x7001),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] ^ 1<8>"
                );
            ExecTest(Words(0x74C3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|TRISB = TRISB ^ 4<8>"
                );
            ExecTest(Words(0x7101),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = Data[BSR:1<8>:byte] ^ 1<8>"
                );
            ExecTest(Words(0x7FC3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = Data[BSR:0xC3<8>:byte] ^ 0x80<8>"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_BZ()
        {
            ExecTest(Words(0xE000),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(EQ,Z)) branch 000202"
                );
            ExecTest(Words(0xE07F),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(EQ,Z)) branch 000300"
                );
            ExecTest(Words(0xE0FF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Test(EQ,Z)) branch 000200"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_CALL()
        {
            ExecTest(Words(0xEC06, 0xF000),
                "0|T--|000200(4): 4 instructions",
                    "1|L--|STKPTR = STKPTR + 1<8>",
                    "2|L--|Stack[STKPTR] = 000204",
                    "3|L--|TOS = 000204",
                    "4|T--|call 00000C (0)"
                );

            ExecTest(Words(0xEC12, 0xF345),
                "0|T--|000200(4): 4 instructions",
                    "1|L--|STKPTR = STKPTR + 1<8>",
                    "2|L--|Stack[STKPTR] = 000204",
                    "3|L--|TOS = 000204",
                    "4|T--|call 068A24 (0)"
                );

            ExecTest(Words(0xED06, 0xF000),
                "0|T--|000200(4): 7 instructions",
                    "1|L--|STKPTR = STKPTR + 1<8>",
                    "2|L--|Stack[STKPTR] = 000204",
                    "3|L--|TOS = 000204",
                    "4|L--|STATUS_CSHAD = STATUS",
                    "5|L--|WREG_CSHAD = WREG",
                    "6|L--|BSR_CSHAD = BSR",
                    "7|T--|call 00000C (0)"
                );

            ExecTest(Words(0xED12, 0xF345),
                "0|T--|000200(4): 7 instructions",
                    "1|L--|STKPTR = STKPTR + 1<8>",
                    "2|L--|Stack[STKPTR] = 000204",
                    "3|L--|TOS = 000204",
                    "4|L--|STATUS_CSHAD = STATUS",
                    "5|L--|WREG_CSHAD = WREG",
                    "6|L--|BSR_CSHAD = BSR",
                    "7|T--|call 068A24 (0)"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_CALLW()
        {
            ExecTest(Words(0x0014),
                "0|T--|000200(2): 4 instructions",
                    "1|L--|STKPTR = STKPTR + 1<8>",
                    "2|L--|Stack[STKPTR] = 000202",
                    "3|L--|TOS = 000202",
                    "4|T--|call __callw(WREG, PCLAT) (0)"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_CLRF()
        {
            ExecTest(Words(0x6A01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = 0<8>",
                    "2|L--|Z = true"
                );

            ExecTest(Words(0x6AC4),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISC = 0<8>",
                    "2|L--|Z = true"
                );

            ExecTest(Words(0x6B02),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:2<8>:byte] = 0<8>",
                    "2|L--|Z = true"
                );

            ExecTest(Words(0x6BC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = 0<8>",
                    "2|L--|Z = true"
                );


        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_CLRWDT()
        {
            ExecTest(Words(0x0004),
                "0|L--|000200(2): 1 instructions",
                "1|L--|STATUS = STATUS | 0x60<8>"
               );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_COMF()
        {
            ExecTest(Words(0x1C00),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = ~Data[FSR2 + 0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1C01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = ~Data[FSR2 + 1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1CC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = ~TRISB",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1D00),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = ~Data[BSR:0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1D01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = ~Data[BSR:1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1DC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = ~Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1E12),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 0x12<8>:byte] = ~Data[FSR2 + 0x12<8>:byte]",
                    "2|L--|ZN = cond(Data[FSR2 + 0x12<8>:byte])"
                );
            ExecTest(Words(0x1EC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = ~TRISB",
                    "2|L--|ZN = cond(TRISB)"
                );
            ExecTest(Words(0x1F3F),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0x3F<8>:byte] = ~Data[BSR:0x3F<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:0x3F<8>:byte])"
                );
            ExecTest(Words(0x1FC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = ~Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_CPFSEQ()
        {
            ExecTest(Words(0x6200),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[FSR2 + 0<8>:byte] == WREG) branch 000204"
                );
            ExecTest(Words(0x6201),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[FSR2 + 1<8>:byte] == WREG) branch 000204"
                );
            ExecTest(Words(0x62C3),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (TRISB == WREG) branch 000204"
                );
            ExecTest(Words(0x6300),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:0<8>:byte] == WREG) branch 000204"
                );
            ExecTest(Words(0x6301),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:1<8>:byte] == WREG) branch 000204"
                );
            ExecTest(Words(0x63C3),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:0xC3<8>:byte] == WREG) branch 000204"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_CPFSGT()
        {
            ExecTest(Words(0x6400),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[FSR2 + 0<8>:byte] >u WREG) branch 000204"
                );
            ExecTest(Words(0x6401),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[FSR2 + 1<8>:byte] >u WREG) branch 000204"
                );
            ExecTest(Words(0x64C3),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (TRISB >u WREG) branch 000204"
                );
            ExecTest(Words(0x6500),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:0<8>:byte] >u WREG) branch 000204"
                );
            ExecTest(Words(0x6501),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:1<8>:byte] >u WREG) branch 000204"
                );
            ExecTest(Words(0x65C3),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:0xC3<8>:byte] >u WREG) branch 000204"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_CPFSLT()
        {
            ExecTest(Words(0x6000),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[FSR2 + 0<8>:byte] <u WREG) branch 000204"
                );
            ExecTest(Words(0x6001),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[FSR2 + 1<8>:byte] <u WREG) branch 000204"
                );
            ExecTest(Words(0x60C3),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (TRISB <u WREG) branch 000204"
                );
            ExecTest(Words(0x6100),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:0<8>:byte] <u WREG) branch 000204"
                );
            ExecTest(Words(0x6101),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:1<8>:byte] <u WREG) branch 000204"
                );
            ExecTest(Words(0x61C3),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:0xC3<8>:byte] <u WREG) branch 000204"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_DAW()
        {
            ExecTest(Words(0x0007),
                "0|L--|000200(2): 2 instructions",
                "1|L--|WREG = __daw(WREG, C, DC)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_DCFSNZ()
        {
            ExecTest(Words(0x4C00),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 0<8>:byte] - 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4C01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 1<8>:byte] - 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4CC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = TRISC - 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4D00),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0<8>:byte] - 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4D01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:1<8>:byte] - 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4DC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0xC4<8>:byte] - 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4E01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] - 1<8>",
                    "2|T--|if (Data[FSR2 + 1<8>:byte] != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4EC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|TRISC = TRISC - 1<8>",
                    "2|T--|if (TRISC != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4F44),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0x44<8>:byte] = Data[BSR:0x44<8>:byte] - 1<8>",
                    "2|T--|if (Data[BSR:0x44<8>:byte] != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4FC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC4<8>:byte] = Data[BSR:0xC4<8>:byte] - 1<8>",
                    "2|T--|if (Data[BSR:0xC4<8>:byte] != 0<8>) branch 000204"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_DECF()
        {
            ExecTest(Words(0x0400),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 0<8>:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x0401),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 1<8>:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x04C4),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = TRISC - 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x0500),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0<8>:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x0501),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:1<8>:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x05C4),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0xC4<8>:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x0601),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x06C4),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISC = TRISC - 1<8>",
                    "2|L--|CDCZOVN = cond(TRISC)"
                );
            ExecTest(Words(0x0744),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0x44<8>:byte] = Data[BSR:0x44<8>:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(Data[BSR:0x44<8>:byte])"
                );
            ExecTest(Words(0x07C4),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC4<8>:byte] = Data[BSR:0xC4<8>:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(Data[BSR:0xC4<8>:byte])"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_DECFSZ()
        {
            ExecTest(Words(0x2C00),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 0<8>:byte] - 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x2C01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 1<8>:byte] - 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x2CC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = TRISC - 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x2D00),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0<8>:byte] - 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x2D01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:1<8>:byte] - 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x2DC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0xC4<8>:byte] - 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x2E01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] - 1<8>",
                    "2|T--|if (Data[FSR2 + 1<8>:byte] == 0<8>) branch 000204"
                );
            ExecTest(Words(0x2EC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|TRISC = TRISC - 1<8>",
                    "2|T--|if (TRISC == 0<8>) branch 000204"
                );
            ExecTest(Words(0x2F44),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0x44<8>:byte] = Data[BSR:0x44<8>:byte] - 1<8>",
                    "2|T--|if (Data[BSR:0x44<8>:byte] == 0<8>) branch 000204"
                );
            ExecTest(Words(0x2FC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC4<8>:byte] = Data[BSR:0xC4<8>:byte] - 1<8>",
                    "2|T--|if (Data[BSR:0xC4<8>:byte] == 0<8>) branch 000204"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_GOTO()
        {
            ExecTest(Words(0xEF00, 0xF000),
                "0|T--|000200(4): 1 instructions",
                    "1|T--|goto 000000"
                );
            ExecTest(Words(0xEF12, 0xF345),
                "0|T--|000200(4): 1 instructions",
                    "1|T--|goto 068A24"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_INCF()
        {
            ExecTest(Words(0x2800),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 0<8>:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2801),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 1<8>:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x28C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = TRISB + 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2900),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0<8>:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2901),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:1<8>:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x29C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0xC3<8>:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x2A01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x2AC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = TRISB + 1<8>",
                    "2|L--|CDCZOVN = cond(TRISB)"
                );
            ExecTest(Words(0x2B33),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0x33<8>:byte] = Data[BSR:0x33<8>:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(Data[BSR:0x33<8>:byte])"
                );
            ExecTest(Words(0x2BC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = Data[BSR:0xC3<8>:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(Data[BSR:0xC3<8>:byte])"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_INCFSZ()
        {
            ExecTest(Words(0x3C00),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 0<8>:byte] + 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x3C01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 1<8>:byte] + 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x3CC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = TRISC + 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x3D00),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0<8>:byte] + 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x3D01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:1<8>:byte] + 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x3DC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0xC4<8>:byte] + 1<8>",
                    "2|T--|if (WREG == 0<8>) branch 000204"
                );
            ExecTest(Words(0x3E01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] + 1<8>",
                    "2|T--|if (Data[FSR2 + 1<8>:byte] == 0<8>) branch 000204"
                );
            ExecTest(Words(0x3EC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|TRISC = TRISC + 1<8>",
                    "2|T--|if (TRISC == 0<8>) branch 000204"
                );
            ExecTest(Words(0x3F44),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0x44<8>:byte] = Data[BSR:0x44<8>:byte] + 1<8>",
                    "2|T--|if (Data[BSR:0x44<8>:byte] == 0<8>) branch 000204"
                );
            ExecTest(Words(0x3FC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC4<8>:byte] = Data[BSR:0xC4<8>:byte] + 1<8>",
                    "2|T--|if (Data[BSR:0xC4<8>:byte] == 0<8>) branch 000204"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_INFSNZ()
        {
            ExecTest(Words(0x4800),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 0<8>:byte] + 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4801),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 1<8>:byte] + 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x48C4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = TRISC + 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4900),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0<8>:byte] + 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4901),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:1<8>:byte] + 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x49C4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0xC4<8>:byte] + 1<8>",
                    "2|T--|if (WREG != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4A01),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] + 1<8>",
                    "2|T--|if (Data[FSR2 + 1<8>:byte] != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4AC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|TRISC = TRISC + 1<8>",
                    "2|T--|if (TRISC != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4B44),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0x44<8>:byte] = Data[BSR:0x44<8>:byte] + 1<8>",
                    "2|T--|if (Data[BSR:0x44<8>:byte] != 0<8>) branch 000204"
                );
            ExecTest(Words(0x4BC4),
                "0|T--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC4<8>:byte] = Data[BSR:0xC4<8>:byte] + 1<8>",
                    "2|T--|if (Data[BSR:0xC4<8>:byte] != 0<8>) branch 000204"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_IORLW()
        {
            ExecTest(Words(0x0900),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG | 0<8>",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x0955),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG | 0x55<8>",
                    "2|L--|ZN = cond(WREG)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_IORWF()
        {
            ExecTest(Words(0x1000),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG | Data[FSR2 + 0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1001),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG | Data[FSR2 + 1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x10C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG | TRISB",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1100),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG | Data[BSR:0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1101),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG | Data[BSR:1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x11C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG | Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1201),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = WREG | Data[FSR2 + 1<8>:byte]",
                    "2|L--|ZN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x12C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = WREG | TRISB",
                    "2|L--|ZN = cond(TRISB)"
                );
            ExecTest(Words(0x1301),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = WREG | Data[BSR:1<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x13C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = WREG | Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_LFSR()
        {
            ExecTest(Words(0xEE01, 0xF023),
                "0|L--|000200(4): 1 instructions",
                    "1|L--|FSR0 = 0x423<u16>"
                );
            ExecTest(Words(0xEE14, 0xF056),
                "0|L--|000200(4): 1 instructions",
                    "1|L--|FSR1 = 0x1056<u16>"
                );
            ExecTest(Words(0xEE27, 0xF089),
                "0|L--|000200(4): 1 instructions",
                    "1|L--|FSR2 = 0x1C89<u16>"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MOVF()
        {
            ExecTest(Words(0x5000),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x5001),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x50C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = TRISB",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x5100),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x5101),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x51C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x5201),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte]",
                    "2|L--|ZN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x52C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = TRISB",
                    "2|L--|ZN = cond(TRISB)"
                );
            ExecTest(Words(0x5301),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = Data[BSR:1<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x53C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MOVFF()
        {
            ExecTest(Words(0xC123, 0xF456),
                "0|L--|000200(4): 1 instructions",
                    "1|L--|Data[0x0456<p16>:byte] = Data[0x0123<p16>:byte]"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MOVFFL()
        {
            ExecTest(Words(0x006F, 0xF123, 0xF456),
                "0|L--|000200(6): 1 instructions",
                    "1|L--|Data[0x3456<p16>:byte] = Data[0x3C48<p16>:byte]"
                );
            ExecTest(Words(0x006F, 0xFDF3, 0xFF78),
                "0|L--|000200(6): 1 instructions",
                    "1|L--|CCPR2L = CCPR1L"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MOVLB()
        {
            ExecTest(Words(0x0100),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|BSR = 0<8>"
                );
            ExecTest(Words(0x0105),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|BSR = 5<8>"
                );
            ExecTest(Words(0x0132),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|BSR = 0x32<8>"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MOVLW()
        {
            ExecTest(Words(0x0E00),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|WREG = 0<8>"
                );
            ExecTest(Words(0x0E55),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|WREG = 0x55<8>"
                );
            ExecTest(Words(0x0EBC),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|WREG = 0xBC<8>"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MOVSF()
        {
            ExecTest(Words(0xEB12, 0xF234),
                "0|L--|000200(4): 1 instructions",
                    "1|L--|Data[0x0234<p16>:byte] = Data[FSR2 + 0x12<8>:byte]"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MOVSFL()
        {
            ExecTest(Words(0x0002, 0xF123, 0xF456),
                "0|L--|000200(6): 1 instructions",
                    "1|L--|Data[0x3456<p16>:byte] = Data[FSR2 + 0x48<8>:byte]"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MOVSS()
        {
            ExecTest(Words(0xEB84, 0xF067),
                "0|L--|000200(4): 1 instructions",
                    "1|L--|Data[FSR2 + 0x67<8>:byte] = Data[FSR2 + 4<8>:byte]"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MOVWF()
        {
            ExecTest(Words(0x6E01),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = WREG"
                );

            ExecTest(Words(0x6EC4),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|TRISC = WREG"
                );

            ExecTest(Words(0x6F02),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:2<8>:byte] = WREG"
                );

            ExecTest(Words(0x6FC3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = WREG"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MULLW()
        {
            ExecTest(Words(0x0D00),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|PROD = WREG *u 0<8>"
                );
            ExecTest(Words(0x0D55),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|PROD = WREG *u 0x55<8>"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_MULWF()
        {
            ExecTest(Words(0x0344),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|PROD = Data[BSR:0x44<8>:byte] *u WREG"
                );
            ExecTest(Words(0x0389),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|PROD = Data[BSR:0x89<8>:byte] *u WREG"
                );
            ExecTest(Words(0x0200),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|PROD = Data[FSR2 + 0<8>:byte] *u WREG"
                );
            ExecTest(Words(0x025F),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|PROD = Data[FSR2 + 0x5F<8>:byte] *u WREG"
                );
            ExecTest(Words(0x02A8),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|PROD = T3GATE *u WREG"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_NEGF()
        {
            ExecTest(Words(0x6C01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = -Data[FSR2 + 1<8>:byte]",
                    "2|L--|CDCZOVN = cond(Data[FSR2 + 1<8>:byte])"
                );

            ExecTest(Words(0x6CC4),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISC = -TRISC",
                    "2|L--|CDCZOVN = cond(TRISC)"
                );

            ExecTest(Words(0x6D02),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:2<8>:byte] = -Data[BSR:2<8>:byte]",
                    "2|L--|CDCZOVN = cond(Data[BSR:2<8>:byte])"
                );

            ExecTest(Words(0x6DC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = -Data[BSR:0xC3<8>:byte]",
                    "2|L--|CDCZOVN = cond(Data[BSR:0xC3<8>:byte])"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_NOP()
        {

            ExecTest(Words(0x0000),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|nop"
                );

            ExecTest(Words(0xF000),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|nop"
                );

            ExecTest(Words(0xF123),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|nop"
                );

            ExecTest(Words(0xFEDC, 0xF256),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|nop",
                "2|L--|000202(2): 1 instructions",
                    "3|L--|nop"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_POP()
        {
            ExecTest(Words(0x0006),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|STKPTR = STKPTR - 1<8>",
                    "2|L--|TOS = Stack[STKPTR]"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_PUSH()
        {
            ExecTest(Words(0x0005),
                "0|L--|000200(2): 3 instructions",
                    "1|L--|STKPTR = STKPTR + 1<8>",
                    "2|L--|Stack[STKPTR] = 000202",
                    "3|L--|TOS = 000202"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_PUSHL()
        {
            ExecTest(Words(0xEAAA),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2:byte] = 0xAA<8>",
                    "2|L--|FSR2 = FSR2 + 1<16>"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_RCALL()
        {
            ExecTest(Words(0xD800),
                "0|T--|000200(2): 4 instructions",
                    "1|L--|STKPTR = STKPTR + 1<8>",
                    "2|L--|Stack[STKPTR] = 000202",
                    "3|L--|TOS = 000202",
                    "4|T--|call 000202 (0)"
                );
            ExecTest(Words(0xDFFF),
                "0|T--|000200(2): 4 instructions",
                    "1|L--|STKPTR = STKPTR + 1<8>",
                    "2|L--|Stack[STKPTR] = 000202",
                    "3|L--|TOS = 000202",
                    "4|T--|call 000200 (0)"
                );
            ExecTest(Words(0xDBFF),
                "0|T--|000200(2): 4 instructions",
                    "1|L--|STKPTR = STKPTR + 1<8>",
                    "2|L--|Stack[STKPTR] = 000202",
                    "3|L--|TOS = 000202",
                    "4|T--|call 000A00 (0)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_RESET()
        {
            ExecTest(Words(0x00FF),
                "0|H--|000200(2): 2 instructions",
                    "1|L--|STKPTR = 0<8>",
                    "2|L--|__reset()"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_RETFIE()
        {
            ExecTest(Words(0x0010),
                "0|T--|000200(2): 3 instructions",
                    "1|L--|STKPTR = STKPTR - 1<8>",
                    "2|L--|TOS = Stack[STKPTR]",
                    "3|R--|return (0,0)"
                );
            ExecTest(Words(0x0011),
                "0|T--|000200(2): 3 instructions",
                    "1|L--|STKPTR = STKPTR - 1<8>",
                    "2|L--|TOS = Stack[STKPTR]",
                    "3|R--|return (0,0)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_RETLW()
        {
            ExecTest(Words(0x0C00),
                "0|R--|000200(2): 4 instructions",
                    "1|L--|WREG = 0<8>",
                    "2|L--|STKPTR = STKPTR - 1<8>",
                    "3|L--|TOS = Stack[STKPTR]",
                    "4|R--|return (0,0)"
                );
            ExecTest(Words(0x0C55),
                "0|R--|000200(2): 4 instructions",
                    "1|L--|WREG = 0x55<8>",
                    "2|L--|STKPTR = STKPTR - 1<8>",
                    "3|L--|TOS = Stack[STKPTR]",
                    "4|R--|return (0,0)"
                );
            ExecTest(Words(0x0CCC),
                "0|R--|000200(2): 4 instructions",
                    "1|L--|WREG = 0xCC<8>",
                    "2|L--|STKPTR = STKPTR - 1<8>",
                    "3|L--|TOS = Stack[STKPTR]",
                    "4|R--|return (0,0)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_RETURN()
        {
            ExecTest(Words(0x0012),
                "0|R--|000200(2): 3 instructions",
                    "1|L--|STKPTR = STKPTR - 1<8>",
                    "2|L--|TOS = Stack[STKPTR]",
                    "3|R--|return (0,0)"
                );
            ExecTest(Words(0x0013),
                "0|R--|000200(2): 6 instructions",
                    "1|L--|STKPTR = STKPTR - 1<8>",
                    "2|L--|TOS = Stack[STKPTR]",
                    "3|L--|BSR = BSR_CSHAD",
                    "4|L--|WREG = WREG_CSHAD",
                    "5|L--|STATUS = STATUS_CSHAD",
                    "6|R--|return (0,0)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_RLCF()
        {
            ExecTest(Words(0x3400),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlcf(Data[FSR2 + 0<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x3401),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlcf(Data[FSR2 + 1<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x34C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlcf(TRISB, C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x3500),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlcf(Data[BSR:0<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x3501),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlcf(Data[BSR:1<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x35C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlcf(Data[BSR:0xC3<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x3601),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = __rlcf(Data[FSR2 + 1<8>:byte], C)",
                    "2|L--|CZN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x36C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = __rlcf(TRISB, C)",
                    "2|L--|CZN = cond(TRISB)"
                );
            ExecTest(Words(0x3701),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = __rlcf(Data[BSR:1<8>:byte], C)",
                    "2|L--|CZN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x37C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = __rlcf(Data[BSR:0xC3<8>:byte], C)",
                    "2|L--|CZN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_RLNCF()
        {
            ExecTest(Words(0x4400),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlncf(Data[FSR2 + 0<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x4401),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlncf(Data[FSR2 + 1<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x44C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlncf(TRISB)",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x4500),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlncf(Data[BSR:0<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x4501),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlncf(Data[BSR:1<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x45C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rlncf(Data[BSR:0xC3<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x4601),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = __rlncf(Data[FSR2 + 1<8>:byte])",
                    "2|L--|ZN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x46C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = __rlncf(TRISB)",
                    "2|L--|ZN = cond(TRISB)"
                );
            ExecTest(Words(0x4701),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = __rlncf(Data[BSR:1<8>:byte])",
                    "2|L--|ZN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x47C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = __rlncf(Data[BSR:0xC3<8>:byte])",
                    "2|L--|ZN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_RRCF()
        {
            ExecTest(Words(0x3000),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrcf(Data[FSR2 + 0<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x3001),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrcf(Data[FSR2 + 1<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x30C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrcf(TRISB, C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x3100),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrcf(Data[BSR:0<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x3101),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrcf(Data[BSR:1<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x31C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrcf(Data[BSR:0xC3<8>:byte], C)",
                    "2|L--|CZN = cond(WREG)"
                );
            ExecTest(Words(0x3201),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = __rrcf(Data[FSR2 + 1<8>:byte], C)",
                    "2|L--|CZN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x32C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = __rrcf(TRISB, C)",
                    "2|L--|CZN = cond(TRISB)"
                );
            ExecTest(Words(0x3301),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = __rrcf(Data[BSR:1<8>:byte], C)",
                    "2|L--|CZN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x33C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = __rrcf(Data[BSR:0xC3<8>:byte], C)",
                    "2|L--|CZN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_RRNCF()
        {
            ExecTest(Words(0x4000),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrncf(Data[FSR2 + 0<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x4001),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrncf(Data[FSR2 + 1<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x40C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrncf(TRISB)",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x4100),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrncf(Data[BSR:0<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x4101),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrncf(Data[BSR:1<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x41C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = __rrncf(Data[BSR:0xC3<8>:byte])",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x4201),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = __rrncf(Data[FSR2 + 1<8>:byte])",
                    "2|L--|ZN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x42C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = __rrncf(TRISB)",
                    "2|L--|ZN = cond(TRISB)"
                );
            ExecTest(Words(0x4301),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = __rrncf(Data[BSR:1<8>:byte])",
                    "2|L--|ZN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x43C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = __rrncf(Data[BSR:0xC3<8>:byte])",
                    "2|L--|ZN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_SETF()
        {
            ExecTest(Words(0x6801),
                "0|L--|000200(2): 1 instructions",
                "1|L--|Data[FSR2 + 1<8>:byte] = 0xFF<8>"
                );

            ExecTest(Words(0x68C4),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|TRISC = 0xFF<8>"
                );

            ExecTest(Words(0x6902),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:2<8>:byte] = 0xFF<8>"
                );

            ExecTest(Words(0x69C3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = 0xFF<8>"
                );


        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_SLEEP()
        {
            ExecTest(Words(0x0003),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|STATUS = STATUS & 0xDF<8>",
                    "2|L--|STATUS = STATUS | 0x40<8>"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_SUBFSR()
        {
            ExecTest(Words(0xE923),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|FSR0 = FSR0 - 0x23<8>"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_SUBFWB()
        {
            ExecTest(Words(0x5400),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG - Data[FSR2 + 0<8>:byte] - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5401),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG - Data[FSR2 + 1<8>:byte] - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x54C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG - TRISB - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5500),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG - Data[BSR:0<8>:byte] - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5501),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG - Data[BSR:1<8>:byte] - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x55C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG - Data[BSR:0xC3<8>:byte] - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5601),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = WREG - Data[FSR2 + 1<8>:byte] - !C",
                    "2|L--|CDCZOVN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x56C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = WREG - TRISB - !C",
                    "2|L--|CDCZOVN = cond(TRISB)"
                );
            ExecTest(Words(0x5701),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = WREG - Data[BSR:1<8>:byte] - !C",
                    "2|L--|CDCZOVN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x57C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = WREG - Data[BSR:0xC3<8>:byte] - !C",
                    "2|L--|CDCZOVN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_SUBLW()
        {
            ExecTest(Words(0x0800),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = 0<8> - WREG",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x0855),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = 0x55<8> - WREG",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_SUBULNK()
        {
            ExecTest(Words(0xE9C6),
                "0|R--|000200(2): 4 instructions",
                "1|L--|FSR2 = FSR2 - 6<8>",
                "2|L--|STKPTR = STKPTR - 1<8>",
                "3|L--|TOS = Stack[STKPTR]",
                "4|R--|return (0,0)"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_SUBWF()
        {
            ExecTest(Words(0x5C00),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 0<8>:byte] - WREG",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5C01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 1<8>:byte] - WREG",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5CC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = TRISB - WREG",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5D00),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0<8>:byte] - WREG",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5D01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:1<8>:byte] - WREG",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5DC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0xC3<8>:byte] - WREG",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5E01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] - WREG",
                    "2|L--|CDCZOVN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x5EC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = TRISB - WREG",
                    "2|L--|CDCZOVN = cond(TRISB)"
                );
            ExecTest(Words(0x5F01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = Data[BSR:1<8>:byte] - WREG",
                    "2|L--|CDCZOVN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x5FC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = Data[BSR:0xC3<8>:byte] - WREG",
                    "2|L--|CDCZOVN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_SUBWFB()
        {
            ExecTest(Words(0x5800),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 0<8>:byte] - WREG - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5801),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + 1<8>:byte] - WREG - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x58C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = TRISB - WREG - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5900),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0<8>:byte] - WREG - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5901),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:1<8>:byte] - WREG - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x59C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[BSR:0xC3<8>:byte] - WREG - !C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            ExecTest(Words(0x5A01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = Data[FSR2 + 1<8>:byte] - WREG - !C",
                    "2|L--|CDCZOVN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x5AC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = TRISB - WREG - !C",
                    "2|L--|CDCZOVN = cond(TRISB)"
                );
            ExecTest(Words(0x5B01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = Data[BSR:1<8>:byte] - WREG - !C",
                    "2|L--|CDCZOVN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x5BC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = Data[BSR:0xC3<8>:byte] - WREG - !C",
                    "2|L--|CDCZOVN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_SWAPF()
        {
            ExecTest(Words(0x3800),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|WREG = __swapf(Data[FSR2 + 0<8>:byte])"
                );
            ExecTest(Words(0x3801),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|WREG = __swapf(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x38C3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|WREG = __swapf(TRISB)"
                );
            ExecTest(Words(0x3900),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|WREG = __swapf(Data[BSR:0<8>:byte])"
                );
            ExecTest(Words(0x3901),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|WREG = __swapf(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x39C3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|WREG = __swapf(Data[BSR:0xC3<8>:byte])"
                );
            ExecTest(Words(0x3A01),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = __swapf(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x3AC3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|TRISB = __swapf(TRISB)"
                );
            ExecTest(Words(0x3B01),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = __swapf(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x3BC3),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = __swapf(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_TBLRD()
        {
            ExecTest(Words(0x0008),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|__tblrd(TBLPTR, 0<8>)"
                );
            ExecTest(Words(0x0009),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|__tblrd(TBLPTR, 1<8>)"
                );
            ExecTest(Words(0x000A),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|__tblrd(TBLPTR, 2<8>)"
                );
            ExecTest(Words(0x000B),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|__tblrd(TBLPTR, 3<8>)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_TBLWT()
        {
            ExecTest(Words(0x000C),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|__tblwt(TBLPTR, 0<8>)"
                );
            ExecTest(Words(0x000D),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|__tblwt(TBLPTR, 1<8>)"
                );
            ExecTest(Words(0x000E),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|__tblwt(TBLPTR, 2<8>)"
                );
            ExecTest(Words(0x000F),
                "0|L--|000200(2): 1 instructions",
                    "1|L--|__tblwt(TBLPTR, 3<8>)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_TSTFSZ()
        {
            ExecTest(Words(0x6600),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[FSR2 + 0<8>:byte] == 0<8>) branch 000204"
                );
            ExecTest(Words(0x6601),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[FSR2 + 1<8>:byte] == 0<8>) branch 000204"
                );
            ExecTest(Words(0x66C3),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (TRISB == 0<8>) branch 000204"
                );
            ExecTest(Words(0x6700),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:0<8>:byte] == 0<8>) branch 000204"
                );
            ExecTest(Words(0x6701),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:1<8>:byte] == 0<8>) branch 000204"
                );
            ExecTest(Words(0x67C3),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[BSR:0xC3<8>:byte] == 0<8>) branch 000204"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_XORLW()
        {
            ExecTest(Words(0x0A00),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ 0<8>",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x0A55),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ 0x55<8>",
                    "2|L--|ZN = cond(WREG)"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_XORWF()
        {
            ExecTest(Words(0x1800),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ Data[FSR2 + 0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1801),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ Data[FSR2 + 1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x18C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ TRISB",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1900),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ Data[BSR:0<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1901),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ Data[BSR:1<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x19C3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(WREG)"
                );
            ExecTest(Words(0x1A01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR2 + 1<8>:byte] = WREG ^ Data[FSR2 + 1<8>:byte]",
                    "2|L--|ZN = cond(Data[FSR2 + 1<8>:byte])"
                );
            ExecTest(Words(0x1AC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|TRISB = WREG ^ TRISB",
                    "2|L--|ZN = cond(TRISB)"
                );
            ExecTest(Words(0x1B01),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:1<8>:byte] = WREG ^ Data[BSR:1<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:1<8>:byte])"
                );
            ExecTest(Words(0x1BC3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[BSR:0xC3<8>:byte] = WREG ^ Data[BSR:0xC3<8>:byte]",
                    "2|L--|ZN = cond(Data[BSR:0xC3<8>:byte])"
                );
        }

        [Test]
        public void PIC18EnhdExtd_Rewriter_Indirect()
        {
            // TSTFSZ INDF2,ACCESS
            ExecTest(Words(0x66DF),
                "0|T--|000200(2): 1 instructions",
                    "1|T--|if (Data[FSR2:byte] == 0<8>) branch 000204"
                );
            // DECF PLUSW2,W,ACCESS
            ExecTest(Words(0x04DB),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR2 + WREG:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            // DECF INDF1,F,ACCESS
            ExecTest(Words(0x06E7),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR1:byte] = Data[FSR1:byte] - 1<8>",
                    "2|L--|CDCZOVN = cond(Data[FSR1:byte])"
                );
            // INCF PLUSW0,W,ACCESS
            ExecTest(Words(0x28EB),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = Data[FSR0 + WREG:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            // INCF INDF0,F,ACCESS
            ExecTest(Words(0x2AEF),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR0:byte] = Data[FSR0:byte] + 1<8>",
                    "2|L--|CDCZOVN = cond(Data[FSR0:byte])"
                );
            // ADDWFC INDF0,W,ACCESS
            ExecTest(Words(0x20EF),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[FSR0:byte] + C",
                    "2|L--|CDCZOVN = cond(WREG)"
                );
            // ANDWF POSTINC1,F,ACCESS
            ExecTest(Words(0x16E6),
                "0|L--|000200(2): 3 instructions",
                    "1|L--|Data[FSR1:byte] = WREG & Data[FSR1:byte]",
                    "2|L--|ZN = cond(Data[FSR1:byte])",
                    "3|L--|FSR1 = FSR1 + 1<16>"
                );
            // ADDWF POSTDEC0,F,ACCESS
            ExecTest(Words(0x26ED),
                "0|L--|000200(2): 3 instructions",
                    "1|L--|Data[FSR0:byte] = WREG + Data[FSR0:byte]",
                    "2|L--|CDCZOVN = cond(Data[FSR0:byte])",
                    "3|L--|FSR0 = FSR0 - 1<16>"
                );
            // SUBWFB PREINC2,F,ACCESS
            ExecTest(Words(0x5ADC),
                "0|L--|000200(2): 3 instructions",
                    "1|L--|FSR2 = FSR2 + 1<16>",
                    "2|L--|Data[FSR2:byte] = Data[FSR2:byte] - WREG - !C",
                    "3|L--|CDCZOVN = cond(Data[FSR2:byte])"
                );
            // IORWF PLUSW1,F,ACCESS
            ExecTest(Words(0x12E3),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|Data[FSR1 + WREG:byte] = WREG | Data[FSR1 + WREG:byte]",
                    "2|L--|ZN = cond(Data[FSR1 + WREG:byte])"
                );
            // MOVFF 0xFD<8>B,0xFE<8>E
            ExecTest(Words(0xCFDB, 0xFFEE),
                "0|L--|000200(4): 1 instructions",
                    "1|L--|Data[0x0FEE<p16>:byte] = Data[0x0FDB<p16>:byte]"
                );
            // MOVFFL PREINC1,POSTDEC0
            ExecTest(Words(0x006F, 0xFF93, 0xFFED),
                "0|L--|000200(6): 3 instructions",
                    "1|L--|FSR1 = FSR1 + 1<16>",
                    "2|L--|Data[FSR0:byte] = Data[FSR1:byte]",
                    "3|L--|FSR0 = FSR0 - 1<16>"
                );
            // MOVFFL PREINC1,PLUSW0
            ExecTest(Words(0x006F, 0xFF93, 0xFFEB),
                "0|L--|000200(6): 2 instructions",
                    "1|L--|FSR1 = FSR1 + 1<16>",
                    "2|L--|Data[FSR0 + WREG:byte] = Data[FSR1:byte]"
                );
            // MOVFFL PREINC1,0x3000<16>
            ExecTest(Words(0x006F, 0xFF93, 0xF000),
                "0|L--|000200(6): 2 instructions",
                    "1|L--|FSR1 = FSR1 + 1<16>",
                    "2|L--|Data[0x3000<p16>:byte] = Data[FSR1:byte]"
                );
            // MOVSF [0x38<8>],0xFE<8>D
            ExecTest(Words(0xEB38, 0xFFED),
                "0|L--|000200(4): 1 instructions",
                    "1|L--|Data[0x0FED<p16>:byte] = Data[FSR2 + 0x38<8>:byte]"
                );
            // MOVSFL [0x38<8>],PREINC1
            ExecTest(Words(0x0002, 0xF0E3, 0xFFE4),
                "0|L--|000200(6): 2 instructions",
                    "1|L--|FSR1 = FSR1 + 1<16>",
                    "2|L--|Data[FSR1:byte] = Data[FSR2 + 0x38<8>:byte]"
                );
            // MOVSS [0x38<8>],[0x83<8>]
            ExecTest(Words(0xEBB8, 0xFA75),
                "0|L--|000200(4): 1 instructions",
                    "1|L--|Data[FSR2 + 0x75<8>:byte] = Data[FSR2 + 0x38<8>:byte]"
                );

        }

        [Test]
        public void PIC18EnhdExtd_Rewriter__Invalid()
        {
            ExecTest(Words(0x0001),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0002),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0002, 0xF000),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 1 instructions",
                    "3|L--|nop"
                );

            ExecTest(Words(0x0002, 0xF000, 0x1234),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 1 instructions",
                    "3|L--|nop",
                "4|L--|000204(2): 2 instructions",
                    "5|L--|Data[FSR2 + 0x34<8>:byte] = WREG | Data[FSR2 + 0x34<8>:byte]",
                    "6|L--|ZN = cond(Data[FSR2 + 0x34<8>:byte])"
                );

            ExecTest(Words(0x0015),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0016),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0017),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0018),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0019),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x001A),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x001B),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x001C),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x001D),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x001E),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x001F),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0020),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0040),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0060),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0067, 0x1234),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 2 instructions",
                    "3|L--|Data[FSR2 + 0x34<8>:byte] = WREG | Data[FSR2 + 0x34<8>:byte]",
                    "4|L--|ZN = cond(Data[FSR2 + 0x34<8>:byte])"
                );

            ExecTest(Words(0x006F, 0xF000),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 1 instructions",
                    "3|L--|nop"
                );

            ExecTest(Words(0x006F, 0xF000, 0x1234),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 1 instructions",
                    "3|L--|nop",
                "4|L--|000204(2): 2 instructions",
                    "5|L--|Data[FSR2 + 0x34<8>:byte] = WREG | Data[FSR2 + 0x34<8>:byte]",
                    "6|L--|ZN = cond(Data[FSR2 + 0x34<8>:byte])"
                );

            ExecTest(Words(0x0080),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x00F0),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0140),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x0180),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0x01E0),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xC000),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xC000, 0x0123),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 1 instructions",
                    "3|L--|BSR = 0x23<8>"
                );

            ExecTest(Words(0xEB00),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xEB00, 0x1234),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 2 instructions",
                    "3|L--|Data[FSR2 + 0x34<8>:byte] = WREG | Data[FSR2 + 0x34<8>:byte]",
                    "4|L--|ZN = cond(Data[FSR2 + 0x34<8>:byte])"
                );

            ExecTest(Words(0xEB80),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xEB80, 0x1234),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 2 instructions",
                    "3|L--|Data[FSR2 + 0x34<8>:byte] = WREG | Data[FSR2 + 0x34<8>:byte]",
                    "4|L--|ZN = cond(Data[FSR2 + 0x34<8>:byte])"
                );

            ExecTest(Words(0xEC00),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xEC00, 0x1234),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 2 instructions",
                    "3|L--|Data[FSR2 + 0x34<8>:byte] = WREG | Data[FSR2 + 0x34<8>:byte]",
                    "4|L--|ZN = cond(Data[FSR2 + 0x34<8>:byte])"
                );

            ExecTest(Words(0xED00),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xED00, 0x989D),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 1 instructions",
                    "3|L--|T5CLK = T5CLK & 0xEF<8>"
                );

            ExecTest(Words(0xEE00),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xEE00, 0x64F3),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|T--|000202(2): 1 instructions",
                    "3|T--|if (PRODL >u WREG) branch 000206"
                );


            ExecTest(Words(0xEE00, 0xF400),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 1 instructions",
                    "3|L--|nop"
                );

            ExecTest(Words(0xEE30, 0xF000),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|L--|000202(2): 1 instructions",
                    "3|L--|nop"
                );

            ExecTest(Words(0xEE40),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xEEF0),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xEF00),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>"
                );

            ExecTest(Words(0xEF00, 0xEDCB),
                "0|---|000200(2): 1 instructions",
                    "1|---|<invalid>",
                "2|---|000202(2): 1 instructions",
                    "3|---|<invalid>"
                );

        }

    }

}
