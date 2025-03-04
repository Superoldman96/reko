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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mos6502
{
    public enum Mnemonic
    {
        illegal,

        adc,
        and,
        asl,
        bcc,
        bcs,
        beq,
        bit,
        bmi,
        bne,
        bpl,
        brk,
        bvc,
        bvs,
        clc,
        cld,
        cli,
        clv,
        cmp,
        cpx,
        cpy,
        dec,
        dex,
        dey,
        eor,
        inc,
        inx,
        iny,
        jmp,
        kil,
        lda,
        ldx,
        ldy,
        lsr,
        nop,
        ora,
        php,
        jsr,
        pha,
        pla,
        plp,
        rol,
        ror,
        rti,
        rts,
        sbc,
        sec,
        sed,
        sei,
        slo,
        sta,
        stx,
        sty,
        tax,
        tay,
        txa,
        tsx,
        txs,
        tya,
        cop,
        tsb,

        // 65816 opcodes
        bra,
        brl,
        mvn,
        mvp,
        phb,
        pei,
        pea,
        per,
        phd,
        phk,
        phx,
        phy,
        plb,
        pld,
        plx,
        ply,
        rep,
        rtl,
        sep,
        stp,
        stz,
        tcd,
        tcs,
        tdc,
        trb,
        tsc,
        txy,
        tyx,
        wai,
        wdm,
        xba,
        xce,

    }
}
