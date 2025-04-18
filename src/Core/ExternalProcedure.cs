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

using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.IO;

namespace Reko.Core
{
    /// <summary>
    /// Models a procedure in an external API, whose signature is known, but 
    /// whose code is not included in decompilation.
    /// </summary>
	public class ExternalProcedure : ProcedureBase
	{
        /// <summary>
        /// Creates an instance of the <see cref="ExternalProcedure"/> class.
        /// </summary>
        /// <param name="name">The name of the external procedure.</param>
        /// <param name="signature">The function signature of the procedure.</param>
        /// <param name="chars">Optional procedure characteristics.</param>
        public ExternalProcedure(
            string name,
            FunctionType signature,
            ProcedureCharacteristics? chars = null) : base(name, true)
        {
            this.Signature = signature ?? throw new ArgumentNullException(
                nameof(signature),
                $"External procedure {name} must have a signature.");
            if (chars != null)
            {
                this.Characteristics = chars;
            }
        }

        /// <summary>
        /// The function signature of the external procedure.
        /// </summary>
		public override FunctionType Signature { get; set; }

        /// <inheritdoc />
		public override string ToString()
		{
			var sw = new StringWriter();
            var fmt = new TextFormatter(sw);
            var cf = new CodeFormatter(fmt);
            var tf = new TypeReferenceFormatter(fmt);
			Signature.Emit(Name, FunctionType.EmitFlags.ArgumentKind, fmt, cf, tf);
			return sw.ToString();
		}
	}
}
