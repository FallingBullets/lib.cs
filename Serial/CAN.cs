using System;
using System.Collections.Generic;

namespace fbstj.Serial.CAN
{
	/// <summary>A timestamped receiver</summary>
	public delegate void TimedReciever(TimeSpan at, Frame f);

	/// <summary>A CAN port interface</summary>
	public interface Port : Serial.Transport<Frame>
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
			return "ID="+ID.ToString("X")+",Data=";
		}
	}
}
