namespace Syroot.NintenTools.NSW.Bfres.Core;

/// <summary>
/// Represents extension methods for <see cref="T:System.Byte" /> instances.
/// </summary>
internal static class ByteExtensions
{
	/// <summary>
	/// Returns an <see cref="T:System.Byte" /> instance represented by the given number of <paramref name="bits" />, starting
	/// at the <paramref name="firstBit" />.
	/// </summary>
	/// <param name="self">The extended <see cref="T:System.Byte" /> instance.</param>
	/// <param name="firstBit">The first bit of the encoded value.</param>
	/// <param name="bits">The number of least significant bits which are used to store the <see cref="T:System.Byte" />
	/// value.</param>
	/// <returns>The decoded <see cref="T:System.Byte" />.</returns>
	internal static byte Decode(this byte self, int firstBit, int bits)
	{
		return (byte)((self >> firstBit) & (uint)((1 << bits) - 1));
	}

	/// <summary>
	/// Returns the current <see cref="T:System.Byte" /> with the bit at the <paramref name="index" /> set (being 1).
	/// </summary>
	/// <param name="self">The extended <see cref="T:System.Byte" /> instance.</param>
	/// <param name="index">The 0-based index of the bit to enable.</param>
	/// <returns>The current <see cref="T:System.Byte" /> with the bit enabled.</returns>
	internal static byte EnableBit(this byte self, int index)
	{
		return (byte)(self | (1 << index));
	}

	/// <summary>
	/// Returns the current <see cref="T:System.Byte" /> with the given <paramref name="value" /> set into the given number
	/// of <paramref name="bits" /> starting at <paramref name="firstBit" />.
	/// </summary>
	/// <param name="self">The extended <see cref="T:System.Byte" /> instance.</param>
	/// <param name="value">The value to encode.</param>
	/// <param name="firstBit">The first bit used for the encoded value.</param>
	/// <param name="bits">The number of bits which are used to store the <see cref="T:System.Byte" /> value.</param>
	/// <returns>The current <see cref="T:System.Byte" /> with the value encoded into it.</returns>
	internal static byte Encode(this byte self, byte value, int firstBit, int bits)
	{
		byte b = (byte)((1 << bits) - 1 << firstBit);
		self &= (byte)(~b);
		value = (byte)((value << firstBit) & b);
		return (byte)(self | value);
	}

	/// <summary>
	/// Returns the current <see cref="T:System.Byte" /> with the bit at the <paramref name="index" /> cleared (being 0).
	/// </summary>
	/// <param name="self">The extended <see cref="T:System.Byte" /> instance.</param>
	/// <param name="index">The 0-based index of the bit to disable.</param>
	/// <returns>The current <see cref="T:System.Byte" /> with the bit disabled.</returns>
	internal static byte DisableBit(this byte self, int index)
	{
		return (byte)(self & ~(1 << index));
	}

	/// <summary>
	/// Returns a value indicating whether the bit at the <paramref name="index" /> in the current
	/// <see cref="T:System.Byte" /> is enabled or disabled.
	/// </summary>
	/// <param name="self">The extended <see cref="T:System.Byte" /> instance.</param>
	/// <param name="index">The 0-based index of the bit to check.</param>
	/// <returns><c>true</c> when the bit is set; otherwise <c>false</c>.</returns>
	internal static bool GetBit(this byte self, int index)
	{
		return (self & (1 << index)) != 0;
	}

	/// <summary>
	/// Returns the current <see cref="T:System.Byte" /> with all bits rotated in the given <paramref name="direction" />,
	/// where positive directions rotate left and negative directions rotate right.
	/// </summary>
	/// <param name="self">The extended <see cref="T:System.Byte" /> instance.</param>
	/// <param name="direction">The direction in which to rotate, where positive directions rotate left.</param>
	/// <returns>The current <see cref="T:System.Byte" /> with the bits rotated.</returns>
	internal static byte RotateBits(this byte self, int direction)
	{
		int num = 8;
		if (direction > 0)
		{
			return (byte)((self << direction) | (self >> num - direction));
		}
		if (direction < 0)
		{
			direction = -direction;
			return (byte)((self >> direction) | (self << num - direction));
		}
		return self;
	}

	/// <summary>
	/// Returns the current <see cref="T:System.Byte" /> with the bit at the <paramref name="index" /> enabled or disabled,
	/// according to <paramref name="enable" />.
	/// </summary>
	/// <param name="self">The extended <see cref="T:System.Byte" /> instance.</param>
	/// <param name="index">The 0-based index of the bit to enable or disable.</param>
	/// <param name="enable"><c>true</c> to enable the bit; otherwise <c>false</c>.</param>
	/// <returns>The current <see cref="T:System.Byte" /> with the bit enabled or disabled.</returns>
	internal static byte SetBit(this byte self, int index, bool enable)
	{
		if (enable)
		{
			return self.EnableBit(index);
		}
		return self.DisableBit(index);
	}

	/// <summary>
	/// Returns the current <see cref="T:System.Byte" /> with the bit at the <paramref name="index" /> enabled when it is
	/// disabled or disabled when it is enabled.
	/// </summary>
	/// <param name="self">The extended <see cref="T:System.Byte" /> instance.</param>
	/// <param name="index">The 0-based index of the bit to toggle.</param>
	/// <returns>The current <see cref="T:System.Byte" /> with the bit toggled.</returns>
	internal static byte ToggleBit(this byte self, int index)
	{
		if (self.GetBit(index))
		{
			return self.DisableBit(index);
		}
		return self.EnableBit(index);
	}
}
