using System;
using System.Linq;  // requred to run linq queries.

namespace PKSmash
{
	public static class Tools
	{
		// reverse byte order (16-bit)
		public static UInt16 ReverseBytes(UInt16 value)
		{
			return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
		}


		// reverse byte order (32-bit)
		public static UInt32 ReverseBytes(UInt32 value)
		{
			return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
						 (value & 0x00FF0000U) >> 8  | (value & 0xFF000000U) >> 24;
		}


		// reverse byte order (64-bit)
		public static UInt64 ReverseBytes(UInt64 value)
		{
			return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
						 (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8  |
						 (value & 0x000000FF00000000UL) >> 8  | (value & 0x0000FF0000000000UL) >> 24 |
						 (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
		}

		public static string ConvertResult(byte[] input, string type)
		{
			string conversion = "";
			string hexInput = BitConverter.ToString(input).Replace("-", "");

			switch (type)
			{
				case "ascii":
					for (int i = 0; i < hexInput.Length; i += 2)
					{
						string hex = hexInput.Substring(i, 2);
						uint dec = Convert.ToUInt32(hex, 16);
						char character = Convert.ToChar(dec);
						conversion += character;
					}
					break;
				case "bitfield":
					conversion = String.Join(String.Empty,
						hexInput.Select(
							c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
						)
					);
					break;
				case "decimal":
					conversion = (int.Parse(hexInput, System.Globalization.NumberStyles.HexNumber)).ToString();
					break;
				case "float":
					Array.Reverse(input);
					conversion = BitConverter.ToSingle(input, 0).ToString();
					break;
				default:
					conversion = hexInput;
					break;
			}

			return conversion;
		}
	}
}
