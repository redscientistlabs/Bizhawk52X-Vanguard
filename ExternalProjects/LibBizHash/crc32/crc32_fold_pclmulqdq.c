/*
 * Compute the CRC32 using a parallelized folding approach with the PCLMULQDQ
 * instruction.
 *
 * A white paper describing this algorithm can be found at:
 *     doc/crc-pclmulqdq.pdf
 *
 * Copyright (C) 2013 Intel Corporation. All rights reserved.
 * Copyright (C) 2016 Marian Beermann (support for initial value)
 * Authors:
 *     Wajdi Feghali   <wajdi.k.feghali@intel.com>
 *     Jim Guilford    <james.guilford@intel.com>
 *     Vinodh Gopal    <vinodh.gopal@intel.com>
 *     Erdinc Ozturk   <erdinc.ozturk@intel.com>
 *     Jim Kukunas     <james.t.kukunas@linux.intel.com>
 *
 * For conditions of distribution and use, see copyright notice in README.md
 */

#include <immintrin.h>
#include <wmmintrin.h>
#include <smmintrin.h> // _mm_extract_epi32

#include <assert.h>

#include "common.h"
#include "crc32_fold.h"

extern uint64_t fold_16_vpclmulqdq(__m128i *xmm_crc0, __m128i *xmm_crc1,
	__m128i *xmm_crc2, __m128i *xmm_crc3, const uint8_t *src, uint64_t len, __m128i init_crc,
	int32_t first);
extern uint64_t fold_16_vpclmulqdq_copy(__m128i *xmm_crc0, __m128i *xmm_crc1,
	__m128i *xmm_crc2, __m128i *xmm_crc3, uint8_t *dst, const uint8_t *src, uint64_t len);

__attribute__((target("sse4.1,pclmul")))
static void fold_1(__m128i *xmm_crc0, __m128i *xmm_crc1, __m128i *xmm_crc2, __m128i *xmm_crc3) {
	const __m128i xmm_fold4 = _mm_set_epi32(0x00000001, 0x54442bd4, 0x00000001, 0xc6e41596);
	__m128i x_tmp3;
	__m128 ps_crc0, ps_crc3, ps_res;

	x_tmp3 = *xmm_crc3;

	*xmm_crc3 = *xmm_crc0;
	*xmm_crc0 = _mm_clmulepi64_si128(*xmm_crc0, xmm_fold4, 0x01);
	*xmm_crc3 = _mm_clmulepi64_si128(*xmm_crc3, xmm_fold4, 0x10);
	ps_crc0 = _mm_castsi128_ps(*xmm_crc0);
	ps_crc3 = _mm_castsi128_ps(*xmm_crc3);
	ps_res = _mm_xor_ps(ps_crc0, ps_crc3);

	*xmm_crc0 = *xmm_crc1;
	*xmm_crc1 = *xmm_crc2;
	*xmm_crc2 = x_tmp3;
	*xmm_crc3 = _mm_castps_si128(ps_res);
}

__attribute__((target("sse4.1,pclmul")))
static void fold_2(__m128i *xmm_crc0, __m128i *xmm_crc1, __m128i *xmm_crc2, __m128i *xmm_crc3) {
	const __m128i xmm_fold4 = _mm_set_epi32(0x00000001, 0x54442bd4, 0x00000001, 0xc6e41596);
	__m128i x_tmp3, x_tmp2;
	__m128 ps_crc0, ps_crc1, ps_crc2, ps_crc3, ps_res31, ps_res20;

	x_tmp3 = *xmm_crc3;
	x_tmp2 = *xmm_crc2;

	*xmm_crc3 = *xmm_crc1;
	*xmm_crc1 = _mm_clmulepi64_si128(*xmm_crc1, xmm_fold4, 0x01);
	*xmm_crc3 = _mm_clmulepi64_si128(*xmm_crc3, xmm_fold4, 0x10);
	ps_crc3 = _mm_castsi128_ps(*xmm_crc3);
	ps_crc1 = _mm_castsi128_ps(*xmm_crc1);
	ps_res31 = _mm_xor_ps(ps_crc3, ps_crc1);

	*xmm_crc2 = *xmm_crc0;
	*xmm_crc0 = _mm_clmulepi64_si128(*xmm_crc0, xmm_fold4, 0x01);
	*xmm_crc2 = _mm_clmulepi64_si128(*xmm_crc2, xmm_fold4, 0x10);
	ps_crc0 = _mm_castsi128_ps(*xmm_crc0);
	ps_crc2 = _mm_castsi128_ps(*xmm_crc2);
	ps_res20 = _mm_xor_ps(ps_crc0, ps_crc2);

	*xmm_crc0 = x_tmp2;
	*xmm_crc1 = x_tmp3;
	*xmm_crc2 = _mm_castps_si128(ps_res20);
	*xmm_crc3 = _mm_castps_si128(ps_res31);
}

