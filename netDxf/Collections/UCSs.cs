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
	/// <summary>Represents a collection of user coordinate systems.</summary>
	/// <remarks>The <b>UCS</b>s collection method <see cref="UCS.GetReferences"/> will always return an empty list since there are no <see cref="DxfObject"/>s that references them.</remarks>
	public sealed class UCSs :
		TableObjects<UCS>
	{
		#region constructor

		internal UCSs(DxfDocument document)
			: this(document, null)
		{
		}
		internal UCSs(DxfDocument document, string handle)
			: base(document, DxfObjectCode.UcsTable, handle)
		{
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override UCS Add(UCS ucs, bool assignHandle)
		{
			if (ucs == null)
			{
				throw new ArgumentNullException(nameof(ucs));
			}

			if (this.List.TryGetValue(ucs.Name, out UCS add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(ucs.Handle))
			{
				this.Owner.NumHandles = ucs.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(ucs.Name, ucs);
			this.References.Add(ucs.Name, new DxfObjectReferences());

			ucs.Owner = this;

			ucs.NameChanged += this.Item_NameChanged;

			this.Owner.AddedObjects.Add(ucs.Handle, ucs);

			return ucs;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(UCS item)
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
				throw new ArgumentException("There is already another UCS with the same name.");
			}

			this.List.Remove(e.OldValue);
			this.List.Add(e.NewValue, (UCS)sender);

			var refs = this.GetReferences(e.OldValue);
			this.References.Remove(e.OldValue);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		#endregion
	}
}