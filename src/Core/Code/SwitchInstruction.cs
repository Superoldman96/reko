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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Code
{
    /// <summary>
    /// Models a computed n-way GOTO instruction, which picks one of its <see cref="Targets" />
    /// depending on the evaluated value of the <see cref="Expression"/>.
    /// </summary>
    public class SwitchInstruction : Instruction
    {
        /// <summary>
        /// Constructs an instance of a <see cref="SwitchInstruction"/> instruction.
        /// </summary>
        /// <param name="expr">Condition expression.</param>
        /// <param name="targets">Target blocks.</param>
        public SwitchInstruction(Expression expr, Block[] targets)
        {
            this.Expression = expr;
            this.Targets = targets;
        }

        /// <summary>
        /// Conditional expression that selects which one of the targets to transfer control to.
        /// </summary>
        public Expression Expression { get; set; }

        /// <inheritdoc/>
        public override bool IsControlFlow => true;

        /// <summary>
        /// The array of targets.
        /// </summary>
        public Block[] Targets { get; set; }

        /// <inheritdoc/>
        public override Instruction Accept(InstructionTransformer xform)
        {
            return xform.TransformSwitchInstruction(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitSwitchInstruction(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitSwitchInstruction(this, ctx);
        }

        /// <inheritdoc/>
        public override void Accept(InstructionVisitor v)
        {
            v.VisitSwitchInstruction(this);
        }
    }
}
