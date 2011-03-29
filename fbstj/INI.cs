using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace fbstj
{
    /// <summary>An INI file</summary>
    public struct INI
    {
        #region static
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string s, string k, string v, string f);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string s, string k, string def, StringBuilder v, int size, string f);

        private static string Get(params string[] psk)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(psk[1], psk[1], "", temp, 255, psk[0]);
            return temp.ToString();
        }

        private static void Set(params string[] pskv)
        {
            WritePrivateProfileString(pskv[1], pskv[2], pskv[3], pskv[0]);
        }

        /// <summary>A named section of an INI file</summary>
        public struct Section
        {
            /// <summary>The path to the INI file</summary>
            public readonly string File;

            /// <summary>The section heading</summary>
            public readonly string Name;

            internal Section(INI ini, string section) { Name = section; File = ini.File; }

            /// <summary></summary>
            public string this[string key] { get { return Get(File, Name, key); } set { Set(File, Name, key, value); } }
        }
        #endregion

        #region state
        /// <summary>The path to the INI file</summary>
        public readonly string File;

        public INI(string path) { File = path; }

        /// <summary>The value of a key under an INI section.</summary>
        internal string this[string section, string key] { get { return Get(File, section, key); } set { Set(File, section, key, value); } }

        /// <summary>An INI section</summary>
        public Section this[string section] { get { return new Section(this, section); } }
        #endregion
    }
}
