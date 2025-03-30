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
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Tables;

namespace netDxf.Collections
{
	/// <summary>Represents a collection of blocks.</summary>
	public sealed class BlockRecords :
		TableObjects<Block>
	{
		#region constructor

		internal BlockRecords(DxfDocument document)
			: this(document, null)
		{
		}
		internal BlockRecords(DxfDocument document, string handle)
			: base(document, DxfObjectCode.BlockRecordTable, handle)
		{
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override Block Add(Block block, bool assignHandle)
		{
			if (block == null)
			{
				throw new ArgumentNullException(nameof(block));
			}

			if (this.List.TryGetValue(block.Name, out Block add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(block.Handle))
			{
				this.Owner.NumHandles = block.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(block.Name, block);
			this.References.Add(block.Name, new DxfObjectReferences());

			block.Layer = this.Owner.Layers.Add(block.Layer);
			this.Owner.Layers.References[block.Layer.Name].Add(block);

			//for new block definitions configure its entities
			foreach (EntityObject entity in block.Entities)
			{
				this.Owner.AddEntityToDocument(entity, assignHandle);
			}

			//for new block definitions configure its attributes
			foreach (AttributeDefinition attDef in block.AttributeDefinitions.Values)
			{
				this.Owner.AddAttributeDefinitionToDocument(attDef, assignHandle);
			}

			block.Record.Owner = this;

			block.NameChanged += this.Item_NameChanged;
			block.BeforeChangingLayerValue += this.Block_BeforeChangingLayerValue;
			block.AfterAddingEntityObject += this.Block_AfterAddingEntityObject;
			block.AfterRemovingEntityObject += this.Block_AfterRemovingEntityObject;
			block.AfterAddingAttributeDefinition += this.Block_AfterAddingAttributeDefinition;
			block.AfterRemovingAttributeDefinition += this.Block_AfterRemovingAttributeDefinition;

			Debug.Assert(!string.IsNullOrEmpty(block.Handle), "The block handle cannot be null or empty.");
			this.Owner.AddedObjects.Add(block.Handle, block);
			this.Owner.AddedObjects.Add(block.Owner.Handle, block.Owner);

			return block;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);

		/// <inheritdoc/>
		public override bool Remove(Block item)
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

			// remove the block from the associated layer
			this.Owner.Layers.References[item.Layer.Name].Remove(item);

			// we will remove all entities from the block definition
			foreach (EntityObject entity in item.Entities)
			{
				this.Owner.RemoveEntityFromDocument(entity);
			}

			// remove all attribute definitions from the associated layers
			foreach (AttributeDefinition attDef in item.AttributeDefinitions.Values)
			{
				this.Owner.RemoveAttributeDefinitionFromDocument(attDef);
			}

			this.Owner.AddedObjects.Remove(item.Handle);
			this.References.Remove(item.Name);
			this.List.Remove(item.Name);

			item.Record.Handle = null;
			item.Record.Owner = null;

			item.Handle = null;
			item.Owner = null;

			item.NameChanged -= this.Item_NameChanged;
			item.BeforeChangingLayerValue -= this.Block_BeforeChangingLayerValue;
			item.AfterAddingEntityObject -= this.Block_AfterAddingEntityObject;
			item.AfterRemovingEntityObject -= this.Block_AfterRemovingEntityObject;
			item.AfterAddingAttributeDefinition -= this.Block_AfterAddingAttributeDefinition;
			item.AfterRemovingAttributeDefinition -= this.Block_AfterRemovingAttributeDefinition;

			return true;
		}

		#endregion

		#region Block events

		private void Item_NameChanged(object sender, AfterValueChangeEventArgs<String> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another block with the same name.");
			}

			this.List.Remove(e.OldValue);
			this.List.Add(e.NewValue, (Block)sender);

			var refs = this.GetReferences(e.OldValue);
			this.References.Remove(e.OldValue);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		private void Block_BeforeChangingLayerValue(object sender, BeforeValueChangeEventArgs<Layer> e)
		{
			var senderT = (DxfObject)sender;
			this.Owner.Layers.References[e.OldValue.Name].Remove(senderT);
			e.NewValue = this.Owner.Layers.Add(e.NewValue);
			this.Owner.Layers.References[e.NewValue.Name].Add(senderT);
		}

		private void Block_AfterAddingEntityObject(object sender, AfterItemChangeEventArgs<EntityObject> e)
			=> this.Owner.AddEntityToDocument(e.Item, string.IsNullOrEmpty(e.Item.Handle));

		private void Block_AfterRemovingEntityObject(object sender, AfterItemChangeEventArgs<EntityObject> e)
			=> this.Owner.RemoveEntityFromDocument(e.Item);

		private void Block_AfterAddingAttributeDefinition(object sender, AfterItemChangeEventArgs<AttributeDefinition> e)
			=> this.Owner.AddAttributeDefinitionToDocument(e.Item, string.IsNullOrEmpty(e.Item.Handle));

		private void Block_AfterRemovingAttributeDefinition(object sender, AfterItemChangeEventArgs<AttributeDefinition> e)
			=> this.Owner.RemoveAttributeDefinitionFromDocument(e.Item);

		#endregion
	}
}