__attribute__((target("sse4.1,pclmul")))
static void fold_3(__m128i *xmm_crc0, __m128i *xmm_crc1, __m128i *xmm_crc2, __m128i *xmm_crc3) {
	const __m128i xmm_fold4 = _mm_set_epi32(0x00000001, 0x54442bd4, 0x00000001, 0xc6e41596);
	__m128i x_tmp3;
	__m128 ps_crc0, ps_crc1, ps_crc2, ps_crc3, ps_res32, ps_res21, ps_res10;

	x_tmp3 = *xmm_crc3;

	*xmm_crc3 = *xmm_crc2;
	*xmm_crc2 = _mm_clmulepi64_si128(*xmm_crc2, xmm_fold4, 0x01);
	*xmm_crc3 = _mm_clmulepi64_si128(*xmm_crc3, xmm_fold4, 0x10);
	ps_crc2 = _mm_castsi128_ps(*xmm_crc2);
	ps_crc3 = _mm_castsi128_ps(*xmm_crc3);
	ps_res32 = _mm_xor_ps(ps_crc2, ps_crc3);

	*xmm_crc2 = *xmm_crc1;
	*xmm_crc1 = _mm_clmulepi64_si128(*xmm_crc1, xmm_fold4, 0x01);
	*xmm_crc2 = _mm_clmulepi64_si128(*xmm_crc2, xmm_fold4, 0x10);
	ps_crc1 = _mm_castsi128_ps(*xmm_crc1);
	ps_crc2 = _mm_castsi128_ps(*xmm_crc2);
	ps_res21 = _mm_xor_ps(ps_crc1, ps_crc2);

	*xmm_crc1 = *xmm_crc0;
	*xmm_crc0 = _mm_clmulepi64_si128(*xmm_crc0, xmm_fold4, 0x01);
	*xmm_crc1 = _mm_clmulepi64_si128(*xmm_crc1, xmm_fold4, 0x10);
	ps_crc0 = _mm_castsi128_ps(*xmm_crc0);
	ps_crc1 = _mm_castsi128_ps(*xmm_crc1);
	ps_res10 = _mm_xor_ps(ps_crc0, ps_crc1);

	*xmm_crc0 = x_tmp3;
	*xmm_crc1 = _mm_castps_si128(ps_res10);
	*xmm_crc2 = _mm_castps_si128(ps_res21);
	*xmm_crc3 = _mm_castps_si128(ps_res32);
}

