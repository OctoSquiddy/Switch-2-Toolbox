namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a 16-bit half-precision floating point value according to the IEEE 754 standard.
/// </summary>
/// <remarks>
/// Examples:
///   SEEEEEFF_FFFFFFFF
/// 0b00000000_00000000 = 0
/// 1b00000000_00000000 = -0
/// 0b00111100_00000000 = 1
/// 0b11000000_00000000 = -2
/// 0b11111011_11111111 = 65504 (MaxValue)
/// 0b01111100_00000000 = PositiveInfinity
/// 0b11111100_00000000 = NegativeInfinity
/// </remarks>
public struct Half
{
	/// <summary>
	/// Represents the smallest positive <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> value greater than zero.
	/// </summary>
	public static readonly Half Epsilon;

	/// <summary>
	/// Represents the largest possible value of <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.
	/// </summary>
	public static readonly Half MaxValue;

	/// <summary>
	/// Represents the smallest possible value of <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.
	/// </summary>
	public static readonly Half MinValue;

	/// <summary>
	/// Represents not a number (NaN).
	/// </summary>
	public static readonly Half NaN;

	/// <summary>
	/// Represents negative infinity.
	/// </summary>
	public static readonly Half NegativeInfinity;

	/// <summary>
	/// Represents positive infinity.
	/// </summary>
	public static readonly Half PositiveInfinity;

	private static readonly uint[] _mantissaTable;

	private static readonly uint[] _exponentTable;

	private static readonly uint[] _offsetTable;

	private static readonly ushort[] _baseTable;

	private static readonly byte[] _shiftTable;

