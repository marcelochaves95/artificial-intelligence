using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Derive from this when writing functions you wish to include in the GraphEditor */
public class FunctionLibrary
{
}

public class MathFunctionLibrary : FunctionLibrary
{

	public static float Add(float A, float B)
	{
		return A + B;
	}

	public static float Subtract(float A, float B)
	{
		return A - B;
	}

	public static float Multiply(float A, float B)
	{
		return A * B;
	}

	public static float Divide(float A, float B)
	{
		return A / B;
	}

	public static float Sin(float Input)
	{
		return Mathf.Sin(Input);
	}

	public static float Cos(float Input)
	{
		return Mathf.Cos(Input);
	}

}
