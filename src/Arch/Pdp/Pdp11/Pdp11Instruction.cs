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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Pdp.Pdp11
{
    public class Pdp11Instruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public PrimitiveType? DataWidth { get; set; }
        public override int MnemonicAsInteger => (int)Mnemonic;
        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteMnemonic(Mnemonic.ToString());
            RenderOperands(renderer, options);
        }

        protected override void RenderOperand(
            MachineOperand op,
            MachineInstructionRenderer renderer,
            MachineInstructionRendererOptions options)
        {
            if (op is Constant)
            {
                renderer.WriteChar('#');
                op.Render(renderer, options);
            }
            else
            {
                op.Render(renderer, options);
            }
        }
    }
}
