﻿namespace BizHawk.Emulation.Cores.Nintendo.SNES
{
	public static class SnesColors
	{
		// the SNES renders colors in a 15 bit RGB space.  in addition, there is a 4 bit "luma" or "brightness" register
		// we want to map this to a 24 bit RGB space; the exactly authentic way to do this is not known

		// none of this is optimized for speed because we make a LUT at load time

		public static void BsnesColor(out int or, out int og, out int ob, int l, int r, int g, int b)
		{
			// bizhawk through r3808, from bsnes
			double luma = l / 15.0;
			int ar = (int)(luma * r + 0.5);
			int ag = (int)(luma * g + 0.5);
			int ab = (int)(luma * b + 0.5);
			or = ar * 255 / 31;
			og = ag * 255 / 31;
			ob = ab * 255 / 31;
		}

		public static void BizColor(out int or, out int og, out int ob, int l, int r, int g, int b)
		{
			// bizhawk r3809.  assumes that luma mixing is done in analog
			or = (r * l * 17 + 15) / 31;
			og = (g * l * 17 + 15) / 31;
			ob = (b * l * 17 + 15) / 31;
		}

		// LUT from snes9x
		private static readonly byte[,] mul_brightness =
{
{ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
{ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02 },
{ 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x02,
0x02, 0x02, 0x02, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x04, 0x04, 0x04, 0x04, 0x04 },
{ 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x02, 0x02, 0x03, 0x03, 0x03,
0x03, 0x03, 0x04, 0x04, 0x04, 0x04, 0x04, 0x05, 0x05, 0x05, 0x05, 0x05, 0x06, 0x06, 0x06, 0x06 },
{ 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x02, 0x03, 0x03, 0x03, 0x03, 0x04, 0x04,
0x04, 0x05, 0x05, 0x05, 0x05, 0x06, 0x06, 0x06, 0x06, 0x07, 0x07, 0x07, 0x07, 0x08, 0x08, 0x08 },
{ 0x00, 0x00, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x03, 0x03, 0x03, 0x04, 0x04, 0x04, 0x05, 0x05,
0x05, 0x06, 0x06, 0x06, 0x07, 0x07, 0x07, 0x08, 0x08, 0x08, 0x09, 0x09, 0x09, 0x0a, 0x0a, 0x0a },
{ 0x00, 0x00, 0x01, 0x01, 0x02, 0x02, 0x02, 0x03, 0x03, 0x04, 0x04, 0x04, 0x05, 0x05, 0x06, 0x06,
0x06, 0x07, 0x07, 0x08, 0x08, 0x08, 0x09, 0x09, 0x0a, 0x0a, 0x0a, 0x0b, 0x0b, 0x0c, 0x0c, 0x0c },
{ 0x00, 0x00, 0x01, 0x01, 0x02, 0x02, 0x03, 0x03, 0x04, 0x04, 0x05, 0x05, 0x06, 0x06, 0x07, 0x07,
0x07, 0x08, 0x08, 0x09, 0x09, 0x0a, 0x0a, 0x0b, 0x0b, 0x0c, 0x0c, 0x0d, 0x0d, 0x0e, 0x0e, 0x0e },
{ 0x00, 0x01, 0x01, 0x02, 0x02, 0x03, 0x03, 0x04, 0x04, 0x05, 0x05, 0x06, 0x06, 0x07, 0x07, 0x08,
0x09, 0x09, 0x0a, 0x0a, 0x0b, 0x0b, 0x0c, 0x0c, 0x0d, 0x0d, 0x0e, 0x0e, 0x0f, 0x0f, 0x10, 0x11 },
{ 0x00, 0x01, 0x01, 0x02, 0x02, 0x03, 0x04, 0x04, 0x05, 0x05, 0x06, 0x07, 0x07, 0x08, 0x08, 0x09,
0x0a, 0x0a, 0x0b, 0x0b, 0x0c, 0x0d, 0x0d, 0x0e, 0x0e, 0x0f, 0x10, 0x10, 0x11, 0x11, 0x12, 0x13 },
{ 0x00, 0x01, 0x01, 0x02, 0x03, 0x03, 0x04, 0x05, 0x05, 0x06, 0x07, 0x07, 0x08, 0x09, 0x09, 0x0a,
0x0b, 0x0b, 0x0c, 0x0d, 0x0d, 0x0e, 0x0f, 0x0f, 0x10, 0x11, 0x11, 0x12, 0x13, 0x13, 0x14, 0x15 },
{ 0x00, 0x01, 0x01, 0x02, 0x03, 0x04, 0x04, 0x05, 0x06, 0x07, 0x07, 0x08, 0x09, 0x0a, 0x0a, 0x0b,
0x0c, 0x0c, 0x0d, 0x0e, 0x0f, 0x0f, 0x10, 0x11, 0x12, 0x12, 0x13, 0x14, 0x15, 0x15, 0x16, 0x17 },
{ 0x00, 0x01, 0x02, 0x02, 0x03, 0x04, 0x05, 0x06, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0a, 0x0b, 0x0c,
0x0d, 0x0e, 0x0e, 0x0f, 0x10, 0x11, 0x12, 0x12, 0x13, 0x14, 0x15, 0x16, 0x16, 0x17, 0x18, 0x19 },
{ 0x00, 0x01, 0x02, 0x03, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0a, 0x0b, 0x0c, 0x0d,
0x0e, 0x0f, 0x10, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x17, 0x18, 0x19, 0x1a, 0x1b },
{ 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e,
0x0f, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d },
{ 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f }
};

		public static void Snes9xColor(out int or, out int og, out int ob, int l, int r, int g, int b)
		{
			int ar = mul_brightness[l, r];
			int ag = mul_brightness[l, g];
			int ab = mul_brightness[l, b];
			or = ar * 8;
			og = ag * 8;
			ob = ab * 8;
		}

		public enum ColorType
		{
			BizHawk,
			BSNES,
			Snes9x
		}

		//I separated these out to try and make them lazy construct, but it didnt work. try it some other way later.

		public static class BIZCOLOR
		{
			public static readonly int[] palette = GenLUT(ColorType.BizHawk);
		}

		public static class SNES9XCOLOR
		{
			public static readonly int[] palette = GenLUT(ColorType.Snes9x);
		}

		public static class BSNESCOLOR
		{
			public static readonly int[] palette = GenLUT(ColorType.BSNES);
		}

		public static int[] GetLUT(ColorType t)
		{
			if (t == ColorType.Snes9x)
				return SNES9XCOLOR.palette;
			else if (t == ColorType.BizHawk)
				return BIZCOLOR.palette;
			else
				return BSNESCOLOR.palette;
		}

		private static int[] GenLUT(ColorType t)
		{
			int[] ret = new int[16 * 32768];
			for (int l = 0; l < 16; l++)
			{
				for (int r = 0; r < 32; r++)
				{
					for (int g = 0; g < 32; g++)
					{
						for (int b = 0; b < 32; b++)
						{
							int ar, ag, ab;
							if (t == ColorType.Snes9x)
								Snes9xColor(out ar, out ag, out ab, l, r, g, b);
							else if (t == ColorType.BizHawk)
								BizColor(out ar, out ag, out ab, l, r, g, b);
							else
								BsnesColor(out ar, out ag, out ab, l, r, g, b);

							int color = (ar << 16) + (ag << 8) + (ab << 0) | unchecked((int)0xFF000000);
							ret[(l << 15) + (b << 10) + (g << 5) + (r << 0)] = color;
						}
					}
				}
			}
			return ret;
		}
	}
}
