using System;
using System.Collections.Generic;
using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.GFX;

namespace Syroot.NintenTools.NSW.Bfres.Core;

/// <summary>
/// Represents extension methods for the <see cref="T:Syroot.BinaryData.BinaryDataReader" /> class.
/// </summary>
public static class BinaryDataReaderExtensions
{
	/// <summary>
	/// Reads a <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instance.</returns>
	public static AnimConstant ReadAnimConstant(this BinaryDataReader self)
	{
		return new AnimConstant
		{
			AnimDataOffset = self.ReadUInt32(),
			Value = self.ReadInt32()
		};
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instance.</returns>
	public static AnimConstant[] ReadAnimConstants(this BinaryDataReader self, int count)
	{
		AnimConstant[] array = new AnimConstant[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadAnimConstant();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.NintenTools.NSW.Bfres.Bounding" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.Bounding" /> instance.</returns>
	public static Bounding ReadBounding(this BinaryDataReader self)
	{
		return new Bounding
		{
			Center = self.ReadVector3F(),
			Extent = self.ReadVector3F()
		};
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.NintenTools.NSW.Bfres.Bounding" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.Bounding" /> instances.</returns>
	public static IList<Bounding> ReadBoundings(this BinaryDataReader self, int count)
	{
		Bounding[] array = new Bounding[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadBounding();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instance.</returns>
	public static Decimal10x5 ReadDecimal10x5(this BinaryDataReader self)
	{
		return new Decimal10x5(self.ReadUInt16());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instances.</returns>
	public static IList<Decimal10x5> ReadDecimal10x5s(this BinaryDataReader self, int count)
	{
		Decimal10x5[] array = new Decimal10x5[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadDecimal10x5();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instance.</returns>
	public static Half ReadHalf(this BinaryDataReader self)
	{
		return new Half(self.ReadUInt16());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.Half" /> instances.</returns>
	public static IList<Half> ReadHalfs(this BinaryDataReader self, int count)
	{
		Half[] array = new Half[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadHalf();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Matrix3x4" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Matrix3x4" /> instance.</returns>
	public static Matrix3x4 ReadMatrix3x4(this BinaryDataReader self)
	{
		return new Matrix3x4(self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Matrix3x4" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Matrix3x4" /> instances.</returns>
	public static IList<Matrix3x4> ReadMatrix3x4s(this BinaryDataReader self, int count)
	{
		Matrix3x4[] array = new Matrix3x4[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadMatrix3x4();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector2" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector2" /> instance.</returns>
	public static Vector2 ReadVector2(this BinaryDataReader self)
	{
		return new Vector2(self.ReadInt32(), self.ReadInt32());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector2" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector2" /> instances.</returns>
	public static IList<Vector2> ReadVector2s(this BinaryDataReader self, int count)
	{
		Vector2[] array = new Vector2[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector2();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector2Bool" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector2Bool" /> instance.</returns>
	public static Vector2Bool ReadVector2Bool(this BinaryDataReader self, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		return new Vector2Bool(self.ReadBoolean(format), self.ReadBoolean(format));
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector2Bool" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector2Bool" /> instances.</returns>
	public static IList<Vector2Bool> ReadVector2Bools(this BinaryDataReader self, int count, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		Vector2Bool[] array = new Vector2Bool[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector2Bool(format);
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector2F" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector2F" /> instance.</returns>
	public static Vector2F ReadVector2F(this BinaryDataReader self)
	{
		return new Vector2F(self.ReadSingle(), self.ReadSingle());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector2F" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector2F" /> instances.</returns>
	public static IList<Vector2F> ReadVector2Fs(this BinaryDataReader self, int count)
	{
		Vector2F[] array = new Vector2F[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector2F();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector2U" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector2U" /> instance.</returns>
	public static Vector2U ReadVector2U(this BinaryDataReader self)
	{
		return new Vector2U(self.ReadUInt32(), self.ReadUInt32());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector2U" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector2U" /> instances.</returns>
	public static IList<Vector2U> ReadVector2Us(this BinaryDataReader self, int count)
	{
		Vector2U[] array = new Vector2U[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector2U();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector3" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector3" /> instance.</returns>
	public static Vector3 ReadVector3(this BinaryDataReader self)
	{
		return new Vector3(self.ReadInt32(), self.ReadInt32(), self.ReadInt32());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector3" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector3" /> instances.</returns>
	public static IList<Vector3> ReadVector3s(this BinaryDataReader self, int count)
	{
		Vector3[] array = new Vector3[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector3();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector3Bool" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector3Bool" /> instance.</returns>
	public static Vector3Bool ReadVector3Bool(this BinaryDataReader self, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		return new Vector3Bool(self.ReadBoolean(format), self.ReadBoolean(format), self.ReadBoolean(format));
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector3Bool" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector3Bool" /> instances.</returns>
	public static IList<Vector3Bool> ReadVector3Bools(this BinaryDataReader self, int count, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		Vector3Bool[] array = new Vector3Bool[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector3Bool(format);
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector3F" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector3F" /> instance.</returns>
	public static Vector3F ReadVector3F(this BinaryDataReader self)
	{
		return new Vector3F(self.ReadSingle(), self.ReadSingle(), self.ReadSingle());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector3F" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector3F" /> instances.</returns>
	public static IList<Vector3F> ReadVector3Fs(this BinaryDataReader self, int count)
	{
		Vector3F[] array = new Vector3F[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector3F();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector3U" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector3U" /> instance.</returns>
	public static Vector3U ReadVector3U(this BinaryDataReader self)
	{
		return new Vector3U(self.ReadUInt32(), self.ReadUInt32(), self.ReadUInt32());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector3U" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector3U" /> instances.</returns>
	public static IList<Vector3U> ReadVector3Us(this BinaryDataReader self, int count)
	{
		Vector3U[] array = new Vector3U[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector3U();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector4" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4" /> instance.</returns>
	public static Vector4 ReadVector4(this BinaryDataReader self)
	{
		return new Vector4(self.ReadInt32(), self.ReadInt32(), self.ReadInt32(), self.ReadInt32());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector4" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4" /> instances.</returns>
	public static IList<Vector4> ReadVector4s(this BinaryDataReader self, int count)
	{
		Vector4[] array = new Vector4[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector4();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector4Bool" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4Bool" /> instance.</returns>
	public static Vector4Bool ReadVector4Bool(this BinaryDataReader self, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		return new Vector4Bool(self.ReadBoolean(format), self.ReadBoolean(format), self.ReadBoolean(format), self.ReadBoolean(format));
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector4Bool" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <param name="format">The <see cref="T:Syroot.BinaryData.BinaryBooleanFormat" /> in which values are stored.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4Bool" /> instances.</returns>
	public static IList<Vector4Bool> ReadVector4Bools(this BinaryDataReader self, int count, BinaryBooleanFormat format = BinaryBooleanFormat.NonZeroByte)
	{
		Vector4Bool[] array = new Vector4Bool[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector4Bool(format);
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector4F" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4F" /> instance.</returns>
	public static Vector4F ReadVector4F(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector4F" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4F" /> instances.</returns>
	public static IList<Vector4F> ReadVector4Fs(this BinaryDataReader self, int count)
	{
		Vector4F[] array = new Vector4F[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector4F();
		}
		return array;
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector4U" /> instance from the current stream and returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4U" /> instance.</returns>
	public static Vector4U ReadVector4U(this BinaryDataReader self)
	{
		return new Vector4U(self.ReadUInt32(), self.ReadUInt32(), self.ReadUInt32(), self.ReadUInt32());
	}

	/// <summary>
	/// Reads <see cref="T:Syroot.Maths.Vector4U" /> instances from the current stream and returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4U" /> instances.</returns>
	public static IList<Vector4U> ReadVector4Us(this BinaryDataReader self, int count)
	{
		Vector4U[] array = new Vector4U[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = self.ReadVector4U();
		}
		return array;
	}

	/// <summary>
	/// Returns the conversion delegate for converting data available in the given <paramref name="attribFormat" />
	/// into a <see cref="T:Syroot.Maths.Vector4F" /> instance. Useful to prevent repetitive lookup for multiple values.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="attribFormat">The <see cref="T:Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat" /> of the data.</param>
	/// <returns>A conversion delegate for the data.</returns>
	public static Func<BinaryDataReader, Vector4F> GetAttribCallback(this BinaryDataReader self, AttribFormat attribFormat)
	{
		return attribFormat switch
		{
			AttribFormat.Format_8_UNorm => (Func<BinaryDataReader, Vector4F>)Read_8_UNorm, 
			AttribFormat.Format_8_UInt => Read_8_UInt, 
			AttribFormat.Format_8_SNorm => Read_8_SNorm, 
			AttribFormat.Format_8_SInt => Read_8_SInt, 
			AttribFormat.Format_8_UIntToSingle => Read_8_UIntToSingle, 
			AttribFormat.Format_8_SIntToSingle => Read_8_SIntToSingle, 
			AttribFormat.Format_4_4_UNorm => Read_4_4_UNorm, 
			AttribFormat.Format_16_UNorm => Read_16_UNorm, 
			AttribFormat.Format_16_UInt => Read_16_UInt, 
			AttribFormat.Format_16_SNorm => Read_16_SNorm, 
			AttribFormat.Format_16_SInt => Read_16_SInt, 
			AttribFormat.Format_16_Single => Read_16_Single, 
			AttribFormat.Format_16_UIntToSingle => Read_16_UIntToSingle, 
			AttribFormat.Format_16_SIntToSingle => Read_16_SIntToSingle, 
			AttribFormat.Format_8_8_UNorm => Read_8_8_UNorm, 
			AttribFormat.Format_8_8_UInt => Read_8_8_UInt, 
			AttribFormat.Format_8_8_SNorm => Read_8_8_SNorm, 
			AttribFormat.Format_8_8_SInt => Read_8_8_SInt, 
			AttribFormat.Format_8_8_UIntToSingle => Read_8_8_UIntToSingle, 
			AttribFormat.Format_8_8_SIntToSingle => Read_8_8_SIntToSingle, 
			AttribFormat.Format_32_UInt => Read_32_UInt, 
			AttribFormat.Format_32_SInt => Read_32_SInt, 
			AttribFormat.Format_32_Single => Read_32_Single, 
			AttribFormat.Format_16_16_UNorm => Read_16_16_UNorm, 
			AttribFormat.Format_16_16_UInt => Read_16_16_UInt, 
			AttribFormat.Format_16_16_SNorm => Read_16_16_SNorm, 
			AttribFormat.Format_16_16_SInt => Read_16_16_SInt, 
			AttribFormat.Format_16_16_Single => Read_16_16_Single, 
			AttribFormat.Format_16_16_UIntToSingle => Read_16_16_UIntToSingle, 
			AttribFormat.Format_16_16_SIntToSingle => Read_16_16_SIntToSingle, 
			AttribFormat.Format_10_11_11_Single => Read_10_11_11_Single, 
			AttribFormat.Format_8_8_8_8_UNorm => Read_8_8_8_8_UNorm, 
			AttribFormat.Format_8_8_8_8_UInt => Read_8_8_8_8_UInt, 
			AttribFormat.Format_8_8_8_8_SNorm => Read_8_8_8_8_SNorm, 
			AttribFormat.Format_8_8_8_8_SInt => Read_8_8_8_8_SInt, 
			AttribFormat.Format_8_8_8_8_UIntToSingle => Read_8_8_8_8_UIntToSingle, 
			AttribFormat.Format_8_8_8_8_SIntToSingle => Read_8_8_8_8_SIntToSingle, 
			AttribFormat.Format_10_10_10_2_UNorm => Read_10_10_10_2_UNorm, 
			AttribFormat.Format_10_10_10_2_UInt => Read_10_10_10_2_UInt, 
			AttribFormat.Format_10_10_10_2_SNorm => Read_10_10_10_2_SNorm, 
			AttribFormat.Format_10_10_10_2_SInt => Read_10_10_10_2_SInt, 
			AttribFormat.Format_32_32_UInt => Read_32_32_UInt, 
			AttribFormat.Format_32_32_SInt => Read_32_32_SInt, 
			AttribFormat.Format_32_32_Single => Read_32_32_Single, 
			AttribFormat.Format_16_16_16_16_UNorm => Read_16_16_16_16_UNorm, 
			AttribFormat.Format_16_16_16_16_UInt => Read_16_16_16_16_UInt, 
			AttribFormat.Format_16_16_16_16_SNorm => Read_16_16_16_16_SNorm, 
			AttribFormat.Format_16_16_16_16_SInt => Read_16_16_16_16_SInt, 
			AttribFormat.Format_16_16_16_16_Single => Read_16_16_16_16_Single, 
			AttribFormat.Format_16_16_16_16_UIntToSingle => Read_16_16_16_16_UIntToSingle, 
			AttribFormat.Format_16_16_16_16_SIntToSingle => Read_16_16_16_16_SIntToSingle, 
			AttribFormat.Format_32_32_32_UInt => Read_32_32_32_UInt, 
			AttribFormat.Format_32_32_32_SInt => Read_32_32_32_SInt, 
			AttribFormat.Format_32_32_32_Single => Read_32_32_32_Single, 
			AttribFormat.Format_32_32_32_32_UInt => Read_32_32_32_32_UInt, 
			AttribFormat.Format_32_32_32_32_SInt => Read_32_32_32_32_SInt, 
			AttribFormat.Format_32_32_32_32_Single => Read_32_32_32_32_Single, 
			_ => throw new ArgumentException(string.Format("Invalid {0} {1}.", "AttribFormat", attribFormat), "attribFormat"), 
		};
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector4F" /> instance converted from the given <paramref name="attribFormat" /> and
	/// returns it.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="attribFormat">The <see cref="T:Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat" /> of the data.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4F" /> instance.</returns>
	public static Vector4F ReadAttrib(this BinaryDataReader self, AttribFormat attribFormat)
	{
		return self.GetAttribCallback(attribFormat)(self);
	}

	/// <summary>
	/// Reads a <see cref="T:Syroot.Maths.Vector4F" /> instances converted from the given <paramref name="attribFormat" /> and
	/// returns them.
	/// </summary>
	/// <param name="self">The extended <see cref="T:Syroot.BinaryData.BinaryDataReader" />.</param>
	/// <param name="count">The number of instances to read.</param>
	/// <param name="attribFormat">The <see cref="T:Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat" /> of the data.</param>
	/// <returns>The <see cref="T:Syroot.Maths.Vector4F" /> instances.</returns>
	public static IList<Vector4F> ReadAttribs(this BinaryDataReader self, int count, AttribFormat attribFormat)
	{
		Func<BinaryDataReader, Vector4F> attribCallback = self.GetAttribCallback(attribFormat);
		Vector4F[] array = new Vector4F[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = attribCallback(self);
		}
		return array;
	}

	private static Vector4F Read_8_UNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)(int)self.ReadByte() / 255f, 0f, 0f, 0f);
	}

	private static Vector4F Read_8_UInt(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadByte(), 0f, 0f, 0f);
	}

	private static Vector4F Read_8_SNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)self.ReadSByte() / 127f, 0f, 0f, 0f);
	}

	private static Vector4F Read_8_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSByte(), 0f, 0f, 0f);
	}

	private static Vector4F Read_8_UIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadByte(), 0f, 0f, 0f);
	}

	private static Vector4F Read_8_SIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSByte(), 0f, 0f, 0f);
	}

	private static Vector4F Read_4_4_UNorm(this BinaryDataReader self)
	{
		byte b = self.ReadByte();
		return new Vector4F((float)(b & 0xF) / 127f, (float)(b >> 4) / 127f, 0f, 0f);
	}

	private static Vector4F Read_16_UNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)(int)self.ReadUInt16() / 65535f, 0f, 0f, 0f);
	}

	private static Vector4F Read_16_UInt(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadUInt16(), 0f, 0f, 0f);
	}

	private static Vector4F Read_16_SNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)self.ReadInt16() / 32767f, 0f, 0f, 0f);
	}

	private static Vector4F Read_16_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt16(), 0f, 0f, 0f);
	}

	private static Vector4F Read_16_Single(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadHalf(), 0f, 0f, 0f);
	}

	private static Vector4F Read_16_UIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadUInt16(), 0f, 0f, 0f);
	}

	private static Vector4F Read_16_SIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt16(), 0f, 0f, 0f);
	}

	private static Vector4F Read_8_8_UNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)(int)self.ReadByte() / 255f, (float)(int)self.ReadByte() / 255f, 0f, 0f);
	}

	private static Vector4F Read_8_8_UInt(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadByte(), (int)self.ReadByte(), 0f, 0f);
	}

	private static Vector4F Read_8_8_SNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)self.ReadSByte() / 127f, (float)self.ReadSByte() / 127f, 0f, 0f);
	}

	private static Vector4F Read_8_8_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSByte(), self.ReadSByte(), 0f, 0f);
	}

	private static Vector4F Read_8_8_UIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadByte(), (int)self.ReadByte(), 0f, 0f);
	}

	private static Vector4F Read_8_8_SIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSByte(), self.ReadSByte(), 0f, 0f);
	}

	private static Vector4F Read_32_UInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadUInt32(), 0f, 0f, 0f);
	}

	private static Vector4F Read_32_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt32(), 0f, 0f, 0f);
	}

	private static Vector4F Read_32_Single(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSingle(), 0f, 0f, 0f);
	}

	private static Vector4F Read_16_16_UNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)(int)self.ReadUInt16() / 65535f, (float)(int)self.ReadUInt16() / 65535f, 0f, 0f);
	}

	private static Vector4F Read_16_16_UInt(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadUInt16(), (int)self.ReadUInt16(), 0f, 0f);
	}

