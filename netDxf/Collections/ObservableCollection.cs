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
	/// <summary>Represent a collection of items that fire events when it is modified.</summary>
	/// <typeparam name="T">Type of items.</typeparam>
	public class ObservableCollection<T> :
		IList<T>
	{
		private readonly List<T> innerArray;

		#region delegates and events

		/// <summary>Generated when an <see cref="T"/> item is about to be added; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<T> BeforeAddingItem;
		/// <summary>Generates the <see cref="BeforeAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be added; otherwise, <see langword="false"/>.</returns>
		protected virtual bool OnBeforeAddingItem(T item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeAddingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<T>(propertyName, ItemChangeAction.Add, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="T"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<T> AfterAddingItem;
		/// <summary>Generates the <see cref="AfterAddingItem"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterAddingItem(T item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="T"/> item is about to be removed; allows to confirm or reject the operation.</summary>
		public event BeforeItemChangeEventHandler<T> BeforeRemovingItem;
		/// <summary>Generates the <see cref="BeforeRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		/// <returns><see langword="true"/> if the item can be removed; otherwise, <see langword="false"/>.</returns>
		protected virtual bool OnBeforeRemovingItem(T item, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeRemovingItem is { } handler)
			{
				var e = new BeforeItemChangeEventArgs<T>(propertyName, ItemChangeAction.Remove, item);
				handler(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <summary>Generated when an <see cref="T"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<T> AfterRemovingItem;
		/// <summary>Generates the <see cref="AfterRemovingItem"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterRemovingItem(T item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingItem is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
		}

		#endregion

		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		public ObservableCollection()
		{
			this.innerArray = new List<T>();
		}
		/// <summary>Initializes a new instance of the class and has the specified initial capacity.</summary>
		/// <param name="capacity">The number of items the collection can initially store.</param>
		public ObservableCollection(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity), "The collection capacity cannot be negative.");
			}
			this.innerArray = new List<T>(capacity);
		}

		#endregion

		#region public properties

		/// <inheritdoc/>
		public T this[int index]
		{
			get => this.innerArray[index];
			set
			{
				T remove = this.innerArray[index];
				T add = value;

				if (!this.OnBeforeRemovingItem(remove))
				{
					return;
				}

				if (!this.OnBeforeAddingItem(add))
				{
					return;
				}

				this.innerArray[index] = value;
				this.OnAfterAddingItem(add);
				this.OnAfterRemovingItem(remove);
			}
		}

		/// <inheritdoc/>
		public int Count => this.innerArray.Count;

		/// <inheritdoc/>
		public virtual bool IsReadOnly => false;

		#endregion

		#region public methods

		/// <summary>Reverses the order of the elements in the entire list.</summary>
		public void Reverse() => this.innerArray.Reverse();

		/// <summary>Sorts the elements in the entire System.Collections.Generic.List&lt;T&gt; using the specified System.Comparison&lt;T&gt;.</summary>
		/// <param name="comparision">The System.Comparison&lt;T&gt; to use when comparing elements.</param>
		public void Sort(Comparison<T> comparision) => this.innerArray.Sort(comparision);

		/// <summary>Sorts the elements in a range of elements in System.Collections.Generic.List&lt;T&gt; using the specified comparer.</summary>
		/// <param name="index">The zero-based starting index of the range to sort.</param>
		/// <param name="count">The length of the range to sort.</param>
		/// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or <see langword="null"/> to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
		public void Sort(int index, int count, IComparer<T> comparer) => this.innerArray.Sort(index, count, comparer);

		/// <summary>Sorts the elements in a range of elements in System.Collections.Generic.List&lt;T&gt; using the specified comparer.</summary>
		/// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or <see langword="null"/> to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
		public void Sort(IComparer<T> comparer) => this.innerArray.Sort(comparer);

		/// <summary>Sorts the elements in the entire System.Collections.Generic.List&lt;T&gt; using the default comparer.</summary>
		public void Sort() => this.innerArray.Sort();

		/// <summary>Adds an object to the collection.</summary>
		/// <param name="item">The object to add to the collection.</param>
		/// <returns><see langword="true"/> if the object has been added to the collection; otherwise, <see langword="false"/>.</returns>
		public void Add(T item)
		{
			if (!this.OnBeforeAddingItem(item, "Item"))
			{
				throw new ArgumentException("The item cannot be added to the collection.", nameof(item));
			}
			this.innerArray.Add(item);
			this.OnAfterAddingItem(item, "Item");
		}

		/// <summary>Adds an object list to the end of the collection.</summary>
		/// <param name="collection">The collection whose elements should be added.</param>
		public void AddRange(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			foreach (T item in collection)
			{
				this.Add(item);
			}
		}

		/// <summary>Inserts an object into the collection at the specified index.</summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert. The value can not be <see langword="null"/>.</param>
		/// <returns><see langword="true"/> if the object has been inserted to the collection; otherwise, <see langword="false"/>.</returns>
		public void Insert(int index, T item)
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
				throw new ArgumentException("The item cannot be added to the collection.", nameof(item));
			}

			this.OnAfterRemovingItem(this.innerArray[index], "Item");
			this.innerArray.Insert(index, item);
			this.OnAfterAddingItem(item, "Item");
		}

		/// <inheritdoc/>
		public bool Remove(T item)
		{
			if (!this.innerArray.Contains(item))
			{
				return false;
			}

			if (!this.OnBeforeRemovingItem(item, "Item"))
			{
				return false;
			}

			this.innerArray.Remove(item);
			this.OnAfterRemovingItem(item, "Item");
			return true;
		}

		/// <summary>Removes the first occurrence of a specific object from the collection</summary>
		/// <param name="items">The list of objects to remove from the collection.</param>
		public void Remove(IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			foreach (T item in items)
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

			T remove = this.innerArray[index];
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
			T[] items = new T[this.innerArray.Count];
			this.innerArray.CopyTo(items, 0);
			foreach (T item in items)
			{
				this.Remove(item);
			}
		}

		/// <inheritdoc/>
		public int IndexOf(T item) => this.innerArray.IndexOf(item);

		/// <inheritdoc/>
		public bool Contains(T item) => this.innerArray.Contains(item);

		/// <inheritdoc/>
		public void CopyTo(T[] array, int arrayIndex) => this.innerArray.CopyTo(array, arrayIndex);

		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator() => this.innerArray.GetEnumerator();

		#endregion

		#region private methods

		void ICollection<T>.Add(T item) => this.Add(item);

		void IList<T>.Insert(int index, T item) => this.Insert(index, item);

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		#endregion
	}
}