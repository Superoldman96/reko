;;; Segment .fini (00000A10)

;; _fini: 00000A10
;;   Called from:
;;     00000A0C (in calloc)
_fini proc
	lui	r28,%hi(FFFF8070)
	addiu	r28,r28,%lo(FFFF8070)
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	sw	ra,001C(sp)
	bal	00000A30
	nop
	lui	r28,%hi(FFFF8050)
	addiu	r28,r28,%lo(FFFF8050)
	addu	r28,r28,ra
	lw	r25,-7FCC(r28)
	addiu	r25,r25,+000006F4
	jalr	ra,r25
	nop
	lw	ra,001C(sp)
	jr	ra
	addiu	sp,sp,+00000020
