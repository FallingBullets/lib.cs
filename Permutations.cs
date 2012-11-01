using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Algebra.Permutations
{
	public interface IPermutable<T> : IEquatable<IPermutable<T>>
	{
		/// <summary>Map an element to its new value</summary>
		T Permute(T e);
		/// <summary>The elements this permutation acts upon</summary>
		ISet<T> Orbit { get; }
		/// <summary>Return the permutation such that this*this.Inverse() == 1</summary>
		IPermutable<T> Inverse();
		/// <summary>Returns a list of transpositions which make up this permutation</summary>
		IList<IPermutable<T>> Transpositions();
	}

	public static class PermutationExtensions
	{
		public static bool IsIdentity<T>(this IPermutable<T> _) { return _.Orbit.Count == 1; }
		public static bool IsTransposition<T>(this IPermutable<T> _) { return _.Orbit.Count == 2; }

		public static bool IsCycle<T>(this IPermutable<T> _)
		{
			var o = new HashSet<T>(_.Orbit);
			if (o.Count == 0)
				return false;
			T e0 = o.Min(), e1 = e0; int i = 0;
			do { e1 = _.Permute(e1); i++; } while (!e1.Equals(e0));
			return o.Count == i;
		}
	}

	#region IPermutable<uint>
	public struct Cycle : IPermutable<uint>
	{
		#region static
		/// <summary>Parses cycle-notation</summary>
		public static Cycle Parse(string s)
		{
			if (!s.StartsWith("(") || !s.EndsWith(")") || s.Contains(")("))
				throw new ArgumentException("Not a single cycle");
			var x = s.Substring(1, s.Length - 2).Split(' ');
			var y = new uint[x.Length];
			for (int i = 0; i < x.Length; i++)
			{
				uint e;
				if (!uint.TryParse(x[i], out e))
					throw new ArgumentException("string incorrectly formatted");
				y[i] = e;
			}
			return new Cycle(y);
		}

		#endregion

		#region state and constructors
		private readonly IList<uint> _;

		/// <summary>Create a cycle using the sequenc of elements passed</summary>
		public Cycle(params uint[] elements)
			: this()
		{
			_ = new List<uint>(elements);
			Orbit = new HashSet<uint>(_);
			if (!Orbit.SetEquals(_))
				throw new ArgumentException("not a cycle: parameters not distinct from each other");
		}
		internal Cycle(IEnumerable<uint> elements) : this(elements.ToArray()) { }
		#endregion

		#region IPermutable<uint>
		/// <summary>Map an element to its new value</summary>
		public uint Permute(uint e)
		{
			int i = _.IndexOf(e);
			if (i > -1)
				return _[(i + 1) % Orbit.Count];
			return e;
		}

		public ISet<uint> Orbit { get; private set; }

		public IPermutable<uint> Inverse() { return _inverse(); }

		/// <summary>Get a set of transpositions that make this cycle</summary>
		public IList<IPermutable<uint>> Transpositions()
		{
			if (Orbit.Count < 2)
				return null;
			var cy = _ordered();
			var trs = new List<IPermutable<uint>>();
			for (int i = Orbit.Count - 1; i > 0; i--)
				trs.Add(new Cycle(cy._[0], cy._[i]));
			return trs;
		}

		public bool Equals(IPermutable<uint> cy) { return Permutation.Equal(this, cy); }
		#endregion

		#region ToString
		public override string ToString() { return "(" + string.Join(" ", _ordered()._) + ")"; }
		#endregion

		internal Cycle _inverse() { return new Cycle(_.Reverse())._ordered(); }

		/// <summary>Reorder this cycle so the smallest element is first</summary>
		internal Cycle _ordered()
		{
			var els = new uint[Orbit.Count];
			els[0] = _.Min();
			for (int i = 1; i < Orbit.Count; i++)
				els[i] = Permute(els[i - 1]);
			return new Cycle(els);
		}
	}

	public struct Permutation : IPermutable<uint>, IEquatable<IEnumerable<IPermutable<uint>>>, IEnumerable
	{
		#region static
		public static readonly IPermutable<uint> Identity = new Permutation(1);

		private static bool _equal(IPermutable<uint> p1, IPermutable<uint> p2)
		{
			var orbits = p1.Orbit.Union(p2.Orbit);
			foreach (uint e in orbits)
				if (p1.Permute(e) != p2.Permute(e))
					return false;
			return true;
		}
		/// <summary>Check n permutations are equal</summary>
		public static bool Equal(params IPermutable<uint>[] perms)
		{
			switch (perms.Length)
			{
				case 0:
				case 1:
					return true;
				case 2:
					return _equal(perms[0], perms[1]);
				default:
					for (int i = 1; i < perms.Length; i++)
						if (!_equal(perms[0], perms[i]))
							return false;
					return true;
			}
		}

		/// <summary>Parses a string of the form "(1 2 3)(4 5 6)" (cycle notation) </summary>
		public static Permutation Parse(string cycles)
		{
			var re = new Regex(@"\([0-9 ]+?\)", RegexOptions.RightToLeft);
			var perm = default(Permutation);

			foreach (Match m in re.Matches(cycles))
				perm.Add(Cycle.Parse(m.Value));

			return perm;
		}
		#endregion

		#region state and constructors
		private uint[] _map;

		/// <summary>Identity permutation of given size</summary>
		public Permutation(uint size)
		{
			_map = new uint[size];
			for (uint i = 0; i < size; i++)
				_map[i] = i;
		}
		#endregion

		#region IPermutable<uint>
		public uint Permute(uint e)
		{
			if (e < _map.Length)
				return _map[e];
			return e;
		}

		public ISet<uint> Orbit { get { _init(); _resize(); return _order(); } }

		public IPermutable<uint> Inverse()
		{
			var p = new Permutation((uint)_map.Length);
			_cycles().All(cy => { p.Add(cy.Inverse()); return true; });
			return p;
		}

		public IList<IPermutable<uint>> Transpositions()
		{
			var cys = new List<IPermutable<uint>>();
			_cycles().All(cy => { cys.AddRange(cy.Transpositions()); return true; });
			return cys;
		}

		public bool Equals(IPermutable<uint> perm) { return Permutation.Equal(this, perm); }
		#endregion

		#region IEquatable<IEnumerable<IPermutable<uint>>>
		public bool Equals(IEnumerable<IPermutable<uint>> perms) { return Equals(Parse(string.Join("", perms))); }
		#endregion

		#region IEnumerable + Add
		public IEnumerator GetEnumerator() { throw new NotImplementedException(); }

		public void Add(IPermutable<uint> perm)
		{
			if (perm.Orbit.Count == 0)
				return;
			_resize(perm.Orbit.Max() + 1);
			for (int i = 0; i < _map.Length; i++)
				_map[i] = perm.Permute(_map[i]);
		}

		public void Add(string cy) { Add(Cycle.Parse(cy)); }
		#endregion

		#region ToString
		public override string ToString() { return string.Join("", _cycles()); }
		#endregion

		private void _init() { if (_map == null) _map = new uint[0]; }

		/// <summary>Shrink the orbit so the largest element is permuted</summary>
		internal void _resize()
		{
			_init();
			var all = _map.Where((v, i) => i != v);
			if (all.Count() > 0)
				Array.Resize(ref _map, (int)(all.Max() + 1));
		}

		/// <summary>Increase the size of the orbit with identity permutations</summary>
		internal void _resize(uint length)
		{
			_init();
			int l = _map.Length;
			if (l < length)
				Array.Resize(ref _map, (int)length);
			for (int i = l; i < length; i++)
				_map[i] = (uint)i;
		}

		/// <summary>Convert a permutation into cycle notation</summary>
		internal ISet<Cycle> _cycles()
		{
			var cys = new List<Cycle>();
			var cyl = new List<uint>();
			var orbit = new HashSet<uint>();
			foreach (var e0 in _map)
			{
				orbit.Add(e0);
				var e1 = _map[e0];
				if (orbit.Contains(e1))
					continue;
				cyl.Add(e0);
				do
				{
					cyl.Add(e1);
					orbit.Add(e1);
					e1 = _map[e1];
				} while (e1 != e0);
				cys.Add(new Cycle(cyl.ToArray()));
				cyl.Clear();
			}
			return new HashSet<Cycle>(cys);
		}

		internal ISet<uint> _order()
		{
			var els = new HashSet<uint>();
			for (int i = 0; i < _map.Length; i++)
				if (_map[i] != i)
					els.Add(_map[i]);
			return els;
		}
	}
	#endregion
}
