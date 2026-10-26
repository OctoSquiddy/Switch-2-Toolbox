using System;
using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a render info in a FMAT section storing uniform parameters required to render the
/// <see cref="T:Syroot.NintenTools.NSW.Bfres.UserData" />.
/// </summary>
[DebuggerDisplay("RenderInfo {Name}")]
public class RenderInfo : IResData
{
	public object _value;

	internal long DataOffset;

	/// <summary>
	/// Gets the <see cref="T:Syroot.NintenTools.NSW.Bfres.RenderInfoType" /> determining the data type of the stored value.
	/// </summary>
	public RenderInfoType Type { get; internal set; }

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in
	/// <see cref="!:ResDict&lt;RenderInfo&gt;" /> instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.RenderInfo" /> class.
	/// </summary>
	public RenderInfo()
	{
		Name = "";
		SetValue(new int[0]);
	}

	/// <summary>
	/// Gets the stored value as an <see cref="T:System.Int32" /> array. Only valid if <see cref="P:Syroot.NintenTools.NSW.Bfres.RenderInfo.Type" /> is
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.RenderInfoType.Int32" />.
	/// </summary>
	/// <returns>The stored value as an <see cref="T:System.Int32" /> array.</returns>
	public int[] GetValueInt32s()
	{
		if (_value == null)
		{
			return new int[0];
		}
		return (int[])_value;
	}

	/// <summary>
	/// Gets the stored value as a <see cref="T:System.Single" /> array. Only valid if <see cref="P:Syroot.NintenTools.NSW.Bfres.RenderInfo.Type" /> is
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.RenderInfoType.Single" />.
	/// </summary>
	/// <returns>The stored value as a <see cref="T:System.Single" /> array.</returns>
	public float[] GetValueSingles()
	{
		if (_value == null)
		{
			return new float[0];
		}
		return (float[])_value;
	}

	/// <summary>
	/// Gets the stored value as a <see cref="T:System.String" /> array. Only valid if <see cref="P:Syroot.NintenTools.NSW.Bfres.RenderInfo.Type" /> is
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.RenderInfoType.String" />.
	/// </summary>
	/// <returns>The stored value as a <see cref="T:System.String" /> array.</returns>
	public string[] GetValueStrings()
	{
		if (_value == null || Type != RenderInfoType.String)
		{
			return new string[0];
		}
		return (string[])_value;
	}

	/// <summary>
	/// Sets the stored value as an <see cref="T:System.Int32" /> array and sets <see cref="P:Syroot.NintenTools.NSW.Bfres.RenderInfo.Type" /> to
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.RenderInfoType.Int32" />.
	/// </summary>
	/// <param name="value">The <see cref="T:System.Int32" /> array to set as the value.</param>
	public void SetValue(int[] value)
	{
		Type = RenderInfoType.Int32;
		_value = value;
	}

	/// <summary>
	/// Sets the stored value as a <see cref="T:System.Single" /> array and sets <see cref="P:Syroot.NintenTools.NSW.Bfres.RenderInfo.Type" /> to
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.RenderInfoType.Single" />.
	/// </summary>
	/// <param name="value">The <see cref="T:System.Single" /> array to set as the value.</param>
	public void SetValue(float[] value)
	{
		Type = RenderInfoType.Single;
		_value = value;
	}

	/// <summary>
	/// Sets the stored value as a <see cref="T:System.String" /> array and sets <see cref="P:Syroot.NintenTools.NSW.Bfres.RenderInfo.Type" /> to
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.RenderInfoType.String" />.
	/// </summary>
	/// <param name="value">The <see cref="T:System.String" /> array to set as the value.</param>
	public void SetValue(string[] value)
	{
		Type = RenderInfoType.String;
		_value = value;
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		long num = loader.ReadOffset();
		ushort count = loader.ReadUInt16();
		Type = loader.ReadEnum<RenderInfoType>(strict: true);
		loader.Seek(5L);
		switch (Type)
		{
		case RenderInfoType.Int32:
			_value = loader.LoadCustom(() => loader.ReadInt32s(count), num);
			break;
		case RenderInfoType.Single:
			_value = loader.LoadCustom(() => loader.ReadSingles(count), num);
			break;
		case RenderInfoType.String:
			if (num == 0)
			{
				string[] value = new string[0];
				_value = value;
			}
			else
			{
				_value = loader.LoadCustom(() => loader.LoadStrings(count), num);
			}
			break;
		}
	}

	internal void ReadData(ResFileLoader loader, RenderInfoType type, int count)
	{
		Type = type;
		switch (Type)
		{
		case RenderInfoType.Int32:
			_value = loader.ReadInt32s(count);
			break;
		case RenderInfoType.Single:
			_value = loader.ReadSingles(count);
			break;
		case RenderInfoType.String:
			_value = loader.LoadStrings(count);
			break;
		}
	}

	internal void SaveStrings(ResFileSaver saver)
	{
		saver.SaveStrings((string[])_value);
	}

	internal void SaveInts(ResFileSaver saver)
	{
		saver.Write((int[])_value);
	}

	internal void SaveFloats(ResFileSaver saver)
	{
		saver.Write((float[])_value);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(Name);
		DataOffset = saver.SaveOffset();
		saver.Write((ushort)((_value != null) ? ((ushort)((Array)_value).Length) : 0));
		saver.Write(Type, strict: true);
		saver.Seek(5L);
	}

	public override string ToString()
	{
		return string.Format("{0} {1} {2}", Name, Type, string.Join(",", _value));
	}
}
