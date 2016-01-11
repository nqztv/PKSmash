using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace PKSmash
{
	public class Utils
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
						 (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
		}


		// reverse byte order (64-bit)
		public static UInt64 ReverseBytes(UInt64 value)
		{
			return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
						 (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
						 (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
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

	public class EnumBindingSourceExtension : MarkupExtension
	{
		private Type _enumType;

		public Type EnumType
		{
			get
			{
				return this._enumType;
			}
			set
			{
				if (value != this._enumType)
				{
					if (null != value)
					{
						Type enumType = Nullable.GetUnderlyingType(value) ?? value;

						if (!enumType.IsEnum)
							throw new ArgumentException("Type must be for an Enum.");
					}

					this._enumType = value;
				}
			}
		}

		public EnumBindingSourceExtension()
		{
		}

		public EnumBindingSourceExtension(Type enumType)
		{
			this.EnumType = enumType;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (null == this._enumType)
				throw new InvalidOperationException("The EnumType must be specified.");

			Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
			Array enumValues = Enum.GetValues(actualEnumType);

			if (actualEnumType == this._enumType)
				return enumValues;

			Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
			enumValues.CopyTo(tempArray, 1);
			return tempArray;
		}
	}

	public class EnumDescriptionTypeConverter : EnumConverter
	{
		public EnumDescriptionTypeConverter(Type type)
				: base(type)
		{
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value != null)
				{
					FieldInfo fi = value.GetType().GetField(value.ToString());
					if (fi != null)
					{
						var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
						return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
					}
				}

				return string.Empty;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
