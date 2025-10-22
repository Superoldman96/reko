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
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Infineon
{
    public class TriCoreRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly TriCoreArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<TriCoreInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private TriCoreInstruction instr;
        private InstrClass iclass;

        public TriCoreRewriter(TriCoreArchitecture triCoreArchitecture, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = triCoreArchitecture;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new TriCoreDisassembler(arch, rdr).GetEnumerator();
            this.rtls = new();
            this.m = new RtlEmitter(rtls);
            instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.add: RewriteArithmetic(Operator.IAdd, 1, 2); break;
                case Mnemonic.add_a: RewriteLogical(m.IAdd); break;
                case Mnemonic.addi: RewriteAddi(); break;
                case Mnemonic.addih: RewriteAddih(); break;
                case Mnemonic.addih_a: RewriteAddih_a(); break;
                case Mnemonic.adds: RewriteAdds(); break;
                case Mnemonic.addsc_a: RewriteAddsc_a(); break;
                case Mnemonic.and: RewriteLogical(m.And); break;
                case Mnemonic.and_eq: RewriteLogicalBitAccumulate(m.And, m.Eq); break;
                case Mnemonic.and_lt_u: RewriteLogicalBitAccumulate(m.And, m.Ult); break;
                case Mnemonic.andn: RewriteLogical(Andn); break;
                case Mnemonic.bisr: RewriteBisr(); break;
                case Mnemonic.cadd: RewriteCadd(); break;
                case Mnemonic.call:
                case Mnemonic.calli: RewriteCall(); break;
                case Mnemonic.cmov: RewriteCmov(m.Ne0); break;
                case Mnemonic.cmovn: RewriteCmov(m.Eq0); break;
                case Mnemonic.debug: RewriteDebug(); break;
                case Mnemonic.dextr: RewriteDextr(); break;
                case Mnemonic.disable: RewriteDisable(); break;
                case Mnemonic.div: RewriteDiv(m.SDiv, m.SMod); break;
                case Mnemonic.eq: RewriteBool(m.Eq); break;
                case Mnemonic.eq_a: RewriteBool(m.Eq); break;
                case Mnemonic.extr: RewriteExtr(PrimitiveType.Int32); break;
                case Mnemonic.extr_u: RewriteExtr(PrimitiveType.UInt32); break;
                case Mnemonic.ge: RewriteBool(m.Ge); break;
                case Mnemonic.ge_a: RewriteBool(m.Uge); break;
                case Mnemonic.ge_u: RewriteBool(m.Uge); break;
                case Mnemonic.insert: RewriteInsert(); break;
                case Mnemonic.isync: RewriteIsync(); break;
                case Mnemonic.j: RewriteJ(); break;
                case Mnemonic.jeq:
                case Mnemonic.jeq_a: RewriteJ(m.Eq); break;
                case Mnemonic.jge: RewriteJ(m.Ge); break;
                case Mnemonic.jge_u: RewriteJ(m.Uge); break;
                case Mnemonic.ji: RewriteJi(); break;
                case Mnemonic.jl: RewriteJl(); break;
                case Mnemonic.jlt: RewriteJ(m.Lt); break;
                case Mnemonic.jlt_u: RewriteJ(m.Ult); break;
                case Mnemonic.jltz: RewriteJ(m.Lt0); break;
                case Mnemonic.jne: RewriteJ(m.Ne); break;
                case Mnemonic.jnz: RewriteJ(m.Ne0); break;
                case Mnemonic.jnz_a: RewriteJ(m.Ne0); break;
                case Mnemonic.jnz_t: RewriteJz_t(false); break;
                case Mnemonic.jz:
                case Mnemonic.jz_a: RewriteJ(m.Eq0); break;
                case Mnemonic.jz_t: RewriteJz_t(true); break;
                case Mnemonic.ld_a: RewriteLd(PrimitiveType.Word32); break;
                case Mnemonic.ld_b: RewriteLd(PrimitiveType.Int8, PrimitiveType.Int32); break;
                case Mnemonic.ld_bu: RewriteLd(PrimitiveType.Byte, PrimitiveType.UInt32); break;
                case Mnemonic.ld_d: RewriteLd(PrimitiveType.Word64); break;
                case Mnemonic.ld_h: RewriteLd(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.ld_hu: RewriteLd(PrimitiveType.Word16, PrimitiveType.UInt32); break;
                case Mnemonic.ld_w: RewriteLd(PrimitiveType.Word32); break;
                case Mnemonic.lea: RewriteLea(); break;
                case Mnemonic.lha: RewriteLha(); break;
                case Mnemonic.loop: RewriteLoop(); break;
                case Mnemonic.lt: RewriteBool(m.Lt); break;
                case Mnemonic.lt_u: RewriteBool(m.Ult); break;
                case Mnemonic.madd: RewriteMadd(Operator.SMul, Operator.IAdd); break;
                case Mnemonic.madd_u: RewriteMadd(Operator.UMul, Operator.IAdd); break;
                case Mnemonic.madds_u: RewriteMadds(m.UMul, adds_u); break;
                case Mnemonic.max: RewriteIntrinsic(CommonOps.Max.MakeInstance(PrimitiveType.Int32)); break;
                case Mnemonic.max_u: RewriteIntrinsic(CommonOps.Max.MakeInstance(PrimitiveType.UInt32)); break;
                case Mnemonic.mfcr: RewriteMfcr(); break;
                case Mnemonic.min: RewriteIntrinsic(CommonOps.Min.MakeInstance(PrimitiveType.Int32)); break;
                case Mnemonic.min_u: RewriteIntrinsic(CommonOps.Min.MakeInstance(PrimitiveType.UInt32)); break;
                case Mnemonic.mov:
                case Mnemonic.mov_a:
                case Mnemonic.mov_aa:
                case Mnemonic.mov_d: RewriteMov(); break;
                case Mnemonic.mov_u: RewriteMov_u(); break;
                case Mnemonic.movh:
                case Mnemonic.movh_a: RewriteMovh(); break;
                case Mnemonic.mtcr: RewriteMtcr(); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.ne: RewriteBool(m.Ne); break;
                case Mnemonic.ne_a: RewriteBool(m.Ne); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.or: RewriteLogical(m.Or); break;
                case Mnemonic.or_eq: RewriteLogicalBitAccumulate(m.Or, m.Eq); break;
                case Mnemonic.or_ne: RewriteLogicalBitAccumulate(m.Or, m.Ne); break;
                case Mnemonic.orn_t: RewriteLogicalBit(Orn); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.rfe: RewriteRfe(); break;
                case Mnemonic.rsub: RewriteArithmetic(Operator.ISub, 2, 1); break;
                case Mnemonic.sat_b: RewriteSaturate(PrimitiveType.SByte, PrimitiveType.Int32); break;
                case Mnemonic.sel: RewriteSelect(2, 3); break;
                case Mnemonic.seln: RewriteSelect(3, 2); break;
                case Mnemonic.sh: RewriteShift(m.Shr); break;
                case Mnemonic.sha: RewriteShift(m.Sar); break;
                case Mnemonic.st_a: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.st_b: RewriteSt(PrimitiveType.Byte); break;
                case Mnemonic.st_h: RewriteSt(PrimitiveType.Word16); break;
                case Mnemonic.st_w: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.stucx: RewriteStucx(); break;
                case Mnemonic.sub: RewriteArithmetic(Operator.ISub, 1, 2); break;
                case Mnemonic.sub_a: RewriteLogical(m.ISub); break;
                case Mnemonic.xnor_t: RewriteLogicalBit(Xnor); break;
                case Mnemonic.xor: RewriteLogical(m.Xor); break;
                case Mnemonic.xor_t: RewriteLogicalBit(m.Xor); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        private void EmitUnitTest()
        {
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("TriCoreRw", dasm.Current, dasm.Current.Mnemonic.ToString(), rdr, "");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Address AddrOp(int iop)
        {
            return (Address)instr.Operands[iop];
        }

        private Expression Andn(Expression a, Expression b)
        {
            return m.And(a, m.Comp(b));
        }

        private void AssignCcCond(FlagGroupStorage grf, Expression e)
        {
            var flags = binder.EnsureFlagGroup(grf);
            m.Assign(flags, m.Cond(grf.DataType, e));
        }

        private void AssignMaybeExtend(Expression dst, Expression src, DataType? dtConvert)
        {
            if (dtConvert is null)
            {
                m.Assign(dst, src);
            }
            else
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                m.Assign(dst, m.Convert(tmp, tmp.DataType, dtConvert));
            }
        }

        private Expression EffectiveAddress(MemoryOperand mop)
        {
            Expression ea;
            if (mop.Base is null)
            {
                ea = Address.Ptr32((uint) mop.Offset);
            }
            else
            {
                ea = binder.EnsureRegister(mop.Base);
                if (mop.Offset != 0)
                {
                    ea = m.AddSubSignedInt(ea, mop.Offset);
                }
            }
            return ea;
        }

        private Expression Operand(int iop, bool extendConstant = false)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case Constant imm:
                if (extendConstant)
                    return m.Word32(imm.ToInt32());
                return imm;
            case Address addr:
                return addr;
            case SequenceStorage seq:
                return binder.EnsureSequence(seq);
            default:
                throw new NotImplementedException($"TriCore Operand type {op.GetType().Name} not implemented yet.");
            }
        }

        private Expression Orn(Expression a, Expression b)
        {
            return m.Or(a, m.Comp(b));
        }

        private Expression Xnor(Expression a, Expression b)
        {
            return m.Comp(m.Xor(a, b));
        }

        private void RewriteAddi()
        {
            var op = Operand(1);
            int imm = ((Constant) instr.Operands[2]).ToInt16();
            var dst = Operand(0);
            m.Assign(dst, m.IAdd(op, m.Word32(imm)));
            AssignCcCond(Registers.V_SV_AV_SAV, dst);
        }

        private void RewriteAddih()
        {
            var op = Operand(1);
            uint imm = ((Constant)instr.Operands[2]).ToUInt16();
            var dst = Operand(0);
            m.Assign(dst, m.IAdd(op, m.Word32(imm << 16)));
            AssignCcCond(Registers.V_SV_AV_SAV, dst);
        }

        private void RewriteAddih_a()
        {
            var op = Operand(1);
            uint imm = ((Constant)instr.Operands[2]).ToUInt16();
            var dst = Operand(0);
            m.Assign(dst, m.IAdd(op, m.Word32(imm << 16)));
        }

        private void RewriteAdds()
        {
            var left = Operand(0);
            var right = Operand(1, true);
            var dst = Operand(0);
            m.Assign(dst, m.Fn(adds, left, right));
            AssignCcCond(Registers.V_SV_AV_SAV, dst);
        }

    private void RewriteAddsc_a()
        {
            var op1 = Operand(1);
            var op2 = Operand(2);
            var sh = ((Constant)instr.Operands[3]).ToInt32();
            var dst = Operand(0);
            if (sh == 0)
            {
                m.Assign(dst, m.IAdd(op1, op2));
            }
            else
            {
                m.Assign(dst, m.IAdd(op1, m.Shl(op2, sh)));
            }
        }
        
        private void RewriteArithmetic(BinaryOperator type, int iopleft, int iopRight)
        {
            Expression dst;
            if (instr.Operands.Length == 2)
            {
                dst = Operand(0);
                var right = Operand(1, true);
                m.Assign(dst, m.Bin(type, dst, right));
            }
            else
            {
                var left = Operand(iopleft);
                var right = Operand(iopRight, true);
                dst = Operand(0);
                m.Assign(dst, m.Bin(type, left, right));
            }
            AssignCcCond(Registers.V_SV_AV_SAV, dst);
        }

        private void RewriteBool(Func<Expression, Expression, Expression> fn)
        {
            var tmp = binder.CreateTemporary(PrimitiveType.Bool);
            var op1 = Operand(1);
            var op2 = Operand(2, true);
            var dst = Operand(0);
            m.Assign(tmp, fn(op1, op2));
            m.Assign(dst, m.Convert(tmp, tmp.DataType, dst.DataType));
        }

        private void RewriteBisr()
        {
            m.SideEffect(m.Fn(bisr, Operand(0)));
        }

        private void RewriteCadd()
        {
            if (instr.Operands.Length != 3)
            {
                EmitUnitTest();
                m.Invalid();
                iclass = InstrClass.Invalid;
                return;
            }
            var d15 = binder.EnsureRegister(Registers.DataRegisters[15]);
            var reg = Operand(0);
            var addend = Operand(2);
            m.Assign(reg, m.Conditional(
                reg.DataType,
                m.Ne0(d15),
                m.IAdd(reg, addend),
                reg));
        }

        private void RewriteCall()
        {
            m.SideEffect(m.Fn(stucx));
            m.Call(Operand(0), 0);
        }

        private void RewriteCmov(Func<Expression, Expression> fn)
        {
            var op1 = Operand(1);
            var op2 = Operand(2);
            var op0 = Operand(0);
            m.Assign(op0, m.Conditional(op0.DataType, fn(op1), op2, op0));
        }

        private void RewriteDebug()
        {
            m.SideEffect(m.Fn(debug));
        }

        private void RewriteDextr()
        {
            var reg64 = binder.EnsureSequence(
                PrimitiveType.Word64,
                (RegisterStorage) instr.Operands[1],
                (RegisterStorage) instr.Operands[2]);
            var shift = Operand(3);
            var tmp = binder.CreateTemporary(reg64.DataType);
            m.Assign(tmp, m.Shr(reg64, shift));
            m.Assign(Operand(0), m.Slice(tmp, PrimitiveType.Word32));
        }
        private void RewriteDisable()
        {
            m.SideEffect(m.Fn(disable));
        }

        private void RewriteDiv(
            Func<Expression, Expression, Expression> fnDiv,
            Func<Expression, Expression, Expression> fnRem)
        {
            var quo = binder.CreateTemporary(PrimitiveType.Word32);
            var rem = binder.CreateTemporary(PrimitiveType.Word32);
            var num = Operand(1);
            var den = Operand(2);
            var dst = Operand(0);
            m.Assign(quo, fnDiv(num, den));
            m.Assign(rem, fnRem(num, den));
            m.Assign(dst, m.Seq(rem, quo));
            AssignCcCond(Registers.V_SV, quo);
            m.Assign(binder.EnsureFlagGroup(Registers.AV), Constant.False());
        }

        private void RewriteExtr(PrimitiveType dtResult)
        {
            if (instr.Operands[2] is Constant immPos &&
                instr.Operands[3] is Constant immLen)
            {
                var src = Operand(1);
                var dtSlice = PrimitiveType.CreateWord(immLen.ToInt32());
                var tmp = binder.CreateTemporary(dtSlice);
                m.Assign(tmp, m.Slice(src, dtSlice, immPos.ToInt32()));
                m.Assign(Operand(0), m.Convert(tmp, tmp.DataType, dtResult));
                return;
            }
            EmitUnitTest();
            m.Invalid();
            iclass = InstrClass.Invalid;
        }

        private void RewriteLogical(Func<Expression,Expression,Expression> fn)
        {
            if (instr.Operands.Length == 2)
            {
                var left = Operand(0);
                var right = Operand(1, true);
                m.Assign(left, fn(left, right));
            }
            else
            {
                var left = Operand(1);
                var right = Operand(2, true);
                var dst = Operand(0);
                m.Assign(dst, fn(left, right));
            }
        }

        private void RewriteLogicalBit(Func<Expression, Expression, Expression> fn)
        {
            var left = Operand(1);
            var leftBit = ((Constant)instr.Operands[2]).ToInt32();
            left = m.Fn(CommonOps.Bit,left, m.Int32(leftBit));
            var right = Operand(3);
            var rightBit = ((Constant)instr.Operands[4]).ToInt32();
            right = m.Fn(CommonOps.Bit, right, m.Int32(rightBit));
            var bitResult = binder.CreateTemporary(PrimitiveType.Bool);
            m.Assign(bitResult, fn(left, right));
            m.Assign(Operand(0), m.Convert(bitResult, bitResult.DataType, PrimitiveType.UInt32));
        }

        private void RewriteLogicalBitAccumulate(
            Func<Expression, Expression, Expression> fnLogical,
            Func<Expression, Expression, Expression> fnCmp)
        {
            var v1 = binder.CreateTemporary(PrimitiveType.Bool);
            var dst = Operand(0);
            m.Assign(v1, m.Fn(CommonOps.Bit, dst, m.Int32(0)));
            m.Assign(dst, m.Fn(
                CommonOps.WriteBit.MakeInstance(PrimitiveType.Word32, PrimitiveType.Int32),
                dst,
                m.Int32(0),
                fnLogical(v1, fnCmp(Operand(1), Operand(2)))));
        }

        private void RewriteInsert()
        {
            if (instr.Operands.Length == 5)
            {
                m.Assign(
                    Operand(0),
                    m.Fn(
                        insert,
                        Operand(1),
                        Operand(2),
                        Operand(3),
                        Operand(4)));
                return;
            }
            Debug.Fail("nyi");
        }

        private void RewriteIntrinsic(IntrinsicProcedure intrinsic)
        {
            var args = Enumerable.Range(1, instr.Operands.Length - 1)
                .Select(i => Operand(i))
                .ToArray();
            var dst = Operand(0);
            m.Assign(dst, m.Fn(intrinsic, args));
        }

        private void RewriteIsync()
        {
            m.SideEffect(m.Fn(isync));
        }

        private void RewriteJ()
        {
            Debug.Assert(instr.Operands.Length == 1);
            m.Goto(AddrOp(0));
        }

        private void RewriteJ(Func<Expression, Expression> fn)
        {
            var src0 = Operand(0);
            m.Branch(fn(src0), AddrOp(1));
        }

        private void RewriteJ(Func<Expression, Expression, Expression> fn)
        {
            var src0 = Operand(0);
            var src1 = Operand(1);
            m.Branch(fn(src0, src1), AddrOp(2));
        }

        private void RewriteJi()
        {
            var target = (RegisterStorage) instr.Operands[0];
            if (target == Registers.a11)
            {
                this.iclass = InstrClass.Transfer | InstrClass.Return;
                m.Return(0, 0);
            }
            else
            {
                m.Goto(binder.EnsureRegister(target));
            }
        }

        private void RewriteJl()
        {
            Debug.Assert(instr.Operands.Length == 1);
            m.Call(Operand(0), 0);
        }

        private void RewriteJz_t(bool invert)
        {
            var reg = Operand(0);
            var bit = Operand(1);
            Expression fn = m.Fn(CommonOps.Bit, reg, bit);
            if (invert)
            {
                fn = m.Not(fn);
            }
            m.Branch(fn, AddrOp(2));
        }

        private void RewriteLd(PrimitiveType dt, PrimitiveType? dtConvert = default)
        {
            Debug.Assert(instr.Operands.Length == 2);
            var mop = (MemoryOperand) instr.Operands[1];
            if (mop.PostIncrement)
            {
                var ea = binder.EnsureRegister(mop.Base!);
                var value = m.Mem(dt, ea);
                var dst = Operand(0);
                AssignMaybeExtend(dst, value, dtConvert);
                int delta = mop.Offset;
                if (delta == 0)
                    delta = dt.Size;
                m.Assign(ea, m.AddSubSignedInt(ea, delta));
            }
            else
            {
                var ea = EffectiveAddress(mop);
                if (mop.PreIncrement)
                {
                    var areg = binder.EnsureRegister(mop.Base!);
                    m.Assign(areg, ea);
                    ea = areg;
                }
                var value = m.Mem(dt, ea);
                var dst = Operand(0);
                AssignMaybeExtend(dst, value, dtConvert);
            }
        }

        private void RewriteLea()
        {
            var mop = (MemoryOperand) instr.Operands[1];
            var ea = EffectiveAddress(mop);
            var dst = Operand(0);
            m.Assign(dst, ea);
        }

        private void RewriteLha()
        {
            var mop = (MemoryOperand) instr.Operands[1];
            var uAddr = (uint) mop.Offset;
            var dst = Operand(0);
            m.Assign(dst, m.Word32(uAddr << 14));
        }

        private void RewriteLoop()
        {
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            var op0 = Operand(0);
            m.Assign(tmp, op0);
            m.Assign(op0, m.ISub(op0, 1));
            m.Branch(m.Ne0(tmp), AddrOp(1));
        }

        private void RewriteMadd(BinaryOperator mul, BinaryOperator add)
        {
            var op1 = Operand(1);
            var op2 = Operand(2);
            var op3 = Operand(3);
            if (op3 is Constant c)
            {
                op3 = Constant.UInt32(c.ToUInt32());
            }
            var dst = Operand(0);
            m.Assign(dst, m.Bin(add, op1, m.Bin(mul, op2, op3)));
            AssignCcCond(Registers.V_SV_AV_SAV, dst);
        }

        private void RewriteMadds(Func<Expression, Expression, Expression> mul, IntrinsicProcedure add)
        {
            var op1 = Operand(1);
            var op2 = Operand(2);
            var op3 = Operand(3);
            if (op3 is Constant c)
            {
                op3 = Constant.UInt32(c.ToUInt32());
            }
            var dst = Operand(0);
            m.Assign(dst, m.Fn(add, op1, mul(op2, op3)));
            AssignCcCond(Registers.V_SV_AV_SAV, dst);
        }


        private void RewriteMfcr()
        {
            var dst = Operand(0);
            var src = Operand(1);
            m.Assign(dst, m.Fn(mfcr, src));
        }

        private void RewriteMov()
        {
            var dst = Operand(0);
            Expression src;
            if (instr.Operands.Length == 3)
            {
                src = m.Seq(
                    PrimitiveType.Word64,
                    Operand(1),
                    Operand(2));
            }
            else if (instr.Operands[1] is Constant imm)
            {
                src = Constant.Create(dst.DataType, imm.ToInt32());
            }
            else
            {
                if (instr.Operands[0].DataType != instr.Operands[1].DataType)
                    EmitUnitTest();
                src = Operand(1);
            }
            m.Assign(dst, src);
        }

        private void RewriteMov_u()
        {
            Debug.Assert(instr.Operands.Length == 2);
            var dst = Operand(0);
            Expression src;
            var imm = (Constant) instr.Operands[1];
            src = Constant.Create(dst.DataType, imm.ToUInt32());
            m.Assign(dst, src);
        }

        private void RewriteMovh()
        {
            var src = ((Constant)instr.Operands[1]).ToUInt32() << 16;
            var dst = Operand(0);
            m.Assign(dst, m.Word32(src));
        }

        private void RewriteMtcr()
        {
            var src0 = Operand(0);
            var src1 = Operand(1);
            m.SideEffect(m.Fn(mtcr, src0, src1));
        }

        private void RewriteMul()
        {
            Expression src0, src1;
            if (instr.Operands.Length == 2)
            {
                src0 = Operand(0);
                src1 = Operand(1,true);
            }
            else
            {
                src0 = Operand(1);
                src1 = Operand(2, true);
            }
            var dst = Operand(0);
            m.Assign(dst, m.SMul(src0, src1));
            AssignCcCond(Registers.V_SV_AV_SAV, dst);
        }

        private void RewriteRet()
        {
            m.SideEffect(m.Fn(TriCoreRewriter.lducx));
            m.Return(0, 0);
        }

        private void RewriteRfe()
        {
            m.SideEffect(m.Fn(rfe));
            m.Return(0, 0);
        }

        private void RewriteSaturate(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            int iSrc;
            if (instr.Operands.Length == 1)
            {
                iSrc = 0;
            }
            else
            {
                iSrc = 1;
            }
            m.Assign(Operand(0), m.Fn(sat.MakeInstance(dtSrc, dtDst), Operand(iSrc)));
        }

        private void RewriteSelect(int iop1, int iop2)
        {
            var sel = m.Ne0(Operand(1));
            var e1 = Operand(iop1);
            var e2 = Operand(iop2);
            var dst = Operand(0);
            m.Assign(
                dst,
                m.Conditional(dst.DataType, sel, e1, e2));
        }

        private void RewriteShift(Func<Expression, Expression, Expression> rightshift)
        {
            Expression src;
            if (instr.Operands.Length == 2)
            {
                src = Operand(0);
            }
            else
            {
                src = Operand(1);
            }
            var dst = Operand(0);
            if (instr.Operands[^1] is Constant imm)
            {
                var amt = imm.ToInt32();
                if (amt > 0)
                {
                    m.Assign(dst, m.Shl(src, m.Int32(amt)));
                }
                else if (amt < 0)
                {
                    m.Assign(dst, rightshift(src, m.Int32(-amt)));
                }
                else
                {
                    m.Assign(dst, src);
                }
                AssignCcCond(Registers.C_V_SV_AV_SAV, dst);
                return;
            }
            Debug.Fail("Nyi");
        }

        private void RewriteSt(PrimitiveType dt)
        {
            Debug.Assert(instr.Operands.Length == 2);
            var src = Operand(1);
            if (src.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(src, dt, 0));
                src = tmp;
            }
            var mop = (MemoryOperand) instr.Operands[0];
            if (mop.PostIncrement)
            {
                var ea = binder.EnsureRegister(mop.Base!);
                var lvalue = m.Mem(dt, ea);
                m.Assign(lvalue, src);
                int delta = mop.Offset;
                if (delta == 0)
                    delta = dt.Size;
                m.Assign(ea, m.AddSubSignedInt(ea, delta));
            }
            else
            {
                Debug.Assert(!mop.PreIncrement);
                var dst = m.Mem(dt, EffectiveAddress(mop));
                m.Assign(dst, src);
            }
        }

        private void RewriteStucx()
        {
            m.SideEffect(m.Fn(stucx));
        }

        private static readonly IntrinsicProcedure adds = IntrinsicBuilder.Binary("__sat_add_signed", PrimitiveType.Int32);
        private static readonly IntrinsicProcedure adds_u = IntrinsicBuilder.Binary("__sat_add_unsigned", PrimitiveType.UInt32);
        private static readonly IntrinsicProcedure bisr = new IntrinsicBuilder("__begin_isr", true)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure debug = new IntrinsicBuilder("__debug", true)
            .Void();
        private static readonly IntrinsicProcedure disable = new IntrinsicBuilder("__disable", true)
            .Void();
        private static readonly IntrinsicProcedure insert = new IntrinsicBuilder("__insert", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure isync = new IntrinsicBuilder("__isync", true)
            .Void();
        private static readonly IntrinsicProcedure lducx = new IntrinsicBuilder("__load_upper_context", false)
            .Void();
        private static readonly IntrinsicProcedure mfcr = new IntrinsicBuilder("__mfcr", true)
            .Param(PrimitiveType.UInt32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure mtcr = new IntrinsicBuilder("__mtcr", true)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.Word32)
            .Void();

        private static readonly IntrinsicProcedure rfe = new IntrinsicBuilder("__return_from_exception", false)
            .Void();

        private static readonly IntrinsicProcedure sat = new IntrinsicBuilder("__saturate", false)
            .GenericTypes("TIn", "TOut")
            .Params("TIn")
            .Returns("TOut");
        private static readonly IntrinsicProcedure stucx = new IntrinsicBuilder("__store_upper_context", false)
            .Void();

    }
}