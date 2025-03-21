#include "mednafen/src/types.h"
#include "nyma.h"
#include <emulibc.h>
#include "mednafen/src/pce_fast/pce.h"
#include <waterboxcore.h>
#include "mednafen/src/pce_fast/pcecd.h"
#include "mednafen/src/pce_fast/huc.h"
#include "mednafen/src/pce_fast/vdc.h"
#include "mednafen/src/hw_misc/arcade_card/arcade_card.h"
#include "mednafen/src/pce_fast/huc6280.h"

using namespace MDFN_IEN_PCE_FAST;

extern Mednafen::MDFNGI EmulatedPCE_Fast;

void SetupMDFNGameInfo()
{
	Mednafen::MDFNGameInfo = &EmulatedPCE_Fast;
}

#define MemoryDomainFunctions(N,R,W)\
static void Access##N(uint8_t* buffer, int64_t address, int64_t count, bool write)\
{\
	if (write)\
	{\
		while (count--)\
			W(address++, *buffer++);\
	}\
	else\
	{\
		while (count--)\
			*buffer++ = R(address++);\
	}\
}
#define MemoryDomainBulkFunctions(N,R,W)\
static void Access##N(uint8_t* buffer, int64_t address, int64_t count, bool write)\
{\
	if (write)\
	{\
		W(address, count, buffer);\
	}\
	else\
	{\
		R(address, count, buffer);\
	}\
}

namespace MDFN_IEN_PCE_FAST
{
	extern ArcadeCard* arcade_card;
	// extern VCE* vce;
	uint8 ZZINPUT_Read(unsigned int A);
	uint8 INPUT_Read(unsigned int A)
	{
		LagFlag = false;
		if (InputCallback)
			InputCallback();
		return ZZINPUT_Read(A);
	}
}

uint8 HucReadVirtual(unsigned int A)
{
	uint8 wmpr = HuCPU.MPR[A >> 13];
	return HuCPU.FastMap[wmpr][A & 0x1FFF];
}
void HucWriteVirtual(unsigned int A, uint8 V)
{
	uint8 wmpr = HuCPU.MPR[A >> 13];
	HuCPU.PCEWrite[wmpr]((wmpr << 13) | (A & 0x1FFF), V);
}
uint8 HucReadActual(unsigned int A)
{
	uint8 wmpr = A >> 13;
	return HuCPU.FastMap[wmpr][A & 0x1FFF];
}
void HucWriteActual(unsigned int A, uint8 V)
{
	uint8 wmpr = A >> 13;
	HuCPU.PCEWrite[wmpr](A, V);
}

MemoryDomainFunctions(ShortBus, HucReadVirtual, HucWriteVirtual);
MemoryDomainFunctions(LongBus, HucReadActual, HucWriteActual);

ECL_EXPORT void GetMemoryAreas(MemoryArea* m)
{
	CheatArea* c;
	int i = 0;

	m[i].Data = (void*)(MemoryFunctionHook)AccessLongBus;
	m[i].Name = "System Bus (21 bit)";
	m[i].Size = 1 << 21;
	m[i].Flags = MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_WORDSIZE1 | MEMORYAREA_FLAGS_FUNCTIONHOOK;
	i++;

	m[i].Data = (void*)(MemoryFunctionHook)AccessShortBus;
	m[i].Name = "System Bus";
	m[i].Size = 1 << 16;
	m[i].Flags = MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_WORDSIZE1 | MEMORYAREA_FLAGS_FUNCTIONHOOK;
	i++;

	c = FindCheatArea(0xf8 * 8192);
	m[i].Data = c->data;
	m[i].Name = "Main Memory";
	m[i].Size = c->size;
	m[i].Flags = MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_WORDSIZE1 | MEMORYAREA_FLAGS_PRIMARY;
	i++;

	// TODO: "ROM"
	// not that important because we have ROM file domain in the frontend

	c = FindCheatArea(0xf7 * 8192);
	if (c)
	{
		m[i].Data = c->data;
		m[i].Name = "Battery RAM";
		m[i].Size = c->size;
		m[i].Flags = MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_WORDSIZE1 | MEMORYAREA_FLAGS_ONEFILLED | MEMORYAREA_FLAGS_SAVERAMMABLE;
		i++;
	}

	if (PCE_IsCD)
	{
		// pce-fast always adds the full 256kiB of turbocd + super system card as a single block
		c = FindCheatArea(0x68 * 8192);
		m[i].Data = (void*)((char*)c->data + 0x18 * 8192);
		m[i].Name = "TurboCD RAM";
		m[i].Size = 8 * 8192;
		m[i].Flags = MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_WORDSIZE1;
		i++;

		// m[i].Data = (void*)(MemoryFunctionHook)AccessADPCM;
		// m[i].Name = "ADPCM RAM";
		// m[i].Size = 1 << 16;
		// m[i].Flags = MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_WORDSIZE1 | MEMORYAREA_FLAGS_FUNCTIONHOOK;
		// i++;

		c = FindCheatArea(0x68 * 8192);
		m[i].Data = c->data;
		m[i].Name = "Super System Card RAM";
		m[i].Size = 0x18 * 8192;
		m[i].Flags = MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_WORDSIZE1;
		i++;

		if (arcade_card)
		{
			m[i].Data = arcade_card->ACRAM;
			m[i].Name = "Arcade Card RAM";
			m[i].Size = sizeof(arcade_card->ACRAM);
			m[i].Flags = MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_WORDSIZE1;
			i++;
		}
	}

	c = FindCheatArea(0x40 * 8192);
	if (c)
	{
		// populous
		m[i].Data = c->data;
		m[i].Name = "Cart Battery RAM";
		m[i].Size = c->size;
		m[i].Flags = MEMORYAREA_FLAGS_WRITABLE | MEMORYAREA_FLAGS_WORDSIZE1;
		i++;
	}
}

