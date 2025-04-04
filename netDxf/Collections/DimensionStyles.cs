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
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Collections
{
	/// <summary>Represents a collection of dimension styles.</summary>
	public sealed class DimensionStyles :
		TableObjects<DimensionStyle>
	{
		#region constructor

		internal DimensionStyles(DxfDocument document)
			: this(document, null)
		{
		}
		internal DimensionStyles(DxfDocument document, string handle)
			: base(document, DxfObjectCode.DimensionStyleTable, handle)
		{
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override DimensionStyle Add(DimensionStyle item, bool assignHandle)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (this.List.TryGetValue(item.Name, out DimensionStyle add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(item.Handle))
			{
				this.Owner.NumHandles = item.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(item.Name, item);
			this.References.Add(item.Name, new DxfObjectReferences());

			// add referenced text style
			item.TextStyle = this.Owner.TextStyles.Add(item.TextStyle, assignHandle);
			this.Owner.TextStyles.References[item.TextStyle.Name].Add(item);

			// add referenced blocks
			if (item.LeaderArrow != null)
			{
				item.LeaderArrow = this.Owner.Blocks.Add(item.LeaderArrow, assignHandle);
				this.Owner.Blocks.References[item.LeaderArrow.Name].Add(item);
			}
			if (item.DimArrow1 != null)
			{
				item.DimArrow1 = this.Owner.Blocks.Add(item.DimArrow1, assignHandle);
				this.Owner.Blocks.References[item.DimArrow1.Name].Add(item);
			}
			if (item.DimArrow2 != null)
			{
				item.DimArrow2 = this.Owner.Blocks.Add(item.DimArrow2, assignHandle);
				this.Owner.Blocks.References[item.DimArrow2.Name].Add(item);
			}

			// add referenced line types
			item.DimLineLinetype = this.Owner.Linetypes.Add(item.DimLineLinetype, assignHandle);
			this.Owner.Linetypes.References[item.DimLineLinetype.Name].Add(item);

			item.ExtLine1Linetype = this.Owner.Linetypes.Add(item.ExtLine1Linetype, assignHandle);
			this.Owner.Linetypes.References[item.ExtLine1Linetype.Name].Add(item);

			item.ExtLine2Linetype = this.Owner.Linetypes.Add(item.ExtLine2Linetype, assignHandle);
			this.Owner.Linetypes.References[item.ExtLine2Linetype.Name].Add(item);

			item.Owner = this;

			item.NameChanged += this.Item_NameChanged;
			item.BeforeChangingLinetypeValue += this.DimensionStyle_BeforeChangingLinetypeValue;
			item.BeforeChangingTextStyleValue += this.DimensionStyle_BeforeChangingTextStyleValue;
			item.BeforeChangingBlockValue += this.DimensionStyle_BeforeChangingBlockValue;

			this.Owner.AddedObjects.Add(item.Handle, item);

			return item;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(DimensionStyle item)
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

			// remove referenced text style
			this.Owner.TextStyles.References[item.TextStyle.Name].Remove(item);

			// remove referenced blocks
			if (item.LeaderArrow != null)
			{
				this.Owner.Blocks.References[item.LeaderArrow.Name].Remove(item);
			}
			if (item.DimArrow1 != null)
			{
				this.Owner.Blocks.References[item.DimArrow1.Name].Remove(item);
			}

			if (item.DimArrow2 != null)
			{
				this.Owner.Blocks.References[item.DimArrow2.Name].Remove(item);
			}

			// remove referenced line types
			this.Owner.Linetypes.References[item.DimLineLinetype.Name].Remove(item);
			this.Owner.Linetypes.References[item.ExtLine1Linetype.Name].Remove(item);
			this.Owner.Linetypes.References[item.ExtLine2Linetype.Name].Remove(item);

			this.References.Remove(item.Name);
			this.List.Remove(item.Name);

			item.Handle = null;
			item.Owner = null;

			item.NameChanged -= this.Item_NameChanged;
			item.BeforeChangingLinetypeValue -= this.DimensionStyle_BeforeChangingLinetypeValue;
			item.BeforeChangingTextStyleValue -= this.DimensionStyle_BeforeChangingTextStyleValue;
			item.BeforeChangingBlockValue -= this.DimensionStyle_BeforeChangingBlockValue;

			return true;
		}

		#endregion

		#region TableObject events

		private void Item_NameChanged(object sender, AfterValueChangeEventArgs<String> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another dimension style with the same name.");
			}

			this.List.Remove(e.OldValue);
			this.List.Add(e.NewValue, (DimensionStyle)sender);

			var refs = this.GetReferences(e.OldValue);
			this.References.Remove(e.OldValue);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		private void DimensionStyle_BeforeChangingLinetypeValue(object sender, BeforeValueChangeEventArgs<Linetype> e)
		{
			var senderT = (DxfObject)sender;
			this.Owner.Linetypes.References[e.OldValue.Name].Remove(senderT);
			e.NewValue = this.Owner.Linetypes.Add(e.NewValue);
			this.Owner.Linetypes.References[e.NewValue.Name].Add(senderT);
		}

		private void DimensionStyle_BeforeChangingTextStyleValue(object sender, BeforeValueChangeEventArgs<TextStyle> e)
		{
			var senderT = (DxfObject)sender;
			this.Owner.TextStyles.References[e.OldValue.Name].Remove(senderT);
			e.NewValue = this.Owner.TextStyles.Add(e.NewValue);
			this.Owner.TextStyles.References[e.NewValue.Name].Add(senderT);
		}

		private void DimensionStyle_BeforeChangingBlockValue(object sender, BeforeValueChangeEventArgs<Block> e)
		{
			var senderT = (DxfObject)sender;
			this.Owner.Blocks.References[e.OldValue.Name].Remove(senderT);
			e.NewValue = this.Owner.Blocks.Add(e.NewValue);
			this.Owner.Blocks.References[e.NewValue.Name].Add(senderT);
		}

		#endregion
	}
}