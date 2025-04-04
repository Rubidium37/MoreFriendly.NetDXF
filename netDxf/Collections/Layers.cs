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
	/// <summary>Represents a collection of layers.</summary>
	public sealed class Layers :
		TableObjects<Layer>
	{
		#region constructor

		internal Layers(DxfDocument document)
			: this(document, null)
		{
		}
		internal Layers(DxfDocument document, string handle)
			: base(document, DxfObjectCode.LayerTable, handle)
		{
			this.StateManager = new LayerStateManager(document);
		}

		#endregion

		#region public properties

		/// <summary>Gets the layer state manager.</summary>
		public LayerStateManager StateManager { get; }

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override Layer Add(Layer layer, bool assignHandle)
		{
			if (layer == null)
			{
				throw new ArgumentNullException(nameof(layer));
			}

			if (this.List.TryGetValue(layer.Name, out Layer add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(layer.Handle))
			{
				this.Owner.NumHandles = layer.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(layer.Name, layer);
			this.References.Add(layer.Name, new DxfObjectReferences());

			layer.Linetype = this.Owner.Linetypes.Add(layer.Linetype);
			this.Owner.Linetypes.References[layer.Linetype.Name].Add(layer);

			layer.Owner = this;

			layer.NameChanged += this.Item_NameChanged;
			layer.BeforeChangingLinetypeValue += this.Layer_BeforeChangingLinetypeValue;

			Debug.Assert(!string.IsNullOrEmpty(layer.Handle), "The layer handle cannot be null or empty.");
			this.Owner.AddedObjects.Add(layer.Handle, layer);

			return layer;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(Layer item)
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

			this.Owner.Linetypes.References[item.Linetype.Name].Remove(item);
			this.Owner.AddedObjects.Remove(item.Handle);
			this.References.Remove(item.Name);
			this.List.Remove(item.Name);

			item.Handle = null;
			item.Owner = null;

			item.NameChanged -= this.Item_NameChanged;
			item.BeforeChangingLinetypeValue -= this.Layer_BeforeChangingLinetypeValue;

			return true;
		}

		#endregion

		#region Layer events

		private void Item_NameChanged(object sender, AfterValueChangeEventArgs<String> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another layer with the same name.");
			}

			this.List.Remove(e.OldValue);
			this.List.Add(e.NewValue, (Layer)sender);

			var refs = this.GetReferences(e.OldValue);
			this.References.Remove(e.OldValue);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		private void Layer_BeforeChangingLinetypeValue(object sender, BeforeValueChangeEventArgs<Linetype> e)
		{
			var senderT = (DxfObject)sender;
			this.Owner.Linetypes.References[e.OldValue.Name].Remove(senderT);
			e.NewValue = this.Owner.Linetypes.Add(e.NewValue);
			this.Owner.Linetypes.References[e.NewValue.Name].Add(senderT);
		}

		#endregion
	}
}