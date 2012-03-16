using System;

namespace fbstj.IO
{
	/// <summary>A transportation interface, sends and recieves T's</summary>
	public interface Transport<T>
	{
		/// <summary>Distributes recieved T's</summary>
		event Action<T> Receive;

		/// <summary>Send a T</summary>
		void Send(T t);
		/// <summary>Send a T and then wait for a reply</summary>
		T SendRecieve(T t);
	}

	/// <summary>A struct for attaching two disparate Transportats together</summary>
	public struct Pipe<Ta, Tb>
	{
		public readonly Transport<Ta> A;
		public readonly Transport<Tb> B;

		public readonly Converter<Ta, Tb> A2B;
		public readonly Converter<Tb, Ta> B2A;

		public Pipe(Transport<Ta> a, Transport<Tb> b, Converter<Ta, Tb> a2b, Converter<Tb, Ta> b2a)
		{
			A = a; B = b; A2B = a2b; B2A = b2a; var _ = this;
			A.Receive += delegate(Ta f) { _.B.Send(_.A2B(f)); };
			B.Receive += delegate(Tb f) { _.A.Send(_.B2A(f)); };
		}
	}

	/// <summary>A struct for attaching two similar Transportats together symetrically</summary>
	public struct Pipe<T>
	{
		public readonly Transport<T> A;
		public readonly Transport<T> B;

		public Pipe(Transport<T> a, Transport<T> b)
		{
			A = a; B = b; var _ = this;
			A.Receive += delegate(T t) { _.B.Send(t); };
			B.Receive += delegate(T t) { _.A.Send(t); };
		}
	}
}
