// pngpixel_text.c
// Generated by decompiling pngpixel
// using Reko decompiler version 0.12.1.0.

#include "pngpixel.h"

// 0000000000400CD0: void _start(Register (ptr64 Eq_n) rdx, Stack word32 dwArg00, Stack (ptr64 char) ptrArg08)
void _start(void (* rdx)(), word32 dwArg00, char * ptrArg08)
{
	__align_stack<word64>(&ptrArg08);
	word64 qwArg00;
	void * fp;
	__libc_start_main(&g_t4012F9, (int32) qwArg00, &ptrArg08, &g_t401780, &g_t4017F0, rdx, fp);
	__halt();
}

// 0000000000400D00: void deregister_tm_clones()
// Called from:
//      __do_global_dtors_aux
void deregister_tm_clones()
{
	if (true || true)
		return;
	null();
}

// 0000000000400D40: void register_tm_clones()
// Called from:
//      frame_dummy
void register_tm_clones()
{
	if (true || true)
		return;
	null();
}

// 0000000000400D80: void __do_global_dtors_aux()
void __do_global_dtors_aux()
{
	if (g_b602108 == 0x00)
	{
		deregister_tm_clones();
		g_b602108 = 0x01;
	}
}

// 0000000000400DA0: void frame_dummy()
void frame_dummy()
{
	if (g_qw601E10 != 0x00 && false)
	{
		fn0000000000000000();
		register_tm_clones();
	}
	else
		register_tm_clones();
}

// 0000000000400DC6: Register word32 component(Register word64 rdi, Register uint32 esi, Register word32 edx, Register Eq_n ecx, Register int32 r8d)
// Called from:
//      print_pixel
word32 component(word64 rdi, uint32 esi, word32 edx, Eq_n ecx, int32 r8d)
{
	word64 rcx;
	ecx = (word32) rcx;
	Eq_n eax_n = (edx + (esi & 0x3F) *s r8d) *s ecx;
	struct Eq_n * v18_n = rdi + ((uint64) (((esi >> 0x06) *s r8d) *s ecx) << 0x03) + (uint64) (eax_n >> 0x03);
	if (ecx > 0x10)
	{
l0000000000400EC1:
		fprintf(stderr, "pngpixel: invalid bit depth %u\n", ecx);
		exit(1);
	}
	else
	{
		uint64 rax_n;
		switch (ecx)
		{
		case 0x00:
		case 0x03:
		case 0x05:
		case 0x06:
		case 0x07:
		case 0x09:
		case 0x0A:
		case 11:
		case 0x0C:
		case 0x0D:
		case 0x0E:
		case 0x0F:
			goto l0000000000400EC1;
		case 0x01:
			rax_n = (uint64) ((word32) v18_n->b0000 >> 0x07 - ((byte) eax_n & 0x07) & 0x01);
			break;
		case 0x02:
			rax_n = (uint64) ((word32) v18_n->b0000 >> 0x06 - ((byte) eax_n & 0x07) & 0x03);
			break;
		case 0x04:
			rax_n = (uint64) ((word32) v18_n->b0000 >> 0x04 - ((byte) eax_n & 0x07) & 0x0F);
			break;
		case 0x08:
			rax_n = (uint64) v18_n->b0000;
			break;
		case 0x10:
			rax_n = (uint64) ((word32) v18_n->b0001 + ((word32) v18_n->b0000 << 0x08));
			break;
		}
		return (word32) rax_n;
	}
}

