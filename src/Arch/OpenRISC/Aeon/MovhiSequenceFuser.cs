using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.OpenRISC.Aeon
{
    public class MovhiSequenceFuser : IEnumerable<AeonInstruction>
    {
        private const int ShiftAmount = 16;

        private readonly IEnumerable<AeonInstruction> instructions;

        public MovhiSequenceFuser(IEnumerable<AeonInstruction> instructions)
        {
            this.instructions = instructions;
        }

        public IEnumerator<AeonInstruction> GetEnumerator()
        {
            var dasm = instructions.GetEnumerator();
            while (dasm.MoveNext())
            {
                var instr = dasm.Current;
                if (instr.Mnemonic != Mnemonic.bg_movhi &&
                    instr.Mnemonic != Mnemonic.bn_movhi__ &&
                    instr.Mnemonic != Mnemonic.bt_movhi__)
                {
                    yield return instr;
                    continue;
                }
                var movhi = instr;
                if (!dasm.MoveNext())
                {
                    yield return movhi;
                    yield break;
                }
                instr = dasm.Current;
                Fuse(movhi, instr);
                yield return movhi;
                yield return instr;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// If the l.movhi is followed by a fuseable instruction, then 
        /// perform fusing operations, which mutates the instructions
        /// involved.
        /// </summary>
        /// <param name="movhi">The l.movhi instruction</param>
        /// <param name="instr">Theh instruction following the l.movhi</param>
        private void Fuse(AeonInstruction movhi, AeonInstruction instr)
        {
            var regHi = (RegisterStorage) movhi.Operands[0];
            switch (instr.Mnemonic)
            {
            case Mnemonic.bg_lbs__:
            case Mnemonic.bn_lbz__:
            case Mnemonic.bg_lbz__:
            case Mnemonic.bn_lhz:
            case Mnemonic.bg_lhz__:
            case Mnemonic.bn_lwz:
            case Mnemonic.bg_lwz:
                var memLd = (MemoryOperand) instr.Operands[1];
                if (memLd.Base != regHi)
                    return;
                uint uFullWord = AddFullWord(movhi.Operands[1], memLd.Offset);
                movhi.Operands[1] = Address.Ptr32(uFullWord);
                memLd.IsFullOffset = true;
                memLd.Offset = (int) uFullWord;
                break;
            case Mnemonic.bn_sb__:
            case Mnemonic.bg_sb__:
            case Mnemonic.bn_sh__:
            case Mnemonic.bg_sh:
            case Mnemonic.bt_swst____:
            case Mnemonic.bn_sw:
            case Mnemonic.bg_sw:
                var memSt = (MemoryOperand) instr.Operands[0];
                if (memSt.Base != regHi)
                    return;
                uFullWord = AddFullWord(movhi.Operands[1], memSt.Offset);
                movhi.Operands[1] = Address.Ptr32(uFullWord);
                memSt.IsFullOffset = true;
                memSt.Offset = (int) uFullWord;
                break;
            case Mnemonic.bt_addi__:
            case Mnemonic.bn_addi:
            case Mnemonic.bg_addi:
                var addRegIndex = instr.Operands.Length == 2 ? 0 : 1;
                var addImmIndex = addRegIndex + 1;
                var addReg = (RegisterStorage) instr.Operands[addRegIndex];
                if (addReg != regHi)
                    return;
                var addImm = (Constant) instr.Operands[addImmIndex];
                uFullWord = AddFullWord(movhi.Operands[1], addImm.ToInt32());
                movhi.Operands[1] = Address.Ptr32(uFullWord);
                instr.Operands[addImmIndex] = Constant.Word32(uFullWord);
                break;
            case Mnemonic.bn_ori:
            case Mnemonic.bg_ori:
                var orReg = (RegisterStorage) instr.Operands[1];
                if (orReg != regHi)
                    return;
                var orImm = (Constant) instr.Operands[2];
                uFullWord = OrFullWord(movhi.Operands[1], orImm.ToUInt32());
                movhi.Operands[1] = Address.Ptr32(uFullWord);
                instr.Operands[2] = Constant.Word32(uFullWord);
                break;
            }
        }

        public static uint AddFullWord(MachineOperand movhiImm, int offset)
        {
            var hiImm = (int)((Constant)movhiImm).ToUInt16();
            return (uint) ((hiImm << ShiftAmount) + offset);
        }

        public static uint OrFullWord(MachineOperand movhiImm, uint offset)
        {
            var hiImm = (int) ((Constant)movhiImm).ToUInt16();
            return (uint) ((hiImm << ShiftAmount) + offset);
        }
    }
}
