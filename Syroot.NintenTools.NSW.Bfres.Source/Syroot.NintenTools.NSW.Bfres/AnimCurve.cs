using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an animation curve used by several sections to control different parameters over time.
/// </summary>
public class AnimCurve : IResData
{
	private const ushort _flagsMaskFrameType = 3;

	private const ushort _flagsMaskKeyType = 12;

	private const ushort _flagsMaskCurveType = 112;

	/// <summary>
	/// Alignment of curve data
	/// </summary>
	public const int AlignmentBuffer = 8;

	private ushort _flags;

	internal long PosFrameOffset;

	internal long PosKeyDataOffset;

	/// <summary>
	/// Gets or sets the data type in which <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Frames" /> are loaded and saved. For simplicity, the class
	/// always stores frames as converted <see cref="T:System.Single" /> instances.
	/// </summary>
	public AnimCurveFrameType FrameType
	{
		get
		{
			return (AnimCurveFrameType)(_flags & 3);
		}
		set
		{
			_flags = (ushort)((uint)(_flags & -4) | (uint)value);
		}
	}

	/// <summary>
	/// Gets or sets the data type in which <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> are loaded and saved. For simplicity, the class
	/// always stores frames as converted <see cref="T:System.Single" /> instances.
	/// </summary>
	public AnimCurveKeyType KeyType
	{
		get
		{
			return (AnimCurveKeyType)(_flags & 0xC);
		}
		set
		{
			_flags = (ushort)((uint)(_flags & -13) | (uint)value);
		}
	}

	/// <summary>
	/// Gets or sets the curve type, determining the number of elements stored with each key.
	/// </summary>
	public AnimCurveType CurveType
	{
		get
		{
			return (AnimCurveType)(_flags & 0x70);
		}
		set
		{
			_flags = (ushort)((uint)(_flags & -113) | (uint)value);
		}
	}

	/// <summary>
	/// Gets or sets the memory offset relative to the start of the corresponding animation data structure to
	/// animate the field stored at that address. Note that enums exist in the specific animation which map offsets
	/// to names.
	/// </summary>
	public uint AnimDataOffset { get; set; }

	/// <summary>
	/// Gets or sets the first frame at which a key is placed.
	/// </summary>
	public float StartFrame { get; set; }

	/// <summary>
	/// Gets or sets the last frame at which a key is placed.
	/// </summary>
	public float EndFrame { get; set; }

	/// <summary>
	/// Gets or sets the scale to multiply values of the curve by.
	/// </summary>
	public float Scale { get; set; }

	/// <summary>
	/// Gets or sets the offset to add to the values of the curve (after multiplicating them).
	/// </summary>
	public DWord Offset { get; set; }

	/// <summary>
	/// Gets or sets the difference between the lowest and highest key value.
	/// </summary>
	public float Delta { get; set; }

	/// <summary>
	/// Gets the frame numbers at which keys of the same index in the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> array are placed.
	/// </summary>
	public float[] Frames { get; set; }

	/// <summary>
	/// Gets an array of elements forming the elements of keys placed at the frames of the same index in the
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Frames" /> array.
	/// </summary>
	public float[,] Keys { get; set; }

	public bool[] KeyStepBoolData { get; set; }

	/// <summary>
	/// True for cubic curves of Switch 2 (version 10) files. They store a normalized value and a slope per key
	/// instead of 4 polynomial coefficients. <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> then holds the
	/// decoded value (element 0) and the slope per frame (element 1) of each key.
	/// </summary>
	public bool IsHermite => IsV10 && CurveType == AnimCurveType.Cubic;

	/// <summary>
	/// True if the keys were read in the Switch 2 (version 10) layout. <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> then holds
	/// the final value in element 0 and the slope per frame (cubic curves, 0 otherwise) in element 1 of each key.
	/// Linear curves have one value per key. The unknown curve type 0x30 also has one value per key and is stepped.
	/// </summary>
	public bool IsV10 { get; private set; }

	private float[] _singleValues;

	private float[] _singleSlopes;

	//The quantized data as stored in the file. It is written back unchanged.
	private ushort[] _rawValues;

	private ushort[] _rawSlopes;

	private float _rawSlopeScale;

	private float _rawSlopeOffset;

	private int ElementsPerKey => CurveType switch
	{
		AnimCurveType.Cubic => 4, 
		AnimCurveType.Linear => 2, 
		_ => 1, 
	};

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> class.
	/// </summary>
	public AnimCurve()
	{
		FrameType = AnimCurveFrameType.Single;
		KeyType = AnimCurveKeyType.Single;
		CurveType = AnimCurveType.Cubic;
		AnimDataOffset = 0u;
		StartFrame = 0f;
		EndFrame = 0f;
		Scale = 0f;
		Frames = new float[0];
		KeyStepBoolData = new bool[0];
		Keys = new float[0, 4];
	}

