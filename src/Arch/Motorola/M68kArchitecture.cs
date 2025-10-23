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

using Reko.Arch.Motorola.M68k;
using Reko.Arch.Motorola.M68k.Disassembler;
using Reko.Arch.Motorola.M68k.Machine;
using Reko.Arch.Motorola.M68k.Rewriter;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Reko.Arch.Motorola
{
    [Designer("Reko.Arch.Motorola.Design.M68kArchitectureDesigner,Reko.Arch.Motorola.Design")]
    public class M68kArchitecture : ProcessorArchitecture
    {
        public M68kArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, Registers.regsByName, [])
        {
            InstructionBitSize = 16;
            Endianness = EndianServices.Big;
            FramePointerType = PrimitiveType.Ptr32;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Word32;
            CarryFlag = Registers.X;     // The X flag is used for long adds on the m68k.
            StackRegister = Registers.a7;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return M68kDisassembler.Create68020(this.Services, rdr);
        }

        public IEnumerable<M68kInstruction> CreateDisassemblerImpl(EndianImageReader rdr)
        {
            return M68kDisassembler.Create68020(this.Services, rdr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new M68kInstructionComparer(norm);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new M68kState(this);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return new M68kPointerScanner(rdr, knownLinAddresses, flags).Select(li => Address.Ptr32(li));
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(
                    v => v.ToString(),
                    v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            if (!Enum.TryParse(name, true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            var reg = Registers.GetRegister(domain - StorageDomain.Register);
            if (reg is { } && reg.Covers(range))
                return reg;
            else
                return null;
        }

        public override RegisterStorage? GetRegister(string name)
        {
            var r = Registers.GetRegister(name);
            if (r == RegisterStorage.None)
                return null;
            return r;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.regs;
        }

        public override FlagGroupStorage[] GetFlags()
        {
            return Registers.flags;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            foreach (var cc in flagRegisters)
            {
                if ((grf & cc.FlagGroupBits) != 0)
                    yield return cc;
            }
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            return Registers.regsByName.TryGetValue(name, out reg);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var fl = new FlagGroupStorage(Registers.ccr, grf, GrfToString(Registers.ccr, "", grf));
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            FlagM grf = 0;
            for (int i = 0; i < name.Length; ++i)
            {
                switch (name[i])
                {
                case 'C': grf |= FlagM.CF; break;
                case 'V': grf |= FlagM.VF; break;
                case 'Z': grf |= FlagM.ZF; break;
                case 'N': grf |= FlagM.NF; break;
                case 'X': grf |= FlagM.XF; break;
                default: throw new ArgumentException($"Flag bit '{name[i]}' is unrecognized.");
                }
            }
            return GetFlagGroup(Registers.ccr, (uint)grf);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new M68kRewriter(this, rdr, (M68kState)state, binder, host);
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~1u;
            return Address.Ptr32(uAddr);
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            if (rdr.TryReadBeUInt32(out var uaddr))
            {
                return Address.Ptr32(uaddr);
            }
            else
            {
                return null;
            }
        }

        private static readonly FlagGroupStorage[] flagRegisters = {
            Registers.C,
            Registers.V,
            Registers.Z,
            Registers.N,
            Registers.X,
        };

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            if (flagregister == Registers.fpsr)
            {
                return "FPUFLAGS";
            }
            StringBuilder sb = new StringBuilder();
            if ((grf & Registers.C.FlagGroupBits) != 0) sb.Append(Registers.C.Name);
            if ((grf & Registers.V.FlagGroupBits) != 0) sb.Append(Registers.V.Name);
            if ((grf & Registers.Z.FlagGroupBits) != 0) sb.Append(Registers.Z.Name);
            if ((grf & Registers.N.FlagGroupBits) != 0) sb.Append(Registers.N.Name);
            if ((grf & Registers.X.FlagGroupBits) != 0) sb.Append(Registers.X.Name);
            return sb.ToString();
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
    }
}