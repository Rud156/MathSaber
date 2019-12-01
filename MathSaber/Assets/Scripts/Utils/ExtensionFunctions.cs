using UnityEngine;

namespace Utils
{
    public static class ExtensionFunctions
    {
        public static int GetClosestMultiple(float number, int? multiple = 5)
        {
            int a = (int) (number / 5) * 5;
            int b;

            if (number < 0)
                b = a - 5;
            else
                b = a + 5;

            return (Mathf.Abs(number - a) > Mathf.Abs(b - number)) ? b : a;
        }

        public static string Format2DecimalPlace(float value) => value.ToString("0.##");

        public static Color ConvertAndClampColor(float r = 0, float g = 0, float b = 0, float a = 0) =>
            new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b),
                Mathf.Clamp(a, 0, 255) / 255);

        public static float To360Angle(float angle)
        {
            while (angle < 0.0f)
                angle += 360.0f;
            while (angle >= 360.0f)
                angle -= 360.0f;

            return angle;
        }

        public static float Map(float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static bool IsZero(Vector3 vector) => vector.sqrMagnitude == 0;
    }
}