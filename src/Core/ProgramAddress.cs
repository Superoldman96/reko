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

namespace Reko.Core
{
    /// <summary>
    /// An address combined with the program in which it lives.
    /// </summary>
    public class ProgramAddress
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="ProgramAddress"/> class.
        /// </summary>
        /// <param name="program">A <see cref="Program"/> instance.</param>
        /// <param name="addr">An <see cref="Address"/> within the program.</param>
        public ProgramAddress(Program program, Address addr)
        {
            this.Program = program;
            this.Address = addr;
        }

        /// <summary>
        /// The program in which the address lives.
        /// </summary>
        public Program Program { get; }

        /// <summary>
        /// The address in the program.
        /// </summary>
        public Address Address { get; }

    }
}