	public AnimCurve Copy()
	{
		AnimCurve animCurve = new AnimCurve();
		animCurve._flags = _flags;
		animCurve.AnimDataOffset = AnimDataOffset;
		animCurve.CurveType = CurveType;
		animCurve.FrameType = FrameType;
		animCurve.Delta = Delta;
		animCurve.EndFrame = EndFrame;
		animCurve.StartFrame = StartFrame;
		animCurve.Offset = Offset;
		animCurve.Scale = Scale;
		animCurve.Frames = new float[Frames.Length];
		int elements = Keys.GetLength(1);
		animCurve.Keys = new float[Keys.GetLength(0), elements];
		for (int i = 0; i < Frames.Length; i++)
		{
			animCurve.Frames[i] = Frames[i];
		}
		for (int j = 0; j < Keys.GetLength(0); j++)
		{
			for (int k = 0; k < elements; k++)
			{
				animCurve.Keys[j, k] = Keys[j, k];
			}
		}
		animCurve.IsV10 = IsV10;
		animCurve._singleValues = _singleValues;
		animCurve._singleSlopes = _singleSlopes;
		animCurve._rawValues = _rawValues;
		animCurve._rawSlopes = _rawSlopes;
		animCurve._rawSlopeScale = _rawSlopeScale;
		animCurve._rawSlopeOffset = _rawSlopeOffset;
		return animCurve;
	}

