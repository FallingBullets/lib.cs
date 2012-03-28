using System;
using System.Collections.Generic;
using Algebra.Extensions;

namespace Algebra
{
	internal struct Ringoid<T> where T : IEquatable<T>
	{
		public readonly ISet<T> Elements;

		public readonly Magma<T> Addition;
		public readonly Magma<T> Multiplication;

		public Func<T, T, T> Add { get { return Addition.Operation; } }
		public Func<T, T, T> Multiply { get { return Multiplication.Operation; } }

		public Ringoid(ISet<T> el, Func<T, T, T> add, Func<T, T, T> multiply)
		{
			Elements = el;
			Addition = new Magma<T>(el, add);
			Multiplication = new Magma<T>(el, multiply);
		}

		/// <summary>Multiplication is distributive over addition</summary>
		public bool Distributive
		{
			get
			{
				foreach (T a in Elements)
					foreach (T b in Elements)
						foreach (T c in Elements)
							if (!Multiply.Distributes(Add, a, b, c))
								return false;
				return true;
			}
		}

		public T Unit { get { return Multiplication.Identity; } }
		public T Zero { get { return Multiplication.Absorber; } }

		public bool Shell { get { return Zero.Equals(Addition.Identity) && Unit.Equals(Unit); } }
	}
}
