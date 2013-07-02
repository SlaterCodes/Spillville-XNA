using System;
using System.Collections.Generic;
using System.Linq;

namespace SpillvilleDataTypes
{
	/// <author>Chris Gonzales</author>
	/// <summary>
	/// This class is a list/search based implementation of the Dictionary class.
	/// It should be more Xbox friendly as it takes up less memory and performance
	/// is dirrectly proportion to its size (O(n)).
	/// 
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class XBoxFriendlyDictionary<TKey, TValue>
	{
		private List<Pair<TKey, TValue>> _data;

		public XBoxFriendlyDictionary()
		{
			_data = new List<Pair<TKey, TValue>>();
		}

		public TValue this[TKey key]
		{
			get
			{
				if (Equals(key, default(TKey)))
				{
					throw new ArgumentNullException(@"key");
				}
				foreach (var pair in _data.Where(pair => pair.Key.Equals(key)))
				{
					return pair.Value;
				}
				throw new ArgumentOutOfRangeException(@"key");
			}

			set
			{
				if (Equals(key, default(TKey)))
				{
					throw new ArgumentNullException(@"key");
				}
				foreach (var pair in _data.Where(pair => pair.Key.Equals(key)))
				{
					pair.Value = value;
					return;
				}
				throw new ArgumentOutOfRangeException(@"key");
			}
		}

        public TKey GetKey(int index)
        {
            return _data.ElementAt(index).Key;
        }

        public int Count
        {
            get 
            {
                return _data.Count;
            }
        }

        

		public void Add(TKey key, TValue value)
		{
			if (Equals(key, default(TKey)))
			{
				throw new ArgumentNullException(@"key");
			}
			if (ContainsKey(key))
			{
				throw new ArgumentException(@"An element with the same key already exists.");
			}
			var pair = new Pair<TKey, TValue>(key, value);
			_data.Add(pair);
		}

		public void Clear()
		{
			_data.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			if (Equals(key, default(TKey)))
			{
				throw new ArgumentNullException(@"key");
			}
			return _data.Any(pair => pair.Key.Equals(key));
		}

		public bool ContainsValue(TValue value)
		{
			return _data.Any(pair => pair.Value.Equals(value));
		}

		public void Remove(TKey key)
		{
			if (Equals(key, default(TKey)))
			{
				throw new ArgumentNullException(@"key");
			}
			var removeArray = _data.Where(pair => pair.Key.Equals(key)).ToList();

			foreach (var pair in removeArray)
			{
				_data.Remove(pair);
			}
		}


		private class Pair<TPairKey, TPairValue>
		{
			public TPairKey Key;
			public TPairValue Value;

			public Pair(TPairKey key, TPairValue value)
			{
				Key = key;
				Value = value;
			}

		}

	}

}