	public float[] GetSlopes(float index)
	{
		float[] result = new float[2];
		if (IsHermite)
		{
			int key = (int)index;
			if (key >= 0 && key < Keys.GetLength(0))
			{
				result[0] = Keys[key, 1];
				result[1] = Keys[key, 1];
			}
			return result;
		}
		if (CurveType == AnimCurveType.Cubic)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < Frames.Length; i++)
			{
				float num3 = Keys[i, 0];
				float num4 = Keys[i, 1];
				float num5 = Keys[i, 2];
				float num6 = Keys[i, 3];
				float time = 0f;
				float delta = 0f;
				if (i < Frames.Length - 1)
				{
					float num7 = Keys[i + 1, 0];
					delta = num7 - num3;
					time = Frames[i + 1] - Frames[i];
				}
				float[] cubicSlopes = CurveInterpolationHelper.GetCubicSlopes(time, delta, new float[4] { num3, num4, num5, num6 });
				if (index == (float)i)
				{
					num2 = ((Keys[i, 1] != 0f) ? cubicSlopes[1] : 0f);
					return new float[2] { num, num2 };
				}
				num = cubicSlopes[0];
			}
		}
		return result;
	}

	void IResData.Load(ResFileLoader loader)
	{
		long value = loader.ReadOffset();
		long value2 = loader.ReadOffset();
		_flags = loader.ReadUInt16();
		ushort numKey = loader.ReadUInt16();
		AnimDataOffset = loader.ReadUInt32();
		StartFrame = loader.ReadSingle();
		EndFrame = loader.ReadSingle();
		Scale = loader.ReadSingle();
		Offset = loader.ReadSingle();
		Delta = loader.ReadSingle();
		int num = loader.ReadInt32();
		if (CurveType == AnimCurveType.StepBool)
		{
			int num2 = 0;
			if (KeyType == AnimCurveKeyType.Single)
			{
				num2 = loader.LoadCustom(() => loader.ReadInt32(), value2);
			}
			else if (KeyType == AnimCurveKeyType.Int16)
			{
				num2 = loader.LoadCustom(() => loader.ReadInt16(), value2);
			}
			else if (KeyType == AnimCurveKeyType.SByte)
			{
				num2 = loader.LoadCustom(() => loader.ReadSByte(), value2);
			}
			KeyStepBoolData = new bool[numKey];
			for (int num3 = 0; num3 < numKey; num3++)
			{
				bool flag = (num2 & 1) != 0;
				num2 >>= 1;
				KeyStepBoolData[num3] = flag;
			}
		}
		Frames = loader.LoadCustom(() =>
		{
			switch (FrameType)
			{
			case AnimCurveFrameType.Single:
				return loader.ReadSingles(numKey);
			case AnimCurveFrameType.Decimal10x5:
			{
				float[] array2 = new float[numKey];
				for (int j = 0; j < numKey; j++)
				{
					//Frames can't be negative. Animations with 1024 or more frames use the sign bit as part of the number.
					float frame = (float)loader.ReadDecimal10x5();
					array2[j] = ((frame < 0f) ? (frame + 2048f) : frame);
				}
				return array2;
			}
			case AnimCurveFrameType.Byte:
			{
				float[] array = new float[numKey];
				for (int i = 0; i < numKey; i++)
				{
					array[i] = (int)loader.ReadByte();
				}
				return array;
			}
			default:
				throw new ResException("Invalid FrameType.");
			}
		}, value);
		Keys = loader.LoadCustom(() =>
		{
			if (loader.ResFile.VersionMajor2 >= 10 && (CurveType == AnimCurveType.Cubic || CurveType == AnimCurveType.Linear || (int)CurveType == 48))
			{
				return LoadV10Keys(loader, numKey);
			}
			int elementsPerKey = ElementsPerKey;
			float[,] array = new float[numKey, elementsPerKey];
			switch (KeyType)
			{
			case AnimCurveKeyType.Single:
			{
				for (int k = 0; k < numKey; k++)
				{
					for (int l = 0; l < elementsPerKey; l++)
					{
						if (CurveType == AnimCurveType.StepInt)
						{
							array[k, l] = loader.ReadUInt32();
						}
						else
						{
							array[k, l] = loader.ReadSingle();
						}
					}
				}
				break;
			}
			case AnimCurveKeyType.Int16:
			{
				for (int m = 0; m < numKey; m++)
				{
					for (int n = 0; n < elementsPerKey; n++)
					{
						array[m, n] = loader.ReadInt16();
					}
				}
				break;
			}
			case AnimCurveKeyType.SByte:
			{
				for (int i = 0; i < numKey; i++)
				{
					for (int j = 0; j < elementsPerKey; j++)
					{
						array[i, j] = loader.ReadSByte();
					}
				}
				break;
			}
			default:
				throw new ResException("Invalid KeyType.");
			}
			return array;
		}, value2);
	}

	//Switch 2 (version 10) key layouts:
	//Cubic, quantized: two floats (scale and offset of the slopes), then an unsigned value and slope for every key.
	//Cubic, float: a float value and slope for every key, no header.
	//Linear and type 0x30: one value per key (float or unsigned quantized).
	//Quantized values are Delta + raw * Offset (curve header fields), slopes per frame are raw * scale + offset.
	private float[,] LoadV10Keys(ResFileLoader loader, int numKey)
	{
		IsV10 = true;
		bool cubic = CurveType == AnimCurveType.Cubic;
		float[,] array = new float[numKey, 2];
		if (KeyType == AnimCurveKeyType.Single)
		{
			_singleValues = new float[numKey];
			_singleSlopes = new float[numKey];
			for (int i = 0; i < numKey; i++)
			{
				_singleValues[i] = loader.ReadSingle();
				if (cubic)
				{
					_singleSlopes[i] = loader.ReadSingle();
				}
				array[i, 0] = _singleValues[i];
				array[i, 1] = _singleSlopes[i];
			}
			return array;
		}
		if (cubic)
		{
			_rawSlopeScale = loader.ReadSingle();
			_rawSlopeOffset = loader.ReadSingle();
		}
		_rawValues = new ushort[numKey];
		_rawSlopes = new ushort[numKey];
		for (int j = 0; j < numKey; j++)
		{
			if (KeyType == AnimCurveKeyType.Int16)
			{
				_rawValues[j] = loader.ReadUInt16();
				if (cubic)
				{
					_rawSlopes[j] = loader.ReadUInt16();
				}
			}
			else
			{
				_rawValues[j] = loader.ReadByte();
				if (cubic)
				{
					_rawSlopes[j] = loader.ReadByte();
				}
			}
			array[j, 0] = Delta + (float)(int)_rawValues[j] * Offset;
			array[j, 1] = (cubic ? ((float)(int)_rawSlopes[j] * _rawSlopeScale + _rawSlopeOffset) : 0f);
		}
		return array;
	}

	/// <summary>
	/// Fills this curve with cubic keys in the Switch 2 (version 10) format. The values are normalized between their
	/// lowest and highest value (stored in Delta and Offset), the slopes (per frame) get a scale and a code for slope 0.
	/// </summary>
	public void SetHermiteKeys(float[] frames, float[] values, float[] slopes, bool use16Bit)
	{
		int count = frames.Length;
		int maxCode = (use16Bit ? 65535 : 255);
		IsV10 = true;
		_singleValues = null;
		_singleSlopes = null;
		CurveType = AnimCurveType.Cubic;
		KeyType = (use16Bit ? AnimCurveKeyType.Int16 : AnimCurveKeyType.SByte);
		_flags = (ushort)(_flags | 0x4800);
		Frames = (float[])frames.Clone();
		float min = float.MaxValue;
		float max = float.MinValue;
		float slopeMin = 0f;
		float slopeMax = 0f;
		for (int i = 0; i < count; i++)
		{
			min = System.Math.Min(min, values[i]);
			max = System.Math.Max(max, values[i]);
			slopeMin = System.Math.Min(slopeMin, slopes[i]);
			slopeMax = System.Math.Max(slopeMax, slopes[i]);
		}
		if (count == 0)
		{
			min = 0f;
			max = 0f;
		}
		Scale = 0f;
		Delta = min;
		Offset = (max - min) / (float)maxCode;
		_rawSlopeScale = (slopeMax - slopeMin) / (float)maxCode;
		int zeroCode = ((_rawSlopeScale > 0f) ? (int)System.Math.Round(0f - slopeMin / _rawSlopeScale) : 0);
		_rawSlopeOffset = (0f - (float)zeroCode) * _rawSlopeScale;
		_rawValues = new ushort[count];
		_rawSlopes = new ushort[count];
		Keys = new float[count, 2];
		for (int j = 0; j < count; j++)
		{
			int value = ((Offset > 0f) ? (int)System.Math.Round((values[j] - min) / Offset) : 0);
			int slope = ((_rawSlopeScale > 0f) ? ((int)System.Math.Round(slopes[j] / _rawSlopeScale) + zeroCode) : 0);
			_rawValues[j] = (ushort)System.Math.Max(0, System.Math.Min(maxCode, value));
			_rawSlopes[j] = (ushort)System.Math.Max(0, System.Math.Min(maxCode, slope));
			Keys[j, 0] = Delta + (float)(int)_rawValues[j] * Offset;
			Keys[j, 1] = (float)(int)_rawSlopes[j] * _rawSlopeScale + _rawSlopeOffset;
		}
	}

	internal void SaveFrames(ResFileSaver saver)
	{
		switch (FrameType)
		{
		case AnimCurveFrameType.Single:
			saver.Write(Frames);
			break;
		case AnimCurveFrameType.Decimal10x5:
		{
			float[] frames2 = Frames;
			foreach (float num2 in frames2)
			{
				saver.Write((Decimal10x5)num2);
			}
			break;
		}
		case AnimCurveFrameType.Byte:
		{
			float[] frames = Frames;
			foreach (float num in frames)
			{
				saver.Write((byte)num);
			}
			break;
		}
		}
	}

	internal void SaveKeyData(ResFileSaver saver)
	{
		if (IsV10 && (_rawValues != null || _singleValues != null))
		{
			bool cubic = CurveType == AnimCurveType.Cubic;
			if (_singleValues != null)
			{
				for (int k = 0; k < _singleValues.Length; k++)
				{
					saver.Write(_singleValues[k]);
					if (cubic)
					{
						saver.Write(_singleSlopes[k]);
					}
				}
				return;
			}
			if (cubic)
			{
				saver.Write(_rawSlopeScale);
				saver.Write(_rawSlopeOffset);
			}
			for (int i = 0; i < _rawValues.Length; i++)
			{
				if (KeyType == AnimCurveKeyType.Int16)
				{
					saver.Write(_rawValues[i]);
					if (cubic)
					{
						saver.Write(_rawSlopes[i]);
					}
				}
				else
				{
					saver.Write((byte)_rawValues[i]);
					if (cubic)
					{
						saver.Write((byte)_rawSlopes[i]);
					}
				}
			}
			return;
		}
		switch (KeyType)
		{
		case AnimCurveKeyType.Single:
		{
			float[,] keys2 = Keys;
			foreach (float num2 in keys2)
			{
				if (CurveType == AnimCurveType.StepInt)
				{
					saver.Write((uint)num2);
				}
				else
				{
					saver.Write(num2);
				}
			}
			break;
		}
		case AnimCurveKeyType.Int16:
		{
			float[,] keys3 = Keys;
			foreach (float num3 in keys3)
			{
				saver.Write((short)num3);
			}
			break;
		}
		case AnimCurveKeyType.SByte:
		{
			float[,] keys = Keys;
			foreach (float num in keys)
			{
				saver.Write((sbyte)num);
			}
			break;
		}
		}
	}

	void IResData.Save(ResFileSaver saver)
	{
		PosFrameOffset = saver.SaveOffset();
		PosKeyDataOffset = saver.SaveOffset();
		saver.Write(_flags);
		saver.Write((ushort)Frames.Length);
		saver.Write(AnimDataOffset);
		saver.Write(StartFrame);
		saver.Write(EndFrame);
		saver.Write(Scale);
		saver.Write(Offset);
		saver.Write(Delta);
		saver.Write(0);
	}
}
