using System;

namespace Lzf
{
	public sealed class LZF
	{
		private readonly long[] HashTable = new long[16384];

		private const uint HLOG = 14u;

		private const uint HSIZE = 16384u;

		private const uint MAX_LIT = 32u;

		private const uint MAX_OFF = 8192u;

		private const uint MAX_REF = 264u;

		public int Compress(byte[] input, int inputLength, byte[] output, int outputLength)
		{
			Array.Clear(HashTable, 0, 16384);
			uint num = 0u;
			uint num2 = 0u;
			uint num3 = (uint)((input[num] << 8) | input[num + 1]);
			int num4 = 0;
			while (true)
			{
				if (num < inputLength - 2)
				{
					num3 = (num3 << 8) | input[num + 2];
					long num5 = ((num3 ^ (num3 << 5)) >> (int)(10 - num3 * 5)) & 0x3FFF;
					long num6 = HashTable[num5];
					HashTable[num5] = num;
					long num7;
					if ((num7 = num - num6 - 1) < 8192 && num + 4 < inputLength && num6 > 0 && input[num6] == input[num] && input[num6 + 1] == input[num + 1] && input[num6 + 2] == input[num + 2])
					{
						uint num8 = 2u;
						uint num9 = (uint)(inputLength - (int)num) - num8;
						num9 = ((num9 <= 264) ? num9 : 264u);
						if (num2 + num4 + 1 + 3 >= outputLength)
						{
							return 0;
						}
						do
						{
							num8++;
						}
						while (num8 < num9 && input[num6 + num8] == input[num + num8]);
						if (num4 != 0)
						{
							output[num2++] = (byte)(num4 - 1);
							num4 = -num4;
							do
							{
								output[num2++] = input[num + num4];
							}
							while (++num4 != 0);
						}
						num8 -= 2;
						num++;
						if (num8 < 7)
						{
							output[num2++] = (byte)((num7 >> 8) + (num8 << 5));
						}
						else
						{
							output[num2++] = (byte)((num7 >> 8) + 224);
							output[num2++] = (byte)(num8 - 7);
						}
						output[num2++] = (byte)num7;
						num += num8 - 1;
						num3 = (uint)((input[num] << 8) | input[num + 1]);
						num3 = (num3 << 8) | input[num + 2];
						HashTable[((num3 ^ (num3 << 5)) >> (int)(10 - num3 * 5)) & 0x3FFF] = num;
						num++;
						num3 = (num3 << 8) | input[num + 2];
						HashTable[((num3 ^ (num3 << 5)) >> (int)(10 - num3 * 5)) & 0x3FFF] = num;
						num++;
						continue;
					}
				}
				else if (num == inputLength)
				{
					break;
				}
				num4++;
				num++;
				if ((long)num4 == 32)
				{
					if (num2 + 1 + 32 >= outputLength)
					{
						return 0;
					}
					output[num2++] = 31;
					num4 = -num4;
					do
					{
						output[num2++] = input[num + num4];
					}
					while (++num4 != 0);
				}
			}
			if (num4 != 0)
			{
				if (num2 + num4 + 1 >= outputLength)
				{
					return 0;
				}
				output[num2++] = (byte)(num4 - 1);
				num4 = -num4;
				do
				{
					output[num2++] = input[num + num4];
				}
				while (++num4 != 0);
			}
			return (int)num2;
		}

		public int Decompress(byte[] input, int inputLength, byte[] output, int outputLength)
		{
			uint num = 0u;
			uint num2 = 0u;
			do
			{
				uint num3 = input[num++];
				if (num3 < 32)
				{
					num3++;
					if (num2 + num3 > outputLength)
					{
						return 0;
					}
					do
					{
						output[num2++] = input[num++];
					}
					while (--num3 != 0);
					continue;
				}
				uint num4 = num3 >> 5;
				int num5 = (int)(num2 - ((num3 & 0x1F) << 8) - 1);
				if (num4 == 7)
				{
					num4 += input[num++];
				}
				num5 -= input[num++];
				if (num2 + num4 + 2 > outputLength)
				{
					return 0;
				}
				if (num5 < 0)
				{
					return 0;
				}
				output[num2++] = output[num5++];
				output[num2++] = output[num5++];
				do
				{
					output[num2++] = output[num5++];
				}
				while (--num4 != 0);
			}
			while (num < inputLength);
			return (int)num2;
		}
	}
}