__attribute__((target("sse4.1,pclmul")))
static void fold_4(__m128i *xmm_crc0, __m128i *xmm_crc1, __m128i *xmm_crc2, __m128i *xmm_crc3) {
	const __m128i xmm_fold4 = _mm_set_epi32(0x00000001, 0x54442bd4, 0x00000001, 0xc6e41596);
	__m128i x_tmp0, x_tmp1, x_tmp2, x_tmp3;
	__m128 ps_crc0, ps_crc1, ps_crc2, ps_crc3;
	__m128 ps_t0, ps_t1, ps_t2, ps_t3;
	__m128 ps_res0, ps_res1, ps_res2, ps_res3;

	x_tmp0 = *xmm_crc0;
	x_tmp1 = *xmm_crc1;
	x_tmp2 = *xmm_crc2;
	x_tmp3 = *xmm_crc3;

	*xmm_crc0 = _mm_clmulepi64_si128(*xmm_crc0, xmm_fold4, 0x01);
	x_tmp0 = _mm_clmulepi64_si128(x_tmp0, xmm_fold4, 0x10);
	ps_crc0 = _mm_castsi128_ps(*xmm_crc0);
	ps_t0 = _mm_castsi128_ps(x_tmp0);
	ps_res0 = _mm_xor_ps(ps_crc0, ps_t0);

	*xmm_crc1 = _mm_clmulepi64_si128(*xmm_crc1, xmm_fold4, 0x01);
	x_tmp1 = _mm_clmulepi64_si128(x_tmp1, xmm_fold4, 0x10);
	ps_crc1 = _mm_castsi128_ps(*xmm_crc1);
	ps_t1 = _mm_castsi128_ps(x_tmp1);
	ps_res1 = _mm_xor_ps(ps_crc1, ps_t1);

	*xmm_crc2 = _mm_clmulepi64_si128(*xmm_crc2, xmm_fold4, 0x01);
	x_tmp2 = _mm_clmulepi64_si128(x_tmp2, xmm_fold4, 0x10);
	ps_crc2 = _mm_castsi128_ps(*xmm_crc2);
	ps_t2 = _mm_castsi128_ps(x_tmp2);
	ps_res2 = _mm_xor_ps(ps_crc2, ps_t2);

	*xmm_crc3 = _mm_clmulepi64_si128(*xmm_crc3, xmm_fold4, 0x01);
	x_tmp3 = _mm_clmulepi64_si128(x_tmp3, xmm_fold4, 0x10);
	ps_crc3 = _mm_castsi128_ps(*xmm_crc3);
	ps_t3 = _mm_castsi128_ps(x_tmp3);
	ps_res3 = _mm_xor_ps(ps_crc3, ps_t3);

	*xmm_crc0 = _mm_castps_si128(ps_res0);
	*xmm_crc1 = _mm_castps_si128(ps_res1);
	*xmm_crc2 = _mm_castps_si128(ps_res2);
	*xmm_crc3 = _mm_castps_si128(ps_res3);
}

static const unsigned _Alignas(32) pshufb_shf_table[60] = {
	0x84838281, 0x88878685, 0x8c8b8a89, 0x008f8e8d, /* shl 15 (16 - 1)/shr1 */
	0x85848382, 0x89888786, 0x8d8c8b8a, 0x01008f8e, /* shl 14 (16 - 3)/shr2 */
	0x86858483, 0x8a898887, 0x8e8d8c8b, 0x0201008f, /* shl 13 (16 - 4)/shr3 */
	0x87868584, 0x8b8a8988, 0x8f8e8d8c, 0x03020100, /* shl 12 (16 - 4)/shr4 */
	0x88878685, 0x8c8b8a89, 0x008f8e8d, 0x04030201, /* shl 11 (16 - 5)/shr5 */
	0x89888786, 0x8d8c8b8a, 0x01008f8e, 0x05040302, /* shl 10 (16 - 6)/shr6 */
	0x8a898887, 0x8e8d8c8b, 0x0201008f, 0x06050403, /* shl  9 (16 - 7)/shr7 */
	0x8b8a8988, 0x8f8e8d8c, 0x03020100, 0x07060504, /* shl  8 (16 - 8)/shr8 */
	0x8c8b8a89, 0x008f8e8d, 0x04030201, 0x08070605, /* shl  7 (16 - 9)/shr9 */
	0x8d8c8b8a, 0x01008f8e, 0x05040302, 0x09080706, /* shl  6 (16 -10)/shr10*/
	0x8e8d8c8b, 0x0201008f, 0x06050403, 0x0a090807, /* shl  5 (16 -11)/shr11*/
	0x8f8e8d8c, 0x03020100, 0x07060504, 0x0b0a0908, /* shl  4 (16 -12)/shr12*/
	0x008f8e8d, 0x04030201, 0x08070605, 0x0c0b0a09, /* shl  3 (16 -13)/shr13*/
	0x01008f8e, 0x05040302, 0x09080706, 0x0d0c0b0a, /* shl  2 (16 -14)/shr14*/
	0x0201008f, 0x06050403, 0x0a090807, 0x0e0d0c0b  /* shl  1 (16 -15)/shr15*/
};

