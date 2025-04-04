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

using Reko.Core.Expressions;
using System.IO;

namespace Reko.Core.Rtl
{
    public sealed class RtlBranch : RtlTransfer
    {
        public RtlBranch(Expression condition, Expression target, InstrClass rtlClass) 
            : base(target, rtlClass)
        {
            this.Condition = condition;
        }

        public Expression Condition { get; }

        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitBranch(this);
        }

        public override T Accept<T, C>(IRtlInstructionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitBranch(this, context);
        }
        protected override void WriteInner(TextWriter writer)
        {
            if (Condition != null)
            {
                writer.Write("if (");
                writer.Write(Condition);
                writer.Write(") ");
            }
            writer.Write("branch {0}", Target);
        }
    }
}
