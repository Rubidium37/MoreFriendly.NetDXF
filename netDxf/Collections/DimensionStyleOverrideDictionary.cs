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
using netDxf.Tables;

namespace netDxf.Collections
{
	/// <summary>Represents a dictionary of <see cref="DimensionStyleOverride">DimensionStyleOverrides</see>.</summary>
	public sealed class DimensionStyleOverrideDictionary :
		IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>
	{
		private readonly Dictionary<DimensionStyleOverrideType, DimensionStyleOverride> innerDictionary;

		#region delegates and events

		/// <summary>Generated when an <see cref="DimensionStyleOverride"/> item is about to be added; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<DimensionStyleOverride> BeforeAddingItem;
		/// <summary>Generates the <see cref="BeforeAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be added; otherwise, <see langword="false"/>.</returns>
		private bool OnBeforeAddingItem(DimensionStyleOverride item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeAddingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<DimensionStyleOverride>(propertyName, ItemChangeAction.Add, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="DimensionStyleOverride"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<DimensionStyleOverride> AfterAddingItem;
		/// <summary>Generates the <see cref="AfterAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		private void OnAfterAddingItem(DimensionStyleOverride item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="DimensionStyleOverride"/> item is about to be removed; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<DimensionStyleOverride> BeforeRemovingItem;
		/// <summary>Generates the <see cref="BeforeRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be removed; otherwise, <see langword="false"/>.</returns>
		private bool OnBeforeRemovingItem(DimensionStyleOverride item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeRemovingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<DimensionStyleOverride>(propertyName, ItemChangeAction.Remove, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="DimensionStyleOverride"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<DimensionStyleOverride> AfterRemovingItem;
		/// <summary>Generates the <see cref="AfterRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		private void OnAfterRemovingItem(DimensionStyleOverride item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
		}

		#endregion

		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		public DimensionStyleOverrideDictionary()
		{
			this.innerDictionary = new Dictionary<DimensionStyleOverrideType, DimensionStyleOverride>();
		}
		/// <summary>Initializes a new instance of the class and has the specified initial capacity.</summary>
		/// <param name="capacity">The number of items the collection can initially store.</param>
		public DimensionStyleOverrideDictionary(int capacity)
		{
			this.innerDictionary = new Dictionary<DimensionStyleOverrideType, DimensionStyleOverride>(capacity);
		}

		#endregion

		#region public properties

		/// <inheritdoc/>
		public DimensionStyleOverride this[DimensionStyleOverrideType type]
		{
			get => this.innerDictionary[type];
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (type != value.Type)
				{
					throw new ArgumentException(string.Format("The dictionary type: {0}, and the DimensionStyleOverride type: {1}, must be the same", type, value.Type));
				}

				DimensionStyleOverride remove = this.innerDictionary[type];
				if (!this.OnBeforeRemovingItem(remove))
				{
					return;
				}

				if (!this.OnBeforeAddingItem(value))
				{
					return;
				}

				this.innerDictionary[type] = value;
				this.OnAfterAddingItem(value);
				this.OnAfterRemovingItem(remove);
			}
		}

		/// <summary>Gets a collection containing the types of the current dictionary.</summary>
		public ICollection<DimensionStyleOverrideType> Types => this.innerDictionary.Keys;

		/// <inheritdoc/>
		public ICollection<DimensionStyleOverride> Values => this.innerDictionary.Values;

		/// <inheritdoc/>
		public int Count => this.innerDictionary.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		#endregion

		#region public methods

		/// <summary>Adds a <see cref="DimensionStyleOverride"/> to the dictionary from its type and value.</summary>
		/// <param name="type">Dimension style override type.</param>
		/// <param name="value">Dimension style override value.</param>
		/// <remarks>A new <see cref="DimensionStyleOverride"/> will be created from the specified arguments.</remarks>
		public void Add(DimensionStyleOverrideType type, object value) => this.Add(new DimensionStyleOverride(type, value));

		/// <summary>Adds an <see cref="DimensionStyleOverride"/> to the dictionary.</summary>
		/// <param name="item">The <see cref="DimensionStyleOverride"/> to add.</param>
		public void Add(DimensionStyleOverride item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (!this.OnBeforeAddingItem(item, "Item"))
			{
				throw new ArgumentException(string.Format("The DimensionStyleOverride {0} cannot be added to the collection.", item), nameof(item));
			}

			this.innerDictionary.Add(item.Type, item);
			this.OnAfterAddingItem(item, "Item");
		}

		/// <summary>Adds an <see cref="DimensionStyleOverride"/> list to the dictionary.</summary>
		/// <param name="collection">The collection whose elements should be added.</param>
		public void AddRange(IEnumerable<DimensionStyleOverride> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			// we will make room for so the collection will fit without having to resize the internal array during the Add method
			foreach (DimensionStyleOverride item in collection)
			{
				this.Add(item);
			}
		}

		/// <inheritdoc/>
		public bool Remove(DimensionStyleOverrideType type)
		{
			if (!this.innerDictionary.TryGetValue(type, out DimensionStyleOverride remove))
			{
				return false;
			}

			if (!this.OnBeforeRemovingItem(remove, "Item"))
			{
				return false;
			}

			this.innerDictionary.Remove(type);
			this.OnAfterRemovingItem(remove, "Item");
			return true;
		}

		/// <inheritdoc/>
		public void Clear()
		{
			DimensionStyleOverrideType[] types = new DimensionStyleOverrideType[this.innerDictionary.Count];
			this.innerDictionary.Keys.CopyTo(types, 0);
			foreach (DimensionStyleOverrideType tag in types)
			{
				this.Remove(tag);
			}
		}

		/// <summary>Determines whether current dictionary contains an <see cref="DimensionStyleOverride"/> of the specified type.</summary>
		/// <param name="type">The type to locate in the current dictionary.</param>
		/// <returns><see langword="true"/> if the current dictionary contains an <see cref="DimensionStyleOverride"/> of the type; otherwise, <see langword="false"/>.</returns>
		public bool ContainsType(DimensionStyleOverrideType type) => this.innerDictionary.ContainsKey(type);

		/// <summary>Determines whether current dictionary contains a specified <see cref="DimensionStyleOverride"/>.</summary>
		/// <param name="value">The <see cref="DimensionStyleOverride"/> to locate in the current dictionary.</param>
		/// <returns><see langword="true"/> if the current dictionary contains the <see cref="DimensionStyleOverride"/>; otherwise, <see langword="false"/>.</returns>
		public bool ContainsValue(DimensionStyleOverride value) => this.innerDictionary.ContainsValue(value);

		/// <inheritdoc/>
		public bool TryGetValue(DimensionStyleOverrideType type, out DimensionStyleOverride value)
			=> this.innerDictionary.TryGetValue(type, out value);

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>> GetEnumerator()
			=> this.innerDictionary.GetEnumerator();

		#endregion

		#region private properties

		ICollection<DimensionStyleOverrideType> IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.Keys
			=> this.innerDictionary.Keys;

		#endregion

		#region private methods

		bool IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.ContainsKey(DimensionStyleOverrideType tag)
			=> this.innerDictionary.ContainsKey(tag);

		void IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.Add(DimensionStyleOverrideType key, DimensionStyleOverride value)
			=> this.Add(value);

		void ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Add(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
			=> this.Add(item.Value);

		bool ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Remove(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
		{
			if (!ReferenceEquals(item.Value, this.innerDictionary[item.Key]))
			{
				return false;
			}

			return this.Remove(item.Key);
		}

		bool ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Contains(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
			=> ((IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>)this.innerDictionary).Contains(item);

		void ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.CopyTo(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>[] array, int arrayIndex)
			=> ((IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>)this.innerDictionary).CopyTo(array, arrayIndex);

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		#endregion
	}
}