// 0000000000400EE9: void print_pixel(Register uint32 ecx, Register word64 rdx, Register word64 rsi, Register word64 rdi, Register (ptr32 Eq_n) fs)
// Called from:
//      main
void print_pixel(uint32 ecx, word64 rdx, word64 rsi, word64 rdi, struct Eq_n * fs)
{
	word64 rax_n;
	word64 rax_n = fs->qw0028;
	Eq_n eax_n = (word32) (byte) png_get_bit_depth(rdi, rsi, rsi, ecx, rdx, rsi, rdi, rax_n);
	word64 rcx_n;
	png_get_color_type((word32) rcx_n, rdi, rsi, rsi, eax_n);
	up32 eax_n = (word32) (byte) rax_n;
	if (eax_n <= 0x06)
	{
		switch (eax_n)
		{
		case 0x00:
			printf("GRAY %u\n", component(rdx, ecx, 0x00, eax_n, 0x01));
			break;
		case 0x01:
		case 0x05:
			goto l00000000004012C9;
		case 0x02:
			printf("RGB %u %u %u\n", component(rdx, ecx, 0x00, eax_n, 0x03), component(rdx, ecx, 0x01, eax_n, 0x03), component(rdx, ecx, 0x02, eax_n, 0x03));
			break;
		case 0x03:
			uint32 eax_n = component(rdx, ecx, 0x00, eax_n, 0x01);
			ptr64 fp;
			png_get_PLTE(fp - 80, rdi, fp - 64, rsi, 0x00, eax_n, 0x00);
			word64 rax_n;
			if (((word32) rax_n & 0x08) != 0x00 && (false && false))
			{
				if (((word32) png_get_tRNS(0x00, 0x00, fp - 76, rdi, fp - 56, rsi, 0x00, 0x00) & 0x10) != 0x00 && (false && false))
				{
					int32 esi_n;
					if (eax_n < 0x00)
						esi_n = (word32) null[(uint64) eax_n];
					else
						esi_n = 0xFF;
					printf("INDEXED %u = %d %d %d %d\n", eax_n, (word32) null[(uint64) eax_n].b0000, (word32) ((Eq_n[]) 1)[(uint64) eax_n].b0000, (word32) ((Eq_n[]) 2)[(uint64) eax_n].b0000, esi_n);
				}
				else
					printf("INDEXED %u = %d %d %d\n", eax_n, (word32) null[(uint64) eax_n].b0000, (word32) ((Eq_n[]) 1)[(uint64) eax_n].b0000, (word32) ((Eq_n[]) 2)[(uint64) eax_n]);
			}
			else
				printf("INDEXED %u = invalid index\n", eax_n);
			break;
		case 0x04:
			printf("GRAY+ALPHA %u %u\n", component(rdx, ecx, 0x00, eax_n, 0x02), component(rdx, ecx, 0x01, eax_n, 0x02));
			break;
		case 0x06:
			printf("RGBA %u %u %u %u\n", component(rdx, ecx, 0x00, eax_n, 0x04), component(rdx, ecx, 0x01, eax_n, 0x04), component(rdx, ecx, 0x02, eax_n, 0x04), component(rdx, ecx, 0x03, eax_n, 0x04));
			break;
		}
	}
	else
	{
l00000000004012C9:
		png_error(4200760, rdi, 4200760);
	}
	if ((rax_n ^ fs->qw0028) == 0x00)
		return;
	__stack_chk_fail();
}

