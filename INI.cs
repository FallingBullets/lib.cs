using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace fbstj
{
    public struct INI
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string s, string k, string v, string f);

        [DllImport("kernel32")]
        private static extern int ReadPrivateProfileString(string s, string k, string d, StringBuilder v, int size, string f);

        private static string Get(string path, string section, string key)
        {
            StringBuilder tmp = new StringBuilder(255);
            int i = ReadPrivateProfileString(section, key, "", tmp, 255, path);
            return tmp.ToString();
        }

        private static void Set(string path, string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, path);
        }

    }
}
