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
using System.Diagnostics;
using netDxf.Tables;

namespace netDxf.Collections
{
	/// <summary>Represents a collection of application registries.</summary>
	public sealed class ApplicationRegistries :
		TableObjects<ApplicationRegistry>
	{
		#region constructor

		internal ApplicationRegistries(DxfDocument document)
			: this(document, null)
		{
		}
		internal ApplicationRegistries(DxfDocument document, string handle)
			: base(document, DxfObjectCode.ApplicationIdTable, handle)
		{
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override ApplicationRegistry Add(ApplicationRegistry appReg, bool assignHandle)
		{
			if (appReg == null)
			{
				throw new ArgumentNullException(nameof(appReg));
			}

			if (this.List.TryGetValue(appReg.Name, out ApplicationRegistry add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(appReg.Handle))
			{
				this.Owner.NumHandles = appReg.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(appReg.Name, appReg);
			this.References.Add(appReg.Name, new DxfObjectReferences());

			appReg.Owner = this;

			appReg.NameChanged += this.Item_NameChanged;

			Debug.Assert(!string.IsNullOrEmpty(appReg.Handle), "The application registry handle cannot be null or empty.");
			this.Owner.AddedObjects.Add(appReg.Handle, appReg);

			return appReg;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(ApplicationRegistry item)
		{
			if (item == null)
			{
				return false;
			}

			if (!this.Contains(item))
			{
				return false;
			}

			if (item.IsReserved)
			{
				return false;
			}

			if (this.HasReferences(item))
			{
				return false;
			}

			this.Owner.AddedObjects.Remove(item.Handle);
			this.References.Remove(item.Name);
			this.List.Remove(item.Name);

			item.Handle = null;
			item.Owner = null;

			item.NameChanged -= this.Item_NameChanged;

			return true;
		}

		#endregion

		#region TableObject events

		private void Item_NameChanged(object sender, AfterValueChangeEventArgs<String> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another application registry with the same name.");
			}

			this.List.Remove(e.OldValue);
			this.List.Add(e.NewValue, (ApplicationRegistry)sender);

			var refs = this.References[e.OldValue].ToList();
			this.References.Remove(e.OldValue);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		#endregion
	}
}