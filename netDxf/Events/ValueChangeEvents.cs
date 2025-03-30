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
	/// <summary>Represents a method that handles value changes before they occur; allows to confirm, replace or reject the new values.</summary>
	/// <typeparam name="TValue">Type of the property values.</typeparam>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">An object that contains the event data.</param>
	public delegate void BeforeValueChangeEventHandler<TValue>(object sender, BeforeValueChangeEventArgs<TValue> e);
	/// <summary>Represents the event data for a <see cref="BeforeValueChangeEventHandler{TValue}"/>.</summary>
	/// <typeparam name="TValue">Type of the property values.</typeparam>
	public class BeforeValueChangeEventArgs<TValue> :
		CancelEventArgs
	{
		/// <summary>Gets the name of the affected property.</summary>
		public string PropertyName { get; }

		/// <summary>Gets the old value, being changed.</summary>
		public TValue OldValue { get; }
		/// <summary>Gets or sets the new value, that will replace the old one.</summary>
		/// <remarks>Can be changed by the event handler.</remarks>
		public TValue NewValue { get; set; }

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="propertyName">Name of the affected property.</param>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		public BeforeValueChangeEventArgs(string propertyName, TValue oldValue, TValue newValue)
			: base()
		{
			this.PropertyName = propertyName;
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="propertyName">Name of the affected property.</param>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="cancel"><inheritdoc cref="CancelEventArgs.CancelEventArgs(bool)" path="/param[@name='cancel']"/></param>
		public BeforeValueChangeEventArgs(string propertyName, TValue oldValue, TValue newValue, bool cancel)
			: base(cancel)
		{
			this.PropertyName = propertyName;
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}
	}

	/// <summary>Represents a method that handles value changes after they have occurred.</summary>
	/// <typeparam name="TValue">Type of the property values.</typeparam>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">An object that contains the event data.</param>
	public delegate void AfterValueChangeEventHandler<TValue>(object sender, AfterValueChangeEventArgs<TValue> e);
	/// <summary>Represents the event data for a <see cref="AfterValueChangeEventHandler{TValue}"/>.</summary>
	/// <typeparam name="TValue">Type of the property values.</typeparam>
	public class AfterValueChangeEventArgs<TValue> :
		EventArgs
	{
		/// <summary>Gets the name of the affected property.</summary>
		public string PropertyName { get; }

		/// <summary>Gets the old value, being changed.</summary>
		public TValue OldValue { get; }
		/// <summary>Gets the new value, that replaced the old one.</summary>
		public TValue NewValue { get; }

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="propertyName">Name of the affected property.</param>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that replaced the old one.</param>
		public AfterValueChangeEventArgs(string propertyName, TValue oldValue, TValue newValue)
			: base()
		{
			this.PropertyName = propertyName;
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}
	}
}
