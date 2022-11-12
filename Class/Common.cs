using System.Globalization;
using System;

namespace WhaleExtension.Class
{
    public static class Common
    {
        public static string ToEnglishNumber(string input)
        {
            if (input == null) return input;

            string[] arabic = new string[10] { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int j = 0; j < persian.Length; j++)
                input = input.Replace(persian[j], j.ToString()).Replace(arabic[j], j.ToString());

            return input;
        }
    }
}