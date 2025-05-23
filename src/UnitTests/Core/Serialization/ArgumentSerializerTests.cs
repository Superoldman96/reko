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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ArgumentSerializerTests
    {
        private ArgumentSerializer argser;

        [SetUp]
        public void Setup()
        {
            argser = new ArgumentSerializer(null);
        }

        [Test]
        public void ArgSer_SerializeNullArgument()
        {
            Assert.IsNull(ArgumentSerializer.Serialize(null, false));
        }

        [Test]
        public void ArgSer_SerializeRegister()
        {
            var arg = new Identifier(Registers.ax.Name, Registers.ax.DataType, Registers.ax);
            Argument_v1 sarg = ArgumentSerializer.Serialize(arg, false);
            Assert.AreEqual("ax", sarg.Name);
            Register_v1 sreg = (Register_v1)sarg.Kind;
            Assert.IsNotNull(sreg);
            Assert.AreEqual("ax", sreg.Name);
        }

        [Test]
        public void ArgSer_SerializeFlag()
        {
            var arg = Identifier.Create(
                new FlagGroupStorage(Registers.eflags, 3, "SZ"));
            Argument_v1 sarg = ArgumentSerializer.Serialize(arg, false);
            Assert.AreEqual("SZ", sarg.Name);
            FlagGroup_v1 sflag = (FlagGroup_v1)sarg.Kind;
            Assert.AreEqual("SZ", sflag.Name);
        }

        [Test]
        public void ArgSer_SerializeOutArgument()
        {
            Identifier id = new Identifier(
                "qOut",
                PrimitiveType.Word32,
                RegisterStorage.Reg32("q", 4));
            Argument_v1 arg = ArgumentSerializer.Serialize(id, true);
            Assert.AreEqual("qOut", arg.Name);
            Assert.IsTrue(arg.OutParameter);
            Register_v1 sr = (Register_v1)arg.Kind;
            Assert.AreEqual("q", sr.Name);
        }

    }
}
