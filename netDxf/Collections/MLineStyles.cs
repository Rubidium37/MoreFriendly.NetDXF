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
			style.AfterAddingMLineStyleElement += this.MLineStyle_AfterAddingMLineStyleElement;
			style.AfterRemovingMLineStyleElement += this.MLineStyle_AfterRemovingMLineStyleElement;
			style.BeforeChangingLinetypeValue += this.MLineStyle_BeforeChangingLinetypeValue;

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
			item.AfterAddingMLineStyleElement -= this.MLineStyle_AfterAddingMLineStyleElement;
			item.AfterRemovingMLineStyleElement -= this.MLineStyle_AfterRemovingMLineStyleElement;
			item.BeforeChangingLinetypeValue -= this.MLineStyle_BeforeChangingLinetypeValue;

			return true;
		}

		#endregion

		#region MLineStyle events

		private void Item_NameChanged(object sender, AfterValueChangeEventArgs<String> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another multiline style with the same name.");
			}

			this.List.Remove(e.OldValue);
			this.List.Add(e.NewValue, (MLineStyle)sender);

			var refs = this.GetReferences(e.OldValue);
			this.References.Remove(e.OldValue);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		private void MLineStyle_BeforeChangingLinetypeValue(object sender, BeforeValueChangeEventArgs<Linetype> e)
		{
			var senderT = (DxfObject)sender;
			this.Owner.Linetypes.References[e.OldValue.Name].Remove(senderT);
			e.NewValue = this.Owner.Linetypes.Add(e.NewValue);
			this.Owner.Linetypes.References[e.NewValue.Name].Add(senderT);
		}

		private void MLineStyle_AfterAddingMLineStyleElement(object sender, AfterItemChangeEventArgs<MLineStyleElement> e)
		{
			if (sender is not MLineStyle senderT)
				return;

			e.Item.Linetype = this.Owner.Linetypes.Add(e.Item.Linetype);
			this.Owner.Linetypes.References[e.Item.Linetype.Name].Add(senderT);
		}

		private void MLineStyle_AfterRemovingMLineStyleElement(object sender, AfterItemChangeEventArgs<MLineStyleElement> e)
		{
			if (sender is not MLineStyle senderT)
				return;

			this.Owner.Linetypes.References[e.Item.Linetype.Name].Remove(senderT);
		}

		#endregion
	}
}