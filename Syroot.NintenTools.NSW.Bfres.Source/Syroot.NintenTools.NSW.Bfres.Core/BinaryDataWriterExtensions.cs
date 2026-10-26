using System;
using System.Collections.Generic;
using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.GFX;

namespace Syroot.NintenTools.NSW.Bfres.Core;

/// <summary>
/// Represents extension methods for the <see cref="T:Syroot.BinaryData.BinaryDataWriter" /> class.
/// </summary>
public static class BinaryDataWriterExtensions
{
	public static float toFloat(int hbits)
	{
		int num = hbits & 0x3FF;
		int num2 = hbits & 0x7C00;
		switch (num2)
		{
		case 31744:
			num2 = 261120;
			break;
		default:
			num2 += 114688;
			if (num == 0 && num2 > 115712)
			{
				return BitConverter.ToSingle(BitConverter.GetBytes(((hbits & 0x8000) << 16) | (num2 << 13) | 0x3FF), 0);
			}
			break;
		case 0:
			if (num != 0)
			{
				num2 = 115712;
				do
				{
					num <<= 1;
					num2 -= 1024;
				}
				while ((num & 0x400) == 0);
				num &= 0x3FF;
			}
			break;
		}
		return BitConverter.ToSingle(BitConverter.GetBytes(((hbits & 0x8000) << 16) | ((num2 | num) << 13)), 0);
	}

	public static int fromFloat(float fval)
	{
		int num = BitConverter.ToInt32(BitConverter.GetBytes(fval), 0);
		int num2 = (num >> 16) & 0x8000;
		int num3 = (num & 0x7FFFFFFF) + 4096;
		if (num3 >= 1199570944)
		{
			if ((num & 0x7FFFFFFF) >= 1199570944)
			{
				if (num3 < 2139095040)
				{
					return num2 | 0x7C00;
				}
				return num2 | 0x7C00 | ((num & 0x7FFFFF) >> 13);
			}
			return num2 | 0x7BFF;
		}
		if (num3 >= 947912704)
		{
			return num2 | (num3 - 939524096 >> 13);
		}
		if (num3 < 855638016)
		{
			return num2;
		}
		num3 = (num & 0x7FFFFFFF) >> 23;
		return num2 | (((num & 0x7FFFFF) | 0x800000) + (8388608 >> num3 - 102) >> 126 - num3);
	}

	public static int FloatToInt2(float value)
	{
		if (value < -1f || value > 1f)
		{
			throw new ArgumentException($"{value} cannot be converted to Int2 (exceeds range -1 to 1).", "value");
		}
		return (int)(((uint)value << 30 >> 30) & 3);
	}

