using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

public class MaterialParserV10
{
	public class ShaderInfo : IResData
	{
		public ShaderAssignV10 ShaderAssign;

		public IList<string> AttribAssigns;

		public IList<string> SamplerAssigns;

		public bool[] OptionToggles = new bool[0];

		public IList<string> OptionValues;

		public short[] OptionIndices;

		public sbyte[] AttributeAssignIndices;

		public sbyte[] SamplerAssignIndices;

		private long[] _optionBitFlags = new long[0];

		void IResData.Load(ResFileLoader loader)
		{
			ShaderAssign = loader.Load<ShaderAssignV10>();
			long num = loader.ReadOffset();
			long offset = loader.ReadOffset();
			long num2 = loader.ReadOffset();
			long offset2 = loader.ReadOffset();
			long num3 = loader.ReadOffset();
			long num4 = loader.ReadOffset();
			long offset3 = loader.ReadOffset();
			loader.ReadUInt32();
			byte numAttributeAssign = loader.ReadByte();
			byte numSamplerAssign = loader.ReadByte();
			ushort num5 = loader.ReadUInt16();
			ushort num6 = loader.ReadUInt16();
			loader.ReadUInt16();
			loader.ReadUInt32();
			int numBitflags = 1 + (num5 - 1) / 64;
			AttribAssigns = loader.LoadCustom(() => loader.LoadStrings(numAttributeAssign), (uint)num);
			SamplerAssigns = loader.LoadCustom(() => loader.LoadStrings(numSamplerAssign), (uint)num2);
			_optionBitFlags = loader.LoadCustom(() => loader.ReadInt64s(numBitflags), (uint)num3);
			OptionIndices = ReadShortIndices(loader, offset3, num6, ShaderAssign.Options.Count);
			AttributeAssignIndices = ReadByteIndices(loader, offset, numAttributeAssign, ShaderAssign.AttributeAssign.Count);
			SamplerAssignIndices = ReadByteIndices(loader, offset2, numSamplerAssign, ShaderAssign.SamplerAssign.Count);
			int numChoiceValues = num6 - num5;
			OptionValues = loader.LoadCustom(() => loader.LoadStrings(numChoiceValues), (uint)num4);
			SetupOptionBooleans(num5);
		}

		void IResData.Save(ResFileSaver saver)
		{
			CreateOptionFlag();
			saver.SaveRelocateEntryToSection(saver.Position, 8u, 1u, 0u, 1, "ShaderInfo");
			saver.Save(ShaderAssign, -1, 0L);
			saver.SaveCustom(AttribAssigns, () =>
			{
				saver.SaveStrings(AttribAssigns, relocate: true);
			}, 0L);
			saver.SaveCustom(AttributeAssignIndices, () =>
			{
				WriteIndices(saver, AttributeAssignIndices);
			}, 0L);
			saver.SaveCustom(SamplerAssigns, () =>
			{
				saver.SaveStrings(SamplerAssigns, relocate: true);
			}, 0L);
			saver.SaveCustom(SamplerAssignIndices, () =>
			{
				WriteIndices(saver, SamplerAssignIndices);
			}, 0L);
			saver.SaveCustom(OptionToggles, () =>
			{
				saver.Write(_optionBitFlags);
			}, 0L);
			saver.SaveCustom(OptionValues, () =>
			{
				saver.SaveStrings(OptionValues, relocate: true);
			}, 0L);
			saver.SaveCustom(OptionIndices, () =>
			{
				WriteIndices(saver, OptionIndices);
			}, 0L);
			saver.Write(0);
			saver.Write((byte)(AttribAssigns?.Count).Value);
			saver.Write((byte)(SamplerAssigns?.Count).Value);
			saver.Write((ushort)(OptionToggles?.Length).Value);
			saver.Write((ushort)(OptionToggles?.Length + OptionValues?.Count).Value);
			saver.Write(new byte[6]);
		}

