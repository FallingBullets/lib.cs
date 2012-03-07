using System;

namespace fbstj.Serial
{
	public delegate void Receiver<T>(T f);
	public delegate T Converter<F,T>(F o);

	public interface Transport<T>
	{
		event Receiver<T> Receive;

		void Send(T t);
	}
}
