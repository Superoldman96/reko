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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Arch.Cray
{
    public class CrayProcessorState : ProcessorState
    {
        public CrayProcessorState(IProcessorArchitecture arch)
        {
            this.Architecture = arch;
        }

        public override IProcessorArchitecture Architecture { get; }

        public override ProcessorState Clone()
        {
            return new CrayProcessorState(this.Architecture);
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            return InvalidConstant.Create(r.DataType);
        }

        public override void OnAfterCall(FunctionType? sigCallee)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            return new CallSite(0, 0);
        }

        public override void OnProcedureEntered(Address addr)
        {
        }

        public override void OnProcedureLeft(FunctionType procedureSignature)
        {
            throw new System.NotImplementedException();
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
        }
    }
}