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

using Reko.Arch.OpenRISC.Aeon;
using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Arch.OpenRISC
{
    public class AeonArchitecture : ProcessorArchitecture
    {
        public AeonArchitecture(
            IServiceProvider services,
            string archId,
            Dictionary<string, object> options)
            : base(services, archId, options, Registers.ByName, Registers.ByDomain)
        {
            this.LoadUserOptions(options);
            this.CarryFlag = Registers.CY;
            this.FramePointerType = PrimitiveType.Ptr32;
            base.InstructionBitSize = 8;    // GCD of 24 and 32-bit instuctions
            base.MemoryGranularity = 8;
            base.PointerType = PrimitiveType.Ptr32;
            base.StackRegister = Registers.GpRegisters[1];
            base.WordWidth = PrimitiveType.Word32;
        }

        public override int ReturnAddressOnStack => 0;

        public override IAssembler CreateAssembler(string? asmDialect)
        {
            return new Aeon.Assembler.AeonTextAssembler(this);
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            var dasm = new Aeon.AeonDisassembler(this, imageReader);
            return new MovhiSequenceFuser(dasm);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            return new Aeon.AeonEmulator(this, segmentMap, envEmulator);
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            return [];
        }

        public override ProcessorState CreateProcessorState()
        {
            return new AeonProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new AeonRewriter(this, rdr, state, binder, host);
        }

        public override ICallingConvention? GetCallingConvention(string? name)
        {
            return new AeonCallingConvention(this);
        }

        public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var fl = new FlagGroupStorage(Registers.SR, grf, GrfToString(flagRegister, "", grf));
            return fl;
        }

        public override FlagGroupStorage? GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            if (flags == Registers.F)
            {
                return new[] { flags }; 
            }
            return Array.Empty<FlagGroupStorage>();
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            StringBuilder s = new StringBuilder();
            if (flagRegister == Registers.SR)
            {
                if ((grf & Registers.F.FlagGroupBits) != 0) s.Append(Registers.F.Name);
            }
            return s.ToString();
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            if (options is null)
                return;
            Endianness = 
                options.TryGetValue(ProcessorOption.Endianness, out var oEndianness) &&
                oEndianness is string endianness &&
                endianness.StartsWith("b", StringComparison.InvariantCultureIgnoreCase)
                ? EndianServices.Big
                : EndianServices.Little;
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
        /*
Abi registers
* #define sp r1
#define lr r9
#define fp r10

#define p0 r3
#define p1 r4
#define p2 r5
#define p3 r6
#define p4 r7
#define p5 r8
        */
    }
}
