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

using Reko.Core.Loading;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class ElfSymbol : IBinarySymbol
    {
        public ElfSymbolType Type;
        public ElfSymbolBinding Bind;
        public uint SectionIndex;
        /// <summary>
        /// Size in bytes
        /// </summary>
        public ulong Size; 

        public ElfSymbol(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
        public ulong Value { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.IsNullOrEmpty(Name) ? "?" : Name);
            sb.AppendFormat(" {0:X}", Value);
            return sb.ToString();
        }
    }
}
