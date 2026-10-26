using System;

namespace Syroot.NintenTools.NSW.Bfres;

[Flags]
public enum BoneFlagsTransformCumulative : uint
{
	None = 0u,
	ScaleUniform = 0x10000000u,
	ScaleVolumeOne = 0x20000000u,
	RotateZero = 0x40000000u,
	TranslateZero = 0x80000000u,
	ScaleOne = ScaleUniform | ScaleVolumeOne,
	RotateTranslateZero = RotateZero | TranslateZero,
	Identity = ScaleOne | RotateTranslateZero
}
