using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string source)
        {
            return source == null || source.Length == 0;
        }

        public static bool IsNotEmpty(this string source)
        {
            return source != null && source.Length > 0;
        }

        public static bool IsSomething(this string source)
        {
            return source.IsNotEmpty();
        }
    }
}
