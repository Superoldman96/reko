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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.PaRisc
{
    public partial class PaRiscRewriter
    {
        private void RewriteAdd(bool setCarry)
        {
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var dst = RewriteOp(2);
            m.Assign(dst, m.IAdd(src1, src2));
            if (setCarry)
            {
                var cf = binder.EnsureFlagGroup(Registers.CF);
                m.Assign(cf, m.Cond(cf.DataType, dst));
            }
            MaybeSkipNextInstruction(InstrClass.ConditionalTransfer, false, dst, null);
        }

        private void RewriteAdd_c()
        {
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var dst = RewriteOp(2);
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(Registers.CF);
            m.Assign(
                dst,
                m.IAdd(
                    m.IAdd(src1, src2),
                                c));
        }


        private void RewriteAddi(bool trapIfCondition)
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(0);
            var dst = RewriteOp(2);
            m.Assign(dst, m.IAdd(src1, src2));
            if (trapIfCondition)
            {
                if (instr.Condition is null)
                {
                    host.Error(instr.Address, "Expected a condition in instruction {0}.", instr);
                    this.iclass = InstrClass.Invalid;
                    m.Invalid();
                    return;
                }
                var zero = Constant.Zero(SizeFromCondition(instr.Condition.Type));
                var c = RewriteCondition(dst, zero);
                if (!c.IsZero)
                {
                    m.BranchInMiddleOfInstruction(c.Invert(), instr.Address + 4, InstrClass.ConditionalTransfer);
                }
                m.SideEffect(m.Fn(trap_intrinsic));
            }
            else
            {
                MaybeSkipNextInstruction(iclass, false, dst, null);
            }
        }

        private void RewriteLogical(Func<Expression,Expression,Expression> fn)
        {
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var dst = RewriteOp(2);
            m.Assign(dst, fn(src1, src2));
            MaybeAnnulNextInstruction(iclass, dst);
        }

        private void RewriteDepw()
        {
            var src = RewriteOp(0);
            var pos = RewriteOp(1);
            var len = ((Constant) instr.Operands[2]).ToInt32();
            var dt = PrimitiveType.CreateWord(len);
            var ins = binder.CreateTemporary(dt);
            m.Assign(ins, m.Slice(src, dt, 0));
            var dst = binder.CreateTemporary(PrimitiveType.Word32);
            var d = RewriteOp(3);

            if (pos is Constant cpos)
            {
                var lePos = 31 - cpos.ToInt32();
                if (instr.Zero)
                {
                    m.Assign(d, Constant.Zero(dst.DataType));
                }
                m.Assign(d, m.Dpb(d, dst, lePos));
            }
            else
            {
                m.Assign(dst, m.Fn(depw_intrinsic, ins, pos));
                m.Assign(d, m.Dpb(d, dst, 0));
            }
            MaybeSkipNextInstruction(instr.InstructionClass, false, dst);
        }

        private void RewriteDepwi()
        {
            var imm = ((Constant)instr.Operands[0]).ToInt32();
            var pos = RewriteOp(1);
            var len = ((Constant)instr.Operands[2]).ToInt32();
            var dst = RewriteOp(3);
                 
            if (pos is Constant cpos)
            {
                var dt = PrimitiveType.CreateWord(len);
                var ins = Constant.Create(dt, imm);
                var lePos = 31 - cpos.ToInt32();
                if (instr.Zero)
                {
                    m.Assign(dst, m.Dpb(Constant.Zero(dst.DataType), ins, lePos));
                }
                else
                {
                    m.Assign(dst, m.Dpb(dst, ins, lePos));
                }
                return;
            }
            throw new NotImplementedException("depwi sar not implemented yet.");
        }

        private void RewriteDs()
        {
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var dst = binder.EnsureIdentifier((RegisterStorage)instr.Operands[2]);
            m.Assign(dst, m.Fn(division_step_intrinsic.MakeInstance(dst.DataType), src1, src2));
        }

        private void RewriteExtrw()
        {
            var src = RewriteOp(0);
            var bePos = ((Constant)instr.Operands[1]).ToInt32();
            var len = ((Constant)instr.Operands[2]).ToInt32();
            var dtSlice = PrimitiveType.CreateWord(len);
            var lePos = 32 - bePos;
            if (lePos < 0)
            {
                host.Warn(instr.Address, "Odd extrw instruction {0}", instr);
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var dst = RewriteOp(3);
            var dt = (instr.Sign == SignExtension.s)
                ? PrimitiveType.Int32
                : PrimitiveType.UInt32;
            m.Assign(dst, m.Convert(m.Slice(src, dtSlice, lePos), dtSlice, dt));
        }

        private void RewriteLd(PrimitiveType size)
        {
            var src = RewriteOp(0);
            var dst = RewriteOp(1);
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                src = m.Convert(src, size, PrimitiveType.Create(Domain.UnsignedInt, dst.DataType.BitSize));
            }
            m.Assign(dst, src);
        }

        private void RewriteLdil()
        {
            var src = RewriteOp(0);
            var dst = RewriteOp(1);
            m.Assign(dst, src);
        }

        private void RewriteLdo()
        {
            var src = (MemoryAccess) RewriteOp(0);
            var dst = RewriteOp(1);
            m.Assign(dst, src.EffectiveAddress);
        }

        private void RewriteLdsid()
        {
            var src = RewriteOp(0);
            var dst = RewriteOp(1);
            m.Assign(dst, src);
        }

        private void RewriteOr()
        {
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var rDst = (RegisterStorage) instr.Operands[2];
            if (rDst == arch.Registers.GpRegs[0])
            {
                // Result thrown away == NOP.
                iclass = InstrClass.Padding | InstrClass.Linear;
                m.Nop();
                return;
            }
            var dst = binder.EnsureRegister(rDst);
            m.Assign(dst, m.IAdd(src1, src2));
            MaybeSkipNextInstruction(iclass, false, dst, null);
        }

        private void RewriteShladd()
        {
            var addend= RewriteOp(0);
            var sh = Constant.Int32(((Constant)instr.Operands[1]).ToInt32());
            Expression e = m.Shl(addend, sh);
            var src = RewriteOp(2);
            e = m.IAdd(src, e);
            var dst = RewriteOp(3);
            m.Assign(dst, e);
            MaybeSkipNextInstruction(iclass, false, e, null);
        }

        private void RewriteShrp(PrimitiveType dt, PrimitiveType dtSeq)
        {
            var rHi = (RegisterStorage) instr.Operands[0];
            var rLo = (RegisterStorage) instr.Operands[1];
            var regp = binder.EnsureSequence(dtSeq, rHi, rLo);
            m.Assign(regp, m.Seq(binder.EnsureRegister(rHi), binder.EnsureRegister(rLo)));
            var shamt = RewriteOp(2);
            var dst = RewriteOp(3);
            m.Assign(dst, m.Slice(m.Shr(regp, shamt), dt));
            MaybeSkipNextInstruction(InstrClass.ConditionalTransfer, false, dst);
        }

        private void RewriteSt(PrimitiveType size)
        {
            var dst = RewriteOp(1);
            var src = RewriteOp(0);
            if (src is Constant cSrc)
            {
                src = Constant.Create(dst.DataType, cSrc.ToInt64());
            }
            else if (src.DataType.BitSize > dst.DataType.BitSize)
            {
                src = m.Slice(src, dst.DataType);
            }
            m.Assign(dst, src);
        }

        private void RewriteSub()
        {
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var dst = RewriteOp(2);
            m.Assign(dst, m.ISub(src1, src2));
            MaybeSkipNextInstruction(iclass, false, dst, null);
        }

        private void RewriteSub_b()
        {
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var dst = RewriteOp(2);
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(Registers.CF);
            m.Assign(
                dst,
                m.ISub(
                    m.ISub(src1, src2),
                                c));
        }

        private void RewriteSubi()
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(0);
            var dst = RewriteOp(2);
            m.Assign(dst, m.ISub(src1, src2));
            MaybeSkipNextInstruction(iclass, false, dst, null);
        }

        private PrimitiveType SizeFromCondition(ConditionType ct)
        {
            switch (ct)
            {
            case ConditionType.Eq:
            case ConditionType.Lt:
            case ConditionType.Le:
            case ConditionType.Nuv:
            case ConditionType.Znv:
            case ConditionType.Sv:
            case ConditionType.Odd:
            case ConditionType.Tr:
            case ConditionType.Ne:
            case ConditionType.Ge:
            case ConditionType.Gt:
            case ConditionType.Uv:
            case ConditionType.Vnz:
            case ConditionType.Nsv:
            case ConditionType.Even:
            case ConditionType.Ult:
            case ConditionType.Ule:
            case ConditionType.Uge:
            case ConditionType.Ugt:
            case ConditionType.Never:
                return PrimitiveType.Word32;
            case ConditionType.Eq64:
            case ConditionType.Lt64:
            case ConditionType.Le64:
            case ConditionType.Nuv64:
            case ConditionType.Znv64:
            case ConditionType.Sv64:
            case ConditionType.Odd64:
            case ConditionType.Ne64:
            case ConditionType.Ge64:
            case ConditionType.Gt64:
            case ConditionType.Uv64:
            case ConditionType.Vnz64:
            case ConditionType.Nsv64:
            case ConditionType.Even64:
            case ConditionType.Ult64:
            case ConditionType.Ule64:
            case ConditionType.Uge64:
            case ConditionType.Ugt64:
            case ConditionType.Never64:
                return PrimitiveType.Word64;
            }
            throw new NotImplementedException($"Condition type {ct} not implemented.");
        }
    }
}
