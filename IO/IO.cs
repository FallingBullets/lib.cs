using System;

namespace fbstj.IO
{
	public delegate void Consumer<T>(T f);
	public delegate T Converter<F, T>(F o);

	/// <summary>A transportation interface, sends and recieves T's</summary>
	public interface Transport<T>
	{
		/// <summary>Distributes recieved T's</summary>
		event Consumer<T> Receive;

		/// <summary>Send a T</summary>
		void Send(T t);
		/// <summary>Send a T and then wait for a reply</summary>
		T SendRecieve(T t);
	}
}
