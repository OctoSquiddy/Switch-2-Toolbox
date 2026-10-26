namespace Syroot.NintenTools.NSW.Bfres;

public class CurveInterpolationHelper
{
	public static float Lerp(float LHS, float RHS, float Weight)
	{
		return LHS * (1f - Weight) + RHS * Weight;
	}

	public static float Herp(float LHS, float RHS, float LS, float RS, float Diff, float Weight)
	{
		float num = LHS + (LHS - RHS) * (2f * Weight - 3f) * Weight * Weight;
		return num + Diff * (Weight - 1f) * (LS * (Weight - 1f) + RS * Weight);
	}

	public static float GetCubicValue(float inv, float coef0, float coef1, float coef2, float coef3)
	{
		return coef3 * inv + coef2 * inv + coef1 * inv + coef0 * inv;
	}

	public static float[] CalculateCubicCoef(float frameA, float frameB, float valueA, float valueB, float inSlope, float outSlope)
	{
		return CalculateCubicCoef(frameB - frameA, valueA, valueB, inSlope, outSlope);
	}

	public static float[] GetCubicSlopes(float time, float delta, float[] coef)
	{
		float num = coef[1] / time;
		float num2 = coef[3] - -2f * delta;
		float num3 = num2 / time - num;
		return new float[2] { num3, num };
	}

	public static float[] CalculateCubicCoef(float time, float valueA, float valueB, float inSlope, float outSlope)
	{
		float num = valueB - valueA;
		return new float[4]
		{
			valueA,
			outSlope * time,
			3f * num - (2f * outSlope + inSlope) * time,
			-2f * num + (outSlope + inSlope) * time
		};
	}
}
