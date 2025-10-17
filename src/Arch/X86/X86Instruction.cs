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
using Reko.Core.Types;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Models an X86 instruction.
    /// </summary>
    public class X86Instruction : MachineInstruction
	{
        /// <summary>
        /// Instruction mnemonic.
        /// </summary>
		public Mnemonic Mnemonic { get; set; }

        /// <summary>
        /// Width of the data (if it's a word).
        /// </summary>
        public DataType DataWidth { get; set; }

        /// <summary>
        /// Width of the address mode.
        /// </summary>
        public PrimitiveType AddressWidth { get; set; }

        /// <summary>
        /// <c>rep</c> prefix used in the instruction:
        /// 0 = no prefix, 2 = <c>repnz</c>, 3 = <c>repz</c>.
        /// </summary>
        public int RepPrefix { get; set; }

        public byte OpMask { get; set; }        // EVEX Mask register to use.
        public byte MergingMode { get; set; }   // EVEX merging mode
        public bool Broadcast { get; set; }     // EVEX broadcast flag
        public EvexRoundMode RoundMode { get; set; }
        //$PERF: is it worth it to pack the rarely used bit- and byte-sized fields into a single word?

        /// <summary>
        /// True if instruction does not modify flags -- the Intel APX "NF" bit.
        /// </summary>
        public bool NoFlags { get; set; }

        public X86Instruction(Mnemonic mnemonic, InstrClass iclass, DataType dataWidth, PrimitiveType addrWidth, params MachineOperand [] ops)
		{
			this.Mnemonic = mnemonic;
            this.InstructionClass = iclass;
			this.DataWidth = dataWidth;
			this.AddressWidth = addrWidth;
            this.Operands = ops;
		}

        public override int MnemonicAsInteger => (int) Mnemonic;
        public override string MnemonicAsString => Mnemonic.ToString();


        public string ToString(string syntax)
        {
            var options = new MachineInstructionRendererOptions(syntax: syntax);
            return base.ToString(options);
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var syntax = ChooseSyntax(options);
            syntax.Render(this, renderer, options);
		}

        private X86AssemblyRenderer ChooseSyntax(MachineInstructionRendererOptions options)
        {
            if (string.IsNullOrEmpty(options.Syntax))
                return X86AssemblyRenderer.Intel;
            switch (options.Syntax[0])
            {
            case 'A': case 'a': return X86AssemblyRenderer.Att;
            case 'I': case 'i': return X86AssemblyRenderer.Intel;
            case 'N': case 'n': return X86AssemblyRenderer.Nasm;
            default: return X86AssemblyRenderer.Intel;
            }
        }

        /// <summary>
        /// Returns the condition codes that an instruction modifies.
        /// </summary>
        public static FlagGroupStorage? DefCc(Mnemonic mnemonic)
		{
			switch (mnemonic)
			{
			case Mnemonic.aaa:
			case Mnemonic.aas:
            case Mnemonic.adcx:
				return Registers.C;
			case Mnemonic.aad:
			case Mnemonic.aam:
                return Registers.SZ;
            case Mnemonic.adox:
                return Registers.O;
			case Mnemonic.bt:
			case Mnemonic.bts:
			case Mnemonic.btc:
			case Mnemonic.btr:
				return Registers.C;
			case Mnemonic.clc:
			case Mnemonic.cmc:
			case Mnemonic.stc:
				return Registers.C;
			case Mnemonic.cld:
			case Mnemonic.std:
				return Registers.D;
			case Mnemonic.daa:
			case Mnemonic.das:
                return Registers.SCZ;
			case Mnemonic.adc:
			case Mnemonic.add:
			case Mnemonic.sbb:
			case Mnemonic.sub:
			case Mnemonic.cmp:
			case Mnemonic.cmpsb:
			case Mnemonic.cmps:
			case Mnemonic.div:
			case Mnemonic.idiv:
			case Mnemonic.imul:
			case Mnemonic.mul:
			case Mnemonic.scas:
			case Mnemonic.scasb:
                return Registers.SCZO;
			case Mnemonic.dec:
			case Mnemonic.inc:
			case Mnemonic.neg:
                return Registers.SZO;
			case Mnemonic.rcl:
			case Mnemonic.rcr:
			case Mnemonic.rol:
			case Mnemonic.ror:
				return Registers.CO;		// if shift count > 1, FlagM.OF is not set.
			case Mnemonic.sar:
			case Mnemonic.shl:
			case Mnemonic.shr:
				return Registers.SCZO;	// if shift count > 1, FlagM.OF is not set.
			case Mnemonic.shld:
			case Mnemonic.shrd:
                return Registers.SCZ;
			case Mnemonic.bsf:
			case Mnemonic.bsr:
				return Registers.Z;
			case Mnemonic.and:
			case Mnemonic.andn:
			case Mnemonic.or:
            case Mnemonic.sahf:
            case Mnemonic.xadd:
            case Mnemonic.xor:
                return Registers.SCZO;
            case Mnemonic.test:
                return Registers.SCZOP;
			default:
				return null;
			}
		}

        /// <summary>
        /// Returns the condition codes an instruction uses.
        /// </summary>
        public static FlagGroupStorage? UseCc(Mnemonic mnemonic)
		{
			switch (mnemonic)
			{
			case Mnemonic.adc:
			case Mnemonic.adcx:
			case Mnemonic.sbb:
				return Registers.C;
			case Mnemonic.daa:
			case Mnemonic.das:
				return Registers.C;
			case Mnemonic.ins:
			case Mnemonic.insb:
			case Mnemonic.lods:
			case Mnemonic.lodsb:
			case Mnemonic.movs:
			case Mnemonic.movsb:
			case Mnemonic.outs:
			case Mnemonic.outsb:
			case Mnemonic.scas:
			case Mnemonic.scasb:
			case Mnemonic.stos:
			case Mnemonic.stosb:
				return Registers.D;
            case Mnemonic.cmova:
            case Mnemonic.ja:
			case Mnemonic.seta:
                return Registers.CZ;
            case Mnemonic.cmovbe:
            case Mnemonic.jbe:
			case Mnemonic.setbe:
				return Registers.CZ;
            case Mnemonic.cmovc:
            case Mnemonic.jc:
			case Mnemonic.setc:
				return Registers.C;
            case Mnemonic.cmovg:
            case Mnemonic.jg:
			case Mnemonic.setg:
				return Registers.SZO;
            case Mnemonic.cmovge:
            case Mnemonic.jge:
			case Mnemonic.setge:
				return Registers.SO;
			case Mnemonic.cmovl:
            case Mnemonic.jl:
            case Mnemonic.setl:
				return Registers.SO;
            case Mnemonic.cmovle:
            case Mnemonic.jle:
			case Mnemonic.setle:
                return Registers.SZO;
            case Mnemonic.cmovnc:
            case Mnemonic.jnc:
            case Mnemonic.setnc:
				return Registers.C;
            case Mnemonic.cmovno:
            case Mnemonic.jno:
			case Mnemonic.setno:
				return Registers.O;
            case Mnemonic.cmovns:
            case Mnemonic.jns:
			case Mnemonic.setns:
				return Registers.S;
            case Mnemonic.cmovnz:
            case Mnemonic.jnz:
			case Mnemonic.setnz:
				return Registers.Z;
            case Mnemonic.cmovo:
            case Mnemonic.jo:
			case Mnemonic.seto:
				return Registers.O;
            case Mnemonic.cmovpe:
            case Mnemonic.jpe:
			case Mnemonic.setpe:
            case Mnemonic.cmovpo:
            case Mnemonic.jpo:
			case Mnemonic.setpo:
                return Registers.P;
            case Mnemonic.cmovs:
            case Mnemonic.js:
			case Mnemonic.sets:
				return Registers.S;
            case Mnemonic.cmovz:
            case Mnemonic.jz:
			case Mnemonic.setz:
				return Registers.Z;
			case Mnemonic.lahf:
				return Registers.SCZO;
			case Mnemonic.loope:
			case Mnemonic.loopne:
				return Registers.Z;
			case Mnemonic.rcl:
			case Mnemonic.rcr:
				return Registers.C;
			default:
				return null;
			}
		}
	}
}
