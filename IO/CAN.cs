using System;

namespace fbstj.IO.CAN
{
	/// <summary>A timestamped receiver</summary>
	public delegate void TimedReciever(TimeSpan at, Frame f);

	/// <summary>A CAN port interface</summary>
	public interface Port : IO.Transport<Frame>
	{
		/// <summary>Distributes a recieved frame along with a TimeSpan timestamp</summary>
		event TimedReciever TimedReceive;
	}

	/// <summary>A CAN frame</summary>
	public struct Frame
	{
		/// <summary>The frame identifier</summary>
		public int ID;
		/// <summary>The frame has a 29-bit address</summary>
		public bool Extended;
		/// <summary>The frame is a Remote-Transfer-Request</summary>
		public bool Remote;
		/// <summary>The number of bytes in </summary>
		public byte Length
		{
			get { return (byte)_data.Length; }
			set { Array.Resize<byte>(ref _data, value); }
		}
		/// <summary></summary>
		public byte this[int i] { get { return _data[i]; } set { _data[i] = value; } }
		private byte[] _data;

		public override string ToString()
		{
			string[] data = new string[Length];
			for (int i = 0; i < Length; i++)
				data[i] = this[i].ToString("X2");
			return "ID=" + ID.ToString("X") + ",Data=" + string.Join(" ", data);
		}
	}
}
