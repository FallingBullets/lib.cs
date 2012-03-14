using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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
				get { return _get(Path, Name, key); }
				set { _set(Path, Name, key, value); }
			}

			/// <summary>All the keys in this INI section</summary>
			public string[] Keys { get { return _keys(Path, Name); } }

			public override string ToString() { return "[" + Name + "]"; }
		}
		#endregion

		#region static
		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string s, string k, string v, string f);

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string s, string k, string d, StringBuilder v, int size, string f);
		[DllImport("kernel32")]
		static extern int GetPrivateProfileString(int Section, string Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);
		[DllImport("kernel32")]
		static extern int GetPrivateProfileString(string Section, int Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);

		/// <summary>Get a value of a section and key pair</summary>
		private static string _get(string path, string section, string key)
		{
			StringBuilder tmp = new StringBuilder(255);
			int i = GetPrivateProfileString(section, key, "", tmp, 255, path);
			return tmp.ToString();
		}

		/// <summary>Set the value of a section and key pair</summary>
		private static void _set(string path, string section, string key, string value)
		{
			WritePrivateProfileString(section, key, value, path);
		}

		/// <summary>Get the sections in a file</summary>
		private static string[] _sections(string path)
		{
			//	Sets the maxsize buffer to 500, if the more is required then doubles the size each time.
			for (int maxsize = 500; true; maxsize *= 2)
			{
				//	Obtains the information in bytes and stores them in the maxsize buffer (Bytes array)
				byte[] bytes = new byte[maxsize];
				int size = GetPrivateProfileString(0, "", "", bytes, maxsize, path);

				// Check the information obtained is not bigger than the allocated maxsize buffer - 2 bytes.
				// if it is, then skip over the next section so that the maxsize buffer can be doubled.
				if (size < maxsize - 2)
				{
					// Converts the bytes value into an ASCII char. This is one long string.
					string Selected = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
					// Splits the Long string into an array based on the "\0" or null (Newline) value and returns the value(s) in an array
					return Selected.Split(new char[] { '\0' });
				}
			}
		}

		/// <summary>Get the keys in a section</summary>
		private static string[] _keys(string path, string section)
		{
			//	Sets the maxsize buffer to 500, if the more is required then doubles the size each time. 
			for (int maxsize = 500; true; maxsize *= 2)
			{
				//	Obtains the EntryKey information in bytes and stores them in the maxsize buffer (Bytes array).
				//	Note that the SectionHeader value has been passed.
				byte[] bytes = new byte[maxsize];
				int size = GetPrivateProfileString(section, 0, "", bytes, maxsize, path);

				// Check the information obtained is not bigger than the allocated maxsize buffer - 2 bytes.
				// if it is, then skip over the next section so that the maxsize buffer can be doubled.
				if (size < maxsize - 2)
				{
					// Converts the bytes value into an ASCII char. This is one long string.
					string entries = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
					// Splits the Long string into an array based on the "\0" or null (Newline) value and returns the value(s) in an array
					return entries.Split(new char[] { '\0' });
				}
			}
		}
		#endregion

		#region state
		/// <summary>The storage location</summary>
		public readonly string Path;

		public INI(string path) { Path = path; }

		/// <summary>The value of a key under an INI section</summary>
		public string this[string section, string key]
		{
			get { return _get(Path, section, key); }
			set { _set(Path, section, key, value); }
		}

		/// <summary>All the INI section names</summary>
		public Section[] Sections
		{
			get
			{
				var o = new List<Section>();
				foreach (string s in _sections(Path))
					o.Add(this[s]);
				return o.ToArray();
			}
		}

		/// <summary>An INI section</summary>
		public Section this[string section] { get { return new Section(this, section); } }
		#endregion
	}
}