		private sbyte[] ReadByteIndices(ResFileLoader loader, long offset, int usedCount, int totalCount)
		{
			if (offset == 0)
			{
				return null;
			}
			using (loader.TemporarySeek((int)offset, SeekOrigin.Begin))
			{
				sbyte[] array = loader.ReadSBytes(usedCount);
				return loader.ReadSBytes(totalCount);
			}
		}

		private short[] ReadShortIndices(ResFileLoader loader, long offset, int usedCount, int totalCount)
		{
			if (offset == 0)
			{
				return null;
			}
			using (loader.TemporarySeek((int)offset, SeekOrigin.Begin))
			{
				short[] array = loader.ReadInt16s(usedCount);
				return loader.ReadInt16s(totalCount);
			}
		}

		private void SetupOptionBooleans(int count)
		{
			OptionToggles = new bool[count];
			//Materials without shader options have no bit flag array in the file
			if (_optionBitFlags == null)
			{
				_optionBitFlags = new long[Math.Max(1, 1 + (count - 1) / 64)];
			}
			long[] array = _optionBitFlags.ToArray();
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				if (i != 0 && i % 64 == 0)
				{
					num++;
				}
				OptionToggles[i] = (_optionBitFlags[num] & (1L << i)) != 0;
			}
			CreateOptionFlag();
			for (int j = 0; j < _optionBitFlags.Length; j++)
			{
				if (_optionBitFlags[j] != array[j])
				{
					throw new Exception();
				}
			}
		}

		private void CreateOptionFlag()
		{
			int num = 1 + (OptionToggles.Length - 1) / 64;
			_optionBitFlags = new long[num];
			int num2 = 0;
			for (int i = 0; i < OptionToggles.Length; i++)
			{
				if (i != 0 && i % 64 == 0)
				{
					num2++;
				}
				if (OptionToggles[i])
				{
					_optionBitFlags[num2] |= 1L << i;
				}
			}
		}

		private void WriteIndices(ResFileSaver saver, short[] indices)
		{
			for (short num = 0; num < indices.Length; num++)
			{
				if (indices[num] != -1)
				{
					saver.Write(num);
				}
			}
			saver.Write(indices);
			saver.Align(8);
		}

