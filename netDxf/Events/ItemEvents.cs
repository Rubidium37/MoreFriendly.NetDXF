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
using System.ComponentModel;

namespace netDxf
{
	/// <summary>Represents a method that handles item changes before they occur; allows to confirm or reject the changes.</summary>
	/// <typeparam name="TItem">Type of the item.</typeparam>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">An object that contains the event data.</param>
	public delegate void BeforeItemChangeEventHandler<TItem>(object sender, BeforeItemChangeEventArgs<TItem> e);
	/// <summary>Represents the event data for a <see cref="BeforeItemChangeEventHandler{TItem}"/>.</summary>
	/// <typeparam name="TItem">Type of the item.</typeparam>
	public class BeforeItemChangeEventArgs<TItem> :
		CancelEventArgs
	{
		/// <summary>Gets the name of the affected collection property.</summary>
		public string PropertyName { get; }

		/// <summary>Gets the action being performed.</summary>
		public ItemChangeAction Action { get; }

		/// <summary>Gets the item being changed.</summary>
		public TItem Item { get; }

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="propertyName">Name of the affected collection property.</param>
		/// <param name="action">Action being performed.</param>
		/// <param name="item">Item being changed.</param>
		public BeforeItemChangeEventArgs(string propertyName, ItemChangeAction action, TItem item)
			: base()
		{
			this.PropertyName = propertyName;
			this.Action = action;
			this.Item = item;
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="propertyName">Name of the affected collection property.</param>
		/// <param name="action">Action being performed.</param>
		/// <param name="item">Item being changed.</param>
		/// <param name="cancel"><inheritdoc cref="CancelEventArgs.CancelEventArgs(bool)" path="/param[@name='cancel']"/></param>
		public BeforeItemChangeEventArgs(string propertyName, ItemChangeAction action, TItem item, bool cancel)
			: base(cancel)
		{
			this.PropertyName = propertyName;
			this.Action = action;
			this.Item = item;
		}
	}

	/// <summary>Represents a method that handles item changes after they have occurred.</summary>
	/// <typeparam name="TItem">Type of the item.</typeparam>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">An object that contains the event data.</param>
	public delegate void AfterItemChangeEventHandler<TItem>(object sender, AfterItemChangeEventArgs<TItem> e);
	/// <summary>Represents the event data for a <see cref="AfterItemChangeEventHandler{TItem}"/>.</summary>
	/// <typeparam name="TItem">Type of the item.</typeparam>
	public class AfterItemChangeEventArgs<TItem> :
		EventArgs
	{
		/// <summary>Gets the name of the affected collection property.</summary>
		public string PropertyName { get; }

		/// <summary>Gets the action being performed.</summary>
		public ItemChangeAction Action { get; }

		/// <summary>Gets the item being changed.</summary>
		public TItem Item { get; }

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="propertyName">Name of the affected collection property.</param>
		/// <param name="action">Action being performed.</param>
		/// <param name="item">Item being changed.</param>
		public AfterItemChangeEventArgs(string propertyName, ItemChangeAction action, TItem item)
			: base()
		{
			this.PropertyName = propertyName;
			this.Action = action;
			this.Item = item;
		}
	}

	/// <summary>Defines the actions of events realted to item changes.</summary>
	public enum ItemChangeAction
	{
		/// <summary>The item is being added.</summary>
		Add,
		/// <summary>The item is being remove.</summary>
		Remove
	}
}
