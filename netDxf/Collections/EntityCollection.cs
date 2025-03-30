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
	/// <summary>Represent a collection of <see cref="EntityObject">entities</see> that fire events when it is modified.</summary>
	public class EntityCollection :
		IList<EntityObject>
	{
		private readonly List<EntityObject> innerArray;

		#region delegates and events

		/// <summary>Generated when an <see cref="EntityObject"/> item is about to be added; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<EntityObject> BeforeAddingItem;
		/// <summary>Generates the <see cref="BeforeAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be added; otherwise, <see langword="false"/>.</returns>
		protected virtual bool OnBeforeAddingItem(EntityObject item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeAddingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<EntityObject>(propertyName, ItemChangeAction.Add, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="EntityObject"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<EntityObject> AfterAddingItem;
		/// <summary>Generates the <see cref="AfterAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterAddingItem(EntityObject item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="EntityObject"/> item is about to be removed; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<EntityObject> BeforeRemovingItem;
		/// <summary>Generates the <see cref="BeforeRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be removed; otherwise, <see langword="false"/>.</returns>
		protected virtual bool OnBeforeRemovingItem(EntityObject item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeRemovingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<EntityObject>(propertyName, ItemChangeAction.Remove, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="EntityObject"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<EntityObject> AfterRemovingItem;
		/// <summary>Generates the <see cref="AfterRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterRemovingItem(EntityObject item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
		}

		#endregion

		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		public EntityCollection()
		{
			this.innerArray = new List<EntityObject>();
		}
		/// <summary>Initializes a new instance of the class and has the specified initial capacity.</summary>
		/// <param name="capacity">The number of items the collection can initially store.</param>
		public EntityCollection(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity), "The collection capacity cannot be negative.");
			}
			this.innerArray = new List<EntityObject>(capacity);
		}

		#endregion

		#region public properties

		/// <inheritdoc/>
		public EntityObject this[int index]
		{
			get => this.innerArray[index];
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				EntityObject remove = this.innerArray[index];

				if (!this.OnBeforeRemovingItem(remove))
				{
					return;
				}

				if (!this.OnBeforeAddingItem(value))
				{
					return;
				}

				this.innerArray[index] = value;
				this.OnAfterAddingItem(value);
				this.OnAfterRemovingItem(remove);
			}
		}

		/// <inheritdoc/>
		public int Count => this.innerArray.Count;

		/// <inheritdoc/>
		public virtual bool IsReadOnly => false;

		#endregion

		#region public methods

		/// <summary>Adds an <see cref="EntityObject">entity</see> to the collection.</summary>
		/// <param name="item">The <see cref="EntityObject">entity</see> to add to the collection.</param>
		/// <returns><see langword="true"/> if the <see cref="EntityObject">entity</see> has been added to the collection; otherwise, <see langword="false"/>.</returns>
		public void Add(EntityObject item)
		{
			if (!this.OnBeforeAddingItem(item, "Item"))
			{
				throw new ArgumentException("The entity cannot be added to the collection.", nameof(item));
			}
			this.innerArray.Add(item);
			this.OnAfterAddingItem(item, "Item");
		}

		/// <summary>Adds an <see cref="EntityObject">entity</see> list to the end of the collection.</summary>
		/// <param name="collection">The collection whose elements should be added.</param>
		public void AddRange(IEnumerable<EntityObject> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			foreach (EntityObject item in collection)
			{
				this.Add(item);
			}
		}

		/// <summary>Inserts an <see cref="EntityObject">entity</see> into the collection at the specified index.</summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The <see cref="EntityObject">entity</see> to insert. The value can not be <see langword="null"/>.</param>
		public void Insert(int index, EntityObject item)
		{
			if (index < 0 || index >= this.innerArray.Count)
			{
				throw new ArgumentOutOfRangeException(string.Format("The parameter index {0} must be in between {1} and {2}.", index, 0, this.innerArray.Count));
			}

			if (!this.OnBeforeRemovingItem(this.innerArray[index], "Item"))
			{
				return;
			}

			if (!this.OnBeforeAddingItem(item, "Item"))
			{
				throw new ArgumentException("The entity cannot be added to the collection.", nameof(item));
			}

			this.OnAfterRemovingItem(this.innerArray[index], "Item");
			this.innerArray.Insert(index, item);
			this.OnAfterAddingItem(item, "Item");
		}

		/// <inheritdoc/>
		public bool Remove(EntityObject item)
		{
			if (!this.OnBeforeRemovingItem(item, "Item"))
			{
				return false;
			}

			bool ok = this.innerArray.Remove(item);
			if (ok)
			{
				this.OnAfterRemovingItem(item, "Item");
			}

			return ok;
		}

		/// <summary>Removes the first occurrence of a specific object from the collection</summary>
		/// <param name="items">The list of objects to remove from the collection.</param>
		/// <returns><see langword="true"/> if object is successfully removed; otherwise, <see langword="false"/>.</returns>
		public void Remove(IEnumerable<EntityObject> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			foreach (EntityObject item in items)
			{
				this.Remove(item);
			}
		}

		/// <inheritdoc/>
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.innerArray.Count)
			{
				throw new ArgumentOutOfRangeException(string.Format("The parameter index {0} must be in between {1} and {2}.", index, 0, this.innerArray.Count));
			}

			EntityObject remove = this.innerArray[index];
			if (!this.OnBeforeRemovingItem(remove, "Item"))
			{
				return;
			}

			this.innerArray.RemoveAt(index);
			this.OnAfterRemovingItem(remove, "Item");
		}

		/// <inheritdoc/>
		public void Clear()
		{
			EntityObject[] entities = new EntityObject[this.innerArray.Count];
			this.innerArray.CopyTo(entities, 0);
			foreach (EntityObject item in entities)
			{
				this.Remove(item);
			}
		}

		/// <inheritdoc/>
		public int IndexOf(EntityObject item) => this.innerArray.IndexOf(item);

		/// <inheritdoc/>
		public bool Contains(EntityObject item) => this.innerArray.Contains(item);

		/// <inheritdoc/>
		public void CopyTo(EntityObject[] array, int arrayIndex) => this.innerArray.CopyTo(array, arrayIndex);

		/// <inheritdoc/>
		public IEnumerator<EntityObject> GetEnumerator() => this.innerArray.GetEnumerator();

		#endregion

		#region private methods

		void ICollection<EntityObject>.Add(EntityObject item) => this.Add(item);

		void IList<EntityObject>.Insert(int index, EntityObject item) => this.Insert(index, item);

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		#endregion
	}
}