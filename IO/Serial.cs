using System.IO.Ports;

namespace fbstj.IO
{
	public class Serial : Transport<byte>
	{
		private SerialPort _port;

		public Serial(ref SerialPort port)
		{
			_port = port;
			_port.DataReceived += delegate(object o, SerialDataReceivedEventArgs e)
			{
				Receive.Invoke((byte)_port.ReadByte());
			};
		}

		public event Consumer<byte> Receive;

		public void Send(byte t)
		{
			byte[] b = new byte[1] { t };
			_port.Write(b, 0, 1);
		}

		public byte SendRecieve(byte t)
		{
			Send(t);
			return (byte)_port.ReadByte();
		}
	}
}
