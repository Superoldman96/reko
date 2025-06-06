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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// An (external) module is described by a name and the services it 
    /// provides.
    /// </summary>
    public class ModuleDescriptor
    {
        /// <summary>
        /// Constructs an instance of a module descriptor.
        /// </summary>
        /// <param name="name"></param>
        public ModuleDescriptor(string name)
        {
            this.ModuleName = name;
            this.ServicesByName = new Dictionary<string, SystemService>();
            this.ServicesByOrdinal = new Dictionary<int, SystemService>();
            this.ServicesByVector = new Dictionary<int, List<SystemService>>();
            this.GlobalsByName = new Dictionary<string, ImageSymbol>();
            this.GlobalsByOrdinal = new Dictionary<int, ImageSymbol>();
        }

        /// <summary>
        /// Constructs a copy of an existing module descriptor.
        /// </summary>
        /// <param name="other">Other module descriptor.</param>
        public ModuleDescriptor(ModuleDescriptor other)
        {
            this.ModuleName = other.ModuleName;
            this.ServicesByName = new Dictionary<string, SystemService>(other.ServicesByName);
            this.ServicesByOrdinal = new Dictionary<int, SystemService>(other.ServicesByOrdinal);
            this.ServicesByVector = other.ServicesByVector.ToDictionary(
                de => de.Key,
                de => de.Value.ToList());
            this.GlobalsByName = new Dictionary<string, ImageSymbol>();
            this.GlobalsByOrdinal = new Dictionary<int, ImageSymbol>();
        }

        /// <summary>
        /// The name of the module. This is typically the name of a DLL.
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// The list of system services provided by this module, sorted by name.
        /// </summary>
        public IDictionary<string, SystemService> ServicesByName { get; private set; }

        /// <summary>
        /// The list of system services provided by this module, sorted by ordinal.
        /// </summary>
        public IDictionary<int, SystemService> ServicesByOrdinal { get; private set; }

        /// <summary>
        /// The list of system services provided by this module, sorted by service vector.
        /// </summary>
        public IDictionary<int, List<SystemService>> ServicesByVector { get; private set; }

        /// <summary>
        /// The list of global variables provided by this module, sorted by name.
        /// </summary>
        public IDictionary<string, ImageSymbol> GlobalsByName { get; private set; }

        /// <summary>
        /// The list of global variables provided by this module, sorted by ordinal.
        /// </summary>
        public IDictionary<int, ImageSymbol> GlobalsByOrdinal { get; private set; }

        /// <summary>
        /// Creates a copy of this module descriptor.
        /// </summary>
        public ModuleDescriptor Clone()
        {
            return new ModuleDescriptor(this);
        }
    }
}
