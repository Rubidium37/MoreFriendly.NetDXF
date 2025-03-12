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
	/// <summary>Represents a collection of <b>DWF</b> underlay definitions.</summary>
	public sealed class UnderlayDwfDefinitions :
		TableObjects<UnderlayDwfDefinition>
	{
		#region constructor

		internal UnderlayDwfDefinitions(DxfDocument document)
			: this(document, null)
		{
		}

		internal UnderlayDwfDefinitions(DxfDocument document, string handle)
			: base(document, DxfObjectCode.UnderlayDwfDefinitionDictionary, handle)
		{
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override UnderlayDwfDefinition Add(UnderlayDwfDefinition underlayDwfDefinition, bool assignHandle)
		{
			if (underlayDwfDefinition == null)
			{
				throw new ArgumentNullException(nameof(underlayDwfDefinition));
			}

			if (this.List.TryGetValue(underlayDwfDefinition.Name, out UnderlayDwfDefinition add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(underlayDwfDefinition.Handle))
			{
				this.Owner.NumHandles = underlayDwfDefinition.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(underlayDwfDefinition.Name, underlayDwfDefinition);
			this.References.Add(underlayDwfDefinition.Name, new DxfObjectReferences());

			underlayDwfDefinition.Owner = this;

			underlayDwfDefinition.NameChanged += this.Item_NameChanged;

			this.Owner.AddedObjects.Add(underlayDwfDefinition.Handle, underlayDwfDefinition);

			return underlayDwfDefinition;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(UnderlayDwfDefinition item)
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

		private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another DWF underlay definition with the same name.");
			}

			this.List.Remove(sender.Name);
			this.List.Add(e.NewValue, (UnderlayDwfDefinition)sender);

			List<DxfObjectReference> refs = this.GetReferences(sender.Name);
			this.References.Remove(sender.Name);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		#endregion
	}
}