		private void WriteIndices(ResFileSaver saver, sbyte[] indices)
		{
			for (sbyte b = 0; b < indices.Length; b++)
			{
				if (indices[b] != -1)
				{
					saver.Write(b);
				}
			}
			saver.Write(indices);
			saver.Align(8);
		}
	}

	public class ShaderAssignV10 : IResData
	{
		public ResDict RenderInfos = new ResDict();

		public ResDict ShaderParameters = new ResDict();

		public ResDict AttributeAssign = new ResDict();

		public ResDict SamplerAssign = new ResDict();

		public ResDict Options = new ResDict();

		public string ShaderArchiveName;

		public string ShadingModelName;

		internal long shaderParamOffset;

		internal long renderInfoListOffset;

		public ushort ShaderParamSize;

		public ushort RenderInfoCount;

		public ushort ParamCount;

		public Material ParentMaterial;

		void IResData.Load(ResFileLoader loader)
		{
			ShaderArchiveName = loader.LoadString();
			ShadingModelName = loader.LoadString();
			renderInfoListOffset = loader.ReadOffset();
			RenderInfos = loader.LoadDict();
			shaderParamOffset = loader.ReadOffset();
			ShaderParameters = loader.LoadDict();
			AttributeAssign = loader.LoadDict();
			SamplerAssign = loader.LoadDict();
			Options = loader.LoadDict();
			RenderInfoCount = loader.ReadUInt16();
			ParamCount = loader.ReadUInt16();
			ShaderParamSize = loader.ReadUInt16();
			loader.ReadUInt16();
			loader.ReadUInt64();
		}

		void IResData.Save(ResFileSaver saver)
		{
			RenderInfos = ParentMaterial.RenderInfoDict;
			ShaderParameters = ParentMaterial.ShaderParamDict;
			saver.SaveRelocateEntryToSection(saver.Position, 9u, 1u, 0u, 1, "ShaderAssignV10");
			saver.SaveString(ShaderArchiveName);
			saver.SaveString(ShadingModelName);
			saver.SaveCustom(new long[ParentMaterial.RenderInfos.Count], () =>
			{
				saver.SaveRelocateEntryToSection(saver.Position, 1u, (uint)ParentMaterial.RenderInfos.Count, 1u, 1, "Render Param Info V10");
				foreach (RenderInfo renderInfo in ParentMaterial.RenderInfos)
				{
					saver.SaveString(renderInfo.Name);
					saver.Write((byte)renderInfo.Type);
					saver.Write(new byte[7]);
				}
			}, 0L);
			saver.SaveDict(RenderInfos, 0L);
			saver.SaveCustom(new long[ParentMaterial.ShaderParams.Count], () =>
			{
				saver.SaveRelocateEntryToSection(saver.Position, 2u, (uint)ParentMaterial.ShaderParams.Count, 1u, 1, "Shader Param Info V10");
				foreach (ShaderParam shaderParam in ParentMaterial.ShaderParams)
				{
					saver.Write(new byte[8]);
					saver.SaveString(shaderParam.Name);
					saver.Write(shaderParam.DataOffset);
					saver.Write((ushort)shaderParam.Type);
					saver.Write(new byte[4]);
				}
			}, 0L);
			saver.SaveDict(ShaderParameters, 0L);
			saver.SaveDict(AttributeAssign, 0L);
			saver.SaveDict(SamplerAssign, 0L);
			saver.SaveDict(Options, 0L);
			saver.Write((ushort)ParentMaterial.RenderInfos.Count);
			saver.Write((ushort)ParentMaterial.ShaderParams.Count);
			saver.Write((ushort)ParentMaterial.ShaderParamData.Length);
			saver.Write((ushort)0);
			saver.Write(0uL);
		}

		public override int GetHashCode()
		{
			int num = 0;
			num += ShaderArchiveName.GetHashCode();
			num += ShadingModelName.GetHashCode();
			foreach (RenderInfo renderInfo in ParentMaterial.RenderInfos)
			{
				num += renderInfo.Name.GetHashCode();
				num += renderInfo.Type.GetHashCode();
			}
			foreach (ShaderParam shaderParam in ParentMaterial.ShaderParams)
			{
				num += shaderParam.Name.GetHashCode();
				num += shaderParam.DataOffset.GetHashCode();
				num += shaderParam.Type.GetHashCode();
			}
			foreach (object item in (IEnumerable)Options)
			{
				num += item.GetHashCode();
			}
			foreach (object item2 in (IEnumerable)AttributeAssign)
			{
				num += item2.GetHashCode();
			}
			foreach (object item3 in (IEnumerable)SamplerAssign)
			{
				num += item3.GetHashCode();
			}
			return num;
		}
	}

	public static void PrepareSave(Material mat)
	{
		ShaderInfo shaderInfo = new ShaderInfo();
		shaderInfo.ShaderAssign = new ShaderAssignV10();
		shaderInfo.ShaderAssign.ParentMaterial = mat;
		shaderInfo.ShaderAssign.ShaderArchiveName = mat.ShaderAssign.ShaderArchiveName;
		shaderInfo.ShaderAssign.ShadingModelName = mat.ShaderAssign.ShadingModelName;
		shaderInfo.ShaderAssign.ParamCount = (ushort)mat.ShaderParams.Count;
		shaderInfo.ShaderAssign.RenderInfoCount = (ushort)mat.RenderInfos.Count;
		shaderInfo.SamplerAssigns = new List<string>();
		shaderInfo.AttribAssigns = new List<string>();
		shaderInfo.OptionValues = new List<string>();
		if (mat.ShaderAssign != null)
		{
			List<sbyte> list = new List<sbyte>();
			List<sbyte> list2 = new List<sbyte>();
			List<short> list3 = new List<short>();
			foreach (string samplerAssign in mat.ShaderAssign.SamplerAssigns)
			{
				if (samplerAssign == "Default Value>")
				{
					list.Add(-1);
					continue;
				}
				shaderInfo.SamplerAssigns.Add(samplerAssign);
				list.Add((sbyte)shaderInfo.SamplerAssigns.IndexOf(samplerAssign));
			}
			foreach (string attribAssign in mat.ShaderAssign.AttribAssigns)
			{
				if (attribAssign == "<Default Value>")
				{
					list2.Add(-1);
					continue;
				}
				shaderInfo.AttribAssigns.Add(attribAssign);
				list2.Add((sbyte)shaderInfo.AttribAssigns.IndexOf(attribAssign));
			}
			int num = 0;
			List<bool> list4 = new List<bool>();
			foreach (string shaderOption in mat.ShaderAssign.ShaderOptions)
			{
				switch (shaderOption)
				{
				case "<Default Value>":
					list3.Add(-1);
					continue;
				case "True":
					list4.Add(item: true);
					break;
				case "False":
					list4.Add(item: false);
					break;
				default:
					shaderInfo.OptionValues.Add(shaderOption);
					break;
				}
				list3.Add((short)num);
				num++;
			}
			shaderInfo.OptionToggles = list4.ToArray();
			if (list.Any((sbyte x) => x == -1))
			{
				shaderInfo.SamplerAssignIndices = list.ToArray();
			}
			if (list2.Any((sbyte x) => x == -1))
			{
				shaderInfo.AttributeAssignIndices = list2.ToArray();
			}
			shaderInfo.OptionIndices = list3.ToArray();
			foreach (string item in (IEnumerable)mat.ShaderAssign.SamplerAssignDict)
			{
				shaderInfo.ShaderAssign.SamplerAssign.Add(item);
			}
			foreach (string item2 in (IEnumerable)mat.ShaderAssign.AttribAssignDict)
			{
				shaderInfo.ShaderAssign.AttributeAssign.Add(item2);
			}
			foreach (string item3 in (IEnumerable)mat.ShaderAssign.ShaderOptionDict)
			{
				shaderInfo.ShaderAssign.Options.Add(item3);
			}
		}
		List<RenderInfo> list5 = new List<RenderInfo>();
		list5.AddRange(mat.RenderInfos.Where((RenderInfo x) => x.Type == RenderInfoType.String));
		list5.AddRange(mat.RenderInfos.Where((RenderInfo x) => x.Type == RenderInfoType.Single));
		list5.AddRange(mat.RenderInfos.Where((RenderInfo x) => x.Type == RenderInfoType.Int32));
		mat.RenderInfos = list5.ToList();
		mat.RenderInfoDict.Clear();
		foreach (RenderInfo renderInfo in mat.RenderInfos)
		{
			mat.RenderInfoDict.Add(renderInfo.Name);
		}
		mat.ShaderInfoV10 = shaderInfo;
	}

	public static void Load(ResFileLoader loader, Material mat)
	{
		ShaderInfo info = loader.Load<ShaderInfo>();
		long num = loader.ReadOffset();
		long num2 = loader.ReadOffset();
		long num3 = loader.ReadOffset();
		long value = loader.ReadOffset();
		mat.SamplerDict = loader.LoadDict();
		long renderInfoDataTable = loader.ReadOffset();
		long renderInfoCounterTable = loader.ReadOffset();
		long renderInfoDataOffsets = loader.ReadOffset();
		long num4 = loader.ReadOffset();
		long num5 = loader.ReadOffset();
		loader.ReadUInt64();
		long value2 = loader.ReadOffset();
		mat.UserDataDict = loader.LoadDict();
		long num6 = loader.ReadOffset();
		long num7 = loader.ReadInt64();
		long num8 = loader.ReadOffset();
		long num9 = loader.ReadOffset();
		ushort num10 = loader.ReadUInt16();
		byte numSampler = loader.ReadByte();
		byte numTextureRef = loader.ReadByte();
		loader.ReadUInt16();
		ushort count = loader.ReadUInt16();
		ushort renderInfoSize = loader.ReadUInt16();
		ushort num11 = loader.ReadUInt16();
		loader.ReadUInt32();
		mat.RenderInfoSize = renderInfoSize;
		long position = loader.Position;
		mat.TextureRefs = loader.LoadCustom(() => loader.LoadStrings(numTextureRef), (uint)num2);
		mat.Samplers = loader.LoadList<Sampler>(numSampler, value);
		mat.UserDatas = loader.LoadList<UserData>(count, value2);
		mat.TextureSlotArray = loader.LoadCustom(() => loader.ReadInt64s(numTextureRef), (uint)num9);
		mat.SamplerSlotArray = loader.LoadCustom(() => loader.ReadInt64s(numSampler), (uint)num8);
		if (info != null)
		{
			mat.ShaderAssign = new ShaderAssign
			{
				ShaderArchiveName = info.ShaderAssign.ShaderArchiveName,
				ShadingModelName = info.ShaderAssign.ShadingModelName
			};
			mat.ShaderParamData = loader.LoadCustom(() => loader.ReadBytes(info.ShaderAssign.ShaderParamSize), (uint)num4);
			mat.ParamIndices = loader.LoadCustom(() => loader.ReadInt32s(info.ShaderAssign.ShaderParameters.Count), (uint)num5);
			ReadRenderInfo(loader, info, mat, renderInfoCounterTable, renderInfoDataOffsets, renderInfoDataTable);
			ReadShaderParams(loader, info, mat);
			LoadAttributeAssign(info, mat);
			LoadSamplerAssign(info, mat);
			LoadShaderOptions(info, mat);
			loader.Seek(position, SeekOrigin.Begin);
		}
	}

	private static void ReadRenderInfo(ResFileLoader loader, ShaderInfo info, Material mat, long renderInfoCounterTable, long renderInfoDataOffsets, long renderInfoDataTable)
	{
		mat.RenderInfoDict = new ResDict();
		mat.RenderInfos = new List<RenderInfo>();
		for (int i = 0; i < info.ShaderAssign.RenderInfos.Count; i++)
		{
			RenderInfo renderInfo = new RenderInfo();
			loader.Seek((int)info.ShaderAssign.renderInfoListOffset + i * 16, SeekOrigin.Begin);
			renderInfo.Name = loader.LoadString();
			renderInfo.Type = (RenderInfoType)loader.ReadByte();
			loader.Seek((int)renderInfoCounterTable + i * 2, SeekOrigin.Begin);
			ushort count = loader.ReadUInt16();
			loader.Seek((int)renderInfoDataOffsets + i * 2, SeekOrigin.Begin);
			renderInfo.DataOffset = loader.ReadUInt16();
			loader.Seek((int)renderInfoDataTable + renderInfo.DataOffset, SeekOrigin.Begin);
			renderInfo.ReadData(loader, renderInfo.Type, count);
			mat.RenderInfos.Add(renderInfo);
			mat.RenderInfoDict.Add(renderInfo.Name);
		}
	}

	private static void ReadShaderParams(ResFileLoader loader, ShaderInfo info, Material mat)
	{
		mat.ShaderParamDict = new ResDict();
		mat.ShaderParams = new List<ShaderParam>();
		for (int i = 0; i < info.ShaderAssign.ShaderParameters.Count; i++)
		{
			ShaderParam shaderParam = new ShaderParam();
			loader.Seek((int)info.ShaderAssign.shaderParamOffset + i * 24, SeekOrigin.Begin);
			ulong num = loader.ReadUInt64();
			shaderParam.Name = loader.LoadString();
			shaderParam.DataOffset = loader.ReadUInt16();
			shaderParam.Type = (ShaderParamType)loader.ReadUInt16();
			uint num2 = loader.ReadUInt32();
			if (num != 0L || num2 != 0)
			{
				throw new Exception();
			}
			mat.ShaderParams.Add(shaderParam);
			mat.ShaderParamDict.Add(shaderParam.Name);
		}
	}

	private static void LoadAttributeAssign(ShaderInfo info, Material mat)
	{
		for (int i = 0; i < info.ShaderAssign.AttributeAssign.Count; i++)
		{
			sbyte[] attributeAssignIndices = info.AttributeAssignIndices;
			int num = ((attributeAssignIndices != null && attributeAssignIndices.Length != 0) ? info.AttributeAssignIndices[i] : i);
			string item = ((num == -1) ? "<Default Value>" : info.AttribAssigns[num]);
			string key = info.ShaderAssign.AttributeAssign.GetKey(i);
			mat.ShaderAssign.AttribAssigns.Add(item);
			mat.ShaderAssign.AttribAssignDict.Add(key);
		}
	}

	private static void LoadSamplerAssign(ShaderInfo info, Material mat)
	{
		for (int i = 0; i < info.ShaderAssign.SamplerAssign.Count; i++)
		{
			sbyte[] samplerAssignIndices = info.SamplerAssignIndices;
			int num = ((samplerAssignIndices != null && samplerAssignIndices.Length != 0) ? info.SamplerAssignIndices[i] : i);
			string item = ((num == -1) ? "Default Value>" : info.SamplerAssigns[num]);
			string key = info.ShaderAssign.SamplerAssign.GetKey(i);
			mat.ShaderAssign.SamplerAssigns.Add(item);
			mat.ShaderAssign.SamplerAssignDict.Add(key);
		}
	}

	private static void LoadShaderOptions(ShaderInfo info, Material mat)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < info.OptionToggles.Length; i++)
		{
			list.Add(info.OptionToggles[i] ? "True" : "False");
		}
		if (info.OptionValues != null)
		{
			list.AddRange(info.OptionValues);
		}
		for (int j = 0; j < info.ShaderAssign.Options.Count; j++)
		{
			short[] optionIndices = info.OptionIndices;
			int num = ((optionIndices != null && optionIndices.Length != 0) ? info.OptionIndices[j] : j);
			string item = ((num == -1) ? "<Default Value>" : list[num]);
			string key = info.ShaderAssign.Options.GetKey(j);
			mat.ShaderAssign.ShaderOptions.Add(item);
			mat.ShaderAssign.ShaderOptionDict.Add(key);
		}
	}

	public static void Save(ResFileSaver saver, Material mat)
	{
		int num = 0;
		foreach (RenderInfo renderInfo in mat.RenderInfos)
		{
			renderInfo.DataOffset = num;
			num = renderInfo.Type switch
			{
				RenderInfoType.String => num + 8 * renderInfo.GetValueStrings().Length, 
				RenderInfoType.Single => num + 4 * renderInfo.GetValueSingles().Length, 
				_ => num + 4 * renderInfo.GetValueInt32s().Length, 
			};
		}
		int num2 = 128;
		num += (-num % num2 + num2) % num2;
		num = mat.RenderInfoSize;
		saver.SaveRelocateEntryToSection(saver.Position, 12u, 1u, 0u, 1, "FMAT");
		saver.SaveString(mat.Name);
		saver.Save(mat.ShaderInfoV10, -1, 0L);
		saver.SaveCustom(new long[mat.TextureRefs.Count], () =>
		{
			saver.Write(new long[mat.TextureRefs.Count]);
		}, 0L);
		saver.SaveCustom(mat.TextureRefs, () =>
		{
			saver.SaveStrings(mat.TextureRefs, relocate: true);
		}, 0L);
		saver.SaveCustom(new long[mat.Samplers.Count], () =>
		{
			saver.Write(new long[mat.Samplers.Count * 15]);
		}, 0L);
		saver.SaveList(mat.Samplers, 0L);
		saver.SaveDict(mat.SamplerDict, 0L);
		saver.SaveCustom(mat.RenderInfos, () =>
		{
			long position = saver.Position;
			saver.Write(new byte[mat.RenderInfoSize]);
			saver.Seek(position, SeekOrigin.Begin);
			List<RenderInfo> list = mat.RenderInfos.Where((RenderInfo x) => x.Type == RenderInfoType.String).ToList();
			if (list.Count > 0)
			{
				int offsetCount = list.Sum((RenderInfo x) => x.GetValueStrings().Length);
				saver.SaveRelocateEntryToSection(saver.Position, (uint)offsetCount, 1u, 0u, 1, "Render Info Strings V10");
			}
			long position2 = saver.Position;
			foreach (RenderInfo renderInfo2 in mat.RenderInfos)
			{
				renderInfo2.DataOffset = saver.Position - position2;
				switch (renderInfo2.Type)
				{
				case RenderInfoType.String:
					renderInfo2.SaveStrings(saver);
					break;
				case RenderInfoType.Single:
					renderInfo2.SaveFloats(saver);
					break;
				default:
					renderInfo2.SaveInts(saver);
					break;
				}
			}
			saver.Seek(position + mat.RenderInfoSize, SeekOrigin.Begin);
		}, 0L);
		saver.SaveCustom(new byte[mat.RenderInfoSize], () =>
		{
			foreach (RenderInfo renderInfo3 in mat.RenderInfos)
			{
				switch (renderInfo3.Type)
				{
				case RenderInfoType.String:
					saver.Write((ushort)(renderInfo3.GetValueStrings()?.Length).Value);
					break;
				case RenderInfoType.Single:
					saver.Write((ushort)(renderInfo3.GetValueSingles()?.Length).Value);
					break;
				default:
					saver.Write((ushort)(renderInfo3.GetValueInt32s()?.Length).Value);
					break;
				}
			}
		}, 0L);
		saver.SaveCustom(new uint[mat.RenderInfos.Count], () =>
		{
			foreach (RenderInfo renderInfo4 in mat.RenderInfos)
			{
				saver.Write((ushort)renderInfo4.DataOffset);
			}
		}, 0L);
		saver.SaveCustom(mat.ShaderParamData, () =>
		{
			saver.Write(mat.ShaderParamData);
			saver.Align(128);
		}, 0L);
		saver.SaveCustom(mat.ParamIndices, () =>
		{
			saver.Write(mat.ParamIndices);
		}, 0L);
		saver.Write(0uL);
		saver.SaveRelocateEntryToSection(saver.Position, 3u, 1u, 0u, 1, "FMAT User Data");
		mat.PosUserDataMaterialOffset = saver.SaveOffset();
		mat.PosUserDataDictMaterialOffset = saver.SaveOffset();
		saver.SaveCustom(new byte[32], () =>
		{
			saver.Write(new byte[32]);
		}, 0L);
		saver.Write(0uL);
		saver.SaveRelocateEntryToSection(saver.Position, 2u, 1u, 0u, 1, "Material texture slots");
		saver.SaveCustom(mat.SamplerSlotArray, () =>
		{
			saver.Write(mat.SamplerSlotArray);
		}, 0L);
		saver.SaveCustom(mat.TextureSlotArray, () =>
		{
			saver.Write(mat.TextureSlotArray);
		}, 0L);
		saver.Write((ushort)saver.CurrentIndex);
		saver.Write((byte)mat.TextureRefs.Count);
		saver.Write((byte)mat.Samplers.Count);
		saver.Write((ushort)0);
		saver.Write((ushort)mat.UserDatas.Count);
		saver.Write((ushort)num);
		saver.Write((ushort)0);
		saver.Write((ushort)0);
		saver.Write((ushort)0);
	}
}