	/// <summary>
	/// Gets the internally stored value to represent the instance.
	/// </summary>
	/// <remarks>Signed to get arithmetic rather than logical shifts.</remarks>
	internal ushort Raw { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> struct from the given <paramref name="raw" />
	/// representation.
	/// </summary>
	/// <param name="raw">The raw representation of the internally stored bits.</param>
	internal Half(ushort raw)
	{
		Raw = raw;
	}

	static Half()
	{
		Epsilon = new Half(1);
		MaxValue = new Half(31743);
		MinValue = new Half(64511);
		NaN = new Half(65024);
		NegativeInfinity = new Half(64512);
		PositiveInfinity = new Half(31744);
		_mantissaTable = new uint[2048];
		_mantissaTable[0] = 0u;
		for (int i = 1; i < 1024; i++)
		{
			uint num = (uint)(i << 13);
			uint num2 = 0u;
			while ((num & 0x800000) == 0)
			{
				num2 -= 8388608;
				num <<= 1;
			}
			num &= 0xFF7FFFFFu;
			num2 += 947912704;
			_mantissaTable[i] = num | num2;
		}
		for (int i = 1024; i < 2048; i++)
		{
			_mantissaTable[i] = (uint)(939524096 + (i - 1024 << 13));
		}
		_exponentTable = new uint[64];
		_exponentTable[0] = 0u;
		for (int i = 1; i < 63; i++)
		{
			if (i < 31)
			{
				_exponentTable[i] = (uint)(i << 23);
			}
			else
			{
				_exponentTable[i] = (uint)(int.MinValue + (i - 32 << 23));
			}
		}
		_exponentTable[31] = 1199570944u;
		_exponentTable[32] = 2147483648u;
		_exponentTable[63] = 3347054592u;
		_offsetTable = new uint[64];
		_offsetTable[0] = 0u;
		for (int i = 1; i < 64; i++)
		{
			_offsetTable[i] = 1024u;
		}
		_offsetTable[32] = 0u;
		_baseTable = new ushort[512];
		_shiftTable = new byte[512];
		for (int i = 0; i < 256; i++)
		{
			int num3 = i - 127;
			if (num3 < -24)
			{
				_baseTable[i | 0] = 0;
				_baseTable[i | 0x100] = 32768;
				_shiftTable[i | 0] = 24;
				_shiftTable[i | 0x100] = 24;
			}
			else if (num3 < -14)
			{
				_baseTable[i | 0] = (ushort)(1024 >> -num3 - 14);
				_baseTable[i | 0x100] = (ushort)((1024 >> -num3 - 14) | 0x8000);
				_shiftTable[i | 0] = (byte)(-num3 - 1);
				_shiftTable[i | 0x100] = (byte)(-num3 - 1);
			}
			else if (num3 <= 15)
			{
				_baseTable[i | 0] = (ushort)(num3 + 15 << 10);
				_baseTable[i | 0x100] = (ushort)((num3 + 15 << 10) | 0x8000);
				_shiftTable[i | 0] = 13;
				_shiftTable[i | 0x100] = 13;
			}
			else if (num3 < 128)
			{
				_baseTable[i | 0] = 31744;
				_baseTable[i | 0x100] = 64512;
				_shiftTable[i | 0] = 24;
				_shiftTable[i | 0x100] = 24;
			}
			else
			{
				_baseTable[i | 0] = 31744;
				_baseTable[i | 0x100] = 64512;
				_shiftTable[i | 0] = 13;
				_shiftTable[i | 0x100] = 13;
			}
		}
	}

	/// <summary>
	/// Returns the given <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.
	/// </summary>
	/// <param name="a">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <returns>The result.</returns>
	public static Half operator +(Half a)
	{
		return a;
	}

	/// <summary>
	/// Adds the first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> to the second one.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <returns>The addition result.</returns>
	public static Half operator +(Half a, Half b)
	{
		return (Half)((float)a + (float)b);
	}

	/// <summary>
	/// Negates the given <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.
	/// </summary>
	/// <param name="a">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> to negate.</param>
	/// <returns>The negated result.</returns>
	public static Half operator -(Half a)
	{
		return new Half((ushort)(a.Raw ^ 0x8000));
	}

	/// <summary>
	/// Subtracts the first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> from the second one.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <returns>The subtraction result.</returns>
	public static Half operator -(Half a, Half b)
	{
		return (Half)((float)a - (float)b);
	}

	/// <summary>
	/// Multiplicates the first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> by the second one.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <returns>The multiplication result.</returns>
	public static Half operator *(Half a, Half b)
	{
		return (Half)((float)a * (float)b);
	}

	/// <summary>
	/// Divides the first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> through the second one.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <returns>The division result.</returns>
	public static Half operator /(Half a, Half b)
	{
		return (Half)((float)a / (float)b);
	}

	/// <summary>
	/// Gets a value indicating whether the first specified <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> is the same as the second
	/// specified <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> to compare.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> to compare.</param>
	/// <returns>true, if both <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> are the same.</returns>
	public static bool operator ==(Half a, Half b)
	{
		return a.Equals(b);
	}

	/// <summary>
	/// Gets a value indicating whether the first specified <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> is not the same as the second
	/// specified <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> to compare.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> to compare.</param>
	/// <returns>true, if both <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> are not the same.</returns>
	public static bool operator !=(Half a, Half b)
	{
		return !a.Equals(b);
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:System.Int32" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />
	/// instance.</param>
	public static explicit operator Half(int value)
	{
		return (Half)(float)value;
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:System.Double" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />
	/// instance.</param>
	public static explicit operator Half(double value)
	{
		return (Half)(float)value;
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:System.Single" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />
	/// instance.</param>
	public static explicit operator Half(float value)
	{
		uint uInt = ((DWord)value).UInt32;
		return new Half((ushort)(_baseTable[(uInt >> 23) & 0x1FF] + ((uInt & 0x7FFFFF) >> (int)_shiftTable[(uInt >> 23) & 0x1FF])));
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:System.Double" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> value to represent in the new <see cref="T:System.Double" />
	/// instance.</param>
	public static implicit operator double(Half value)
	{
		return (float)value;
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:System.Int32" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> value to represent in the new <see cref="T:System.Int32" />
	/// instance.</param>
	public static explicit operator int(Half value)
	{
		return (int)(float)value;
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:System.Single" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> value to represent in the new <see cref="T:System.Single" />
	/// instance.</param>
	public static implicit operator float(Half value)
	{
		return ((DWord)(_mantissaTable[(int)_offsetTable[value.Raw >> 10] + (value.Raw & 0x3FF)] + _exponentTable[value.Raw >> 10])).Single;
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> is the same as the second specified
	/// <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.
	/// </summary>
	/// <param name="obj">The object to compare, if it is a <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <returns>true, if both <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> are the same.</returns>
	public override bool Equals(object obj)
	{
		if (!(obj is Half other))
		{
			return false;
		}
		return Equals(other);
	}

	/// <summary>
	/// Gets a hash code as an indication for object equality.
	/// </summary>
	/// <returns>The hash code.</returns>
	public override int GetHashCode()
	{
		return Raw;
	}

	/// <summary>
	/// Gets a string describing this <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.
	/// </summary>
	/// <returns>A string describing this <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</returns>
	public override string ToString()
	{
		return ((double)this).ToString();
	}

	/// <summary>
	/// Indicates whether the current <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> is equal to another <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.
	/// </summary>
	/// <param name="other">A <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> to compare with this <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" />.</param>
	/// <returns>true if the current <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> is equal to the other parameter; otherwise, false.
	/// </returns>
	public bool Equals(Half other)
	{
		return Equals(Raw == other.Raw);
	}

	/// <summary>
	/// Returns a value indicating whether the specified number evaluates to not a number (<see cref="F:Syroot.NintenTools.NSW.Bfres.Half.NaN" />).
	/// </summary>
	/// <param name="half">A half-precision floating-point number.</param>
	/// <returns><c>true</c> if value evaluates to not a number (<see cref="F:Syroot.NintenTools.NSW.Bfres.Half.NaN" />); otherwise <c>false</c>.</returns>
	public static bool IsNaN(Half half)
	{
		return (half.Raw & 0x7FFF) > PositiveInfinity.Raw;
	}

	/// <summary>
	/// Returns a value indicating whether the specified number evaluates to negative or positive infinity.
	/// </summary>
	/// <param name="half">A half-precision floating-point number.</param>
	/// <returns><c>true</c> if half evaluates to <see cref="F:Syroot.NintenTools.NSW.Bfres.Half.PositiveInfinity" /> or <see cref="F:Syroot.NintenTools.NSW.Bfres.Half.NegativeInfinity" />;
	/// otherwise <c>false</c>.</returns>
	public static bool IsInfinity(Half half)
	{
		return (half.Raw & 0x7FFF) == PositiveInfinity.Raw;
	}

	/// <summary>
	/// Returns a value indicating whether the specified number evaluates to negative infinity.
	/// </summary>
	/// <param name="half">A half-precision floating-point number.</param>
	/// <returns><c>true</c> if half evaluates to <see cref="F:Syroot.NintenTools.NSW.Bfres.Half.NegativeInfinity" />; otherwise <c>false</c>.</returns>
	public static bool IsNegativeInfinity(Half half)
	{
		return half.Raw == NegativeInfinity.Raw;
	}

	/// <summary>
	/// Returns a value indicating whether the specified number evaluates to positive infinity.
	/// </summary>
	/// <param name="half">A half-precision floating-point number.</param>
	/// <returns><c>true</c> if half evaluates to <see cref="F:Syroot.NintenTools.NSW.Bfres.Half.PositiveInfinity" />; otherwise <c>false</c>.</returns>
	public static bool IsPositiveInfinity(Half half)
	{
		return half.Raw == PositiveInfinity.Raw;
	}
}
