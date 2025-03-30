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
using netDxf.Entities;

namespace netDxf.Collections
{
	/// <summary>Represents a dictionary of <see cref="AttributeDefinition">AttributeDefinitions</see>.</summary>
	public sealed class AttributeDefinitionDictionary :
		IDictionary<string, AttributeDefinition>
	{
		private readonly Dictionary<string, AttributeDefinition> innerDictionary;

		#region delegates and events

		/// <summary>Generated when an <see cref="AttributeDefinition"/> item is about to be added; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<AttributeDefinition> BeforeAddingItem;
		/// <summary>Generates the <see cref="BeforeAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be added; otherwise, <see langword="false"/>.</returns>
		private bool OnBeforeAddingItem(AttributeDefinition item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeAddingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<AttributeDefinition>(propertyName, ItemChangeAction.Add, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="AttributeDefinition"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<AttributeDefinition> AfterAddingItem;
		/// <summary>Generates the <see cref="AfterAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		private void OnAfterAddingItem(AttributeDefinition item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="AttributeDefinition"/> item is about to be removed; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<AttributeDefinition> BeforeRemovingItem;
		/// <summary>Generates the <see cref="BeforeRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be removed; otherwise, <see langword="false"/>.</returns>
		private bool OnBeforeRemovingItem(AttributeDefinition item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeRemovingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<AttributeDefinition>(propertyName, ItemChangeAction.Remove, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="AttributeDefinition"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<AttributeDefinition> AfterRemovingItem;
		/// <summary>Generates the <see cref="AfterRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		private void OnAfterRemovingItem(AttributeDefinition item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
		}

		#endregion

		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		public AttributeDefinitionDictionary()
		{
			this.innerDictionary = new Dictionary<string, AttributeDefinition>(StringComparer.OrdinalIgnoreCase);
		}
		/// <summary>Initializes a new instance of the class and has the specified initial capacity.</summary>
		/// <param name="capacity">The number of items the collection can initially store.</param>
		public AttributeDefinitionDictionary(int capacity)
		{
			this.innerDictionary = new Dictionary<string, AttributeDefinition>(capacity, StringComparer.OrdinalIgnoreCase);
		}

		#endregion

		#region public properties

		/// <inheritdoc/>
		public AttributeDefinition this[string tag]
		{
			get => this.innerDictionary[tag];
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (!string.Equals(tag, value.Tag, StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException(string.Format("The dictionary tag: {0}, and the attribute definition tag: {1}, must be the same", tag, value.Tag));
				}

				// there is no need to add the same object, it might cause overflow issues
				if (ReferenceEquals(this.innerDictionary[tag], value))
				{
					return;
				}

				AttributeDefinition remove = this.innerDictionary[tag];
				if (!this.OnBeforeRemovingItem(remove))
				{
					return;
				}

				if (!this.OnBeforeAddingItem(value))
				{
					return;
				}
				this.innerDictionary[tag] = value;
				this.OnAfterAddingItem(value);
				this.OnAfterRemovingItem(remove);
			}
		}

		/// <summary>Gets a collection containing the tags of the current dictionary.</summary>
		public ICollection<string> Tags => this.innerDictionary.Keys;

		/// <inheritdoc/>
		public ICollection<AttributeDefinition> Values => this.innerDictionary.Values;

		/// <inheritdoc/>
		public int Count => this.innerDictionary.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		#endregion

		#region public methods

		/// <summary>Adds an <see cref="AttributeDefinition">attribute definition</see> to the dictionary.</summary>
		/// <param name="item">The <see cref="AttributeDefinition">attribute definition</see> to add.</param>
		public void Add(AttributeDefinition item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (!this.OnBeforeAddingItem(item, "Item"))
			{
				throw new ArgumentException("The attribute definition cannot be added to the collection.", nameof(item));
			}

			this.innerDictionary.Add(item.Tag, item);
			this.OnAfterAddingItem(item, "Item");
		}

		/// <summary>Adds an <see cref="AttributeDefinition">attribute definition</see> list to the dictionary.</summary>
		/// <param name="collection">The collection whose elements should be added.</param>
		public void AddRange(IEnumerable<AttributeDefinition> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			// we will make room for so the collection will fit without having to resize the internal array during the Add method
			foreach (AttributeDefinition item in collection)
			{
				this.Add(item);
			}
		}

		/// <inheritdoc/>
		public bool Remove(string tag)
		{
			if (!this.innerDictionary.TryGetValue(tag, out AttributeDefinition remove))
			{
				return false;
			}

			if (!this.OnBeforeRemovingItem(remove, "Item"))
			{
				return false;
			}

			this.innerDictionary.Remove(tag);
			this.OnAfterRemovingItem(remove, "Item");
			return true;
		}

		/// <inheritdoc/>
		public void Clear()
		{
			string[] tags = new string[this.innerDictionary.Count];
			this.innerDictionary.Keys.CopyTo(tags, 0);
			foreach (string tag in tags)
			{
				this.Remove(tag);
			}
		}

		/// <summary>Determines whether current dictionary contains an <see cref="AttributeDefinition">attribute definition</see> with the specified tag.</summary>
		/// <param name="tag">The tag to locate in the current dictionary.</param>
		/// <returns><see langword="true"/> if the current dictionary contains an <see cref="AttributeDefinition">attribute definition</see> with the tag; otherwise, <see langword="false"/>.</returns>
		public bool ContainsTag(string tag) => this.innerDictionary.ContainsKey(tag);

		/// <summary>Determines whether current dictionary contains a specified <see cref="AttributeDefinition">attribute definition</see>.</summary>
		/// <param name="value">The <see cref="AttributeDefinition">attribute definition</see> to locate in the current dictionary.</param>
		/// <returns><see langword="true"/> if the current dictionary contains the <see cref="AttributeDefinition">attribute definition</see>; otherwise, <see langword="false"/>.</returns>
		public bool ContainsValue(AttributeDefinition value) => this.innerDictionary.ContainsValue(value);

		/// <inheritdoc/>
		public bool TryGetValue(string tag, out AttributeDefinition value) => this.innerDictionary.TryGetValue(tag, out value);

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<string, AttributeDefinition>> GetEnumerator() => this.innerDictionary.GetEnumerator();

		#endregion

		#region private properties

		ICollection<string> IDictionary<string, AttributeDefinition>.Keys => this.innerDictionary.Keys;

		#endregion

		#region private methods

		bool IDictionary<string, AttributeDefinition>.ContainsKey(string tag) => this.innerDictionary.ContainsKey(tag);

		void IDictionary<string, AttributeDefinition>.Add(string key, AttributeDefinition value) => this.Add(value);

		void ICollection<KeyValuePair<string, AttributeDefinition>>.Add(KeyValuePair<string, AttributeDefinition> item)
			=> this.Add(item.Value);

		bool ICollection<KeyValuePair<string, AttributeDefinition>>.Remove(KeyValuePair<string, AttributeDefinition> item)
		{
			if (!ReferenceEquals(item.Value, this.innerDictionary[item.Key]))
			{
				return false;
			}
			return this.Remove(item.Key);
		}

		bool ICollection<KeyValuePair<string, AttributeDefinition>>.Contains(KeyValuePair<string, AttributeDefinition> item)
			=> ((IDictionary<string, AttributeDefinition>)this.innerDictionary).Contains(item);

		void ICollection<KeyValuePair<string, AttributeDefinition>>.CopyTo(KeyValuePair<string, AttributeDefinition>[] array, int arrayIndex)
			=> ((IDictionary<string, AttributeDefinition>)this.innerDictionary).CopyTo(array, arrayIndex);

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		#endregion
	}
}