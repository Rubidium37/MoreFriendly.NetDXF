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
	/// <summary>Represents a dictionary of <see cref="XData"/>.</summary>
	public sealed class XDataDictionary :
		IDictionary<string, XData>
	{
		private readonly Dictionary<string, XData> innerDictionary;

		#region delegates and events

		/// <summary>Generated when an <see cref="ApplicationRegistry"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<ApplicationRegistry> AfterAddingApplicationRegistry;
		/// <summary>Generates the <see cref="AfterAddingApplicationRegistry"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		private void OnAfterAddingApplicationRegistry(ApplicationRegistry item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingApplicationRegistry is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="ApplicationRegistry"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<ApplicationRegistry> AfterRemovingApplicationRegistry;
		/// <summary>Generates the <see cref="AfterRemovingApplicationRegistry"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		private void OnAfterRemovingApplicationRegistry(ApplicationRegistry item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingApplicationRegistry is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
		}

		#endregion

		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		public XDataDictionary()
		{
			this.innerDictionary = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase);
		}
		/// <summary>Initializes a new instance of the class and has the specified items.</summary>
		/// <param name="items">The list of <see cref="XData">extended data</see> items initially stored.</param>
		public XDataDictionary(IEnumerable<XData> items)
		{
			this.innerDictionary = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase);
			this.AddRange(items);
		}
		/// <summary>Initializes a new instance of the class and has the specified initial capacity.</summary>
		/// <param name="capacity">The number of items the collection can initially store.</param>
		public XDataDictionary(int capacity)
		{
			this.innerDictionary = new Dictionary<string, XData>(capacity, StringComparer.OrdinalIgnoreCase);
		}

		#endregion

		#region public properties

		/// <inheritdoc/>
		public XData this[string appId]
		{
			get => this.innerDictionary[appId];
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				if (!string.Equals(value.ApplicationRegistry.Name, appId, StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException(string.Format("The extended data application registry name {0} must be equal to the specified appId {1}.", value.ApplicationRegistry.Name, appId));
				}

				this.innerDictionary[appId] = value;
			}
		}

		/// <summary>Gets a collection containing the application registry names of the current dictionary.</summary>
		public ICollection<string> AppIds => this.innerDictionary.Keys;

		/// <inheritdoc/>
		public ICollection<XData> Values => this.innerDictionary.Values;

		/// <inheritdoc/>
		public int Count => this.innerDictionary.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		#endregion

		#region public methods

		/// <summary>Adds an <see cref="XData">extended data</see> to the current dictionary.</summary>
		/// <param name="item">The <see cref="XData">extended data</see> to add.</param>
		/// <remarks>
		/// If the current dictionary already contains an appId equals to the extended data that is being added
		/// the <see cref="XDataRecord">XDataRecords</see> will be added to the existing one.
		/// </remarks>
		public void Add(XData item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (this.innerDictionary.TryGetValue(item.ApplicationRegistry.Name, out XData xdata))
			{
				xdata.XDataRecord.AddRange(item.XDataRecord);
			}
			else
			{
				this.innerDictionary.Add(item.ApplicationRegistry.Name, item);
				item.ApplicationRegistry.NameChanged += this.ApplicationRegistry_NameChanged;
				this.OnAfterAddingApplicationRegistry(item.ApplicationRegistry, "Item");
			}
		}

		/// <summary>Adds a list of <see cref="XData">extended data</see> to the current dictionary.</summary>
		/// <param name="items">The list of <see cref="XData">extended data</see> to add.</param>
		public void AddRange(IEnumerable<XData> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			foreach (XData data in items)
			{
				this.Add(data);
			}
		}

		/// <inheritdoc/>
		public bool Remove(string appId)
		{
			if (!this.innerDictionary.ContainsKey(appId))
			{
				return false;
			}

			XData xdata = this.innerDictionary[appId];
			xdata.ApplicationRegistry.NameChanged -= this.ApplicationRegistry_NameChanged;
			this.innerDictionary.Remove(appId);
			this.OnAfterRemovingApplicationRegistry(xdata.ApplicationRegistry, "Item");
			return true;
		}

		/// <inheritdoc/>
		public void Clear()
		{
			string[] ids = new string[this.innerDictionary.Count];
			this.innerDictionary.Keys.CopyTo(ids, 0);
			foreach (string appId in ids)
			{
				this.Remove(appId);
			}
		}

		/// <summary>Determines whether current dictionary contains an <see cref="XData">extended data</see> with the specified application registry name.</summary>
		/// <param name="appId">The application registry name to locate in the current dictionary.</param>
		/// <returns><see langword="true"/> if the current dictionary contains an <see cref="XData">extended data</see> with the application registry name; otherwise, <see langword="false"/>.</returns>
		public bool ContainsAppId(string appId) => this.innerDictionary.ContainsKey(appId);

		/// <summary>Determines whether current dictionary contains a specified <see cref="XData">extended data</see>.</summary>
		/// <param name="value">The <see cref="XData">extended data</see> to locate in the current dictionary.</param>
		/// <returns><see langword="true"/> if the current dictionary contains the <see cref="XData">extended data</see>; otherwise, <see langword="false"/>.</returns>
		public bool ContainsValue(XData value) => this.innerDictionary.ContainsValue(value);

		/// <inheritdoc/>
		public bool TryGetValue(string appId, out XData value) => this.innerDictionary.TryGetValue(appId, out value);

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<string, XData>> GetEnumerator() => this.innerDictionary.GetEnumerator();

		#endregion

		#region private properties

		ICollection<string> IDictionary<string, XData>.Keys => this.innerDictionary.Keys;

		#endregion

		#region private methods

		bool IDictionary<string, XData>.ContainsKey(string tag) => this.innerDictionary.ContainsKey(tag);

		void IDictionary<string, XData>.Add(string key, XData value) => this.Add(value);

		void ICollection<KeyValuePair<string, XData>>.Add(KeyValuePair<string, XData> item) => this.Add(item.Value);

		bool ICollection<KeyValuePair<string, XData>>.Remove(KeyValuePair<string, XData> item)
			=> ReferenceEquals(item.Value, this.innerDictionary[item.Key]) && this.Remove(item.Key);

		bool ICollection<KeyValuePair<string, XData>>.Contains(KeyValuePair<string, XData> item)
			=> ((IDictionary<string, XData>)this.innerDictionary).Contains(item);

		void ICollection<KeyValuePair<string, XData>>.CopyTo(KeyValuePair<string, XData>[] array, int arrayIndex)
			=> ((IDictionary<string, XData>)this.innerDictionary).CopyTo(array, arrayIndex);

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		#endregion

		#region ApplicationRegistry events

		private void ApplicationRegistry_NameChanged(object sender, AfterValueChangeEventArgs<String> e)
		{
			XData xdata = this.innerDictionary[e.OldValue];
			this.innerDictionary.Remove(e.OldValue);
			this.innerDictionary.Add(e.NewValue, xdata);
		}

		#endregion
	}
}