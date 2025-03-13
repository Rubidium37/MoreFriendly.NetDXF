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
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Collections
{
	/// <summary>Represents a collection of multiline styles.</summary>
	public sealed class MLineStyles :
		TableObjects<MLineStyle>
	{
		#region constructor

		internal MLineStyles(DxfDocument document)
			: this(document, null)
		{
		}
		internal MLineStyles(DxfDocument document, string handle)
			: base(document, DxfObjectCode.MLineStyleDictionary, handle)
		{
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override MLineStyle Add(MLineStyle style, bool assignHandle)
		{
			if (style == null)
			{
				throw new ArgumentNullException(nameof(style));
			}

			if (this.List.TryGetValue(style.Name, out MLineStyle add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(style.Handle))
			{
				this.Owner.NumHandles = style.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(style.Name, style);
			this.References.Add(style.Name, new DxfObjectReferences());
			foreach (MLineStyleElement element in style.Elements)
			{
				element.Linetype = this.Owner.Linetypes.Add(element.Linetype);
				this.Owner.Linetypes.References[element.Linetype.Name].Add(style);
			}

			style.Owner = this;

			style.NameChanged += this.Item_NameChanged;
			style.MLineStyleElementAdded += this.MLineStyle_ElementAdded;
			style.MLineStyleElementRemoved += this.MLineStyle_ElementRemoved;
			style.MLineStyleElementLinetypeChanged += this.MLineStyle_ElementLinetypeChanged;

			this.Owner.AddedObjects.Add(style.Handle, style);

			return style;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(MLineStyle item)
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

			foreach (MLineStyleElement element in item.Elements)
			{
				this.Owner.Linetypes.References[element.Linetype.Name].Remove(item);
			}

			this.Owner.AddedObjects.Remove(item.Handle);
			this.References.Remove(item.Name);
			this.List.Remove(item.Name);

			item.Handle = null;
			item.Owner = null;

			item.NameChanged -= this.Item_NameChanged;
			item.MLineStyleElementAdded -= this.MLineStyle_ElementAdded;
			item.MLineStyleElementRemoved -= this.MLineStyle_ElementRemoved;
			item.MLineStyleElementLinetypeChanged -= this.MLineStyle_ElementLinetypeChanged;

			return true;
		}

		#endregion

		#region MLineStyle events

		private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another multiline style with the same name.");
			}

			this.List.Remove(sender.Name);
			this.List.Add(e.NewValue, (MLineStyle)sender);

			List<DxfObjectReference> refs = this.GetReferences(sender.Name);
			this.References.Remove(sender.Name);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		private void MLineStyle_ElementLinetypeChanged(MLineStyle sender, TableObjectChangedEventArgs<Linetype> e)
		{
			this.Owner.Linetypes.References[e.OldValue.Name].Remove(sender);

			e.NewValue = this.Owner.Linetypes.Add(e.NewValue);
			this.Owner.Linetypes.References[e.NewValue.Name].Add(sender);
		}

		private void MLineStyle_ElementAdded(MLineStyle sender, MLineStyleElementChangeEventArgs e)
		{
			e.Item.Linetype = this.Owner.Linetypes.Add(e.Item.Linetype);
			this.Owner.Linetypes.References[e.Item.Linetype.Name].Add(sender);
		}

		private void MLineStyle_ElementRemoved(MLineStyle sender, MLineStyleElementChangeEventArgs e)
			=> this.Owner.Linetypes.References[e.Item.Linetype.Name].Remove(sender);

		#endregion
	}
}