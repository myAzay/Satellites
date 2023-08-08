using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Extensions
{
    public static class StringExtension
    {
        public static string AddTxtExtension(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "fileName.txt";
            var substringLength = name.Length < 5 ? 0 : name.Length - 4;
            string txtExtension = name.Substring(substringLength);
            if (txtExtension != ".txt")
                name += ".txt";
            return name;
        }
    }
}