struct VramInfo
{
	int32_t BatWidth;
	int32_t BatHeight;
	const uint32_t* PaletteCache;
	const uint8_t* BackgroundCache;
	const uint8_t* SpriteCache;
	const uint16_t* Vram;
};

static uint8_t* SpriteCache;

static const int bat_width_tab[4] = { 32, 64, 128, 128 };
static const int bat_height_tab[2] = { 32, 64 };
ECL_EXPORT void GetVramInfo(VramInfo& v, int vdcIndex)
{
	// pce-fast does have a spr_tile_cache,
	// but it's only updated when a particular vram area is actually rendered as a sprite
	if (!SpriteCache)
		SpriteCache = (uint8_t*)alloc_invisible(0x20000);
	auto& vdc = vdc_chips[vdcIndex];
	v.BatWidth = bat_width_tab[(vdc.MWR >> 4) & 3];
	v.BatHeight = bat_height_tab[(vdc.MWR >> 6) & 1];
	v.PaletteCache = vce.color_table_cache;
	v.BackgroundCache = (uint8_t*)vdc.bg_tile_cache;
	v.SpriteCache = SpriteCache;
	v.Vram = vdc.VRAM;

	uint16_t* ssrc = vdc.VRAM;
	uint8_t* sdst = SpriteCache;
	for (int spriteNum = 0; spriteNum < 512; spriteNum++)
	{
		auto lsrc = ssrc;
		auto ldst = sdst;
		for (int line = 0; line < 16; line++)
		{
			auto a = lsrc[0], b = lsrc[16], c = lsrc[32], d = lsrc[48];
			*ldst++ = a >> 15 & 1 | b >> 14 & 2 | c >> 13 & 4 | d >> 12 & 8;
			*ldst++ = a >> 14 & 1 | b >> 13 & 2 | c >> 12 & 4 | d >> 11 & 8;
			*ldst++ = a >> 13 & 1 | b >> 12 & 2 | c >> 11 & 4 | d >> 10 & 8;
			*ldst++ = a >> 12 & 1 | b >> 11 & 2 | c >> 10 & 4 | d >>  9 & 8;
			*ldst++ = a >> 11 & 1 | b >> 10 & 2 | c >>  9 & 4 | d >>  8 & 8;
			*ldst++ = a >> 10 & 1 | b >>  9 & 2 | c >>  8 & 4 | d >>  7 & 8;
			*ldst++ = a >>  9 & 1 | b >>  8 & 2 | c >>  7 & 4 | d >>  6 & 8;
			*ldst++ = a >>  8 & 1 | b >>  7 & 2 | c >>  6 & 4 | d >>  5 & 8;
			*ldst++ = a >>  7 & 1 | b >>  6 & 2 | c >>  5 & 4 | d >>  4 & 8;
			*ldst++ = a >>  6 & 1 | b >>  5 & 2 | c >>  4 & 4 | d >>  3 & 8;
			*ldst++ = a >>  5 & 1 | b >>  4 & 2 | c >>  3 & 4 | d >>  2 & 8;
			*ldst++ = a >>  4 & 1 | b >>  3 & 2 | c >>  2 & 4 | d >>  1 & 8;
			*ldst++ = a >>  3 & 1 | b >>  2 & 2 | c >>  1 & 4 | d >>  0 & 8;
			*ldst++ = a >>  2 & 1 | b >>  1 & 2 | c >>  0 & 4 | d <<  1 & 8;
			*ldst++ = a >>  1 & 1 | b >>  0 & 2 | c <<  1 & 4 | d <<  2 & 8;
			*ldst++ = a >>  0 & 1 | b <<  1 & 2 | c <<  2 & 4 | d <<  3 & 8;
			lsrc += 1;
		}

		ssrc += 64;
		sdst += 256;
	}
}