__attribute__((target("sse4.1,pclmul")))
static void partial_fold(const size_t len, __m128i *xmm_crc0, __m128i *xmm_crc1, __m128i *xmm_crc2,
                         __m128i *xmm_crc3, __m128i *xmm_crc_part) {

	const __m128i xmm_fold4 = _mm_set_epi32(0x00000001, 0x54442bd4, 0x00000001, 0xc6e41596);
	const __m128i xmm_mask3 = _mm_set1_epi32((int32_t)0x80808080);

	__m128i xmm_shl, xmm_shr, xmm_tmp1, xmm_tmp2, xmm_tmp3;
	__m128i xmm_a0_0, xmm_a0_1;
	__m128 ps_crc3, psa0_0, psa0_1, ps_res;

	xmm_shl = _mm_load_si128((__m128i *)pshufb_shf_table + (len - 1));
	xmm_shr = xmm_shl;
	xmm_shr = _mm_xor_si128(xmm_shr, xmm_mask3);

	xmm_a0_0 = _mm_shuffle_epi8(*xmm_crc0, xmm_shl);

	*xmm_crc0 = _mm_shuffle_epi8(*xmm_crc0, xmm_shr);
	xmm_tmp1 = _mm_shuffle_epi8(*xmm_crc1, xmm_shl);
	*xmm_crc0 = _mm_or_si128(*xmm_crc0, xmm_tmp1);

	*xmm_crc1 = _mm_shuffle_epi8(*xmm_crc1, xmm_shr);
	xmm_tmp2 = _mm_shuffle_epi8(*xmm_crc2, xmm_shl);
	*xmm_crc1 = _mm_or_si128(*xmm_crc1, xmm_tmp2);

	*xmm_crc2 = _mm_shuffle_epi8(*xmm_crc2, xmm_shr);
	xmm_tmp3 = _mm_shuffle_epi8(*xmm_crc3, xmm_shl);
	*xmm_crc2 = _mm_or_si128(*xmm_crc2, xmm_tmp3);

	*xmm_crc3 = _mm_shuffle_epi8(*xmm_crc3, xmm_shr);
	*xmm_crc_part = _mm_shuffle_epi8(*xmm_crc_part, xmm_shl);
	*xmm_crc3 = _mm_or_si128(*xmm_crc3, *xmm_crc_part);

	xmm_a0_1 = _mm_clmulepi64_si128(xmm_a0_0, xmm_fold4, 0x10);
	xmm_a0_0 = _mm_clmulepi64_si128(xmm_a0_0, xmm_fold4, 0x01);

	ps_crc3 = _mm_castsi128_ps(*xmm_crc3);
	psa0_0 = _mm_castsi128_ps(xmm_a0_0);
	psa0_1 = _mm_castsi128_ps(xmm_a0_1);

	ps_res = _mm_xor_ps(ps_crc3, psa0_0);
	ps_res = _mm_xor_ps(ps_res, psa0_1);

	*xmm_crc3 = _mm_castps_si128(ps_res);
}

__attribute__((target("sse4.1,pclmul")))
static inline void crc32_fold_load(__m128i *fold, __m128i *fold0, __m128i *fold1, __m128i *fold2, __m128i *fold3) {
	*fold0 = _mm_load_si128(fold + 0);
	*fold1 = _mm_load_si128(fold + 1);
	*fold2 = _mm_load_si128(fold + 2);
	*fold3 = _mm_load_si128(fold + 3);
}

__attribute__((target("sse4.1,pclmul")))
static inline void crc32_fold_save(__m128i *fold, __m128i fold0, __m128i fold1, __m128i fold2, __m128i fold3) {
	_mm_storeu_si128(fold + 0, fold0);
	_mm_storeu_si128(fold + 1, fold1);
	_mm_storeu_si128(fold + 2, fold2);
	_mm_storeu_si128(fold + 3, fold3);
}

__attribute__((target("sse4.1,pclmul")))
static uint32_t crc32_fold_pclmulqdq_reset(crc32_fold *crc) {
	__m128i xmm_crc0 = _mm_cvtsi32_si128(0x9db42487);
	__m128i xmm_zero = _mm_setzero_si128();
	crc32_fold_save((__m128i *)crc->fold, xmm_crc0, xmm_zero, xmm_zero, xmm_zero);
	return 0;
}

#define ONCE(op)            if (first) { first = 0; op; }
#define XOR_INITIAL(where)  ONCE(where = _mm_xor_si128(where, xmm_initial))