	private static Vector4F Read_16_16_SNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)self.ReadInt16() / 32767f, (float)self.ReadInt16() / 32767f, 0f, 0f);
	}

	private static Vector4F Read_16_16_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt16(), self.ReadInt16(), 0f, 0f);
	}

	private static Vector4F Read_16_16_Single(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadHalf(), self.ReadHalf(), 0f, 0f);
	}

	private static Vector4F Read_16_16_UIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadUInt16(), (int)self.ReadUInt16(), 0f, 0f);
	}

	private static Vector4F Read_16_16_SIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt16(), self.ReadInt16(), 0f, 0f);
	}

	private static Vector4F Read_10_11_11_Single(this BinaryDataReader self)
	{
		throw new NotImplementedException("10-bit and 11-bit Single values have not yet been implemented.");
	}

	private static Vector4F Read_8_8_8_8_UNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)(int)self.ReadByte() / 255f, (float)(int)self.ReadByte() / 255f, (float)(int)self.ReadByte() / 255f, (float)(int)self.ReadByte() / 255f);
	}

	private static Vector4F Read_8_8_8_8_UInt(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadByte(), (int)self.ReadByte(), (int)self.ReadByte(), (int)self.ReadByte());
	}

	private static Vector4F Read_8_8_8_8_SNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)self.ReadSByte() / 127f, (float)self.ReadSByte() / 127f, (float)self.ReadSByte() / 127f, (float)self.ReadSByte() / 127f);
	}

	private static Vector4F Read_8_8_8_8_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSByte(), self.ReadSByte(), self.ReadSByte(), self.ReadSByte());
	}

	private static Vector4F Read_8_8_8_8_UIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadByte(), (int)self.ReadByte(), (int)self.ReadByte(), (int)self.ReadByte());
	}

	private static Vector4F Read_8_8_8_8_SIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSByte(), self.ReadSByte(), self.ReadSByte(), self.ReadSByte());
	}

	private static Vector4F Read_10_10_10_2_UNorm(this BinaryDataReader self)
	{
		uint num = self.ReadUInt32();
		return new Vector4F((float)(num & 0x3FF) / 1023f, (float)((num & 0xFFC00) >> 10) / 1023f, (float)((num & 0x3FF00000) >> 20) / 1023f, (float)((num & 0xC0000000u) >> 30) / 3f);
	}

	private static Vector4F Read_10_10_10_2_UInt(this BinaryDataReader self)
	{
		uint num = self.ReadUInt32();
		return new Vector4F(num & 0x3FF, (num & 0xFFC00) >> 10, (num & 0x3FF00000) >> 20, (num & 0xC0000000u) >> 30);
	}

	private static Vector4F Read_10_10_10_2_SNorm(this BinaryDataReader self)
	{
		int num = self.ReadInt32();
		return new Vector4F((float)(num << 22 >> 22) / 511f, (float)(num << 12 >> 22) / 511f, (float)(num << 2 >> 22) / 511f, num >> 30);
	}

	private static Vector4F Read_10_10_10_2_SInt(this BinaryDataReader self)
	{
		int num = self.ReadInt32();
		return new Vector4F(num << 22 >> 22, num << 12 >> 22, num << 2 >> 22, num >> 30);
	}

	private static Vector4F Read_32_32_UInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadUInt32(), self.ReadUInt32(), 0f, 0f);
	}

	private static Vector4F Read_32_32_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt32(), self.ReadInt32(), 0f, 0f);
	}

	private static Vector4F Read_32_32_Single(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSingle(), self.ReadSingle(), 0f, 0f);
	}

	private static Vector4F Read_16_16_16_16_UNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)(int)self.ReadUInt16() / 65535f, (float)(int)self.ReadUInt16() / 65535f, (float)(int)self.ReadUInt16() / 65535f, (float)(int)self.ReadUInt16() / 65535f);
	}

	private static Vector4F Read_16_16_16_16_UInt(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadUInt16(), (int)self.ReadUInt16(), (int)self.ReadUInt16(), (int)self.ReadUInt16());
	}

	private static Vector4F Read_16_16_16_16_SNorm(this BinaryDataReader self)
	{
		return new Vector4F((float)self.ReadInt16() / 32767f, (float)self.ReadInt16() / 32767f, (float)self.ReadInt16() / 32767f, (float)self.ReadInt16() / 32767f);
	}

	private static Vector4F Read_16_16_16_16_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt16(), self.ReadInt16(), self.ReadInt16(), self.ReadInt16());
	}

	private static Vector4F Read_16_16_16_16_Single(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadHalf(), self.ReadHalf(), self.ReadHalf(), self.ReadHalf());
	}

	private static Vector4F Read_16_16_16_16_UIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F((int)self.ReadUInt16(), (int)self.ReadUInt16(), (int)self.ReadUInt16(), (int)self.ReadUInt16());
	}

	private static Vector4F Read_16_16_16_16_SIntToSingle(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt16(), self.ReadInt16(), self.ReadInt16(), self.ReadInt16());
	}

	private static Vector4F Read_32_32_32_UInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadUInt32(), self.ReadUInt32(), self.ReadUInt32(), 0f);
	}

	private static Vector4F Read_32_32_32_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt32(), self.ReadInt32(), self.ReadInt32(), 0f);
	}

	private static Vector4F Read_32_32_32_Single(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), 0f);
	}

	private static Vector4F Read_32_32_32_32_UInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadUInt32(), self.ReadUInt32(), self.ReadUInt32(), self.ReadUInt32());
	}

	private static Vector4F Read_32_32_32_32_SInt(this BinaryDataReader self)
	{
		return new Vector4F(self.ReadInt32(), self.ReadInt32(), self.ReadInt32(), self.ReadInt32());
	}

	private static Vector4F Read_32_32_32_32_Single(this BinaryDataReader reader)
	{
		return new Vector4F(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
	}
}
