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
using netDxf.Tables;

namespace netDxf.Collections
{
	/// <summary>Represents a collection of views.</summary>
	public sealed class Views :
		TableObjects<View>
	{
		#region constructor

		internal Views(DxfDocument document)
			: this(document, null)
		{
		}
		internal Views(DxfDocument document, string handle)
			: base(document, DxfObjectCode.ViewTable, handle)
		{
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override View Add(View view, bool assignHandle)
		{
			if (view == null)
			{
				throw new ArgumentNullException(nameof(view));
			}

			if (this.List.TryGetValue(view.Name, out View add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(view.Handle))
			{
				this.Owner.NumHandles = view.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(view.Name, view);
			this.References.Add(view.Name, new DxfObjectReferences());

			view.Owner = this;

			view.NameChanged += this.Item_NameChanged;

			this.Owner.AddedObjects.Add(view.Handle, view);

			return view;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(View item)
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

		#region UCS events

		private void Item_NameChanged(object sender, AfterValueChangeEventArgs<String> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another View with the same name.");
			}

			this.List.Remove(e.OldValue);
			this.List.Add(e.NewValue, (View)sender);

			var refs = this.GetReferences(e.OldValue);
			this.References.Remove(e.OldValue);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		#endregion
	}
}