__attribute__((target("sse4.1,pclmul")))
void crc32_fold_pclmulqdq(crc32_fold *crc, const uint8_t *src, uint64_t len, uint32_t init_crc) {
	size_t algn_diff;
	__m128i xmm_t0, xmm_t1, xmm_t2, xmm_t3;
	__m128i xmm_crc0, xmm_crc1, xmm_crc2, xmm_crc3;
	__m128i xmm_crc_part = _mm_setzero_si128();
	__m128i xmm_initial = _mm_cvtsi32_si128(init_crc);
	int32_t first = init_crc != 0;

	/* Technically the CRC functions don't even call this for input < 64, but a bare minimum of 31
	 * bytes of input is needed for the aligning load that occurs.  If there's an initial CRC, to
	 * carry it forward through the folded CRC there must be 16 - src % 16 + 16 bytes available, which
	 * by definition can be up to 15 bytes + one full vector load. */
	assert(len >= 31 || first == 0);
	crc32_fold_load((__m128i *)crc->fold, &xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3);

	if (len < 16) {
		goto partial;
	}

	algn_diff = ((uintptr_t)16 - ((uintptr_t)src & 0xF)) & 0xF;
	if (algn_diff) {
		xmm_crc_part = _mm_loadu_si128((__m128i *)src);
		XOR_INITIAL(xmm_crc_part);

		if (algn_diff < 4 && init_crc != 0) {
			xmm_t0 = xmm_crc_part;
			xmm_crc_part = _mm_loadu_si128((__m128i*)src + 1);
			fold_1(&xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3);
			xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_t0);
			src += 16;
			len -= 16;
		}

		partial_fold(algn_diff, &xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3, &xmm_crc_part);

		src += algn_diff;
		len -= algn_diff;
	}

	if (x86_cpu_has_vpclmulqdq && x86_cpu_has_avx512 && (len >= 256)) {
		uint64_t n = fold_16_vpclmulqdq(&xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3, src, len, xmm_initial, first);
		first = 0;
		len -= n;
		src += n;
	}

	while (len >= 64) {
		len -= 64;
		xmm_t0 = _mm_load_si128((__m128i *)src);
		xmm_t1 = _mm_load_si128((__m128i *)src + 1);
		xmm_t2 = _mm_load_si128((__m128i *)src + 2);
		xmm_t3 = _mm_load_si128((__m128i *)src + 3);
		src += 64;

		fold_4(&xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3);
		XOR_INITIAL(xmm_t0);

		xmm_crc0 = _mm_xor_si128(xmm_crc0, xmm_t0);
		xmm_crc1 = _mm_xor_si128(xmm_crc1, xmm_t1);
		xmm_crc2 = _mm_xor_si128(xmm_crc2, xmm_t2);
		xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_t3);
	}

	// len = num bytes left - 64
	if (len >= 48) {
		len -= 48;

		xmm_t0 = _mm_load_si128((__m128i *)src);
		xmm_t1 = _mm_load_si128((__m128i *)src + 1);
		xmm_t2 = _mm_load_si128((__m128i *)src + 2);
		src += 48;
		XOR_INITIAL(xmm_t0);
		fold_3(&xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3);

		xmm_crc1 = _mm_xor_si128(xmm_crc1, xmm_t0);
		xmm_crc2 = _mm_xor_si128(xmm_crc2, xmm_t1);
		xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_t2);
	} else if (len >= 32) {
		len -= 32;

		xmm_t0 = _mm_load_si128((__m128i *)src);
		xmm_t1 = _mm_load_si128((__m128i *)src + 1);
		src += 32;
		XOR_INITIAL(xmm_t0);
		fold_2(&xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3);

		xmm_crc2 = _mm_xor_si128(xmm_crc2, xmm_t0);
		xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_t1);
	} else if (len >= 16) {
		len -= 16;
		xmm_t0 = _mm_load_si128((__m128i *)src);
		src += 16;
		XOR_INITIAL(xmm_t0);
		fold_1(&xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3);

		xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_t0);
	}

partial:
	if (len) {
		memcpy(&xmm_crc_part, src, (size_t)len);
		partial_fold((size_t)len, &xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3, &xmm_crc_part);
	}

	crc32_fold_save((__m128i *)crc->fold, xmm_crc0, xmm_crc1, xmm_crc2, xmm_crc3);
}

static const unsigned _Alignas(16) crc_k[] = {
	0xccaa009e, 0x00000000, /* rk1 */
	0x751997d0, 0x00000001, /* rk2 */
	0xccaa009e, 0x00000000, /* rk5 */
	0x63cd6124, 0x00000001, /* rk6 */
	0xf7011640, 0x00000001, /* rk7 */
	0xdb710640, 0x00000001  /* rk8 */
};

