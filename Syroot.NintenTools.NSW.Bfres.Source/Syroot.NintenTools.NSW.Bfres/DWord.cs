using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a 4-byte value which can hold differently typed data.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
[DebuggerDisplay("{Int32} / {Single}")]
public struct DWord : IConvertible
{
	/// <summary>
	/// The data as an <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.Int32" />.
	/// </summary>
	[FieldOffset(0)]
	public int Int32;

	/// <summary>
	/// The data as a <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.Single" />.
	/// </summary>
	[FieldOffset(0)]
	public float Single;

	/// <summary>
	/// The data as an <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.UInt32" />.
	/// </summary>
	[FieldOffset(0)]
	public uint UInt32;

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.DWord" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.Int32" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.DWord" /> instance.
	/// </param>
	public static implicit operator DWord(int value)
	{
		return new DWord
		{
			Int32 = value
		};
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.DWord" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.Single" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.DWord" /> instance.
	/// </param>
	public static implicit operator DWord(float value)
	{
		return new DWord
		{
			Single = value
		};
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.DWord" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.UInt32" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.DWord" /> instance.
	/// </param>
	public static implicit operator DWord(uint value)
	{
		return new DWord
		{
			UInt32 = value
		};
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to an <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.Int32" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.DWord" /> value to represent in the new <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.Int32" /> instance.
	/// </param>
	public static implicit operator int(DWord value)
	{
		return value.Int32;
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.Single" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.DWord" /> value to represent in the new <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.Single" /> instance.
	/// </param>
	public static implicit operator float(DWord value)
	{
		return value.Single;
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to an <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.UInt32" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.DWord" /> value to represent in the new <see cref="F:Syroot.NintenTools.NSW.Bfres.DWord.UInt32" /> instance.
	/// </param>
	public static implicit operator uint(DWord value)
	{
		return value.UInt32;
	}

	/// <summary>
	/// Returns the <see cref="T:System.TypeCode" /> for this instance.
	/// </summary>
	/// <returns>The enumerated constant that is the <see cref="T:System.TypeCode" /> of the class or value type that
	/// implements this interface.</returns>
	public TypeCode GetTypeCode()
	{
		return TypeCode.Int32;
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public bool ToBoolean(IFormatProvider provider)
	{
		throw new InvalidOperationException(string.Format("Cannot convert {0} to type {1}.", "DWord", typeof(bool)));
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public byte ToByte(IFormatProvider provider)
	{
		throw new InvalidOperationException(string.Format("Cannot convert {0} to type {1}.", "DWord", typeof(byte)));
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public char ToChar(IFormatProvider provider)
	{
		throw new InvalidOperationException(string.Format("Cannot convert {0} to type {1}.", "DWord", typeof(char)));
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public DateTime ToDateTime(IFormatProvider provider)
	{
		throw new InvalidOperationException(string.Format("Cannot convert {0} to type {1}.", "DWord", typeof(DateTime)));
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public decimal ToDecimal(IFormatProvider provider)
	{
		throw new InvalidOperationException(string.Format("Cannot convert {0} to type {1}.", "DWord", typeof(decimal)));
	}

	/// <summary>
	/// Converts the value of this instance to an equivalent double-precision floating-point number using the
	/// specified culture-specific formatting information.
	/// </summary>
	/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies
	/// culture-specific formatting information.</param>
	/// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
	public double ToDouble(IFormatProvider provider)
	{
		return Single;
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public short ToInt16(IFormatProvider provider)
	{
		throw new InvalidOperationException(string.Format("Cannot convert {0} to type {1}.", "DWord", typeof(short)));
	}

	/// <summary>
	/// Converts the value of this instance to an equivalent 32-bit signed integer using the specified
	/// culture-specific formatting information.
	/// </summary>
	/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies
	/// culture-specific formatting information.</param>
	/// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
	public int ToInt32(IFormatProvider provider)
	{
		return Int32;
	}

	/// <summary>
	/// Converts the value of this instance to an equivalent 64-bit signed integer using the specified
	/// culture-specific formatting information.
	/// </summary>
	/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies
	/// culture-specific formatting information.</param>
	/// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
	public long ToInt64(IFormatProvider provider)
	{
		return Int32;
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public sbyte ToSByte(IFormatProvider provider)
	{
		throw new InvalidOperationException("Cannot convert DWord to type String.");
	}

	/// <summary>
	/// Converts the value of this instance to an equivalent single-precision floating-point number using the
	/// specified culture-specific formatting information.
	/// </summary>
	/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies
	/// culture-specific formatting information.</param>
	/// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
	public float ToSingle(IFormatProvider provider)
	{
		return Single;
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public string ToString(IFormatProvider provider)
	{
		throw new InvalidOperationException("Cannot convert DWord to type String.");
	}

	/// <summary>
	/// Converts the value of this instance to an <see cref="T:System.Object" /> of the specified <see cref="T:System.Type" /> that has
	/// an equivalent value, using the specified culture-specific formatting information.
	/// </summary>
	/// <param name="conversionType">The <see cref="T:System.Type" /> to which the value of this instance is converted.
	/// </param>
	/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies
	/// culture-specific formatting information.</param>
	/// <returns>An <see cref="T:System.Object" /> instance of type conversionType whose value is equivalent to the value of
	/// this instance.</returns>
	public object ToType(Type conversionType, IFormatProvider provider)
	{
		if (conversionType == typeof(int) || conversionType == typeof(long))
		{
			return Int32;
		}
		if (conversionType == typeof(float) || conversionType == typeof(double))
		{
			return Single;
		}
		throw new ArgumentException(string.Format("Cannot convert {0} to type {1}.", "DWord", conversionType));
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public ushort ToUInt16(IFormatProvider provider)
	{
		throw new InvalidOperationException("Cannot convert DWord to type UInt16.");
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public uint ToUInt32(IFormatProvider provider)
	{
		return UInt32;
	}

	/// <summary>
	/// This operation is not supported.
	/// </summary>
	public ulong ToUInt64(IFormatProvider provider)
	{
		throw new InvalidOperationException("Cannot convert DWord to type UInt64.");
	}
}