	public static int FloatToInt10(float value)
	{
		if (value < -512f || value > 511f)
		{
			throw new ArgumentException($"{value} cannot be converted to Int10 (exceeds range -512 to 511).", "value");
		}
		return (int)(((uint)value << 22 >> 22) & 0x3FF);
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instance.</param>
	public static void Write(this BinaryDataWriter self, AnimConstant value)
	{
		self.Write(value.AnimDataOffset);
		self.Write(value.Value);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<AnimConstant> values)
	{
		foreach (AnimConstant value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.NintenTools.NSW.Bfres.Bounding" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Bounding" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Bounding value)
	{
		self.Write(value.Center);
		self.Write(value.Extent);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.NintenTools.NSW.Bfres.Bounding" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Bounding" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Bounding> values)
	{
		foreach (Bounding value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Decimal10x5 value)
	{
		self.Write(value.Raw);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Decimal10x5> values)
	{
		foreach (Decimal10x5 value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Half value)
	{
		self.Write(value.Raw);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Half> values)
	{
		foreach (Half value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Matrix3x4" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Matrix3x4" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Matrix3x4 value)
	{
		self.Write(value.M11);
		self.Write(value.M12);
		self.Write(value.M13);
		self.Write(value.M14);
		self.Write(value.M21);
		self.Write(value.M22);
		self.Write(value.M23);
		self.Write(value.M24);
		self.Write(value.M31);
		self.Write(value.M32);
		self.Write(value.M33);
		self.Write(value.M34);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Matrix3x4" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Matrix3x4" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Matrix3x4> values)
	{
		foreach (Matrix3x4 value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector2Bool" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector2Bool" /> instance.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	public static void Write(this BinaryDataWriter self, Vector2Bool value, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		self.Write(value.X, format);
		self.Write(value.Y, format);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector2Bool" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector2Bool" /> instances.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector2Bool> values, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		foreach (Vector2Bool value in values)
		{
			self.Write(value, format);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector2F" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector2F" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Vector2F value)
	{
		self.Write(value.X);
		self.Write(value.Y);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector2F" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector2F" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector2F> values)
	{
		foreach (Vector2F value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector2U" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector2U" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Vector2U value)
	{
		self.Write(value.X);
		self.Write(value.Y);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector2U" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector2U" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector2U> values)
	{
		foreach (Vector2U value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector3" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector3" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Vector3 value)
	{
		self.Write(value.X);
		self.Write(value.Y);
		self.Write(value.Z);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector3" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector3" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector3> values)
	{
		foreach (Vector3 value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector3Bool" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector3Bool" /> instance.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	public static void Write(this BinaryDataWriter self, Vector3Bool value, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		self.Write(value.X, format);
		self.Write(value.Y, format);
		self.Write(value.Z, format);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector3Bool" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector3Bool" /> instances.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector3Bool> values, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		foreach (Vector3Bool value in values)
		{
			self.Write(value, format);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector3F" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector3F" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Vector3F value)
	{
		self.Write(value.X);
		self.Write(value.Y);
		self.Write(value.Z);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector3F" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector3F" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector3F> values)
	{
		foreach (Vector3F value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector3U" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector3U" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Vector3U value)
	{
		self.Write(value.X);
		self.Write(value.Y);
		self.Write(value.Z);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector3U" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector3U" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector3U> values)
	{
		foreach (Vector3U value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector4" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector4" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Vector4 value)
	{
		self.Write(value.X);
		self.Write(value.Y);
		self.Write(value.Z);
		self.Write(value.W);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector4" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector4" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector4> values)
	{
		foreach (Vector4 value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector4Bool" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector4Bool" /> instance.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	public static void Write(this BinaryDataWriter self, Vector4Bool value, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		self.Write(value.X, format);
		self.Write(value.Y, format);
		self.Write(value.Z, format);
		self.Write(value.W, format);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector4Bool" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector4Bool" /> instances.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector4Bool> values, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		foreach (Vector4Bool value in values)
		{
			self.Write(value, format);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector4F" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector4F" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Vector4F value)
	{
		self.Write(value.X);
		self.Write(value.Y);
		self.Write(value.Z);
		self.Write(value.W);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector4F" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector4F" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector4F> values)
	{
		foreach (Vector4F value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector4U" /> instance into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector4U" /> instance.</param>
	public static void Write(this BinaryDataWriter self, Vector4U value)
	{
		self.Write(value.X);
		self.Write(value.Y);
		self.Write(value.Z);
		self.Write(value.W);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector4U" /> instances into the current stream.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector4U" /> instances.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector4U> values)
	{
		foreach (Vector4U value in values)
		{
			self.Write(value);
		}
	}

	/// <summary>
	/// Returns the conversion delegate for converting data available in the given <paramref name="attribFormat" />
	/// from a <see cref="T:Syroot.Maths.Vector4F" /> instance. Useful to prevent repetitive lookup for multiple values.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="attribFormat">The <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderParamType" /> of the data.</param>
	/// <returns>A conversion delegate for the data.</returns>
	public static Action<BinaryDataWriter, Vector4F> GetShaderParamTypeCallback(this BinaryDataWriter self, ShaderParamType paramType)
	{
		return paramType switch
		{
			ShaderParamType.Bool => (Action<BinaryDataWriter, Vector4F>)Write_8_UNorm, 
			ShaderParamType.Bool2 => Write_8_UNorm, 
			ShaderParamType.Bool3 => Write_8_UNorm, 
			ShaderParamType.Bool4 => Write_8_UNorm, 
			ShaderParamType.Float => Write_8_UNorm, 
			ShaderParamType.Float2 => Write_8_UNorm, 
			ShaderParamType.Float3 => Write_8_UNorm, 
			ShaderParamType.Float4 => Write_8_UNorm, 
			ShaderParamType.Float2x2 => Write_8_UNorm, 
			ShaderParamType.Float2x3 => Write_8_UNorm, 
			ShaderParamType.Float2x4 => Write_8_UNorm, 
			ShaderParamType.Float4x2 => Write_8_UNorm, 
			ShaderParamType.Float4x3 => Write_8_UNorm, 
			ShaderParamType.Float4x4 => Write_8_UNorm, 
			ShaderParamType.Int => Write_8_UNorm, 
			ShaderParamType.Int2 => Write_8_UNorm, 
			ShaderParamType.Int3 => Write_8_UNorm, 
			ShaderParamType.Int4 => Write_8_UNorm, 
			ShaderParamType.Reserved2 => Write_8_UNorm, 
			ShaderParamType.Reserved3 => Write_8_UNorm, 
			ShaderParamType.Reserved4 => Write_8_UNorm, 
			ShaderParamType.Srt2D => Write_8_UNorm, 
			ShaderParamType.Srt3D => Write_8_UNorm, 
			ShaderParamType.TexSrt => Write_8_UNorm, 
			ShaderParamType.TexSrtEx => Write_8_UNorm, 
			ShaderParamType.UInt => Write_8_UNorm, 
			ShaderParamType.UInt2 => Write_8_UNorm, 
			ShaderParamType.UInt3 => Write_8_UNorm, 
			ShaderParamType.UInt4 => Write_8_UNorm, 
			_ => throw new ArgumentException(string.Format("Invalid {0} {1}.", "ShaderParamType", paramType), "paramType"), 
		};
	}

	/// <summary>
	/// Returns the conversion delegate for converting data available in the given <paramref name="attribFormat" />
	/// from a <see cref="T:Syroot.Maths.Vector4F" /> instance. Useful to prevent repetitive lookup for multiple values.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="attribFormat">The <see cref="T:Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat" /> of the data.</param>
	/// <returns>A conversion delegate for the data.</returns>
	public static Action<BinaryDataWriter, Vector4F> GetAttribCallback(this BinaryDataWriter self, AttribFormat attribFormat)
	{
		return attribFormat switch
		{
			AttribFormat.Format_8_UNorm => (Action<BinaryDataWriter, Vector4F>)Write_8_UNorm, 
			AttribFormat.Format_8_UInt => Write_8_UInt, 
			AttribFormat.Format_8_SNorm => Write_8_SNorm, 
			AttribFormat.Format_8_SInt => Write_8_SInt, 
			AttribFormat.Format_8_UIntToSingle => Write_8_UIntToSingle, 
			AttribFormat.Format_8_SIntToSingle => Write_8_SIntToSingle, 
			AttribFormat.Format_4_4_UNorm => Write_4_4_UNorm, 
			AttribFormat.Format_16_UNorm => Write_16_UNorm, 
			AttribFormat.Format_16_UInt => Write_16_UInt, 
			AttribFormat.Format_16_SNorm => Write_16_SNorm, 
			AttribFormat.Format_16_SInt => Write_16_SInt, 
			AttribFormat.Format_16_Single => Write_16_Single, 
			AttribFormat.Format_16_UIntToSingle => Write_16_UIntToSingle, 
			AttribFormat.Format_16_SIntToSingle => Write_16_SIntToSingle, 
			AttribFormat.Format_8_8_UNorm => Write_8_8_UNorm, 
			AttribFormat.Format_8_8_UInt => Write_8_8_UInt, 
			AttribFormat.Format_8_8_SNorm => Write_8_8_SNorm, 
			AttribFormat.Format_8_8_SInt => Write_8_8_SInt, 
			AttribFormat.Format_8_8_UIntToSingle => Write_8_8_UIntToSingle, 
			AttribFormat.Format_8_8_SIntToSingle => Write_8_8_SIntToSingle, 
			AttribFormat.Format_32_UInt => Write_32_UInt, 
			AttribFormat.Format_32_SInt => Write_32_SInt, 
			AttribFormat.Format_32_Single => Write_32_Single, 
			AttribFormat.Format_16_16_UNorm => Write_16_16_UNorm, 
			AttribFormat.Format_16_16_UInt => Write_16_16_UInt, 
			AttribFormat.Format_16_16_SNorm => Write_16_16_SNorm, 
			AttribFormat.Format_16_16_SInt => Write_16_16_SInt, 
			AttribFormat.Format_16_16_Single => Write_16_16_Single, 
			AttribFormat.Format_16_16_UIntToSingle => Write_16_16_UIntToSingle, 
			AttribFormat.Format_16_16_SIntToSingle => Write_16_16_SIntToSingle, 
			AttribFormat.Format_10_11_11_Single => Write_10_11_11_Single, 
			AttribFormat.Format_8_8_8_8_UNorm => Write_8_8_8_8_UNorm, 
			AttribFormat.Format_8_8_8_8_UInt => Write_8_8_8_8_UInt, 
			AttribFormat.Format_8_8_8_8_SNorm => Write_8_8_8_8_SNorm, 
			AttribFormat.Format_8_8_8_8_SInt => Write_8_8_8_8_SInt, 
			AttribFormat.Format_8_8_8_8_UIntToSingle => Write_8_8_8_8_UIntToSingle, 
			AttribFormat.Format_8_8_8_8_SIntToSingle => Write_8_8_8_8_SIntToSingle, 
			AttribFormat.Format_10_10_10_2_UNorm => Write_10_10_10_2_UNorm, 
			AttribFormat.Format_10_10_10_2_UInt => Write_10_10_10_2_UInt, 
			AttribFormat.Format_10_10_10_2_SNorm => Write_10_10_10_2_SNorm, 
			AttribFormat.Format_10_10_10_2_SInt => Write_10_10_10_2_SInt, 
			AttribFormat.Format_32_32_UInt => Write_32_32_UInt, 
			AttribFormat.Format_32_32_SInt => Write_32_32_SInt, 
			AttribFormat.Format_32_32_Single => Write_32_32_Single, 
			AttribFormat.Format_16_16_16_16_UNorm => Write_16_16_16_16_UNorm, 
			AttribFormat.Format_16_16_16_16_UInt => Write_16_16_16_16_UInt, 
			AttribFormat.Format_16_16_16_16_SNorm => Write_16_16_16_16_SNorm, 
			AttribFormat.Format_16_16_16_16_SInt => Write_16_16_16_16_SInt, 
			AttribFormat.Format_16_16_16_16_Single => Write_16_16_16_16_Single, 
			AttribFormat.Format_16_16_16_16_UIntToSingle => Write_16_16_16_16_UIntToSingle, 
			AttribFormat.Format_16_16_16_16_SIntToSingle => Write_16_16_16_16_SIntToSingle, 
			AttribFormat.Format_32_32_32_UInt => Write_32_32_32_UInt, 
			AttribFormat.Format_32_32_32_SInt => Write_32_32_32_SInt, 
			AttribFormat.Format_32_32_32_Single => Write_32_32_32_Single, 
			AttribFormat.Format_32_32_32_32_UInt => Write_32_32_32_32_UInt, 
			AttribFormat.Format_32_32_32_32_SInt => Write_32_32_32_32_SInt, 
			AttribFormat.Format_32_32_32_32_Single => Write_32_32_32_32_Single, 
			_ => throw new ArgumentException(string.Format("Invalid {0} {1}.", "AttribFormat", attribFormat), "attribFormat"), 
		};
	}

	/// <summary>
	/// Writes a <see cref="T:Syroot.Maths.Vector4U" /> instance into the current stream with the given
	/// <paramref name="attribFormat" />.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="value">The <see cref="T:Syroot.Maths.Vector4F" /> instance.</param>
	/// <param name="attribFormat">The <see cref="T:Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat" /> of the data.</param>
	public static void Write(this BinaryDataWriter self, Vector4F value, AttribFormat attribFormat)
	{
		self.GetAttribCallback(attribFormat)(self, value);
	}

	/// <summary>
	/// Writes <see cref="T:Syroot.Maths.Vector4U" /> instances into the current stream with the given
	/// <paramref name="attribFormat" />.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataWriter" />.</param>
	/// <param name="values">The <see cref="T:Syroot.Maths.Vector4U" /> instances.</param>
	/// <param name="attribFormat">The <see cref="T:Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat" /> of the data.</param>
	public static void Write(this BinaryDataWriter self, IEnumerable<Vector4F> values, AttribFormat attribFormat)
	{
		Action<BinaryDataWriter, Vector4F> attribCallback = self.GetAttribCallback(attribFormat);
		foreach (Vector4F value in values)
		{
			attribCallback(self, value);
		}
	}

	private static void Write_8_UNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((byte)(Algebra.Clamp(value.X, 0f, 1f) * 255f));
	}

	private static void Write_8_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((byte)value.X);
	}

	private static void Write_8_SNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((sbyte)(Algebra.Clamp(value.X, -1f, 1f) * 127f));
	}

	private static void Write_8_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((sbyte)value.X);
	}

	private static void Write_8_UIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((byte)value.X);
	}

	private static void Write_8_SIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((sbyte)value.X);
	}

	private static void Write_4_4_UNorm(this BinaryDataWriter self, Vector4F value)
	{
		byte b = (byte)(Algebra.Clamp(value.X, 0f, 1f) * 127f);
		byte b2 = (byte)(Algebra.Clamp(value.Y, 0f, 1f) * 127f);
		self.Write((byte)(b | (b2 << 4)));
	}

	private static void Write_16_UNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((ushort)(Algebra.Clamp(value.X, 0f, 1f) * 65535f));
	}

	private static void Write_16_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((ushort)value.X);
	}

	private static void Write_16_SNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((short)(Algebra.Clamp(value.X, -1f, 1f) * 32767f));
	}

	private static void Write_16_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((short)value.X);
	}

	private static void Write_16_Single(this BinaryDataWriter self, Vector4F value)
	{
		Write(self, (Half)value.X);
	}

	private static void Write_16_UIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((ushort)value.X);
	}

	private static void Write_16_SIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((short)value.X);
	}

	private static void Write_8_8_UNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((byte)(Algebra.Clamp(value.X, 0f, 1f) * 255f));
		self.Write((byte)(Algebra.Clamp(value.Y, 0f, 1f) * 255f));
	}

	private static void Write_8_8_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((byte)value.X);
		self.Write((byte)value.Y);
	}

	private static void Write_8_8_SNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((sbyte)(Algebra.Clamp(value.X, -1f, 1f) * 127f));
		self.Write((sbyte)(Algebra.Clamp(value.Y, -1f, 1f) * 127f));
	}

	private static void Write_8_8_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((sbyte)value.X);
		self.Write((sbyte)value.Y);
	}

	private static void Write_8_8_UIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((byte)value.X);
		self.Write((byte)value.Y);
	}

	private static void Write_8_8_SIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((sbyte)value.X);
		self.Write((sbyte)value.Y);
	}

	private static void Write_32_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((uint)value.X);
	}

	private static void Write_32_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((int)value.X);
	}

