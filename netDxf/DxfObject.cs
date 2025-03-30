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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using netDxf.Collections;
using netDxf.Tables;

namespace netDxf
{
	/// <summary>Represents the base class for all <b>DXF</b> objects.</summary>
	public abstract class DxfObject
	{
		#region delegates and events

		/// <summary>Generated when an <see cref="ApplicationRegistry"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<ApplicationRegistry> AfterAddingApplicationRegistry;
		/// <summary>Generates the <see cref="AfterAddingApplicationRegistry"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterAddingApplicationRegistry(ApplicationRegistry item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingApplicationRegistry is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="ApplicationRegistry"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<ApplicationRegistry> AfterRemovingApplicationRegistry;
		/// <summary>Generates the <see cref="AfterRemovingApplicationRegistry"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterRemovingApplicationRegistry(ApplicationRegistry item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingApplicationRegistry is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="codename"><see cref="DxfObjectCode">DXF object name</see>.</param>
		protected DxfObject(string codename)
		{
			this.CodeName = codename;
			this.XData.AfterAddingApplicationRegistry += this.XData_AfterAddingApplicationRegistry;
			this.XData.AfterRemovingApplicationRegistry += this.XData_AfterRemovingApplicationRegistry;
		}

		#endregion

		#region public properties

		/// <summary>Gets the <see cref="DxfObjectCode">DXF object name</see>.</summary>
		public string CodeName { get; protected set; }

		/// <summary>Gets the handle assigned to the <b>DXF</b> object.</summary>
		/// <remarks>
		/// The handle is a unique hexadecimal number assigned automatically to every <b>DXF</b> object,
		/// that has been added to a <see cref="DxfDocument"/>.
		/// </remarks>
		public string Handle { get; internal set; }

		/// <summary>Gets the owner of the actual <see cref="DxfObject"/>.</summary>
		public DxfObject Owner { get; internal set; }

		/// <summary>Gets the entity <see cref="XDataDictionary">extended data</see>.</summary>
		public XDataDictionary XData { get; } = new XDataDictionary();

		#endregion

		#region internal methods

		/// <summary>Assigns a handle to the object based in a integer counter.</summary>
		/// <param name="entityNumber">Number to assign to the actual object.</param>
		/// <returns>Next available entity number.</returns>
		/// <remarks>
		/// Some objects might consume more than one, this is the case, for example, of polylines that will assign
		/// automatically a handle to its vertexes. The entity number will be converted to an hexadecimal number.
		/// </remarks>
		internal virtual long AssignHandle(long entityNumber)
		{
			this.Handle = entityNumber.ToString("X");
			return entityNumber + 1;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString() => this.CodeName;

		#endregion

		#region XData events

		private void XData_AfterAddingApplicationRegistry(object sender, AfterItemChangeEventArgs<ApplicationRegistry> e)
			=> this.OnAfterAddingApplicationRegistry(e.Item, nameof(this.XData));

		private void XData_AfterRemovingApplicationRegistry(object sender, AfterItemChangeEventArgs<ApplicationRegistry> e)
			=> this.OnAfterRemovingApplicationRegistry(e.Item, nameof(this.XData));

		#endregion
	}
}