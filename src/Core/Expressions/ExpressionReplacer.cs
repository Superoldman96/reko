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
using System.Linq;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// An expression visitor that replaces all occurrences of a given expression with a replacement.
    /// </summary>
    public class ExpressionReplacer : ExpressionVisitor<Expression>
    {
        private readonly ExpressionValueComparer cmp;
        private readonly Expression original;
        private readonly Expression replacement;

        private ExpressionReplacer(Expression original, Expression replacement)
        {
            this.original = original;
            this.replacement = replacement;
            this.cmp = new ExpressionValueComparer();
        }

        /// <summary>
        /// Replaces all occurrances of <paramref name="original"/> in 
        /// <paramref name="root"/> with <paramref name="replacement" />.
        /// </summary>
        /// <returns></returns>
        public static Expression Replace(Expression original, Expression replacement, Expression root)
        {
            var rep = new ExpressionReplacer(original, replacement);
            return root.Accept(rep);
        }

        /// <inheritdoc/>
        public Expression VisitAddress(Address addr)
        {
            if (cmp.Equals(addr, original))
                return replacement;
            else
                return addr;
        }

        /// <inheritdoc/>
        public Expression VisitApplication(Application appl)
        {
            if (cmp.Equals(appl, original))
                return replacement;
            var args = appl.Arguments.Select(a => a.Accept(this)).ToArray();
            var fn = appl.Procedure;
            return new Application(fn, appl.DataType, args);
        }

        /// <inheritdoc/>
        public Expression VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            if (cmp.Equals(binExp, original))
                return replacement;
            var left = binExp.Left.Accept(this);
            var right = binExp.Right.Accept(this);
            return new BinaryExpression(binExp.Operator, binExp.DataType, left, right);
        }

        /// <inheritdoc/>
        public Expression VisitCast(Cast cast)
        {
            if (cmp.Equals(cast, original))
                return replacement;
            var exp = cast.Expression.Accept(this);
            return new Cast(cast.DataType, exp);
        }

        /// <inheritdoc/>
        public Expression VisitConditionalExpression(ConditionalExpression cond)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Expression VisitConditionOf(ConditionOf cof)
        {
            if (cmp.Equals(cof, original))
                return replacement;
            var expr = cof.Expression.Accept(this);
            return new ConditionOf(cof.DataType, expr);
        }

        /// <inheritdoc/>
        public Expression VisitConstant(Constant c)
        {
            if (cmp.Equals(c, original))
                return replacement;
            else
                return c;
        }

        /// <inheritdoc/>
        public Expression VisitConversion(Conversion conversion)
        {
            if (cmp.Equals(conversion, original))
                return replacement;
            var expr = conversion.Expression.Accept(this);
            return new Conversion(expr, conversion.SourceDataType, conversion.DataType);
        }

        /// <inheritdoc/>
        public Expression VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Expression VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Expression VisitIdentifier(Identifier id)
        {
            if (cmp.Equals(id, original))
                return replacement;
            else
                return id;
        }

        /// <inheritdoc/>
        public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Expression VisitMemoryAccess(MemoryAccess access)
        {
            if (cmp.Equals(access, original))
                return replacement;
            var ea = access.EffectiveAddress.Accept(this);
            return new MemoryAccess(ea, access.DataType);
        }

        /// <inheritdoc/>
        public Expression VisitMkSequence(MkSequence seq)
        {
            if (cmp.Equals(seq, original))
                return replacement;
            var exprs = seq.Expressions
                .Select(e => e.Accept(this))
                .ToArray();
            return new MkSequence(seq.DataType, exprs);
        }

        /// <inheritdoc/>
        public Expression VisitOutArgument(OutArgument outArgument)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Expression VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Expression VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Expression VisitProcedureConstant(ProcedureConstant pc)
        {
            if (cmp.Equals(pc, original))
                return replacement;
            else
                return pc;
        }

        /// <inheritdoc/>
        public Expression VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Expression VisitSegmentedAddress(SegmentedPointer address)
        {
            if (cmp.Equals(address, original))
                return replacement;
            var sel = address.BasePointer.Accept(this);
            var off = address.Offset.Accept(this);
            return new SegmentedPointer(address.DataType, sel, off);
        }

        /// <inheritdoc/>
        public Expression VisitSlice(Slice slice)
        {
            if (cmp.Equals(slice, original))
                return replacement;
            var exp = slice.Expression.Accept(this);
            return new Slice(slice.DataType, exp, slice.Offset);
        }

        /// <inheritdoc/>
        public Expression VisitStringConstant(StringConstant str)
        {
            if (cmp.Equals(str, original))
                return replacement;
            else
                return str;
        }

        /// <inheritdoc/>
        public Expression VisitTestCondition(TestCondition tc)
        {
            if (cmp.Equals(tc, original))
                return replacement;
            var exp = tc.Expression.Accept(this);
            return new TestCondition(tc.ConditionCode, exp);
        }

        /// <inheritdoc/>
        public Expression VisitUnaryExpression(UnaryExpression unary)
        {
            if (cmp.Equals(unary, original))
                return replacement;
            var exp = unary.Expression.Accept(this);
            return new UnaryExpression(unary.Operator, unary.DataType, exp);
        }
    }
}