	private static void Write_32_Single(this BinaryDataWriter self, Vector4F value)
	{
		self.Write(value.X);
	}

	private static void Write_16_16_UNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((ushort)(Algebra.Clamp(value.X, 0f, 1f) * 65535f));
		self.Write((ushort)(Algebra.Clamp(value.Y, 0f, 1f) * 65535f));
	}

	private static void Write_16_16_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((ushort)value.X);
		self.Write((ushort)value.Y);
	}

	private static void Write_16_16_SNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((short)(Algebra.Clamp(value.X, -1f, 1f) * 32767f));
		self.Write((short)(Algebra.Clamp(value.Y, -1f, 1f) * 32767f));
	}

	private static void Write_16_16_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((short)value.X);
		self.Write((short)value.Y);
	}

	private static void Write_16_16_Single(this BinaryDataWriter self, Vector4F value)
	{
		Write(self, (Half)value.X);
		Write(self, (Half)value.Y);
	}

	private static void Write_16_16_UIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((ushort)value.X);
		self.Write((ushort)value.Y);
	}

	private static void Write_16_16_SIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((short)value.X);
		self.Write((short)value.Y);
	}

	private static void Write_10_11_11_Single(this BinaryDataWriter self, Vector4F value)
	{
		throw new NotImplementedException("10-bit and 11-bit Single values have not yet been implemented.");
	}

	private static void Write_8_8_8_8_UNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((byte)(Algebra.Clamp(value.X, 0f, 1f) * 255f));
		self.Write((byte)(Algebra.Clamp(value.Y, 0f, 1f) * 255f));
		self.Write((byte)(Algebra.Clamp(value.Z, 0f, 1f) * 255f));
		self.Write((byte)(Algebra.Clamp(value.W, 0f, 1f) * 255f));
	}

	private static void Write_8_8_8_8_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((byte)value.X);
		self.Write((byte)value.Y);
		self.Write((byte)value.Z);
		self.Write((byte)value.W);
	}

	private static void Write_8_8_8_8_SNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((sbyte)(Algebra.Clamp(value.X, -1f, 1f) * 127f));
		self.Write((sbyte)(Algebra.Clamp(value.Y, -1f, 1f) * 127f));
		self.Write((sbyte)(Algebra.Clamp(value.Z, -1f, 1f) * 127f));
		self.Write((sbyte)(Algebra.Clamp(value.W, -1f, 1f) * 127f));
	}

	private static void Write_8_8_8_8_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((sbyte)value.X);
		self.Write((sbyte)value.Y);
		self.Write((sbyte)value.Z);
		self.Write((sbyte)value.W);
	}

	private static void Write_8_8_8_8_UIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((byte)value.X);
		self.Write((byte)value.Y);
		self.Write((byte)value.Z);
		self.Write((byte)value.W);
	}

	private static void Write_8_8_8_8_SIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((sbyte)value.X);
		self.Write((sbyte)value.Y);
		self.Write((sbyte)value.Z);
		self.Write((sbyte)value.W);
	}

	private static void Write_10_10_10_2_UNorm(this BinaryDataWriter self, Vector4F value)
	{
		uint num = SingleToUInt10(Algebra.Clamp(value.X, 0f, 1f) * 1023f);
		uint num2 = SingleToUInt10(Algebra.Clamp(value.Y, 0f, 1f) * 1023f);
		uint num3 = SingleToUInt10(Algebra.Clamp(value.Z, 0f, 1f) * 1023f);
		uint num4 = SingleToUInt2(Algebra.Clamp(value.W, 0f, 1f) * 3f);
		self.Write(num | (num2 << 10) | (num3 << 20) | (num4 << 30));
	}

	private static void Write_10_10_10_2_UInt(this BinaryDataWriter self, Vector4F value)
	{
		uint num = SingleToUInt10(value.X);
		uint num2 = SingleToUInt10(value.Y);
		uint num3 = SingleToUInt10(value.Z);
		uint num4 = SingleToUInt2(value.W);
		self.Write(num | (num2 << 10) | (num3 << 20) | (num4 << 30));
	}

	private static void Write_10_10_10_2_SNorm(this BinaryDataWriter self, Vector4F value)
	{
		int num = SingleToInt10(Algebra.Clamp(value.X, -1f, 1f) * 511f);
		int num2 = SingleToInt10(Algebra.Clamp(value.Y, -1f, 1f) * 511f);
		int num3 = SingleToInt10(Algebra.Clamp(value.Z, -1f, 1f) * 511f);
		int num4 = SingleToInt2(Algebra.Clamp(value.W, 0f, 1f));
		self.Write(num | (num2 << 10) | (num3 << 20) | (num4 << 30));
	}

	private static void Write_10_10_10_2_SInt(this BinaryDataWriter self, Vector4F value)
	{
		int num = SingleToInt10(value.X);
		int num2 = SingleToInt10(value.Y);
		int num3 = SingleToInt10(value.Z);
		int num4 = SingleToInt2(value.W);
		self.Write(num | (num2 << 10) | (num3 << 20) | (num4 << 30));
	}

	private static void Write_32_32_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((uint)value.X);
		self.Write((uint)value.Y);
	}

	private static void Write_32_32_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((int)value.X);
		self.Write((int)value.Y);
	}

	private static void Write_32_32_Single(this BinaryDataWriter self, Vector4F value)
	{
		self.Write(value.X);
		self.Write(value.Y);
	}

	private static void Write_16_16_16_16_UNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((ushort)(Algebra.Clamp(value.X, 0f, 1f) * 65535f));
		self.Write((ushort)(Algebra.Clamp(value.Y, 0f, 1f) * 65535f));
		self.Write((ushort)(Algebra.Clamp(value.Z, 0f, 1f) * 65535f));
		self.Write((ushort)(Algebra.Clamp(value.W, 0f, 1f) * 65535f));
	}

	private static void Write_16_16_16_16_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((ushort)value.X);
		self.Write((ushort)value.Y);
		self.Write((ushort)value.Z);
		self.Write((ushort)value.W);
	}

	private static void Write_16_16_16_16_SNorm(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((short)(Algebra.Clamp(value.X, -1f, 1f) * 32767f));
		self.Write((short)(Algebra.Clamp(value.Y, -1f, 1f) * 32767f));
		self.Write((short)(Algebra.Clamp(value.Z, -1f, 1f) * 32767f));
		self.Write((short)(Algebra.Clamp(value.W, -1f, 1f) * 32767f));
	}

	private static void Write_16_16_16_16_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((short)value.X);
		self.Write((short)value.Y);
		self.Write((short)value.Z);
		self.Write((short)value.W);
	}

	private static void Write_16_16_16_16_Single(this BinaryDataWriter self, Vector4F value)
	{
		Write(self, (Half)value.X);
		Write(self, (Half)value.Y);
		Write(self, (Half)value.Z);
		Write(self, (Half)value.W);
	}

	private static void Write_16_16_16_16_UIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((ushort)value.X);
		self.Write((ushort)value.Y);
		self.Write((ushort)value.Z);
		self.Write((ushort)value.W);
	}

	private static void Write_16_16_16_16_SIntToSingle(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((short)value.X);
		self.Write((short)value.Y);
		self.Write((short)value.Z);
		self.Write((short)value.W);
	}

	private static void Write_32_32_32_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((uint)value.X);
		self.Write((uint)value.Y);
		self.Write((uint)value.Z);
	}

	private static void Write_32_32_32_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((int)value.X);
		self.Write((int)value.Y);
		self.Write((int)value.Z);
	}

	private static void Write_32_32_32_Single(this BinaryDataWriter self, Vector4F value)
	{
		self.Write(value.X);
		self.Write(value.Y);
		self.Write(value.Z);
	}

	private static void Write_32_32_32_32_UInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((uint)value.X);
		self.Write((uint)value.Y);
		self.Write((uint)value.Z);
		self.Write((uint)value.W);
	}

	private static void Write_32_32_32_32_SInt(this BinaryDataWriter self, Vector4F value)
	{
		self.Write((int)value.X);
		self.Write((int)value.Y);
		self.Write((int)value.Z);
		self.Write((int)value.W);
	}

	private static void Write_32_32_32_32_Single(this BinaryDataWriter self, Vector4F value)
	{
		self.Write(value.X);
		self.Write(value.Y);
		self.Write(value.Z);
		self.Write(value.W);
	}

	private static int SingleToInt2(float value)
	{
		if (value < -1f || value > 1f)
		{
			throw new ArgumentException($"{value} cannot be converted to Int2 (exceeds range -1 to 1).", "value");
		}
		return (int)(((uint)value << 30 >> 30) & 3);
	}

	private static int SingleToInt10(float value)
	{
		if (value < -512f || value > 511f)
		{
			throw new ArgumentException($"{value} cannot be converted to Int10 (exceeds range -512 to 511).", "value");
		}
		return (int)(((uint)value << 22 >> 22) & 0x3FF);
	}

	private static uint SingleToUInt2(float value)
	{
		if (value < 0f || value > 3f)
		{
			throw new ArgumentException($"{value} cannot be converted to UInt2 (exceeds range 0 to 3).", "value");
		}
		return (uint)value;
	}

	private static uint SingleToUInt10(float value)
	{
		if (value < 0f || value > 1023f)
		{
			throw new ArgumentException($"{value} cannot be converted to UInt10 (exceeds range 0 to 1023).", "value");
		}
		return (uint)value;
	}
}
