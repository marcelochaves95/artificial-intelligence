using UnityEngine;

namespace NodeGraph
{
    public class MathFunctionLibrary : FunctionLibrary
    {
        public static float Add(float value1, float value2)
        {
            return value1 + value2;
        }

        public static float Subtract(float value1, float value2)
        {
            return value1 - value2;
        }

        public static float Multiply(float value1, float value2)
        {
            return value1 * value2;
        }

        public static float Divide(float value1, float value2)
        {
            return value1 / value2;
        }

        public static float Sin(float value)
        {
            return Mathf.Sin(value);
        }

        public static float Cos(float value)
        {
            return Mathf.Cos(value);
        }
    }
}