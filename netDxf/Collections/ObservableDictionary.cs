#region netDxf library licensed under the MIT License
//
//                       netDxf library
// Copyright (c) Daniel Carvajal (haplokuon@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace netDxf.Collections
{
	public sealed class ObservableDictionary<TKey, TValue> :
		IDictionary<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> innerDictionary;

		#region delegates and events

		public delegate void AddItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

		public delegate void BeforeAddItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

		public delegate void RemoveItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

		public delegate void BeforeRemoveItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

		public event BeforeAddItemEventHandler BeforeAddItem;
		public event AddItemEventHandler AddItem;
		public event BeforeRemoveItemEventHandler BeforeRemoveItem;
		public event RemoveItemEventHandler RemoveItem;

		private bool BeforeAddItemEvent(KeyValuePair<TKey, TValue> item)
		{
			BeforeAddItemEventHandler ae = this.BeforeAddItem;
			if (ae != null)
			{
				ObservableDictionaryEventArgs<TKey, TValue> e = new ObservableDictionaryEventArgs<TKey, TValue>(item);
				ae(this, e);
				return e.Cancel;
			}
			return false;
		}

		private void AddItemEvent(KeyValuePair<TKey, TValue> item)
		{
			AddItemEventHandler ae = this.AddItem;
			if (ae != null)
				ae(this, new ObservableDictionaryEventArgs<TKey, TValue>(item));
		}

		private bool BeforeRemoveItemEvent(KeyValuePair<TKey, TValue> item)
		{
			BeforeRemoveItemEventHandler ae = this.BeforeRemoveItem;
			if (ae != null)
			{
				ObservableDictionaryEventArgs<TKey, TValue> e = new ObservableDictionaryEventArgs<TKey, TValue>(item);
				ae(this, e);
				return e.Cancel;
			}
			return false;
		}

		private void RemoveItemEvent(KeyValuePair<TKey, TValue> item)
		{
			RemoveItemEventHandler ae = this.RemoveItem;
			if (ae != null)
				ae(this, new ObservableDictionaryEventArgs<TKey, TValue>(item));
		}

		#endregion

		#region constructor

		public ObservableDictionary()
		{
			this.innerDictionary = new Dictionary<TKey, TValue>();
		}
		public ObservableDictionary(int capacity)
		{
			this.innerDictionary = new Dictionary<TKey, TValue>(capacity);
		}
		public ObservableDictionary(IEqualityComparer<TKey> comparer)
		{
			this.innerDictionary = new Dictionary<TKey, TValue>(comparer);
		}
		public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			this.innerDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
		}

		#endregion

		#region public properties

		/// <inheritdoc/>
		public TValue this[TKey key]
		{
			get => this.innerDictionary[key];
			set
			{
				KeyValuePair<TKey, TValue> remove = new KeyValuePair<TKey, TValue>(key, this.innerDictionary[key]);
				KeyValuePair<TKey, TValue> add = new KeyValuePair<TKey, TValue>(key, value);

				if (this.BeforeRemoveItemEvent(remove))
				{
					return;
				}

				if (this.BeforeAddItemEvent(add))
				{
					return;
				}

				this.innerDictionary[key] = value;
				this.AddItemEvent(add);
				this.RemoveItemEvent(remove);
			}
		}

		/// <inheritdoc/>
		public ICollection<TKey> Keys => this.innerDictionary.Keys;

		/// <inheritdoc/>
		public ICollection<TValue> Values => this.innerDictionary.Values;

		/// <inheritdoc/>
		public int Count => this.innerDictionary.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		#endregion

		#region public methods

		/// <inheritdoc/>
		public void Add(TKey key, TValue value)
		{
			KeyValuePair<TKey, TValue> add = new KeyValuePair<TKey, TValue>(key, value);
			if (this.BeforeAddItemEvent(add))
			{
				throw new ArgumentException("The item cannot be added to the dictionary.", nameof(value));
			}
			this.innerDictionary.Add(key, value);
			this.AddItemEvent(add);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

		/// <inheritdoc/>
		public bool Remove(TKey key)
		{
			if (!this.innerDictionary.ContainsKey(key))
			{
				return false;
			}

			KeyValuePair<TKey, TValue> remove = new KeyValuePair<TKey, TValue>(key, this.innerDictionary[key]);
			if (this.BeforeRemoveItemEvent(remove))
			{
				return false;
			}

			this.innerDictionary.Remove(key);
			this.RemoveItemEvent(remove);

			return true;
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			if (!ReferenceEquals(item.Value, this.innerDictionary[item.Key]))
			{
				return false;
			}

			return this.Remove(item.Key);
		}

		/// <inheritdoc/>
		public void Clear()
		{
			TKey[] keys = new TKey[this.innerDictionary.Count];
			this.innerDictionary.Keys.CopyTo(keys, 0);
			foreach (TKey key in keys)
			{
				this.Remove(key);
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
			=> ((IDictionary<TKey, TValue>)this.innerDictionary).Contains(item);

		/// <inheritdoc/>
		public bool ContainsKey(TKey key) => this.innerDictionary.ContainsKey(key);

		public bool ContainsValue(TValue value) => this.innerDictionary.ContainsValue(value);

		/// <inheritdoc/>
		public bool TryGetValue(TKey key, out TValue value) => this.innerDictionary.TryGetValue(key, out value);

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			=> ((IDictionary<TKey, TValue>)this.innerDictionary).CopyTo(array, arrayIndex);

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.innerDictionary.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		#endregion
	}
}