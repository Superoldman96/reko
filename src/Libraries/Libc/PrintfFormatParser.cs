#region License
/* 
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

using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Hll.C;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Libraries.Libc
{
    /// <summary>
    /// Parses standard C printf format
    /// </summary>
    public class PrintfFormatParser : IVarargsFormatParser
    {
        protected readonly Program program;
        protected readonly Address addr;
        protected int i;
        protected readonly string format;
        protected readonly int intSize;
        protected readonly int longSize;
        protected readonly int doubleSize;
        protected readonly int longDoubleSize;
        protected readonly int pointerSize;
        protected readonly int wordSize;
        private readonly IServiceProvider services;

        public PrintfFormatParser(
            Program program,
            Address addrInstr,
            string format,
            IServiceProvider services)
        {
            this.ArgumentTypes = new List<DataType>();
            this.program = program;
            this.addr = addrInstr;
            this.format = format;
            var platform = program.Platform;

            this.intSize = platform.GetBitSizeFromCBasicType(CBasicType.Int);
            this.longSize = platform.GetBitSizeFromCBasicType(CBasicType.Long);
            this.doubleSize = platform.GetBitSizeFromCBasicType(CBasicType.Double);
            this.longDoubleSize = platform.GetBitSizeFromCBasicType(CBasicType.LongDouble);
            this.wordSize = platform.Architecture.WordWidth.BitSize;
            this.pointerSize = platform.PointerType.BitSize;
            this.services = services;
        }

        public List<DataType> ArgumentTypes { get; private set; }

        public void Parse()
        {
            // %[flags] [width] [.precision] [{h | l | ll | w | I | I32 | I64}] type

            for (this.i = 0; i < format.Length; ++i)
            {
                if (format[i] != '%' || i == format.Length-1)
                    continue;
                char ch = format[++i];
                if (ch == '%')
                    continue;
                // Possible flag?
                SkipFlag();
                SkipNumber();
                if (i < format.Length && format[i] == '.')
                {
                    ++i;
                    SkipNumber();
                }

                var byteSize = CollectSize();

                char domain = CollectDataType();

                DataType dt = MakeDataType(byteSize, domain);
                ArgumentTypes.Add(dt);
            }
        }

        protected enum PrintfSize
        {
            Default,
            HalfHalf,
            Half,
            Long,
            LongLong,
            I32,
            I64,
            Size_t,
        }

        protected virtual DataType MakeDataType(PrintfSize size, char cDomain)
        {
            Domain domain;
            int bitSize = this.intSize;
            switch (cDomain)
            {
            case 'c':
                switch (size)
                {
                case PrintfSize.Long:
                    return PrimitiveType.WChar;
                default:
                    return PrimitiveType.Char;
                }
            case 'o':
            case 'u':
            case 'x':
            case 'X':
                switch (size)
                {
                case PrintfSize.HalfHalf: bitSize = 8; break;
                case PrintfSize.Half: bitSize = 16; break;
                case PrintfSize.Long: bitSize = this.longSize; break;
                case PrintfSize.LongLong: bitSize = 64; break;
                case PrintfSize.I32: bitSize = 32; break;
                case PrintfSize.I64: bitSize = 64; break;
                case PrintfSize.Size_t: bitSize = this.wordSize; break;
                }
                domain = Domain.UnsignedInt;
                break;
            case 'd':
            case 'i':
                switch (size)
                {
                case PrintfSize.HalfHalf: bitSize = 8; break;
                case PrintfSize.Half: bitSize = 16; break;
                case PrintfSize.Long: bitSize = this.longSize; break;
                case PrintfSize.LongLong: bitSize = 64; break;
                case PrintfSize.I32: bitSize = 32; break;
                case PrintfSize.I64: bitSize = 64; break;
                case PrintfSize.Size_t: bitSize = this.wordSize; break;
                }
                domain = Domain.SignedInt;
                break;
            case 'a':
            case 'A':
            case 'e':
            case 'E':
            case 'f':
            case 'F':
            case 'g':
            case 'G':
                switch (size)
                {
                case PrintfSize.LongLong:
                    bitSize = this.longDoubleSize;
                    break;
                default:
                    // floats are promoted to doubles.
                    bitSize = this.doubleSize;
                    break;
                }
                domain = Domain.Real;
                break;
            case 'p':
                bitSize = this.pointerSize;
                domain = Domain.Pointer;
                break;
            case 's':
            case 'b':
                return program.TypeFactory.CreatePointer(
                    size == PrintfSize.Long ? PrimitiveType.WChar : PrimitiveType.Char,
                    this.pointerSize);
            default:
                var el = this.services.RequireService<IEventListener>();
                el.Warn(
                    el.CreateAddressNavigator(program, addr),
                    "The format specifier '%{0}' passed to *printf is not known.", cDomain);
                return new UnknownType();
            }
            return PrimitiveType.Create(domain, bitSize);
        }

        private char CollectDataType()
        {
            if (i < format.Length)
                return format[i];
            else
                return '\0';
        }

        protected virtual PrintfSize CollectSize()
        {
            PrintfSize size = PrintfSize.Default;
            if (i < format.Length-1)
            {
                switch (format[i])
                {
                case 'h':
                    ++i;
                    size = PrintfSize.Half;
                    if (i < format.Length-1 && format[i] == 'h')
                    {
                        ++i;
                        size = PrintfSize.HalfHalf;
                    }
                    break;
                case 'l':
                    ++i;
                    size = PrintfSize.Long;
                    if (i < format.Length - 1 && format[i] == 'l')
                    {
                        ++i;
                        size = PrintfSize.LongLong;
                    }
                    break;
                case 'L':
                    ++i;
                    size = PrintfSize.LongLong;
                    break;
                case 'z':
                    ++i;
                    size = PrintfSize.Size_t;
                    break;
                default:
                    return PrintfSize.Default;
                }
            }
            return size;
        }

        private void SkipNumber()
        {
            if (format[i] == '*')
            {
                var dt = MakeDataType(PrintfSize.Default, 'd'); //$ int
                ArgumentTypes.Add(dt);
                ++i;
                return;
            }
            while (i < format.Length && Char.IsDigit(format[i]))
                ++i;
        }

        private void SkipFlag()
        {
            while (i < format.Length)
            {
                switch (format[i])
                {
                case '-':
                case '+':
                case '0':
                case '#':
                case ' ':
                    ++i;
                    break;
                default:
                    return;
                }
            }
        }
    }
}
