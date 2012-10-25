using System;
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
			// order so minimum is at start
		}
		internal Cycle(IEnumerable<uint> elements) : this(elements.ToArray()) { }
		#endregion

		#region IPermutable<uint> and ToString
		/// <summary>Map an element to its new value</summary>
		public uint Permute(uint e)
		{
			int i = _.IndexOf(e);
			if (i > -1)
				return _[(i + 1) % Length];
			return e;
		}

		public ISet<uint> Orbit { get; private set; }

		public IPermutable<uint> Inverse() { return _inverse(); }

		/// <summary>Get a set of transpositions that make this cycle</summary>
		public IList<IPermutable<uint>> Transpositions()
		{
			if (Identity)
				return null;
			var cy = Ordered();
			var trs = new List<IPermutable<uint>>();
			for (int i = Length - 1; i > 0; i--)
				trs.Add(new Cycle(cy._[0], cy._[i]));
			return trs;
		}

		public bool Equals(IPermutable<uint> cy) { return Permutation.Equal(this, cy); }

		public override string ToString() { return "(" + string.Join(" ", Ordered()._) + ")"; }
		#endregion

		/// <summary>The number of elements in the orbit</summary>
		public int Length { get { return _.Count; } }
		/// <summary>This cycle permutes every element to itsself</summary>
		public bool Identity { get { return Orbit.Count == 1; } }
		/// <summary>This cycle permutes 2 elements between each other</summary>
		public bool Transposition { get { return Orbit.Count == 2; } }

		internal Cycle _inverse() { return new Cycle(_.Reverse()).Ordered(); }

		/// <summary>Map every element of an array</summary>
		internal void PermuteMap(ref uint[] map)
		{
			for (int i = 0; i < map.Length; i++)
				map[i] = Permute(map[i]);
		}

		/// <summary>Reorder this cycle so the smallest element is first</summary>
		public Cycle Ordered()
		{
			var els = new uint[Length];
			els[0] = _.Min();
			for (int i = 1; i < Length; i++)
				els[i] = Permute(els[i - 1]);
			return new Cycle(els);
		}
	}

	public struct Permutation : IPermutable<uint>, IEquatable<IEnumerable<IPermutable<uint>>>
	{
		#region static
		public static readonly IPermutable<uint> Identity = new Cycle();

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
				case 1:
					return true;
				case 2:
					return _equal(perms[0], perms[1]);
				case 0:
					return false;
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
			var re = new Regex(@"\([0-9 ]+?\)");
			var _cys = new List<IPermutable<uint>>();

			foreach (Match m in re.Matches(cycles))
				_cys.Add(Cycle.Parse(m.Value));

			return new Permutation(_cys);
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

		public Permutation(IPermutable<uint> perm)
			: this(perm.Orbit.Max() + 1)
		{
			for (uint i = 0; i < _map.Length; i++)
				_map[i] = perm.Permute(i);
		}

		/// <summary>Merge a number of permutations</summary>
		public Permutation(IEnumerable<IPermutable<uint>> perms)
			: this(perms.Max(perm => perm.Orbit.Max()) + 1)
		{
			var cycles = new List<Cycle>();
			foreach (var p in perms.Reverse())
				cycles.AddRange(new Permutation(p).Cycles());
			foreach (var cy in cycles)
				cy.PermuteMap(ref _map);
		}

		/// <summary>Merge the passed permutaitons together</summary>
		public Permutation(params IPermutable<uint>[] perms) : this((IEnumerable<IPermutable<uint>>)perms) { }
		#endregion

		#region IPermutable<uint> IEquatable<IEnmerable<IPermutable<uint>>> and ToString
		public uint Permute(uint e)
		{
			if (e > _map.Length)
				return e;
			return _map[e];
		}

		public ISet<uint> Orbit { get { return new HashSet<uint>(_map); } }

		public IPermutable<uint> Inverse()
		{
			var p = new Permutation((uint)_map.Length);
			Cycles().All(cy => { cy._inverse().PermuteMap(ref p._map); return true; });
			return p;
		}

		public IList<IPermutable<uint>> Transpositions()
		{
			var cys = new List<IPermutable<uint>>();
			Cycles().All(cy => { cys.AddRange(cy.Transpositions()); return true; });
			return cys;
		}

		public bool Equals(IPermutable<uint> perm) { return Permutation.Equal(this, perm); }
		public bool Equals(IEnumerable<IPermutable<uint>> perms) { return Equals(Parse(string.Join("", perms))); }

		public override string ToString() { return string.Join("", Cycles()); }
		#endregion

		/// <summary>Shrink the orbit so the largest element is permuted</summary>
		public void Trim()
		{
			// shrink map down to minimal length
			int j = _map.Length - 1;
			do
			{
				if (_map[j] != j)
					break;
				j--;
			} while (j > 0);
			Array.Resize(ref _map, j + 1);
		}

		/// <summary>Convert a permutation into cycle notation</summary>
		public ISet<Cycle> Cycles()
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
	}
}
