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
        private static extern int GetPrivateProfileString(string s, string k, string def, StringBuilder v, int size, string f);

        private readonly string _path;

        public INI(string path) { _path = path; }

        public string this[string section, string key]
        {
            get
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(section, key, "", temp, 255, _path);
                return temp.ToString();
            }
            set { WritePrivateProfileString(section, key, value, _path); }
        }
    }
}