static const unsigned _Alignas(16) crc_mask[4] = {
	0xFFFFFFFF, 0xFFFFFFFF, 0x00000000, 0x00000000
};

static const unsigned _Alignas(16) crc_mask2[4] = {
	0x00000000, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF
};

__attribute__((target("sse4.1,pclmul")))
static uint32_t crc32_fold_pclmulqdq_final(crc32_fold *crc) {
	const __m128i xmm_mask  = _mm_load_si128((__m128i *)crc_mask);
	const __m128i xmm_mask2 = _mm_load_si128((__m128i *)crc_mask2);
	__m128i xmm_crc0, xmm_crc1, xmm_crc2, xmm_crc3;
	__m128i x_tmp0, x_tmp1, x_tmp2, crc_fold;

	crc32_fold_load((__m128i *)crc->fold, &xmm_crc0, &xmm_crc1, &xmm_crc2, &xmm_crc3);

	// k1
	crc_fold = _mm_load_si128((__m128i *)crc_k);

	x_tmp0 = _mm_clmulepi64_si128(xmm_crc0, crc_fold, 0x10);
	xmm_crc0 = _mm_clmulepi64_si128(xmm_crc0, crc_fold, 0x01);
	xmm_crc1 = _mm_xor_si128(xmm_crc1, x_tmp0);
	xmm_crc1 = _mm_xor_si128(xmm_crc1, xmm_crc0);

	x_tmp1 = _mm_clmulepi64_si128(xmm_crc1, crc_fold, 0x10);
	xmm_crc1 = _mm_clmulepi64_si128(xmm_crc1, crc_fold, 0x01);
	xmm_crc2 = _mm_xor_si128(xmm_crc2, x_tmp1);
	xmm_crc2 = _mm_xor_si128(xmm_crc2, xmm_crc1);

	x_tmp2 = _mm_clmulepi64_si128(xmm_crc2, crc_fold, 0x10);
	xmm_crc2 = _mm_clmulepi64_si128(xmm_crc2, crc_fold, 0x01);
	xmm_crc3 = _mm_xor_si128(xmm_crc3, x_tmp2);
	xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_crc2);

	// k5
	crc_fold = _mm_load_si128((__m128i *)crc_k + 1);

	xmm_crc0 = xmm_crc3;
	xmm_crc3 = _mm_clmulepi64_si128(xmm_crc3, crc_fold, 0);
	xmm_crc0 = _mm_srli_si128(xmm_crc0, 8);
	xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_crc0);

	xmm_crc0 = xmm_crc3;
	xmm_crc3 = _mm_slli_si128(xmm_crc3, 4);
	xmm_crc3 = _mm_clmulepi64_si128(xmm_crc3, crc_fold, 0x10);
	xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_crc0);
	xmm_crc3 = _mm_and_si128(xmm_crc3, xmm_mask2);

	// k7
	xmm_crc1 = xmm_crc3;
	xmm_crc2 = xmm_crc3;
	crc_fold = _mm_load_si128((__m128i *)crc_k + 2);

	xmm_crc3 = _mm_clmulepi64_si128(xmm_crc3, crc_fold, 0);
	xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_crc2);
	xmm_crc3 = _mm_and_si128(xmm_crc3, xmm_mask);

	xmm_crc2 = xmm_crc3;
	xmm_crc3 = _mm_clmulepi64_si128(xmm_crc3, crc_fold, 0x10);
	xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_crc2);
	xmm_crc3 = _mm_xor_si128(xmm_crc3, xmm_crc1);

	crc->value = (uint32_t)_mm_extract_epi32(xmm_crc3, 2);

	return crc->value;
}

__attribute__((target("sse4.1,pclmul")))
__attribute__((visibility("hidden")))
uint32_t crc32_pclmulqdq(uint32_t crc32, const uint8_t *buf, uint32_t len) {
	// For lens < 64, crc32_braid method is faster.
	if (len < 64)
		return crc32_braid(crc32, buf, len);

	crc32_fold _Alignas(16) crc_state;
	crc32_fold_pclmulqdq_reset(&crc_state);
	crc32_fold_pclmulqdq(&crc_state, buf, len, crc32 ^ 0xFFFFFFFFU);
	return crc32_fold_pclmulqdq_final(&crc_state);
}
