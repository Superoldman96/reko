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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using Reko.Core.Memory;
using System;
using Reko.Core.Loading;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    public class GlobalDataWriterTests
    {
        private ByteMemoryArea bmem;
        private Program program;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            sc.AddService<IEventListener>(new FakeDecompilerEventListener());
        }

        private ImageWriter Given_Memory(uint address)
        {
            this.bmem = new ByteMemoryArea(Address.Ptr32(address), new byte[1024]);
            var mem = new LeImageWriter(this.bmem.Bytes);
            return mem;
        }

        private void Given_ProcedureAtAddress(uint address, string name)
        {
            var addr = Address.Ptr32(address);
            program.Procedures.Add(
                addr, 
                new Procedure(program.Architecture, name, addr, program.Architecture.CreateFrame())
            );
        }

        private void Given_Globals(params StructureField[] fields)
        {
            var sc = new ServiceContainer();
            var arch = new Mocks.FakeArchitecture(sc);
            var segmentMap = new SegmentMap(
                    bmem.BaseAddress,
                    new ImageSegment("code", bmem, AccessMode.ReadWriteExecute));
            this.program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                new DefaultPlatform(sc, arch));
            var globalStruct = new StructureType();
            globalStruct.Fields.AddRange(fields);
            var store = program.TypeStore;
            var tvGlobals = new TypeVariable("globals_t", 1) { DataType = globalStruct };
            store.SetTypeVariable(program.Globals, tvGlobals);
            tvGlobals.Class = new EquivalenceClass(tvGlobals)
            {
                DataType = globalStruct
            };
            var ptr = new Pointer(globalStruct, 32);
            tvGlobals.DataType = ptr;
        }

        private void RunTest(string sExp)
        {
            var sw = new StringWriter();
            var formatter = new TextFormatter(sw)
            {
                Indentation = 0,
                UseTabs = false,
            };
            var gdw = new GlobalDataWriter(program, formatter, true, true, sc);
            gdw.Write();
            Assert.AreEqual(sExp, sw.ToString());
        }

        private StructureField Given_Field(int offset, DataType dt)
        {
            return new StructureField(offset, dt);
        }

        [Test]
        public void GdwInt32()
        {
            Given_Memory(0x1000)
                .WriteLeUInt32(0xFFFFFFFF);
            Given_Globals(
                new StructureField(0x1000, PrimitiveType.Int32));

            RunTest(
@"int32 g_dw1000 = -1; // 00001000
");
        }

        [Test]
        public void GdwReal32()
        {
            Given_Memory(0x1000)
                .WriteLeUInt32(0x3F800000); // 1.0F
            Given_Globals(
                Given_Field(0x1000, PrimitiveType.Real32));

            RunTest(
@"real32 g_r1000 = 1.0F; // 00001000
");
        }

        [Test]
        public void GdwTwoFields()
        {
            Given_Memory(0x1000)
                .WriteLeUInt32(0xC0800000)  // -4.0F
                .WriteLeUInt32(0x48);       // 'H'
            Given_Globals(
                Given_Field(0x1000, PrimitiveType.Real32),
                Given_Field(0x1004, PrimitiveType.Char));

            RunTest(
@"real32 g_r1000 = -4.0F; // 00001000
char g_b1004 = 'H'; // 00001004
");
        }

        [Test]
        public void GdwFixedArray()
        {
            Given_Memory(0x1000)
                .WriteLeUInt32(1)
                .WriteLeUInt32(10)
                .WriteLeUInt32(100);
            Given_Globals(
                Given_Field(0x1000, new ArrayType(PrimitiveType.UInt32, 3)));
            RunTest(
@"uint32 g_a1000[3] = // 00001000
{
    0x01,
    0x0A,
    100,
};
");
        }

        [Test]
        public void GdwPointer()
        {
            Given_Memory(0x1000)
                .WriteLeUInt32(0x1008)
                .WriteLeUInt32(0)
                .WriteLeUInt32(1234);
            Given_Globals(
                Given_Field(0x1000, new Pointer(PrimitiveType.Int32, 32)),
                Given_Field(0x1008, PrimitiveType.Int32));

            RunTest(
@"int32 * g_ptr1000 = &g_dw1008; // 00001000
int32 g_dw1008 = 0x04D2; // 00001008
");
        }

        [Test]
        public void GdwStructure()
        {
            Given_Memory(0x1000)
                .WriteLeUInt16(4)
                .WriteLeUInt16(unchecked((ushort)-104));
            var eqStr = new EquivalenceClass(new TypeVariable(2));
            var str = new StructureType
            {
                Name = "Eq_2",
                Fields = {
                    { 0, PrimitiveType.Int16 },
                    { 2, PrimitiveType.Int16 },
                }
            };
            eqStr.DataType = str;
            Given_Globals(
                Given_Field(0x1000, eqStr));
            RunTest(
@"Eq_2 g_t1000 = // 00001000
{
    4,
    -0x0068,
};
");
        }

        [Test]
        public void GdwVisitLinkedList()
        {
            Given_Memory(0x1000)
                .WriteLeUInt32(1)
                .WriteLeUInt32(0x1008)
                .WriteLeUInt32(2)
                .WriteLeUInt32(0x0000)
                .WriteLeUInt32(0x1000);
            var eqLink = new EquivalenceClass(new TypeVariable(2));
            var link = new StructureType
            {
                Name = "Eq_2",
                Fields = {
                    { 0, PrimitiveType.Int32 },
                    { 4, new Pointer(eqLink, 32) }
                }
            };
            eqLink.DataType = link;
            Given_Globals(
                Given_Field(0x1000, eqLink),
                Given_Field(0x1008, eqLink),
                Given_Field(0x1010, new Pointer(eqLink, 32)));
            RunTest(
@"Eq_2 g_t1000 = // 00001000
{
    1,
    &g_t1008,
};
Eq_2 g_t1008 = // 00001008
{
    2,
    null,
};
struct Eq_2 * g_ptr1010 = &g_t1000; // 00001010
");
        }

        [Test]
        public void GdwNullTerminatedString()
        {
            Given_Memory(0x1000)
                .WriteString("Hello, world!", Encoding.UTF8)
                .WriteByte(0);
            Given_Globals(
                Given_Field(0x1000, StringType.NullTerminated(PrimitiveType.Char)));
            RunTest(
@"char g_str1000[] = ""Hello, world!""; // 00001000
");
        }

        [Test]
        public void GdwArrayStrings()
        {
            Given_Memory(0x1000)
                .WriteString("Low", Encoding.UTF8)
                .WriteBytes(0, 5)
                .WriteString("High", Encoding.UTF8)
                .WriteBytes(0, 4)
                .WriteString("Medium", Encoding.UTF8)
                .WriteBytes(0, 2);
            Given_Globals(
                Given_Field(0x1000, new ArrayType(
                    new StringType(
                        PrimitiveType.Char,
                        null,
                        0)
                    { Length = 8 },
                    3)));
            RunTest(
@"char g_a1000[3][8] = // 00001000
{
    ""Low"",
    ""High"",
    ""Medium"",
};
");
        }

        [Test]
        public void GdwDiscoverNewStruct()
        {
            Given_Memory(0x1000)
                .WriteLeUInt32(0x1004)      // points to the two fields below.
                .WriteLeInt32(1)
                .WriteLeInt32(2);
            Given_Globals(
                Given_Field(0x1000, new Pointer(new StructureType("test", 0)
                {
                    Fields =
                    {
                        { 0, PrimitiveType.Int32 },
                        { 4, PrimitiveType.Int32 },
                    }
                }, 32)));
            RunTest(
@"struct test * g_ptr1000 = &g_t1004; // 00001000
struct test g_t1004 = // 00001004
{
    1,
    2,
};
");

        }

        [Test]
        public void GdwTypeReference()
        {
            Given_Memory(0x1000)
               .WriteLeInt32(1)
               .WriteLeInt32(2)
               .WriteLeUInt32(0x2000);
            Given_Globals(
                Given_Field(0x1000, new TypeReference("refTest", new StructureType("_test", 0)
                {
                    Fields =
                    {
                        { 0, PrimitiveType.Int32 },
                        { 4, PrimitiveType.Int32 },
                        { 8, new Pointer(FunctionType.Action(Array.Empty<Identifier>()), 32) }
                    }
                })));
            Given_ProcedureAtAddress(0x2000, "funcTest");
            RunTest(
@"refTest g_t1000 = // 00001000
{
    1,
    2,
    funcTest,
};
");
        }

        [Test]
        public void GdwUnionInts()
        {
            var u = new UnionType("u", null,
                PrimitiveType.UInt32,
                PrimitiveType.Int32);
            Given_Memory(0x1000)
                .WriteLeInt32(2);
            Given_Globals(
                Given_Field(0x1000, u));
            RunTest(
@"union u g_u1000 = // 00001000
{
    2
};
");
        }

        [Test]
        public void GdwLargeBlob()
        {
            var blobType = PrimitiveType.CreateWord(0x50);
            Given_Memory(0x1000)
                .WriteBytes(Enumerable.Range(0x30, 0x0A).Select(b => (byte) b).ToArray());
            Given_Globals(
                Given_Field(0x1000, blobType));
            RunTest(@"word80 g_n1000 = // 00001000
{
    0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 
};
");
        }


        [Test]
        public void GdwNonAsciiChar()
        {
            Given_Memory(0x1000)
                .WriteByte(0x9D);
            Given_Globals(
                Given_Field(0x1000, PrimitiveType.Char));
            RunTest(@"char g_b1000 = '\x9D'; // 00001000
");

        }
    }
}