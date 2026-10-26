using System;

namespace Syroot.NintenTools.NSW.Bfres;

[Flags]
public enum BoneFlagsTransform : uint
{
	None = 0u,
	ScaleUniform = 0x1000000u,
	ScaleVolumeOne = 0x2000000u,
	RotateZero = 0x4000000u,
	TranslateZero = 0x8000000u,
	ScaleOne = ScaleUniform | ScaleVolumeOne,
	RotateTranslateZero = RotateZero | TranslateZero,
	Identity = ScaleOne | RotateTranslateZero
}
