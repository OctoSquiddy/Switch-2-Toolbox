using System;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a 16-bit fixed-point decimal consisting of 1 sign bit, 10 integer bits and 5 fractional bits (denoted
/// as Q10.5). Note that the implementation is not reporting over- and underflowing errors.
/// </summary>
/// <remarks>
/// Examples:
///   SIIIIIII_IIIFFFFF
/// 0b00000000_00010000 = 0.5
/// 0b00000000_00100000 = 1
/// 0b00000001_00000000 = 8
/// 0b01000000_00000000 = 512
/// 0b10000000_00000000 = -1024
/// </remarks>
public struct Decimal10x5
{
	/// <summary>
	/// Represents the largest possible value of <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.
	/// </summary>
	public static readonly Decimal10x5 MaxValue = new Decimal10x5(32767);

	/// <summary>
	/// Represents the smallest possible value of <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.
	/// </summary>
	public static readonly Decimal10x5 MinValue = new Decimal10x5(-32768);

	private const int _m = 10;

	private const int _n = 5;

	/// <summary>
	/// Gets the internally stored value to represent the instance.
	/// </summary>
	/// <remarks>Signed to get arithmetic rather than logical shifts.</remarks>
	internal short Raw { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> struct from the given <paramref name="raw" />
	/// representation.
	/// </summary>
	/// <param name="raw">The raw representation of the internally stored bits.</param>
	internal Decimal10x5(int raw)
	{
		Raw = (short)raw;
	}

	/// <summary>
	/// Returns the given <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.
	/// </summary>
	/// <param name="a">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <returns>The result.</returns>
	public static Decimal10x5 operator +(Decimal10x5 a)
	{
		return a;
	}

	/// <summary>
	/// Adds the first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> to the second one.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <returns>The addition result.</returns>
	public static Decimal10x5 operator +(Decimal10x5 a, Decimal10x5 b)
	{
		return new Decimal10x5(a.Raw + b.Raw);
	}

	/// <summary>
	/// Negates the given <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.
	/// </summary>
	/// <param name="a">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> to negate.</param>
	/// <returns>The negated result.</returns>
	public static Decimal10x5 operator -(Decimal10x5 a)
	{
		return new Decimal10x5(-a.Raw);
	}

	/// <summary>
	/// Subtracts the first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> from the second one.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <returns>The subtraction result.</returns>
	public static Decimal10x5 operator -(Decimal10x5 a, Decimal10x5 b)
	{
		return new Decimal10x5(a.Raw - b.Raw);
	}

	/// <summary>
	/// Multiplicates the given <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> by the scalar.
	/// </summary>
	/// <param name="a">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <param name="s">The scalar.</param>
	/// <returns>The multiplication result.</returns>
	public static Decimal10x5 operator *(Decimal10x5 a, int s)
	{
		return new Decimal10x5(a.Raw * s);
	}

	/// <summary>
	/// Multiplicates the first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> by the second one.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <returns>The multiplication result.</returns>
	public static Decimal10x5 operator *(Decimal10x5 a, Decimal10x5 b)
	{
		int num = 16;
		return new Decimal10x5(a.Raw * b.Raw + num >> 5);
	}

	/// <summary>
	/// Divides the given <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> through the scalar.
	/// </summary>
	/// <param name="a">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <param name="s">The scalar.</param>
	/// <returns>The division result.</returns>
	public static Decimal10x5 operator /(Decimal10x5 a, int s)
	{
		return new Decimal10x5(a.Raw / s);
	}

	/// <summary>
	/// Divides the first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> through the second one.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <returns>The division result.</returns>
	public static Decimal10x5 operator /(Decimal10x5 a, Decimal10x5 b)
	{
		return new Decimal10x5((a.Raw << 5) / b.Raw);
	}

	/// <summary>
	/// Gets a value indicating whether the first specified <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> is the same as the second
	/// specified <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> to compare.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> to compare.</param>
	/// <returns>true, if both <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> are the same.</returns>
	public static bool operator ==(Decimal10x5 a, Decimal10x5 b)
	{
		return a.Equals(b);
	}

	/// <summary>
	/// Gets a value indicating whether the first specified <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> is not the same as the second
	/// specified <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.
	/// </summary>
	/// <param name="a">The first <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> to compare.</param>
	/// <param name="b">The second <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> to compare.</param>
	/// <returns>true, if both <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> are not the same.</returns>
	public static bool operator !=(Decimal10x5 a, Decimal10x5 b)
	{
		return !a.Equals(b);
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:System.Int32" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />
	/// instance.</param>
	public static explicit operator Decimal10x5(int value)
	{
		return new Decimal10x5(value << 5);
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:System.Double" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />
	/// instance.</param>
	public static explicit operator Decimal10x5(double value)
	{
		return new Decimal10x5((int)Math.Round(value * 32.0));
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:System.Single" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />
	/// instance.</param>
	public static explicit operator Decimal10x5(float value)
	{
		return new Decimal10x5((int)Math.Round(value * 32f));
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:System.Double" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> value to represent in the new <see cref="T:System.Double" />
	/// instance.</param>
	public static explicit operator double(Decimal10x5 value)
	{
		return (double)value.Raw / 32.0;
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:System.Int32" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> value to represent in the new <see cref="T:System.Int32" />
	/// instance.</param>
	public static explicit operator int(Decimal10x5 value)
	{
		int num = 16;
		return value.Raw + num >> 5;
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:System.Single" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> value to represent in the new <see cref="T:System.Single" />
	/// instance.</param>
	public static explicit operator float(Decimal10x5 value)
	{
		return (float)((double)value.Raw / 32.0);
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> is the same as the second specified
	/// <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.
	/// </summary>
	/// <param name="obj">The object to compare, if it is a <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <returns>true, if both <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> are the same.</returns>
	public override bool Equals(object obj)
	{
		if (!(obj is Decimal10x5 other))
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
	/// Gets a string describing this <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.
	/// </summary>
	/// <returns>A string describing this <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</returns>
	public override string ToString()
	{
		return ((double)this).ToString();
	}

	/// <summary>
	/// Indicates whether the current <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> is equal to another <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.
	/// </summary>
	/// <param name="other">A <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> to compare with this <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" />.</param>
	/// <returns>true if the current <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> is equal to the other parameter; otherwise, false.
	/// </returns>
	public bool Equals(Decimal10x5 other)
	{
		return Equals(Raw == other.Raw);
	}
}
