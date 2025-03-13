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
	/// <summary>Represents a collection of image definitions.</summary>
	public sealed class ImageDefinitions :
		TableObjects<ImageDefinition>
	{
		#region constructor

		internal ImageDefinitions(DxfDocument document)
			: this(document, null)
		{
		}
		internal ImageDefinitions(DxfDocument document, string handle)
			: base(document, DxfObjectCode.ImageDefDictionary, handle)
		{
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override ImageDefinition Add(ImageDefinition imageDefinition, bool assignHandle)
		{
			if (imageDefinition == null)
			{
				throw new ArgumentNullException(nameof(imageDefinition));
			}

			if (this.List.TryGetValue(imageDefinition.Name, out ImageDefinition add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(imageDefinition.Handle))
			{
				this.Owner.NumHandles = imageDefinition.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(imageDefinition.Name, imageDefinition);
			this.References.Add(imageDefinition.Name, new DxfObjectReferences());

			imageDefinition.Owner = this;

			imageDefinition.NameChanged += this.Item_NameChanged;

			this.Owner.AddedObjects.Add(imageDefinition.Handle, imageDefinition);

			return imageDefinition;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(ImageDefinition item)
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
				throw new ArgumentException("There is already another image definition with the same name.");
			}

			this.List.Remove(sender.Name);
			this.List.Add(e.NewValue, (ImageDefinition)sender);

			List<DxfObjectReference> refs = this.GetReferences(sender.Name);
			this.References.Remove(sender.Name);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		#endregion
	}
}