using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Peak.Can.Basic;

namespace fbstj.IO.CAN
{
	/// <summary>
	/// PCAN Port implementation
	/// requires PCANBasic.cs and PCANBasic.dll
	/// </summary>
	public class PCAN : Port
	{
		public enum Ports
		{
			USB1 = PCANBasic.PCAN_USBBUS1,
			USB2 = PCANBasic.PCAN_USBBUS2,
			USB3 = PCANBasic.PCAN_USBBUS3,
			USB4 = PCANBasic.PCAN_USBBUS4,
			USB5 = PCANBasic.PCAN_USBBUS5,
			USB6 = PCANBasic.PCAN_USBBUS6,
			USB7 = PCANBasic.PCAN_USBBUS7,
			USB8 = PCANBasic.PCAN_USBBUS8
		}

		#region PCAN type casting
		/// <summary>Converts a TPCANMsg into a CAN.Frame</summary>
		public static Frame Frame(TPCANMsg msg)
		{
			var f = new Frame();
			f.ID = (int)msg.ID;
			f.Extended = (msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) == TPCANMessageType.PCAN_MESSAGE_EXTENDED;
			f.Remote = (msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) == TPCANMessageType.PCAN_MESSAGE_RTR;
			f.Length = msg.LEN;
			if (msg.DATA != null)
				for (int i = 0; i < f.Length; i++)
					f[i] = msg.DATA[i];
			return f;
		}
		/// <summary>Converts a Frame into a TPCANMsg</summary>
		public static TPCANMsg Frame(Frame f)
		{
			var m = new TPCANMsg();
			m.ID = (uint)f.ID;
			m.DATA = new byte[8];	// needs to be the full 8 bytes
			for (int i = 0; i < f.Length; i++)
				m.DATA[i] = f[i];
			m.LEN = f.Length;
			m.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
			m.MSGTYPE |= f.Extended ? TPCANMessageType.PCAN_MESSAGE_EXTENDED : 0;
			m.MSGTYPE |= f.Remote ? TPCANMessageType.PCAN_MESSAGE_RTR : 0;
			return m;
		}
		/// <summary>Converts a PCANTimestamp into a TimeSpan</summary>
		public static TimeSpan Time(TPCANTimestamp ts)
		{
			long us = ts.micros + 1000 * ts.millis + (long)0xFFFFFFFF * 1000 * ts.millis_overflow;
			return new TimeSpan(us * TimeSpan.TicksPerMillisecond / 1000);
		}
		#endregion

		#region Port enumeration
		/// <summary>Enmerate all ports, and their USB Device Number</summary>
		public static List<Ports> All()
		{
			var all = new List<Ports>();
			uint fff = 0;
			TPCANStatus status;
			foreach (Ports d in Enum.GetValues(typeof(Ports)))
			{
				PCANBasic.Initialize((byte)d, TPCANBaudrate.PCAN_BAUD_1M);
				status = PCANBasic.GetValue((byte)d, TPCANParameter.PCAN_DEVICE_NUMBER, out fff, 1);
				if (status != TPCANStatus.PCAN_ERROR_INITIALIZE)
					all.Add(d);
				PCANBasic.Uninitialize((byte)d);
			}
			return all;
		}
		#endregion

		public static explicit operator Ports(PCAN _) { return _.iface; }

		private Ports iface;
		private Thread rx;
		private TPCANBaudrate baud = TPCANBaudrate.PCAN_BAUD_1M;

		#region Bitrate
		public long Bitrate
		{
			get
			{
				switch (baud)
				{
					case TPCANBaudrate.PCAN_BAUD_100K:
						return 100000;
					case TPCANBaudrate.PCAN_BAUD_10K:
						return 10000;
					case TPCANBaudrate.PCAN_BAUD_125K:
						return 125000;
					case TPCANBaudrate.PCAN_BAUD_1M:
						return 1000000;
					case TPCANBaudrate.PCAN_BAUD_20K:
						return 20000;
					case TPCANBaudrate.PCAN_BAUD_250K:
						return 250000;
					case TPCANBaudrate.PCAN_BAUD_33K:
						return 33000;
					case TPCANBaudrate.PCAN_BAUD_47K:
						return 47000;
					case TPCANBaudrate.PCAN_BAUD_500K:
						return 500000;
					case TPCANBaudrate.PCAN_BAUD_50K:
						return 50000;
					case TPCANBaudrate.PCAN_BAUD_5K:
						return 5000;
					case TPCANBaudrate.PCAN_BAUD_800K:
						return 800000;
					case TPCANBaudrate.PCAN_BAUD_83K:
						return 83000;
					case TPCANBaudrate.PCAN_BAUD_95K:
						return 95000;
				}
				return 0;
			}
			set
			{
				if (value <= 5000)
					baud = TPCANBaudrate.PCAN_BAUD_5K;
				else if (value <= 10000)
					baud = TPCANBaudrate.PCAN_BAUD_10K;
				else if (value <= 20000)
					baud = TPCANBaudrate.PCAN_BAUD_20K;
				else if (value <= 33000)
					baud = TPCANBaudrate.PCAN_BAUD_33K;
				else if (value <= 47000)
					baud = TPCANBaudrate.PCAN_BAUD_47K;
				else if (value <= 50000)
					baud = TPCANBaudrate.PCAN_BAUD_50K;
				else if (value <= 83000)
					baud = TPCANBaudrate.PCAN_BAUD_83K;
				else if (value <= 95000)
					baud = TPCANBaudrate.PCAN_BAUD_95K;
				else if (value <= 100000)
					baud = TPCANBaudrate.PCAN_BAUD_100K;
				else if (value <= 125000)
					baud = TPCANBaudrate.PCAN_BAUD_125K;
				else if (value <= 250000)
					baud = TPCANBaudrate.PCAN_BAUD_250K;
				else if (value <= 500000)
					baud = TPCANBaudrate.PCAN_BAUD_500K;
				else if (value <= 800000)
					baud = TPCANBaudrate.PCAN_BAUD_800K;
				else
					baud = TPCANBaudrate.PCAN_BAUD_1M;
			}
		}
		#endregion

