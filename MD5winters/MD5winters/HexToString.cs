using System;
using System.Collections.Generic;


namespace tgenaux.MD5Winters
{
    public static class HexToString
    {
        public static string ToHexString(this byte[] bytes)
        {
            List<string> parts = new List<string>();
            foreach (byte x in bytes)
            {
                parts.Add(String.Format("{0:X2}", x));
            }

            return string.Join("", parts); ;
        }
        public static string ToHexDashed(this byte[] bytes)
        {
            List<string> parts = new List<string>();
            foreach (byte x in bytes)
            {
                parts.Add(String.Format("{0:X2}", x));
            }

            return string.Join("-", parts); ;
        }

        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }


    }
}
