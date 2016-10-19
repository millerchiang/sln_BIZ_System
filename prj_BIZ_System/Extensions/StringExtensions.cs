using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace prj_BIZ_System.Extensions
{
    public static class StringExtensions
    {
        public delegate string StringContent(string contentPath);

        public static bool IsNullOrEmpty(this string s)
        {
            return (String.IsNullOrEmpty(s)) ? true : false;
        }

        public static bool isValidLetter(this string s)
        {
            return Regex.IsMatch(s, @"^[a-zA-Z]+$");
        }
    }
}