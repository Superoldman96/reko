#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
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

using Reko;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Microchip PIC")]
[assembly: AssemblyDescription("Provides disassembly support for Microchip PIC16/PIC18 architecture")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("C. Hostelet")]
[assembly: AssemblyProduct(AssemblyMetadata.Product)]
[assembly: AssemblyCopyright("Copyright © 2017-2025 Christian Hostelet - based on work from John Källén")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a959c1a2-a622-4600-81ad-29e08545b2f7")]

[assembly: AssemblyVersion(AssemblyMetadata.AssemblyVersion)]
[assembly: AssemblyFileVersion(AssemblyMetadata.AssemblyFileVersion)]
