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
using netDxf.Entities;

namespace netDxf.Collections
{
	/// <summary>Represent a collection of <see cref="EntityObject">entities</see> that fire events when it is modified.</summary>
	public class EntityCollection :
		IList<EntityObject>
	{
		#region delegates and events

		public delegate void BeforeAddItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);
		public event BeforeAddItemEventHandler BeforeAddItem;
		protected virtual bool OnBeforeAddItemEvent(EntityObject item)
		{
			BeforeAddItemEventHandler ae = this.BeforeAddItem;
			if (ae != null)
			{
				EntityCollectionEventArgs e = new EntityCollectionEventArgs(item);
				ae(this, e);
				return e.Cancel;
			}
			return false;
		}

		public delegate void AddItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);
		public event AddItemEventHandler AddItem;
		protected virtual void OnAddItemEvent(EntityObject item)
		{
			AddItemEventHandler ae = this.AddItem;
			if (ae != null)
			{
				ae(this, new EntityCollectionEventArgs(item));
			}
		}

		public delegate void RemoveItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);
		public event BeforeRemoveItemEventHandler BeforeRemoveItem;
		protected virtual bool OnBeforeRemoveItemEvent(EntityObject item)
		{
			BeforeRemoveItemEventHandler ae = this.BeforeRemoveItem;
			if (ae != null)
			{
				EntityCollectionEventArgs e = new EntityCollectionEventArgs(item);
				ae(this, e);
				return e.Cancel;
			}
			return false;
		}

		public delegate void BeforeRemoveItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);
		public event RemoveItemEventHandler RemoveItem;
		protected virtual void OnRemoveItemEvent(EntityObject item)
		{
			RemoveItemEventHandler ae = this.RemoveItem;
			if (ae != null)
			{
				ae(this, new EntityCollectionEventArgs(item));
			}
		}

		#endregion

		#region private fields

		private readonly List<EntityObject> innerArray;

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
			get { return this.innerArray[index]; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				EntityObject remove = this.innerArray[index];

				if (this.OnBeforeRemoveItemEvent(remove))
				{
					return;
				}

				if (this.OnBeforeAddItemEvent(value))
				{
					return;
				}

				this.innerArray[index] = value;
				this.OnAddItemEvent(value);
				this.OnRemoveItemEvent(remove);
			}
		}

		/// <inheritdoc/>
		public int Count
		{
			get { return this.innerArray.Count; }
		}

		/// <inheritdoc/>
		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region public methods

		/// <summary>Adds an <see cref="EntityObject">entity</see> to the collection.</summary>
		/// <param name="item">The <see cref="EntityObject">entity</see> to add to the collection.</param>
		/// <returns><see langword="true"/> if the <see cref="EntityObject">entity</see> has been added to the collection; otherwise, <see langword="false"/>.</returns>
		public void Add(EntityObject item)
		{
			if (this.OnBeforeAddItemEvent(item))
			{
				throw new ArgumentException("The entity cannot be added to the collection.", nameof(item));
			}
			this.innerArray.Add(item);
			this.OnAddItemEvent(item);
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

			if (this.OnBeforeRemoveItemEvent(this.innerArray[index]))
			{
				return;
			}

			if (this.OnBeforeAddItemEvent(item))
			{
				throw new ArgumentException("The entity cannot be added to the collection.", nameof(item));
			}

			this.OnRemoveItemEvent(this.innerArray[index]);
			this.innerArray.Insert(index, item);
			this.OnAddItemEvent(item);
		}

		/// <inheritdoc/>
		public bool Remove(EntityObject item)
		{
			if (this.OnBeforeRemoveItemEvent(item))
			{
				return false;
			}

			bool ok = this.innerArray.Remove(item);
			if (ok)
			{
				this.OnRemoveItemEvent(item);
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
			if (this.OnBeforeRemoveItemEvent(remove))
			{
				return;
			}

			this.innerArray.RemoveAt(index);
			this.OnRemoveItemEvent(remove);
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
		public int IndexOf(EntityObject item)
		{
			return this.innerArray.IndexOf(item);
		}

		/// <inheritdoc/>
		public bool Contains(EntityObject item)
		{
			return this.innerArray.Contains(item);
		}

		/// <inheritdoc/>
		public void CopyTo(EntityObject[] array, int arrayIndex)
		{
			this.innerArray.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc/>
		public IEnumerator<EntityObject> GetEnumerator()
		{
			return this.innerArray.GetEnumerator();
		}

		#endregion

		#region private methods

		void ICollection<EntityObject>.Add(EntityObject item)
		{
			this.Add(item);
		}

		void IList<EntityObject>.Insert(int index, EntityObject item)
		{
			this.Insert(index, item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}