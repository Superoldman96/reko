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

using Moq;
using Reko.Analysis;
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Environments.Msdos;
using Reko.Loading;
using Reko.Scanning;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Decompiler.Structure
{
    public class StructureTestBase
	{
		protected Program program;
        private ServiceContainer sc;

        protected Program RewriteProgramMsdos(string sourceFilename, Address addrBase)
		{
            var cfgSvc = new Mock<IConfigurationService>();
            var env = new Mock<PlatformDefinition>();
            var tlSvc = new Mock<ITypeLibraryLoaderService>();
            cfgSvc.Setup(c => c.GetEnvironment("ms-dos")).Returns(env.Object);
            cfgSvc.Setup(c => c.GetSignatureFiles()).Returns(new List<SignatureFileDefinition>());
            env.Setup(e => e.TypeLibraries).Returns(new List<TypeLibraryDefinition>());
            env.Setup(e => e.CharacteristicsLibraries).Returns(new List<TypeLibraryDefinition>());
            sc = new ServiceContainer();
            sc.AddService<IConfigurationService>(cfgSvc.Object);
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            var decompilerEventListener = new FakeDecompilerEventListener();
            sc.AddService<IEventListener>(decompilerEventListener);
            sc.AddService<IDecompilerEventListener>(decompilerEventListener);
            sc.AddService<IFileSystemService>(new FileSystemService());
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
            sc.AddService<ITypeLibraryLoaderService>(tlSvc.Object);
            var ldr = new Loader(sc);
            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());

            program = ldr.AssembleExecutable(
                ImageLocation.FromUri(FileUnitTester.MapTestPath(sourceFilename)),
                new X86TextAssembler(arch),
                new MsdosPlatform(sc, arch),
                addrBase);
            return RewriteProgram();
		}

        protected Program RewriteProgram32(string sourceFilename, Address addrBase)
        {
            sc = new ServiceContainer();
            sc.AddService<IConfigurationService>(new FakeDecompilerConfiguration());
            sc.AddService<IFileSystemService>(new FileSystemService());
            var eventListener = new FakeDecompilerEventListener();
            sc.AddService<IEventListener>(eventListener);
            sc.AddService<IDecompilerEventListener>(eventListener);
            var ldr = new Loader(sc);
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            program = ldr.AssembleExecutable(
                ImageLocation.FromUri(FileUnitTester.MapTestPath(sourceFilename)),
                new X86TextAssembler(arch),
                new DefaultPlatform(sc, arch),
                addrBase);
            return RewriteProgram();
        }

        protected Program RewriteX86RealFragment(string asmFragment, Address addrBase)
        {
            sc = new ServiceContainer();
            var eventListener = new FakeDecompilerEventListener();
            sc.AddService<IEventListener>(eventListener);
            sc.AddService<IDecompilerEventListener>(eventListener);
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
            var asm = new X86TextAssembler(new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>()));
            program = asm.AssembleFragment(addrBase, asmFragment);
            program.Platform = new DefaultPlatform(sc, program.Architecture);
            program.EntryPoints.Add(
                addrBase,
                ImageSymbol.Procedure(program.Architecture,addrBase));
            return RewriteProgram();
        }

        protected Program RewriteX86_32Fragment(string asmFragment, Address addrBase)
        {
            sc = new ServiceContainer();
            var eventListener = new FakeDecompilerEventListener();
            sc.AddService<IEventListener>(eventListener);
            sc.AddService<IDecompilerEventListener>(eventListener);
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());

            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            var asm = new X86TextAssembler(arch);
            program = asm.AssembleFragment(addrBase, asmFragment);
            program.Platform = new DefaultPlatform(sc, program.Architecture);
            program.EntryPoints.Add(
                addrBase,
                ImageSymbol.Procedure(arch, addrBase));
            return RewriteProgram();
        }

        private Program RewriteProgram()
        {
            var dynamicLinker = new Mock<IDynamicLinker>();
            var scan = new Scanner(
                program,
                new TypeLibrary(),
                dynamicLinker.Object,
                sc);
            foreach (ImageSymbol ep in program.EntryPoints.Values)
            {
                scan.EnqueueImageSymbol(ep, true);
            }
            scan.ScanImage();

            var dfa = new DataFlowAnalysis(program, dynamicLinker.Object, sc);
            dfa.AnalyzeProgram();

            return program;
        }
	}
}
