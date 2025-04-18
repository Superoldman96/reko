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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Text.RegularExpressions;

namespace Reko.Typing
{
	/// <summary>
	/// Transform certain expressions to equivalents, to simplify type inference.
	/// Locates array expressions and converts them to array access expressions. 
	/// This simplifies work for later stages of the type inference, when we want
    /// to identify array accesses quickly.
	/// </summary>
	public class ExpressionNormalizer : InstructionTransformer
	{
        private readonly ArrayExpressionMatcher aem;

        public ExpressionNormalizer(PrimitiveType pointerType)
        {
		    this.aem = new ArrayExpressionMatcher(pointerType);
        }

		public override Expression VisitMemoryAccess(MemoryAccess access)
        {
            var ea = access.EffectiveAddress.Accept(this);
            var segptr = ea as SegmentedPointer;
            if (segptr is not null)
                ea = segptr.Offset;
            if (aem.Match(ea))
            {
                if (aem.ArrayPointer is null)
                {
                    aem.ArrayPointer = Constant.Create(
                        PrimitiveType.Create(
                            Domain.Pointer,
                            ea.DataType.BitSize),
                        0);
                }
                return aem.Transform(segptr?.BasePointer, access.DataType);
            }
            Expression newEa;
            if (segptr is not null)
            {
                newEa = ExtendEffectiveAddress(segptr.Offset);
                newEa = new SegmentedPointer(segptr.DataType, segptr.BasePointer, newEa);
            }
            else
            {
                newEa = ExtendEffectiveAddress(ea);
            }
            return new MemoryAccess(access.MemoryId, newEa, access.DataType);
        }

        /// <summary>
        /// Extends an effective address ''id'' to ''id'' + 0. 
        /// </summary>
        /// <remarks>
        /// The purpose here is to extend the effective address to avoid premature typing of id.
        /// If later in the type inference process [[id]] is discovered to be a signed integer, the
        /// decompiler can accomodate that by having the added 0 be [[pointer]] or [[member pointer]].
        /// This is not possible if all we have is the id.
        /// </remarks>
        /// <param name="ea">Effective address.</param>
        /// <returns>A normalized effective address.</returns>
        private Expression ExtendEffectiveAddress(Expression ea)
        {
            switch (ea)
            {
            case BinaryExpression bin:
                if (bin.Operator.Type == OperatorType.IAdd)
                    return ea;
                if (bin.Operator.Type == OperatorType.ISub)
                {
                    if (bin.Right is Constant offset)
                    {
                        offset = offset.Negate();
                        return new BinaryExpression(Operator.IAdd, ea.DataType, bin.Left, offset);
                    }
                    return ea;
                }
                break;
            case Address _:
            case Constant _:
                return ea;
            }
            var w = PrimitiveType.CreateWord(ea.DataType.BitSize);
            var newEa = new BinaryExpression(Operator.IAdd, w, ea, Constant.Zero(w));
            return newEa;
        }

        public override Expression VisitSegmentedAddress(SegmentedPointer address)
        {
            var ea = address.Offset.Accept(this);
            var basePtr = address.BasePointer;
            return new SegmentedPointer(address.DataType, basePtr, ea);
        }

		public void Transform(Program program)
		{
			foreach (Procedure proc in program.Procedures.Values)
			{
                foreach (var stm in proc.Statements)
                {
                    stm.Instruction = stm.Instruction.Accept(this);
                }
			}
		}
	}
}
