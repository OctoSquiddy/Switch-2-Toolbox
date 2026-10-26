using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents custom user variables which can be attached to many sections and subfiles of a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" />.
/// </summary>
[DebuggerDisplay("UserData {Name}")]
public class UserData : IResData
{
	private object _value;

	internal long DataOffset;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;UserData&gt;" />
	/// instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// The data type of the stored values.
	/// </summary>
	public UserDataType Type { get; private set; }

	public UserData()
	{
		Name = "";
		SetValue(new int[1]);
	}

	/// <summary>
	/// Returns the stored value as an array of <see cref="T:System.Int32" /> instances when the <see cref="P:Syroot.NintenTools.NSW.Bfres.UserData.Type" /> is
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.Int32" />.
	/// </summary>
	/// <returns>The typed value.</returns>
	public int[] GetValueInt32Array()
	{
		return (int[])_value;
	}

	/// <summary>
	/// Returns the stored value as an array of <see cref="T:System.Single" /> instances when the <see cref="P:Syroot.NintenTools.NSW.Bfres.UserData.Type" /> is
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.Single" />.
	/// </summary>
	/// <returns>The typed value.</returns>
	public float[] GetValueSingleArray()
	{
		if (_value == null)
		{
			return new float[0];
		}
		return (float[])_value;
	}

	/// <summary>
	/// Returns the stored value as an array of <see cref="T:System.String" /> instances when the <see cref="P:Syroot.NintenTools.NSW.Bfres.UserData.Type" /> is
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.String" /> or <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.WString" />.
	/// </summary>
	/// <returns>The typed value.</returns>
	public string[] GetValueStringArray()
	{
		if (_value == null || (Type != UserDataType.String && Type != UserDataType.WString))
		{
			return new string[0];
		}
		return (string[])_value;
	}

	/// <summary>
	/// Returns the stored value as an array of <see cref="T:System.Byte" /> instances when the <see cref="P:Syroot.NintenTools.NSW.Bfres.UserData.Type" /> is
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.Byte" />.
	/// </summary>
	/// <returns>The typed value.</returns>
	public byte[] GetValueByteArray()
	{
		if (_value == null)
		{
			return new byte[0];
		}
		return (byte[])_value;
	}

	/// <summary>
	/// Sets the stored <paramref name="value" /> as an <see cref="T:System.Int32" /> array and the <see cref="P:Syroot.NintenTools.NSW.Bfres.UserData.Type" /> to
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.Int32" />
	/// </summary>
	/// <param name="value">The value to store.</param>
	public void SetValue(int[] value)
	{
		Type = UserDataType.Int32;
		_value = value;
	}

	/// <summary>
	/// Sets the stored <paramref name="value" /> as a <see cref="T:System.Single" /> array and the <see cref="P:Syroot.NintenTools.NSW.Bfres.UserData.Type" /> to
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.Single" />
	/// </summary>
	/// <param name="value">The value to store.</param>
	public void SetValue(float[] value)
	{
		Type = UserDataType.Single;
		_value = value;
	}

	/// <summary>
	/// Sets the stored <paramref name="value" /> as a <see cref="T:System.String" /> array and the <see cref="P:Syroot.NintenTools.NSW.Bfres.UserData.Type" /> to
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.String" /> or <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.WString" /> depending on
	/// <paramref name="asUnicode" />.
	/// </summary>
	/// <param name="asUnicode"><c>true</c> to store data as UTF-16 encoded strings, or <c>false</c> to store it
	/// as ASCII encoded strings.</param>
	/// <param name="value">The value to store.</param>
	public void SetValue(string[] value, bool asUnicode = false)
	{
		Type = (asUnicode ? UserDataType.WString : UserDataType.String);
		_value = value;
	}

	/// <summary>
	/// Sets the stored <paramref name="value" /> as a <see cref="T:System.Byte" /> array and the <see cref="P:Syroot.NintenTools.NSW.Bfres.UserData.Type" /> to
	/// <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.Byte" />
	/// </summary>
	/// <param name="value">The value to store.</param>
	public void SetValue(byte[] value)
	{
		Type = UserDataType.Byte;
		_value = value;
	}

	public UserData Copy()
	{
		UserData userData = new UserData();
		userData.Name = Name;
		switch (Type)
		{
		case UserDataType.Byte:
			userData.SetValue(GetValueByteArray());
			break;
		case UserDataType.Int32:
			userData.SetValue(GetValueInt32Array());
			break;
		case UserDataType.Single:
			userData.SetValue(GetValueSingleArray());
			break;
		case UserDataType.String:
			userData.SetValue(GetValueStringArray());
			break;
		case UserDataType.WString:
			userData.SetValue(GetValueStringArray());
			break;
		}
		return userData;
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		long value = loader.ReadOffset(Relocated: true);
		uint count = 0u;
		if (loader.ResFile.VersionMajor2 <= 2 && loader.ResFile.VersionMajor == 0)
		{
			char[] array = loader.ReadChars(8);
			count = loader.ReadUInt32();
			Type = (UserDataType)loader.ReadUInt32();
		}
		else
		{
			count = loader.ReadUInt32();
			Type = loader.ReadEnum<UserDataType>(strict: true);
			char[] array2 = loader.ReadChars(43);
		}
		switch (Type)
		{
		case UserDataType.Byte:
			_value = loader.LoadCustom(() => loader.ReadSBytes((int)count), value);
			break;
		case UserDataType.Int32:
			_value = loader.LoadCustom(() => loader.ReadInt32s((int)count), value);
			break;
		case UserDataType.Single:
			_value = loader.LoadCustom(() => loader.ReadSingles((int)count), value);
			break;
		case UserDataType.String:
			_value = loader.LoadCustom(() => loader.LoadStrings((int)count, Encoding.UTF8), value);
			break;
		case UserDataType.WString:
			_value = loader.LoadCustom(() => loader.LoadStrings((int)count, Encoding.Unicode), value);
			break;
		}
	}

	private bool ContainsUnicodeCharacter(object input)
	{
		if (input is string[])
		{
			string[] array = (string[])input;
			foreach (string source in array)
			{
				if (source.Any((char c) => c > 'ÿ'))
				{
					return true;
				}
			}
		}
		return false;
	}

	internal void SaveData(ResFileSaver saver)
	{
		switch (Type)
		{
		case UserDataType.Int32:
			saver.Write((int[])_value);
			break;
		case UserDataType.Single:
			saver.Write((float[])_value);
			break;
		case UserDataType.String:
			saver.SaveStrings((string[])_value, relocate: false, Encoding.UTF8);
			break;
		case UserDataType.WString:
			saver.SaveStrings((string[])_value, relocate: false, Encoding.Unicode);
			break;
		case UserDataType.Byte:
			saver.Write((byte[])_value);
			break;
		}
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(Name);
		DataOffset = saver.SaveOffset();
		saver.Write((_value != null) ? ((Array)_value).Length : 0);
		saver.Write(Type, strict: true);
		saver.Seek(43L);
	}
}
