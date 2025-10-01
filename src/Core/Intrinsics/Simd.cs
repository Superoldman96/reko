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

using Reko.Core.Operators;

namespace Reko.Core.Intrinsics
{
    /// <summary>
    /// Common Single-Instruction, Multiple Data instructions.
    /// </summary>
    public static class Simd
    {
#pragma warning disable CS1591
        public static readonly IntrinsicProcedure Abs = IntrinsicBuilder.SimdUnary("__simd_abs", CommonOps.Abs);
        public static readonly IntrinsicProcedure Add = IntrinsicBuilder.SimdBinary("__simd_add", Operator.IAdd);
        public static readonly IntrinsicProcedure And = IntrinsicBuilder.SimdBinary("__simd_and", Operator.And);
        public static readonly IntrinsicProcedure FAbs = IntrinsicBuilder.SimdUnary("__simd_fabs", FpOps.fabs);
        public static readonly IntrinsicProcedure FAdd = IntrinsicBuilder.SimdBinary("__simd_fadd", Operator.FAdd);
        public static readonly IntrinsicProcedure FDiv = IntrinsicBuilder.SimdBinary("__simd_fdiv", Operator.FDiv);
        public static readonly IntrinsicProcedure FMul = IntrinsicBuilder.SimdBinary("__simd_fmul", Operator.FMul);
        public static readonly IntrinsicProcedure FNeg = IntrinsicBuilder.SimdUnary("__simd_fneg", Operator.FNeg);
        public static readonly IntrinsicProcedure FSub = IntrinsicBuilder.SimdBinary("__simd_fsub", Operator.FSub);
        public static readonly IntrinsicProcedure Max = IntrinsicBuilder.SimdBinary("__simd_max", CommonOps.Max);
        public static readonly IntrinsicProcedure Min = IntrinsicBuilder.SimdBinary("__simd_min", CommonOps.Min);
        public static readonly IntrinsicProcedure Mul = IntrinsicBuilder.SimdBinary("__simd_mul", Operator.IMul);
        public static readonly IntrinsicProcedure Not = IntrinsicBuilder.SimdUnary("__simd_not", Operator.Comp);
        public static readonly IntrinsicProcedure Shl = new IntrinsicBuilder("__simd_shl", Operator.Shl)
            .GenericTypes("TValue", "TShift")
            .Params("TValue", "TShift")
            .Returns("TValue");
        public static readonly IntrinsicProcedure Sqrt = IntrinsicBuilder.GenericUnary("__simd_sqrt");
        public static readonly IntrinsicProcedure Sub = IntrinsicBuilder.GenericBinary("__simd_sub");
    }
}
