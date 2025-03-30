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
using System.Runtime.CompilerServices;

namespace netDxf.Collections
{
	public sealed class ObservableDictionary<TKey, TValue> :
		IDictionary<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> innerDictionary;

		#region delegates and events

		/// <summary>Generated when an <see cref="KeyValuePair{TKey, TValue}"/> item is about to be added; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<KeyValuePair<TKey, TValue>> BeforeAddingItem;
		/// <summary>Generates the <see cref="BeforeAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be added; otherwise, <see langword="false"/>.</returns>
		private bool OnBeforeAddingItem(KeyValuePair<TKey, TValue> item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeAddingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<KeyValuePair<TKey, TValue>>(propertyName, ItemChangeAction.Add, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="KeyValuePair{TKey, TValue}"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<KeyValuePair<TKey, TValue>> AfterAddingItem;
		/// <summary>Generates the <see cref="AfterAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		private void OnAfterAddingItem(KeyValuePair<TKey, TValue> item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="KeyValuePair{TKey, TValue}"/> item is about to be removed; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<KeyValuePair<TKey, TValue>> BeforeRemovingItem;
		/// <summary>Generates the <see cref="BeforeRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be removed; otherwise, <see langword="false"/>.</returns>
		private bool OnBeforeRemovingItem(KeyValuePair<TKey, TValue> item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeRemovingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<KeyValuePair<TKey, TValue>>(propertyName, ItemChangeAction.Remove, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="KeyValuePair{TKey, TValue}"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<KeyValuePair<TKey, TValue>> AfterRemovingItem;
		/// <summary>Generates the <see cref="AfterRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		private void OnAfterRemovingItem(KeyValuePair<TKey, TValue> item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
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

				if (!this.OnBeforeRemovingItem(remove))
				{
					return;
				}

				if (!this.OnBeforeAddingItem(add))
				{
					return;
				}

				this.innerDictionary[key] = value;
				this.OnAfterAddingItem(add);
				this.OnAfterRemovingItem(remove);
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
			if (!this.OnBeforeAddingItem(add, "Item"))
			{
				throw new ArgumentException("The item cannot be added to the dictionary.", nameof(value));
			}
			this.innerDictionary.Add(key, value);
			this.OnAfterAddingItem(add, "Item");
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
			if (!this.OnBeforeRemovingItem(remove, "Item"))
			{
				return false;
			}

			this.innerDictionary.Remove(key);
			this.OnAfterRemovingItem(remove, "Item");

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