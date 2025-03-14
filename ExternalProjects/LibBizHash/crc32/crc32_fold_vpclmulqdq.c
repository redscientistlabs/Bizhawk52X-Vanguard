/* crc32_fold_vpclmulqdq.c -- VPCMULQDQ-based CRC32 folding implementation.
 * Copyright Wangyang Guo (wangyang.guo@intel.com)
 * For conditions of distribution and use, see copyright notice in README.md
 */

#include <immintrin.h>
#include <stdint.h>

#define ONCE(op)            if (first) { first = 0; op; }
#define XOR_INITIAL(where)  ONCE(where = _mm512_xor_si512(where, zmm_initial))

__attribute__((target("sse4.1,pclmul,avx512f,vpclmulqdq")))
uint64_t fold_16_vpclmulqdq(__m128i *xmm_crc0, __m128i *xmm_crc1,
	__m128i *xmm_crc2, __m128i *xmm_crc3, const uint8_t *src, uint64_t len,
	__m128i init_crc, int32_t first) {
	__m512i zmm_initial = _mm512_zextsi128_si512(init_crc);
	__m512i zmm_t0, zmm_t1, zmm_t2, zmm_t3;
	__m512i zmm_crc0, zmm_crc1, zmm_crc2, zmm_crc3;
	__m512i z0, z1, z2, z3;
	uint64_t len_tmp = len;
	const __m512i zmm_fold4 = _mm512_set4_epi32(0x00000001, 0x54442bd4, 0x00000001, 0xc6e41596);
	const __m512i zmm_fold16 = _mm512_set4_epi32(0x00000001, 0x1542778a, 0x00000001, 0x322d1430);

	// zmm register init
	zmm_crc0 = _mm512_setzero_si512();
	zmm_t0 = _mm512_loadu_si512((__m512i *)src);
	XOR_INITIAL(zmm_t0);
	zmm_crc1 = _mm512_loadu_si512((__m512i *)src + 1);
	zmm_crc2 = _mm512_loadu_si512((__m512i *)src + 2);
	zmm_crc3 = _mm512_loadu_si512((__m512i *)src + 3);

	// already have intermediate CRC in xmm registers
	// fold4 with 4 xmm_crc to get zmm_crc0
	zmm_crc0 = _mm512_inserti32x4(zmm_crc0, *xmm_crc0, 0);
	zmm_crc0 = _mm512_inserti32x4(zmm_crc0, *xmm_crc1, 1);
	zmm_crc0 = _mm512_inserti32x4(zmm_crc0, *xmm_crc2, 2);
	zmm_crc0 = _mm512_inserti32x4(zmm_crc0, *xmm_crc3, 3);
	z0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold4, 0x01);
	zmm_crc0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold4, 0x10);
	zmm_crc0 = _mm512_xor_si512(z0, zmm_crc0);
	zmm_crc0 = _mm512_xor_si512(zmm_crc0, zmm_t0);

	len -= 256;
	src += 256;

	// fold-16 loops
	while (len >= 256) {
		zmm_t0 = _mm512_loadu_si512((__m512i *)src);
		zmm_t1 = _mm512_loadu_si512((__m512i *)src + 1);
		zmm_t2 = _mm512_loadu_si512((__m512i *)src + 2);
		zmm_t3 = _mm512_loadu_si512((__m512i *)src + 3);

		z0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold16, 0x01);
		z1 = _mm512_clmulepi64_epi128(zmm_crc1, zmm_fold16, 0x01);
		z2 = _mm512_clmulepi64_epi128(zmm_crc2, zmm_fold16, 0x01);
		z3 = _mm512_clmulepi64_epi128(zmm_crc3, zmm_fold16, 0x01);

		zmm_crc0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold16, 0x10);
		zmm_crc1 = _mm512_clmulepi64_epi128(zmm_crc1, zmm_fold16, 0x10);
		zmm_crc2 = _mm512_clmulepi64_epi128(zmm_crc2, zmm_fold16, 0x10);
		zmm_crc3 = _mm512_clmulepi64_epi128(zmm_crc3, zmm_fold16, 0x10);

		zmm_crc0 = _mm512_xor_si512(z0, zmm_crc0);
		zmm_crc1 = _mm512_xor_si512(z1, zmm_crc1);
		zmm_crc2 = _mm512_xor_si512(z2, zmm_crc2);
		zmm_crc3 = _mm512_xor_si512(z3, zmm_crc3);

		zmm_crc0 = _mm512_xor_si512(zmm_crc0, zmm_t0);
		zmm_crc1 = _mm512_xor_si512(zmm_crc1, zmm_t1);
		zmm_crc2 = _mm512_xor_si512(zmm_crc2, zmm_t2);
		zmm_crc3 = _mm512_xor_si512(zmm_crc3, zmm_t3);

		len -= 256;
		src += 256;
	}
	// zmm_crc[0,1,2,3] -> zmm_crc0
	z0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold4, 0x01);
	zmm_crc0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold4, 0x10);
	zmm_crc0 = _mm512_xor_si512(z0, zmm_crc0);
	zmm_crc0 = _mm512_xor_si512(zmm_crc0, zmm_crc1);

	z0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold4, 0x01);
	zmm_crc0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold4, 0x10);
	zmm_crc0 = _mm512_xor_si512(z0, zmm_crc0);
	zmm_crc0 = _mm512_xor_si512(zmm_crc0, zmm_crc2);

	z0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold4, 0x01);
	zmm_crc0 = _mm512_clmulepi64_epi128(zmm_crc0, zmm_fold4, 0x10);
	zmm_crc0 = _mm512_xor_si512(z0, zmm_crc0);
	zmm_crc0 = _mm512_xor_si512(zmm_crc0, zmm_crc3);

	// zmm_crc0 -> xmm_crc[0, 1, 2, 3]
	*xmm_crc0 = _mm512_extracti32x4_epi32(zmm_crc0, 0);
	*xmm_crc1 = _mm512_extracti32x4_epi32(zmm_crc0, 1);
	*xmm_crc2 = _mm512_extracti32x4_epi32(zmm_crc0, 2);
	*xmm_crc3 = _mm512_extracti32x4_epi32(zmm_crc0, 3);

	return (len_tmp - len);  // return n bytes processed
}