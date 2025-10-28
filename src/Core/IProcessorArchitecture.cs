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

using Reko.Core.Analysis;
using Reko.Core.Assemblers;
using Reko.Core.Code;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Abstraction of a CPU architecture.
    /// </summary>
	public interface IProcessorArchitecture
	{
        /// <summary>
        /// Used when building large adds/subs when carry flag is used.
        /// Nullable because not all architectures have condition code
        /// flags.
        /// </summary>
        FlagGroupStorage? CarryFlag { get; }

        /// <summary>
        /// Preferred base used to render numbers.
        /// </summary>
        int DefaultBase { get; }

        /// <summary>
        /// Longer description used to refer to architecture. Typically loaded
        /// from app.config
        /// </summary>
        string? Description { get; set; }

        /// <summary>
        /// The endianness of this processor architecture.
        /// </summary>
        EndianServices Endianness { get; }

        /// <summary>
        /// FPU stack pointer used by this machine, or null if none exists.
        /// </summary>
        RegisterStorage? FpuStackRegister { get; }

        /// <summary>
        /// Size of a pointer into the stack frame (near pointer in x86 real mode)
        /// </summary>
        PrimitiveType FramePointerType { get; }

        /// <summary>
        /// Instruction "granularity" or alignment.
        /// </summary>
        int InstructionBitSize { get; }

        /// <summary>
        /// Size of the smallest addressable memory unit where machine code is stored, in bits.
        /// </summary>
        /// <remarks>
        /// Most modern CPU:s have byte addressability, so this will typically be 8, but
        /// some architectures define separate memory areas for their machine code instructions
        /// with wider word sizes.
        /// </remarks>
        int CodeMemoryGranularity { get; }

        /// <summary>
        /// Size of the smallest addressable memory unit, in bits
        /// </summary>
        /// <remarks>Most modern CPU:s have byte addressability, so this will typically be 8.
        /// </remarks>
        int MemoryGranularity { get; }

        /// <summary>
        /// Some architectures, especially microcomputers, have well-known
        /// procedures and global variables at absolute addresses. These are
        /// available in the MemoryMap.
        /// </summary>
        MemoryMap_v1? MemoryMap { get; set; }

        /// <summary>
        /// Short name used to refer to an architecture.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Pointer size that reaches anywhere in the address space (far
        /// pointer in x86 real mode )
        /// </summary>
        PrimitiveType PointerType { get; }

        /// <summary>
        /// Byte patterns that match procedure prologs on this platform.
        /// </summary>
        MaskedPattern[] ProcedurePrologs { get; }

        /// <summary>
        /// The size of the return address (in bytes) if pushed on stack.
        /// </summary>
        int ReturnAddressOnStack { get; }

        /// <summary>
        /// Access to services from the Reko process.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Stack pointer used by this machine.
        /// </summary>
        RegisterStorage StackRegister { get; set; }

        /// <summary>
        /// Processor's native word size.
        /// </summary>
        PrimitiveType WordWidth { get; }

        /// <summary>
        /// Creates an IEnumerable of disassembled <see cref="MachineInstruction" />s which consumes 
        /// its input from the provided <paramref name="imageReader"/>.
        /// </summary>
        /// <remarks>The IEnumerable lets us use Linq expressions
        /// like Take() on a stream of disassembled instructions.</remarks>
        /// <param name="imageReader">The <see cref="EndianImageReader" /> from
        /// which to read machine code.</param>
        /// <returns>
        /// An <see cref="IEnumerable{MachineInstruction}"/>, which can be 
        /// viewed as a stream of disassembled instructions.
        /// </returns>
        IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader);

        /// <summary>
        /// Creates an instance of a ProcessorState appropriate for this
        /// processor.
        /// </summary>
        /// <returns>An instance of <see cref="ProcessorState"/>.</returns>
		ProcessorState CreateProcessorState();

        /// <summary>
        /// Returns a stream of machine-independent instructions, which it
        /// generates by successively disassembling machine-specific instructions
        /// and rewriting them into one or more machine-independent <see cref="RtlInstruction" />
        /// codes. These are then returned as <see cref="RtlInstructionCluster"/>.
        /// </summary>
        IEnumerable<RtlInstructionCluster> CreateRewriter(
            EndianImageReader rdr,
            ProcessorState state,
            IStorageBinder binder,
            IRewriterHost host);

        /// <summary>
        /// Given a set of addresses, returns a set of address where something
        /// is referring to one of those addresses. The referent may be a
        /// machine instruction calling or jumping to the address, or a 
        /// reference to the address stored in memory.
        /// </summary>
        /// <param name="map">Segment map of valid memory areas.</param>
        /// <param name="rdr">Image reader used to read data.</param>
        /// <param name="knownAddresses">A collection of known addresses.</param>
        /// <param name="flags">Flags controlling the pointer scanner.</param>
        /// <returns>Addresses being referred to.</returns>
        IEnumerable<Address> CreatePointerScanner(
            SegmentMap map,
            EndianImageReader rdr,
            IEnumerable<Address> knownAddresses,
            PointerScannerFlags flags);

        /// <summary>
        /// Creates a <see cref="Frame" /> instance appropriate for this architecture type.
        /// </summary>
        /// <returns></returns>
        Frame CreateFrame();

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred endianness of the processor.
        /// </summary>
        /// <param name="memory">Memory to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <param name="rdr">An <see cref="EndianImageReader"/> of the appropriate endianness</param>
        /// <returns>True if the address was a valid memory address, false if not.</returns>
        bool TryCreateImageReader(IMemory memory, Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor, limited to the specified number of units.
        /// </summary>
        /// <param name="memory">Memory to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <param name="cbUnits">Number of memory units after which stop reading.</param>
        /// <param name="rdr">An <see cref="EndianImageReader"/> of the appropriate endianness</param>
        /// <returns>True if the address was a valid memory address, false if not.</returns>
        bool TryCreateImageReader(IMemory memory, Address addr, long cbUnits, [MaybeNullWhen(false)] out EndianImageReader rdr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred endianness of the processor.
        /// </summary>
        /// <param name="memoryArea">Memory area to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <returns>An <see cref="EndianImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, Address addr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor, limited to the specified number of units.
        /// </summary>
        /// <param name="memoryArea">Memory area to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <param name="cbUnits">Number of memory units after which stop reading.</param>
        /// <returns>An <see cref="EndianImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, Address addr, long cbUnits);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred 
        /// endianness of the processor, limited to the specified offset
        /// range.
        /// </summary>
        /// <param name="memoryArea">Memory area from which to read.</param>
        /// <param name="offsetBegin">Starting offset within the memory area.</param>
        /// <param name="offsetEnd">Ending offset within the memory area.</param>
        /// <returns>An <see cref="EndianImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, long offsetBegin, long offsetEnd);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor.
        /// </summary>
        /// <param name="memoryArea">Program image to read</param>
        /// <param name="offset">offset from the start of the image</param>
        /// <returns>An <see cref="EndianImageReader"/> of the appropriate endianness</returns>
        EndianImageReader CreateImageReader(MemoryArea memoryArea, long offset);

        /// <summary>
        /// Creates an <see cref="ImageWriter" /> with the preferred 
        /// endianness of the processor.
        /// </summary>
        /// <returns>An <see cref="ImageWriter"/> of the appropriate endianness.</returns>
        ImageWriter CreateImageWriter();

        /// <summary>
        /// Creates an <see cref="ImageWriter"/> with the preferred endianness
        /// of the processor, which will write into the given <paramref name="memoryArea"/>
        /// starting at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="memoryArea">Memory area to write to.</param>
        /// <param name="addr">Address to start writing at.</param>
        /// <returns>An <see cref="ImageWriter"/> of the appropriate endianness.</returns>
        ImageWriter CreateImageWriter(MemoryArea memoryArea, Address addr);

        /// <summary>
        /// For a given type <typeparamref name="T"/>, returns this architecture's support
        /// for that type.
        /// </summary>
        /// <typeparam name="T">Type of extension.</typeparam>
        /// <returns>An instance of that extension, or null of this architecture doesn't
        /// support the extension.
        /// </returns>
        T? CreateExtension<T>() where T : class;

        /// <summary>
        /// Given a machine instruction, renders its opcode to a string.
        /// </summary>
        /// <param name="instr">Machine instruction to render.</param>
        /// <param name="rdr">Image reader, positioned at the beginning of the instruction.</param>
        /// <returns>String representation of the instruction's opcode(s).</returns>
        string RenderInstructionOpcode(MachineInstruction instr, EndianImageReader rdr);

        /// <summary>
        /// Creates an <see cref="IAssembler"/> instance which can be used to translate
        /// assembly language to machine code for this processor architecture.
        /// </summary>
        /// <param name="asmDialect">On some processors there are many "dialects" of assembly
        /// language. This parameter allows the caller to select a dialect. Passing null
        /// uses the default, manufacturer dialect.</param>
        /// <returns>An instance implementing the <see cref="IAssembler"/> interface.</returns>
        IAssembler CreateAssembler(string? asmDialect);

        /// <summary>
        /// Reads a value from memory, respecting the processor's endianness. Use this
        /// instead of <see cref="ImageReader"/> when random access of memory is requored.
        /// </summary>
        /// <param name="mem">Memory to read from</param>
        /// <param name="addr">Address to read from</param>
        /// <param name="dt">Data type of the data to be read</param>
        /// <param name="value">The value read from memory, if successful.</param>
        /// <returns>True if the read succeeded, false if the address was out of range.</returns>
        bool TryRead(IMemory mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value);

        /// <summary>
        /// Reads a value from memory, respecting the processor's endianness. Use this
        /// instead of <see cref="ImageReader"/> when random access of memory is required.
        /// </summary>
        /// <param name="mem">Memory area to read from</param>
        /// <param name="addr">Address to read from</param>
        /// <param name="dt">Data type of the data to be read</param>
        /// <param name="value">The value read from memory, if successful.</param>
        /// <returns>True if the read succeeded, false if the address was out of range.</returns>
        bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value);

        /// <summary>
        /// Reads a value from memory and interpret the resulting bits in a way appropriate
        /// for the processor and the given <see cref="PrimitiveType"/>.
        /// </summary>
        /// <param name="rdr"><see cref="EndianImageReader"/> from which to read.</param>
        /// <param name="dt">The <see cref="PrimitiveType"/> of the data to be read.</param>
        /// <param name="value">Variable receiving the read value, if reading was possible.</param>
        /// <returns>True if reading was possible.</returns>
        bool TryRead(EndianImageReader rdr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value);

        /// <summary>
        /// Optionally creates a comparer that compares instructions for equality. 
        /// Normalization means some attributes of the instruction are 
        /// treated as wildcards.
        /// </summary>
        /// <param name="norm"></param>
        /// <returns>An instruction comparer if the architecture supports it.</returns>
        IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm);

        /// <summary>
        /// Creates a frame application builder for this architecture.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        FrameApplicationBuilder CreateFrameApplicationBuilder(
            IStorageBinder binder,
            CallSite site);

        /// <summary>
        /// Create a <see cref="MemoryArea"/> appropriate for storing the
        /// machine code for this processor.
        /// </summary>
        MemoryArea CreateCodeMemoryArea(Address baseAddress, byte[] bytes);

        /// <summary>
        /// Reinterprets a string of raw bits as a floating point number appropriate
        /// for the current architecture.
        /// </summary>
        /// <param name="rawBits">Raw bits to be interpreted.</param>
        /// <returns></returns>
        Constant ReinterpretAsFloat(Constant rawBits);

        /// <summary>
        /// Creates a processor emulator for this architecture.
        /// </summary>
        /// <param name="segmentMap">The memory image containing the program 
        /// image and initial data.
        /// </param>
        /// <param name="envEmulator">Simulated operating system.</param>
        /// <returns>The emulator ready to run.</returns>
        IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator);

        /// <summary>
        /// Provide an architecture-defined <see cref="ICallingConvention"/>.
        /// </summary>
        /// <param name="ccName">The name of the calling convention, if any. Most
        /// architectures define a default calling convention, which is returned
        /// by passing 'null' or the empty string.
        /// </param>
        /// <returns>Returns an instance of <see cref="ICallingConvention"/>, or
        /// null if no calling convention with the name <paramref name="ccName"/> 
        /// is known.
        /// </returns>
        ICallingConvention? GetCallingConvention(string? ccName);

        /// <summary>
        /// Returns a list of all the available mnemonics as strings.
        /// </summary>
        /// <returns>
        /// A <see cref="Dictionary{TKey, TValue}"/> mapping mnemonic names to their 
        /// internal Reko numbers.
        /// </returns>
        SortedList<string, int> GetMnemonicNames();

        /// <summary>
        /// Returns an internal Reko number for a given instruction mnemonic, or
        /// null if none is available.
        /// </summary>
        int? GetMnemonicNumber(string sMnemonic);
        
        /// <summary>
        /// Returns register whose name is <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the register</param>
        /// <returns>A <see cref="RegisterStorage"/> representing the register,
        /// or null if no such register exists.
        /// </returns>
        RegisterStorage? GetRegister(string name);

        /// <summary>
        /// Given a storage domain, returns any sub register occupying the 
        /// given bit range.
        /// </summary>
        /// <param name="domain">The <see cref="StorageDomain"/> of the register.</param>
        /// <param name="range">Bit range inside the <paramref name="domain"/>.</param>
        /// <returns>If an invalid domain is passed, null is returned. If 
        /// the bitrange exceeds the extent of a register, returns null.
        /// Otherwise return the smallest register that completely covers
        /// the <paramref name="range"/>.
        /// </returns>
        RegisterStorage? GetRegister(StorageDomain domain, BitRange range);

        /// <summary>
        /// If the <paramref name="flags"/> parameter consists of multiple sub fields, separate them
        /// into distinct fields.
        /// </summary>
        /// <param name="flags"></param>
        IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags);

        /// <summary>
        /// Returns all registers of this architecture.
        /// </summary>
        /// <returns></returns>
        RegisterStorage[] GetRegisters();

        /// <summary>
        /// Attempts to find a register with name <paramref>name</paramref>
        /// </summary>
        bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg);

        /// <summary>
        /// Get all processor flags of this architecture.
        /// </summary>
        /// <returns></returns>
        FlagGroupStorage[] GetFlags();

        /// <summary>
        /// Returns a <see cref="FlagGroupStorage"/> instance matching the provided 
        /// combination of bitflags in <paramref name="grf"/>.
        /// </summary>
        /// <param name="flagRegister"></param>
        /// <param name="grf"></param>
        /// <returns>A <see cref="FlagGroupStorage" /> instance based on <paramref name="flagRegister"/>
        /// with the flags in <paramref name="grf"/> set to TRUE.
        /// </returns>
        FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf);

        /// <summary>
        /// Given the name of a flag register bit group, returns the corresponding
        /// FlagGroupStorage.
        /// </summary>
        /// <remarks>
        /// This method is used principally in deserialization from text.
        /// A critical assumption here is that all flag groups can be distinguished from each
        /// other. The safest approach seems to be to use single-character flag names for the 
        /// condition code bits which tend to be the most commonly used, and use a string prefix
        /// if bits from other registers are needed.
        /// </remarks>
        /// <param name="name">The combined flag registers to deserialize</param>
        /// <returns>A <see cref="FlagGroupStorage"/> if the name was successfully decoded
        /// as a flag group; otherwise null.</returns>
        FlagGroupStorage? GetFlagGroup(string name);

        /// <summary>
        /// Creates a stack access expression for the given offset and data type.
        /// </summary>
        /// <param name="binder">Storage binder to use.</param>
        /// <param name="cbOffset">Offset from the stack pointer.</param>
        /// <param name="dataType">Data type of the access.</param>
        /// <returns>A stack access, usually a <see cref="MemoryAccess"/>.</returns>
        Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType);


        /// <summary>
        /// Creates a floating point stack access expression for the given offset and data type.
        /// </summary>
        /// <param name="binder">Storage binder to use.</param>
        /// <param name="offset">Offset from the FPU stack pointer.</param>
        /// <param name="dataType">Data type of the access.</param>
        /// <returns>An access to the FPU stack.</returns>
        //$REFACTOR: this should probably live in FrameApplicationBuilder instead.
        Expression CreateFpuStackAccess(IStorageBinder binder, int offset, DataType dataType);  //$REVIEW: generalize these two methods?

        /// <summary>
        /// Attempt to inline the instructions at <paramref name="addrCallee"/>. If the instructions
        /// can be inlined, return them as a list of RtlInstructions.
        /// </summary>
        /// <remarks>
        /// This call is used to inline very short procedures. A specific application
        /// is to handle idioms like:
        /// <code>
        /// call foo
        /// ...
        /// foo proc
        ///    mov ebx,[esp+0]
        ///    ret
        /// </code>
        /// whose purpose is to collect the return address into a register. This idiom is 
        /// commonly used in position independent (PIC) code.
        /// </remarks>
        /// <param name="addrCallee">Address of a procedure that might need inlining.</param>
        /// <param name="addrContinuation">The address at which control should resume after 
        /// the call.</param>
        /// <param name="rdr">Image reader primed to start at <paramref name="addrCallee"/>.
        /// </param>
        /// <param name="binder">A <see cref="IStorageBinder"/> instance used to materialize
        /// actual arguments.</param>
        /// <returns>null if no inlining was performed, otherwise a list of the inlined
        /// instructions.</returns>
        List<RtlInstruction>? InlineCall(Address addrCallee, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder);

        /// <summary>
        /// Returns true if <paramref name="frameOffset"/> refers to a location that could be
        /// an argument to a called procedure.
        /// </summary>
        bool IsStackArgumentOffset(long frameOffset);

        /// <summary>
        /// Reads an address large enough to hold a code address from the given image reader.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="rdr"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state);

        /// <summary>
        /// Creates a segmented address from the given segment and offset.
        /// </summary>
        /// <param name="seg">Segment selector.</param>
        /// <param name="offset">Segment offset.</param>
        /// <returns>The resulting address.</returns>
        Address MakeSegmentedAddress(Constant seg, Constant offset);

        /// <summary>
        /// Given a flag register and a bitmask, returns a string representation
        /// of the bits set.
        /// </summary>
        /// <param name="flagRegister">Backing flag registers.</param>
        /// <param name="prefix">Prefix to use.</param>
        /// <param name="grf">Bit mask to render as a string.</param>
        /// <returns>The resulting string representation.</returns>
        string GrfToString(RegisterStorage flagRegister, string prefix, uint grf);                       // Converts a union of processor flag bits to its string representation

        /// <summary>
        /// Parses an address according to the preferred base of the 
        /// architecture.
        /// </summary>
        /// <param name="txtAddr">String representation of the address.</param>
        /// <param name="addr">Resulting <see cref="Address"/> if <paramref name="txtAddr"/>
        /// is a valid textual representation of an address.</param>
        /// <returns>Returns true if <paramref name="txtAddr"/> is a valid textual
        /// representation of an address, false if not.
        /// </returns>
        bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr);

        /// <summary>
        /// Given a <see cref="Constant"/>, returns an Address of the correct size for this architecture.
        /// </summary>
        /// <param name="c">Constant to be converted to address.</param>
        /// <param name="codeAlign">If true, aligns the address to a valid code address.</param>
        /// <returns>An address.</returns>
        Address MakeAddressFromConstant(Constant c, bool codeAlign);

        /// <summary>
        /// After the program has been loaded, the architecture is given a final
        /// chance to mutate the SegmentMap or any other property.
        /// </summary>
        /// <param name="program">The program to postprocess.</param>
        void PostprocessProgram(Program program);

        /// <summary>
        /// The dictionary contains options that were loaded from the 
        /// configuration file or the executable image. These can be used
        /// to customize the properties of the processor.
        /// </summary>
        /// <param name="options"></param>
        void LoadUserOptions(Dictionary<string, object>? options);

        /// <summary>
        /// Retrieves any settings on the architecture that may need persisting.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object>? SaveUserOptions();
    }

    /// <summary>
    /// Normalize enumeration controls the operation of instruction
    /// comparer. 
    /// </summary>
    [Flags]
    public enum Normalize
    {
        /// <summary>
        /// Match identically
        /// </summary> 
        Nothing,
        /// <summary>
        /// all constants treated as wildcards
        /// </summary>
        Constants,
        /// <summary>
        /// all registers treated as wildcards.
        /// </summary>
        Registers,
    }

    /// <summary>
    /// Abstract base class from which most <see cref="IProcessorArchitecture"/>.
    /// implementations derive.
    /// </summary>
    [Designer("Reko.Gui.Design.ArchitectureDesigner,Reko.Gui")]
    public abstract class ProcessorArchitecture : IProcessorArchitecture
    {
        private RegisterStorage? regStack;
        /// <summary>
        /// Registers indexed by name.
        /// </summary>
        protected IReadOnlyDictionary<string, RegisterStorage>? regsByName;
        /// <summary>
        /// Registers indexed by storage domain.
        /// </summary>
        protected IReadOnlyDictionary<StorageDomain, RegisterStorage>? regsByDomain;

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        /// <param name="services">Object that provides services available in the execution environment.</param>
        /// <param name="archId">Short string identifier for the processor architecture.</param>
        /// <param name="options">A dictionary of architecture options to apply (e.g. processor endianness,
        /// word size, or processor features.)</param>
        /// <param name="regsByName">An optional dictionary of all the CPU's registers,
        /// by their names.</param>
        /// <param name="regsByDomain">An optional dictionary of all the CPU's registers,
        /// ordered by their <see cref="StorageDomain"/>.</param>
        public ProcessorArchitecture(
            IServiceProvider services,
            string archId, 
            Dictionary<string, object> options,
            Dictionary<string, RegisterStorage>? regsByName,
            Dictionary<StorageDomain, RegisterStorage>? regsByDomain)
        {
            //$REVIEW: consider exposing these 4 properties in the constructor,
            // as nothing can work without these values.
            this.Endianness = default!;
            this.FramePointerType = default!;
            this.PointerType = default!;
            this.WordWidth = default!;

            this.ProcedurePrologs = Array.Empty<MaskedPattern>();
            this.Services = services;
            this.Name = archId;
            this.Options = options;
            this.MemoryGranularity = 8; // Most architectures are byte-addressable.
            this.CodeMemoryGranularity = 8; // Most architectures are byte-addressable.
            this.DefaultBase = 16;      // Most architectures display hexadecimal.
            this.regsByName = regsByName;
            this.regsByDomain = regsByDomain;
        }

        /// <inheritdoc/>
        public IServiceProvider Services { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public int DefaultBase { get; set; }

        /// <inheritdoc/>
        public EndianServices Endianness { get; protected set; }

        /// <inheritdoc/>
        public PrimitiveType FramePointerType { get; protected set; }

        /// <inheritdoc/>
        public int MemoryGranularity { get; protected set; }

        /// <inheritdoc/>
        public int CodeMemoryGranularity { get; protected set; }

        /// <inheritdoc/>
        public MemoryMap_v1? MemoryMap { get; set; }

        /// <inheritdoc/>
        public PrimitiveType PointerType { get; protected set; }

        /// <inheritdoc/>
        public MaskedPattern[] ProcedurePrologs { get; internal set; }

        /// <inheritdoc/>
        public PrimitiveType WordWidth { get; protected set; }

        /// <inheritdoc/>
        public Dictionary<string, object> Options { get; protected set; }

        /// <summary>
        /// The size of the return address (in bytes) if pushed on stack.
        /// </summary>
        /// <remarks>
        /// Size of the return address equals to pointer size on the most of
        /// architectures.
        /// </remarks>
        public virtual int ReturnAddressOnStack => PointerType.Size; //$TODO: deal with near/far calls in x86-realmode
        
        /// <inheritdoc />
        public int InstructionBitSize { get; protected set; }

        /// <summary>
        /// The stack register used by the architecture.
        /// </summary>
        /// <remarks>
        /// Many architectures reserve a specific register to be used as a stack
        /// pointer register, but not all. ProcessorArchitecture subclasses for 
        /// architectures that have a predefined register must set this property
        /// in their respective constructors. Architectures that don't have a 
        /// predefined register must leave it null and expect the IPlatform
        /// instance to set this property.
        /// </remarks>
        public RegisterStorage StackRegister
        {
            get
            {
                if (this.regStack is null)
                    throw new InvalidOperationException("This architecture has no stack pointer. The platform must define it.");
                return regStack;
            }
            set { this.regStack = value; }
        }

        /// <inheritdoc/>
        public RegisterStorage? FpuStackRegister { get; protected set; }

        /// <inheritdoc/>
        public FlagGroupStorage? CarryFlag { get; protected set; }

        /// <inheritdoc/>
        public virtual IAssembler CreateAssembler(string? asmDialect) => throw new NotSupportedException("This architecture doesn't support assembly language.");

        /// <inheritdoc/>
        public abstract IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader);

        /// <inheritdoc/>
        public virtual T? CreateExtension<T>() where T : class => default;

        /// <inheritdoc/>
        public Frame CreateFrame() { return new Frame(this, FramePointerType); }

        /// <inheritdoc/>
        public bool TryCreateImageReader(IMemory mem, Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr) => this.Endianness.TryCreateImageReader(mem, addr, out rdr);

        /// <inheritdoc/>
        public bool TryCreateImageReader(IMemory mem, Address addr, long cbUnits, [MaybeNullWhen(false)] out EndianImageReader rdr) => this.Endianness.TryCreateImageReader(mem, addr, cbUnits, out rdr);

        /// <inheritdoc/>
        public EndianImageReader CreateImageReader(MemoryArea mem, Address addr) => this.Endianness.CreateImageReader(mem, addr);

        /// <inheritdoc/>
        public EndianImageReader CreateImageReader(MemoryArea mem, Address addr, long cbUnits) => this.Endianness.CreateImageReader(mem, addr, cbUnits);

        /// <inheritdoc/>
        public EndianImageReader CreateImageReader(MemoryArea mem, long offsetBegin, long offsetEnd) => Endianness.CreateImageReader(mem, offsetBegin, offsetEnd);

        /// <inheritdoc/>
        public EndianImageReader CreateImageReader(MemoryArea mem, long off) => Endianness.CreateImageReader(mem, off);

        /// <inheritdoc/>
        public ImageWriter CreateImageWriter() => Endianness.CreateImageWriter();

        /// <inheritdoc/>
        public ImageWriter CreateImageWriter(MemoryArea mem, Address addr) => Endianness.CreateImageWriter(mem, addr);

        /// <inheritdoc/>
        public bool TryRead(IMemory mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value) => Endianness.TryRead(mem, addr, dt, out value);

        /// <inheritdoc/>
        public bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value) => Endianness.TryRead(mem, addr, dt, out value);

        /// <inheritdoc/>
        public virtual bool TryRead(EndianImageReader rdr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value) => rdr.TryRead(dt, out value);

        /// <inheritdoc/>
        public abstract IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm);

        /// <inheritdoc/>
        public abstract ProcessorState CreateProcessorState();

        /// <inheritdoc/>
        public abstract IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags);

        /// <inheritdoc/>
        public abstract IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host);

        /// <inheritdoc/>
        /// <inheritdoc/>
        public virtual IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException("Emulation has not been implemented for this processor architecture yet.");
        }

        /// <inheritdoc/>
        /// <remarks>
        /// By default, there is no calling convention defined for architectures. Some
        /// manufacturers however, define calling conventions.
        /// </remarks>
        public virtual ICallingConvention? GetCallingConvention(string? name)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual RegisterStorage? GetRegister(string name)
        {
            if (regsByName is null)
                throw new NotImplementedException("Need to provide regsByName.");
            if (regsByName.TryGetValue(name, out var reg))
                return reg;
            else
                return null;
        }

        /// <inheritdoc/>
        public virtual RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            if (regsByDomain is null)
                throw new NotImplementedException("Need to provide regsByDomain.");
            if (regsByDomain.TryGetValue(domain, out var reg) && reg.Covers(range))
                return reg;
            else
                return null;
        }

        /// <inheritdoc/>
        public abstract RegisterStorage[] GetRegisters();

        /// <inheritdoc/>
        public virtual FlagGroupStorage[] GetFlags() => throw new NotImplementedException("GetFlags not implemented this architecture.");

        /// <inheritdoc/>
        public virtual FrameApplicationBuilder CreateFrameApplicationBuilder(
            IStorageBinder binder,
            CallSite site)
        {
            return new FrameApplicationBuilder(this, binder, site);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Most CPU's -- but not all -- are byte-addressed.
        /// </remarks>
        public virtual MemoryArea CreateCodeMemoryArea(Address addr, byte[] bytes)
        {
            return new ByteMemoryArea(addr, bytes);
        }

        /// <summary>
        /// Create a stack access to a variable offset by <paramref name="cbOffset"/>
        /// from the stack pointer
        /// </summary>
        /// <remarks>
        /// This method is the same for all _sane_ architectures. The crazy madness
        /// of x86 segmented memory accesses is dealt with in that processor's 
        /// implementation of this method.
        /// </remarks>
        /// <param name="binder">Storage binder to use.</param>
        /// <param name="cbOffset">Byte offset from the stack pointer.</param>
        /// <param name="dataType">Data type of the resulting access.</param>
        /// <returns>A <see cref="MemoryAccess"/> to a stack offset.</returns>
        public virtual Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            var sp = binder.EnsureRegister(StackRegister);
            return MemoryAccess.Create(sp, cbOffset, dataType);
        }

        /// <inheritdoc/>
        public virtual Expression CreateFpuStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            // Only Intel x86/x87 has a FPU stack
            throw new NotSupportedException();
        }

        /// <summary>
        /// For a particular mnemonic, returns its internal (Reko) number.
        /// </summary>
        /// <returns></returns>
        public abstract int? GetMnemonicNumber(string name);

        /// <summary>
        /// Returns a map of mnemonics to their internal (Reko) numbers.
        /// </summary>
        /// <returns></returns>
        public abstract SortedList<string, int> GetMnemonicNames();

        /// <inheritdoc/>
        public virtual IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            throw new NotImplementedException($"Your architecture must implement {nameof(GetSubFlags)}.");
        }

        /// <inheritdoc/>
        public virtual bool IsStackArgumentOffset(long frameOffset)
        {
            // In the majority of architectures, the stack grows towards lower
            // addresses. Items on the stack previous to a call with have
            // non-negative offsets with respect to the stack/frame pointer.
            return frameOffset >= 0;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Most modern CPU architectures are byte-oriented, and share this
        /// common implementation where the bytes of the machine instruction
        /// are rended as hex digits in chunks corresponding to the instruction
        /// bit size of the CPU.
        /// </remarks>
        public virtual string RenderInstructionOpcode(MachineInstruction instr, EndianImageReader rdr)
        {
            // Assumes byte granularity.
            var bitSize = this.InstructionBitSize;
            var instrSize = PrimitiveType.CreateWord(bitSize);
            var sb = new StringBuilder();
            var numBase = this.DefaultBase;
            int digits = numBase switch
            {
                16 => (bitSize + 3) / 4,
                8 => (bitSize + 2) / 3,
                _ => throw new NotSupportedException($"Unsupported numeric base {this.DefaultBase}.")
            };
            var units = (instr.Length * rdr.CellBitSize) / this.InstructionBitSize;
            for (int i = 0; i < units; ++i)
            {
                if (rdr.TryRead(instrSize, out var v))
                {
                    sb.Append(Convert.ToString((long) v.ToUInt64(), numBase)
                        .PadLeft(digits, '0'));
                    sb.Append(' ');
                }
            }
            return sb.ToString().ToUpperInvariant();
        }
            
        /// <inheritdoc/>
        public virtual bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            if (this.regsByName is null)
                throw new NotImplementedException($"Need a value for {nameof(regsByName)}.");
            return regsByName.TryGetValue(name, out reg);
        }

        /// <inheritdoc/>
        public abstract FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf);

        /// <inheritdoc/>
        public abstract FlagGroupStorage? GetFlagGroup(string name);

        /// <inheritdoc/>
        public abstract string GrfToString(RegisterStorage flagRegister, string prefix, uint grf);

        /// <inheritdoc/>
        public virtual List<RtlInstruction>? InlineCall(Address addrCallee, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual void LoadUserOptions(Dictionary<string, object>? options) { }

        /// <inheritdoc/>
        public abstract Address MakeAddressFromConstant(Constant c, bool codeAlign);

        /// <inheritdoc/>
        public virtual Address MakeSegmentedAddress(Constant seg, Constant offset) { throw new NotSupportedException("This architecture doesn't support segmented addresses."); }

        /// <inheritdoc/>
        public virtual void PostprocessProgram(Program program) { }

        /// <inheritdoc/>
        public abstract Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state);

        /// <inheritdoc/>
        public virtual Constant ReinterpretAsFloat(Constant rawBits)
        {
            // Most platforms -- but certainly not all -- use IEEE 754 float representation.
            if (rawBits.DataType.Size == 4)
            {
                return Constant.FloatFromBitpattern(rawBits.ToInt32());
            }
            else if (rawBits.DataType.Size == 8)
            {
                return Constant.FloatFromBitpattern(rawBits.ToInt64());
            }
            throw new NotImplementedException($"Unsupported IEEE floating point size {rawBits.DataType.BitSize}.");
        }

        /// <inheritdoc/>
        public virtual Dictionary<string, object>? SaveUserOptions() { return null; }

        /// <inheritdoc/>
        public abstract bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr);

        /// <summary>
        /// Reads an integer option from the Options dictionary.
        /// </summary>
        /// <param name="name">Name of the option.</param>
        /// <returns>The option's value, or null if none present.</returns>
        protected int? IntegerOption(string name)
        {
            if (!Options.TryGetValue(name, out var objValue))
                return null;
            if (objValue is int intValue)
                return intValue;
            if (objValue is string sValue && int.TryParse(sValue, out intValue))
                return intValue;
            return null;
        }

        /// <summary>
        /// Reads a string option from the Options dictionary.
        /// </summary>
        /// <param name="name">Name of the option.</param>
        /// <returns>The option's value, or null if none present.</returns>
        protected string? StringOption(string name)
        {
            if (!Options.TryGetValue(name, out var objValue))
                return null;
            return objValue as string;
        }
    }

    /// <summary>
    /// Interface implemented by extensions to the <see cref="IProcessorArchitecture"/> interface.
    /// </summary>
    public interface IExtension { }
}
