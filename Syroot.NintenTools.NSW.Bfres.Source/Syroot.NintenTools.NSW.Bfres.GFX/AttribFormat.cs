namespace Syroot.NintenTools.NSW.Bfres.GFX;

/// <summary>
/// Represents the format of a vertex attribute entry. Possible type conversions:
/// <para />UNorm: attrib unsigned integer is converted to/from [0.0, 1.0] in shader.
/// <para />UInt: attrib unsigned integer is copied to/from shader as unsigned int.
/// <para />SNorm: attrib signed integer is converted to/from [-1.0, 1.0] in shader.
/// <para />SInt: attrib signed integer is copied to/from shader as signed int.
/// <para />Single: attrib single is copied to/from shader as Single.
/// <para />UIntToSingle: attrib unsigned integer is converted Single in shader.
/// <para />SIntToSingle: attrib signed integer is converted Single in shader.
/// </summary>
public enum AttribFormat : ushort
{
	Format_8_UNorm = 258,
	Format_8_UInt = 770,
	Format_8_SNorm = 514,
	Format_8_SInt = 1026,
	Format_8_UIntToSingle = 2050,
	Format_8_SIntToSingle = 2562,
	Format_4_4_UNorm = 1,
	Format_16_UNorm = 266,
	Format_16_UInt = 522,
	Format_16_SNorm = 778,
	Format_16_SInt = 1034,
	Format_16_Single = 1290,
	Format_16_UIntToSingle = 2051,
	Format_16_SIntToSingle = 2563,
	Format_8_8_UNorm = 265,
	Format_8_8_UInt = 777,
	Format_8_8_SNorm = 521,
	Format_8_8_SInt = 1033,
	Format_8_8_UIntToSingle = 2052,
	Format_8_8_SIntToSingle = 2564,
	Format_16_16_UNorm = 274,
	Format_16_16_SNorm = 530,
	Format_16_16_UInt = 786,
	Format_16_16_SInt = 1042,
	Format_16_16_Single = 1298,
	Format_16_16_UIntToSingle = 2055,
	Format_16_16_SIntToSingle = 2567,
	Format_10_11_11_Single = 2057,
	Format_8_8_8_8_UNorm = 267,
	Format_8_8_8_8_SNorm = 523,
	Format_8_8_8_8_UInt = 779,
	Format_8_8_8_8_SInt = 1035,
	Format_8_8_8_8_UIntToSingle = 2059,
	Format_8_8_8_8_SIntToSingle = 2571,
	Format_10_10_10_2_UNorm = 11,
	Format_10_10_10_2_UInt = 2315,
	Format_10_10_10_2_SNorm = 526,
	Format_10_10_10_2_SInt = 2459,
	Format_16_16_16_16_UNorm = 277,
	Format_16_16_16_16_SNorm = 533,
	Format_16_16_16_16_UInt = 789,
	Format_16_16_16_16_SInt = 1045,
	Format_16_16_16_16_Single = 1301,
	Format_16_16_16_16_UIntToSingle = 2062,
	Format_16_16_16_16_SIntToSingle = 2574,
	Format_32_UInt = 788,
	Format_32_SInt = 1046,
	Format_32_Single = 1302,
	Format_32_32_UInt = 791,
	Format_32_32_SInt = 1047,
	Format_32_32_Single = 1303,
	Format_32_32_32_UInt = 792,
	Format_32_32_32_SInt = 1048,
	Format_32_32_32_Single = 1304,
	Format_32_32_32_32_UInt = 793,
	Format_32_32_32_32_SInt = 1049,
	Format_32_32_32_32_Single = 1305
}
