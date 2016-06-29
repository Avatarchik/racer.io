using UnityEngine;

namespace Area730.Notifications
{
    public class ColorUtils
    {

        private static string GetHex(int num)
        {
            string alpha = "0123456789ABCDEF";
            string res = "" + alpha[num];
            return res;
        }

        public static string ToHtmlStringRGB(Color color)
        {
            float red = color.r * 255;
            float green = color.g * 255;
            float blue = color.b * 255;

            string a = GetHex((int)Mathf.Floor((int)red / 16));
            string b = GetHex((int)Mathf.Round((int)red % 16));
            string c = GetHex((int)Mathf.Floor((int)green / 16));
            string d = GetHex((int)Mathf.Round((int)green % 16));
            string e = GetHex((int)Mathf.Floor((int)blue / 16));
            string f = GetHex((int)Mathf.Round((int)blue % 16));

            string z = a + b + c + d + e + f;

            return z;
        }

    }
}