		public PCAN(Ports device, long baudrate)
		{
			iface = device;
			Bitrate = baudrate;
			PCANBasic.Initialize((byte)iface, baud);
			rx = new Thread(_read);
			rx.IsBackground = true;
			rx.Start();
		}
		~PCAN() { PCANBasic.Uninitialize((byte)iface); }

		public event Action<Frame> Receive = (f) => { };
		public event Action<TimeSpan, Frame> TimedReceive = (t, f) => { };

		/// <summary>Send a frame</summary>
		public void Send(Frame f)
		{
			TPCANMsg msg = Frame(f);
			PCANBasic.Write((byte)iface, ref msg);
		}

		private void _read()
		{	// threaded recieve listener
			AutoResetEvent m_ReceiveEvent = new AutoResetEvent(false);
			UInt32 iBuffer;
			TPCANStatus stsResult;

			iBuffer = Convert.ToUInt32(m_ReceiveEvent.SafeWaitHandle.DangerousGetHandle().ToInt32());
			// Sets the handle of the Receive-Event.
			stsResult = PCANBasic.SetValue((byte)iface, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));

			if (stsResult != TPCANStatus.PCAN_ERROR_OK)
				throw new Exception(GetFormatedError(stsResult));

			while (true)
			{
				if (m_ReceiveEvent.WaitOne(50))
				{	// wait or sleep
					TPCANMsg m; TPCANTimestamp ts;
					do
					{	// read all messages from queue, and 'receive' them
						stsResult = PCANBasic.Read((byte)iface, out m, out ts);

						if (stsResult == TPCANStatus.PCAN_ERROR_OK)
						{
							Receive.Invoke(Frame(m));
							TimedReceive.Invoke(Time(ts), Frame(m));
						}
					} while ((!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY)));
				}
			}
		}

		private string GetFormatedError(TPCANStatus error)
		{	/// Help Function used to get an error as text
			StringBuilder strTemp;

			// Creates a buffer big enough for a error-text
			strTemp = new StringBuilder(256);
			// Gets the text using the GetErrorText API function
			// If the function success, the translated error is returned. If it fails,
			// a text describing the current error is returned.
			if (PCANBasic.GetErrorText(error, 0, strTemp) != TPCANStatus.PCAN_ERROR_OK)
				return string.Format("An error occurred. Error-code's text ({0:X}) couldn't be retrieved", error);
			else
				return strTemp.ToString();
		}
	}

	public static class PCAN_View
	{
		/// <summary>Read(ID, Time, Frame, Rx|!Tx)</summary>
		public static event Action<uint, TimeSpan, Frame, bool> Read = (i, time, frame, rx) => { };

		/// <summary>
		/// Implements a parsing method for PCAN-View log files
		/// </summary>
		public static long ParsePCAN(this StreamReader file)
		{
			string line; long i = 0;
			while ((line = file.ReadLine()) != null)
			{
				try { ParseLine(line); i++; }
				catch { }
			}
			return i;
		}

		static readonly Regex record = new Regex(@"(\d+).\s+([\d.]+)\s+([RT]x)\s+([0-9A-Fa-f]+)\s+(\d)");
		static readonly Regex data = new Regex(@" ([0-9a-fA-F]{2})");
		private static void ParseLine(string line)
		{
			Match m = record.Match(line);
			if (!m.Success)
				throw new ArgumentException("Line incorrectly formatted");
			var id = uint.Parse(m.Groups[1].Value);
			var time = TimeSpan.FromMilliseconds(double.Parse(m.Groups[2].Value));
			var rx = m.Groups[3].Value == "Rx";
			var frame = default(Frame);
			frame.ID = int.Parse(m.Groups[4].Value, NumberStyles.HexNumber);
			frame.Length = byte.Parse(m.Groups[5].Value);
			var ms = data.Matches(line);
			var offset = ms.Count - frame.Length;
			if (offset < 0)
				throw new FormatException("not enough data segments for length");
			for (int i = 0; i < frame.Length; i++)
				frame[i] = byte.Parse(ms[offset + i].Groups[1].Value, NumberStyles.HexNumber);
			Read.Invoke(id, time, frame, rx);
		}
	}
}