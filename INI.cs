using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace fbstj
{
    public struct INI
    {
        #region section
        public struct Section
        {
            /// <summary>The storage location</summary>
            public readonly string Path;

            /// <summary>The section heading</summary>
            public readonly string Name;

            internal Section(INI parent, string section) { Path = parent.Path; Name = section; }

            /// <summary>The value of a key under this INI section</summary>
            public string this[string key]
            {
                get { return Get(Path,Name,key); }
                set { Set(Path, Name, key, value); }
            }

            public override string ToString() { return "[" + Name + "]"; }
        }
        #endregion

        #region static
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
        #endregion

        #region state
        /// <summary>The storage location</summary>
        public readonly string Path;

        public INI(string path) { Path = path; }

        /// <summary>The value of a key under an INI section</summary>
        public string this[string section, string key]
        {
            get { return Get(Path, section, key); }
            set { Set(Path, section, key, value); }
        }

        /// <summary>An INI section</summary>
        public Section this[string section] { get { return new Section(this, section); } }
        #endregion
    }
}