// 00000000004012F9: void main(Register (ptr64 Eq_n) rsi, Register word32 edi, Register (ptr32 Eq_n) fs)
void main(struct Eq_n * rsi, word32 edi, struct Eq_n * fs)
{
	word64 rax_n = fs->qw0028;
	if (edi != 0x04)
	{
		fwrite(&g_v401A70, 0x01, 0x27, stderr);
		goto l000000000040175D;
	}
	char * rax_n = rsi->ptr0008;
	uint64 rax_n = SEQ(SLICE(rax_n, word32, 32), atol(rax_n));
	char * rax_n = rsi->ptr0010;
	uint64 rax_n = SEQ(SLICE(rax_n, word32, 32), atol(rax_n));
	FILE * rax_n = fopen(rsi->ptr0018, "rb");
	if (rax_n == null)
	{
		fprintf(stderr, "pngpixel: %s: could not open file\n", rsi->ptr0018);
		goto l000000000040175D;
	}
	word64 rax_n = png_create_read_struct(0x00, 0x00401993, 0x00, 0x00, 0x00, 0x00401993, 0x00, 0x00);
	if (rax_n == 0x00)
	{
		fwrite(&g_v401A18, 0x01, 0x2E, stderr);
		goto l000000000040175D;
	}
	word64 rax_n = png_create_info_struct(rax_n);
	if (rax_n != 0x00)
	{
		png_init_io(rax_n, rax_n, rax_n);
		png_read_info(rax_n, rax_n, rax_n);
		word64 rax_n = png_get_rowbytes(rax_n, rax_n, rax_n);
		word64 rax_n = png_malloc(rax_n, rax_n, rax_n);
		ptr64 fp;
		if ((word32) png_get_IHDR(fp - 0x0080, fp - 0x007C, fp - 0x0084, fp - 112, fp - 116, fp - 0x0078, rax_n, fp - 0x0088, rax_n, fp - 0x0078, fp - 116, fp - 112, rax_n, rax_n) != 0x00)
		{
			word32 dwLoc78;
			int32 dwLoc6C;
			if (dwLoc78 != 0x00)
			{
				if (dwLoc78 != 0x01)
					png_error(0x0040199A, rax_n, 0x0040199A);
				else
					dwLoc6C = 0x07;
			}
			else
				dwLoc6C = 0x01;
			png_start_read_image(rax_n);
			int32 dwLoc68_n;
			for (dwLoc68_n = 0x00; dwLoc68_n < dwLoc6C; ++dwLoc68_n)
			{
				uint32 dwLoc88;
				uint32 dwLoc60_n;
				uint32 dwLoc64_n;
				ui32 dwLoc58_n;
				int32 dwLoc5C_n;
				if (dwLoc78 == 0x01)
				{
					ui32 eax_n;
					if (dwLoc68_n > 0x01)
						eax_n = (0x01 << (byte) (0x07 - dwLoc68_n >> 0x01)) - 0x01;
					else
						eax_n = 0x07;
					uint32 edx_n = eax_n - ((dwLoc68_n & 0x01) << 0x03 - (byte) (dwLoc68_n + 0x01 >> 0x01) & 0x07) + dwLoc88;
					int32 eax_n;
					if (dwLoc68_n > 0x01)
						eax_n = 0x07 - dwLoc68_n >> 0x01;
					else
						eax_n = 0x03;
					if (edx_n >> (byte) eax_n == 0x00)
						goto l000000000040166F;
					dwLoc60_n = (dwLoc68_n & 0x01) << 0x03 - (byte) (dwLoc68_n + 0x01 >> 0x01) & 0x07;
					dwLoc64_n = (word32) ((dwLoc68_n & 0x01) == 0x00) << 0x03 - (byte) (dwLoc68_n >> 0x01) & 0x07;
					dwLoc58_n = 0x01 << (byte) (0x07 - dwLoc68_n >> 0x01);
					int32 eax_n;
					if (dwLoc68_n > 0x02)
						eax_n = 0x08 >> (byte) (dwLoc68_n - 0x01 >> 0x01);
					else
						eax_n = 0x08;
					dwLoc5C_n = eax_n;
				}
				else
				{
					dwLoc60_n = 0x00;
					dwLoc64_n = 0x00;
					dwLoc58_n = 0x01;
					dwLoc5C_n = 0x01;
				}
				uint32 dwLoc54_n;
				uint32 dwLoc84;
				for (dwLoc54_n = dwLoc64_n; dwLoc54_n < dwLoc84; dwLoc54_n += dwLoc5C_n)
				{
					puts("png_read_row");
					png_read_row(4200886, 0x00, rax_n, 4200886, rax_n, 0x00, rax_n);
					if ((uint64) dwLoc54_n == rax_n)
					{
						uint32 dwLoc50_n;
						uint32 dwLoc4C_n = 0x00;
						for (dwLoc50_n = dwLoc60_n; dwLoc50_n < dwLoc88; dwLoc50_n += dwLoc58_n)
						{
							if ((uint64) dwLoc50_n == rax_n)
							{
								print_pixel(dwLoc4C_n, rax_n, rax_n, rax_n, fs);
								goto l000000000040167F;
							}
							++dwLoc4C_n;
						}
					}
				}
l000000000040166F:
			}
l000000000040167F:
			png_free(rax_n, rax_n, rax_n, 0x00);
			png_destroy_info_struct(rax_n, fp - 56, fp - 56);
l00000000004016DE:
			png_destroy_read_struct(0x00, 0x00, fp - 64, 0x00, 0x00);
l000000000040175D:
			if ((rax_n ^ fs->qw0028) == 0x00)
				return;
			__stack_chk_fail();
		}
		png_error(4200899, rax_n, 4200899);
	}
	fwrite(&g_v4019E8, 0x01, 44, stderr);
	goto l00000000004016DE;
}

// 0000000000401780: void __libc_csu_init(Register word32 edi, Register word64 rsi, Register word64 rdx)
void __libc_csu_init(word32 edi, word64 rsi, word64 rdx)
{
	word64 rdi;
	edi = (word32) rdi;
	_init();
	int64 rbp_n = 0x00601E08 - g_a601E00;
	if (rbp_n >> 0x03 != 0x00)
	{
		Eq_n rbx_n;
		rbx_n.u1 = 0x00;
		do
		{
			(*((char *) g_a601E00 + rbx_n * 0x08))();
			rbx_n = (word64) rbx_n.u1 + 1;
		} while (rbx_n != rbp_n >> 0x03);
	}
}

// 00000000004017F0: void __libc_csu_fini()
void __libc_csu_fini()
{
}

