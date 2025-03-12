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
using System.IO;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Blocks
{
	/// <summary>Represents a block definition.</summary>
	public class Block :
		TableObject
	{
		#region delegates and events

		public delegate void LayerChangedEventHandler(Block sender, TableObjectChangedEventArgs<Layer> e);
		public event LayerChangedEventHandler LayerChanged;
		protected virtual Layer OnLayerChangedEvent(Layer oldLayer, Layer newLayer)
		{
			LayerChangedEventHandler ae = this.LayerChanged;
			if (ae != null)
			{
				TableObjectChangedEventArgs<Layer> eventArgs = new TableObjectChangedEventArgs<Layer>(oldLayer, newLayer);
				ae(this, eventArgs);
				return eventArgs.NewValue;
			}
			return newLayer;
		}

		public delegate void EntityAddedEventHandler(Block sender, BlockEntityChangeEventArgs e);
		public event EntityAddedEventHandler EntityAdded;
		protected virtual void OnEntityAddedEvent(EntityObject item)
		{
			EntityAddedEventHandler ae = this.EntityAdded;
			if (ae != null)
			{
				ae(this, new BlockEntityChangeEventArgs(item));
			}
		}

		public delegate void EntityRemovedEventHandler(Block sender, BlockEntityChangeEventArgs e);
		public event EntityRemovedEventHandler EntityRemoved;
		protected virtual void OnEntityRemovedEvent(EntityObject item)
		{
			EntityRemovedEventHandler ae = this.EntityRemoved;
			if (ae != null)
			{
				ae(this, new BlockEntityChangeEventArgs(item));
			}
		}

		public delegate void AttributeDefinitionAddedEventHandler(Block sender, BlockAttributeDefinitionChangeEventArgs e);
		public event AttributeDefinitionAddedEventHandler AttributeDefinitionAdded;
		protected virtual void OnAttributeDefinitionAddedEvent(AttributeDefinition item)
		{
			AttributeDefinitionAddedEventHandler ae = this.AttributeDefinitionAdded;
			if (ae != null)
			{
				ae(this, new BlockAttributeDefinitionChangeEventArgs(item));
			}
		}

		public delegate void AttributeDefinitionRemovedEventHandler(Block sender, BlockAttributeDefinitionChangeEventArgs e);
		public event AttributeDefinitionRemovedEventHandler AttributeDefinitionRemoved;
		protected virtual void OnAttributeDefinitionRemovedEvent(AttributeDefinition item)
		{
			AttributeDefinitionRemovedEventHandler ae = this.AttributeDefinitionRemoved;
			if (ae != null)
			{
				ae(this, new BlockAttributeDefinitionChangeEventArgs(item));
			}
		}

		#endregion

		#region private fields

		private string description;
		private Layer layer;

		#endregion

		#region constants

		/// <summary>Default <see cref="ModelSpace"/> block name.</summary>
		public const string DefaultModelSpaceName = "*Model_Space";

		/// <summary>Default <see cref="PaperSpace"/> block name.</summary>
		public const string DefaultPaperSpaceName = "*Paper_Space";

		/// <summary>Gets the default <b>*Model_Space</b> block.</summary>
		public static Block ModelSpace => new Block(DefaultModelSpaceName, null, null, false);

		/// <summary>Gets the default <b>*Paper_Space</b> block.</summary>
		public static Block PaperSpace => new Block(DefaultPaperSpaceName, null, null, false);

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class as an external reference drawing.</summary>
		/// <param name="name">Block name.</param>
		/// <param name="xrefFile">External reference path name.</param>
		/// <remarks>Only <b>DWG</b> files can be used as externally referenced blocks.</remarks>
		public Block(string name, string xrefFile)
			: this(name, xrefFile, false)
		{
		}

		/// <summary>Initializes a new instance of the class as an external reference drawing.</summary>
		/// <param name="name">Block name.</param>
		/// <param name="xrefFile">External reference path name.</param>
		/// <param name="overlay">Specifies if the external reference is an overlay, by default it is set to <see langword="false"/>.</param>
		/// <remarks>Only <b>DWG</b> files can be used as externally referenced blocks.</remarks>
		public Block(string name, string xrefFile, bool overlay)
			: this(name, null, null, true)
		{
			if (string.IsNullOrEmpty(xrefFile))
			{
				throw new ArgumentNullException(nameof(xrefFile));
			}

			if (xrefFile.IndexOfAny(Path.GetInvalidPathChars()) == 0)
			{
				throw new ArgumentException("File path contains invalid characters.", nameof(xrefFile));
			}

			this.XrefFile = xrefFile;
			this.Flags = BlockTypeFlags.XRef | BlockTypeFlags.ResolvedExternalReference;
			if (overlay)
			{
				this.Flags |= BlockTypeFlags.XRefOverlay;
			}
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Block name.</param>
		public Block(string name)
			: this(name, null, null, true)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Block name.</param>
		/// <param name="entities">The list of entities that make the block.</param>
		public Block(string name, IEnumerable<EntityObject> entities)
			: this(name, entities, null, true)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Block name.</param>
		/// <param name="entities">The list of entities that make the block.</param>
		/// <param name="attributes">The list of attribute definitions that make the block.</param>
		public Block(string name, IEnumerable<EntityObject> entities, IEnumerable<AttributeDefinition> attributes)
			: this(name, entities, attributes, true)
		{
		}

		internal Block(string name, IEnumerable<EntityObject> entities, IEnumerable<AttributeDefinition> attributes, bool checkName)
			: base(name, DxfObjectCode.Block, checkName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			this.IsReserved = string.Equals(name, DefaultModelSpaceName, StringComparison.OrdinalIgnoreCase);
			this.IsForInternalUseOnly = name.StartsWith("*");
			this.description = string.Empty;
			this.Origin = Vector3.Zero;
			this.layer = Layer.Default;
			this.XrefFile = string.Empty;
			this.Owner = new BlockRecord(name);
			this.Flags = BlockTypeFlags.None;
			this.End = new EndBlock(this);

			this.Entities = new EntityCollection();
			this.Entities.BeforeAddItem += this.Entities_BeforeAddItem;
			this.Entities.AddItem += this.Entities_AddItem;
			this.Entities.BeforeRemoveItem += this.Entities_BeforeRemoveItem;
			this.Entities.RemoveItem += this.Entities_RemoveItem;
			if (entities != null)
			{
				this.Entities.AddRange(entities);
			}

			this.AttributeDefinitions = new AttributeDefinitionDictionary();
			this.AttributeDefinitions.BeforeAddItem += this.AttributeDefinitions_BeforeAddItem;
			this.AttributeDefinitions.AddItem += this.AttributeDefinitions_ItemAdd;
			this.AttributeDefinitions.BeforeRemoveItem += this.AttributeDefinitions_BeforeRemoveItem;
			this.AttributeDefinitions.RemoveItem += this.AttributeDefinitions_RemoveItem;
			if (attributes != null)
			{
				this.AttributeDefinitions.AddRange(attributes);
			}
		}

		#endregion

		#region public properties

		/// <summary>Gets the name of the table object.</summary>
		/// <remarks>
		/// Table object names are case insensitive.<br />
		/// The internal blocks that start with "*U" or "*T" can be safely renamed.
		/// They are internally created to represent dynamic blocks, arrays, and tables;
		/// although the information of those objects is lost when importing the DXF,
		/// the block that represent its graphical appearance is imported.
		/// </remarks>
		public new string Name
		{
			get => base.Name;
			set
			{
				if (this.IsForInternalUseOnly)
				{
					if (this.Name.StartsWith("*U", StringComparison.InvariantCultureIgnoreCase) || this.Name.StartsWith("*T", StringComparison.InvariantCultureIgnoreCase))
					{
						// The internal blocks that starts with "*U" and "*T" are created by AutoCad as a graphical representation of other kind of entities
						// like dynamic blocks, arrays, and tables; although the information of those objects is lost when importing the DXF,
						// the block that represent its graphical appearance is imported.
						// They should be safe to rename.
						this.Flags &= ~BlockTypeFlags.AnonymousBlock;
					}
					else
					{
						throw new ArgumentException("Blocks for internal use cannot be renamed.", nameof(value));
					}
				}
				base.Name = value;
				this.Record.Name = value;
			}
		}

		/// <summary>Gets or sets the block description.</summary>
		/// <remarks>
		/// <b>AutoCAD</b> has an unknown limit on the number of characters for the description when loading an external DXF,
		/// while, on the other hand is perfectly capable of saving a <see cref="Block"/> description that surpasses such limit.<br />
		/// Keep in mind that when saving a <b>DXF</b> prior to the <b>AutoCad2007</b> version, non-ASCII characters will be encoded,
		/// therefore a single letter might consume more characters when saved into the DXF.<br />
		/// New line characters are not allowed.
		/// </remarks>
		public string Description
		{
			get => this.description;
			set => this.description = string.IsNullOrEmpty(value) ? string.Empty : value;
		}

		/// <summary>Gets or sets the block origin in world coordinates, it is recommended to always keep this value to the default Vector3.Zero.</summary>
		public Vector3 Origin { get; set; }

		/// <summary>Gets or sets the block <see cref="Layer">layer</see>.</summary>
		/// <remarks>It seems that the block layer is always the default "0" regardless of what is defined here, so it is pointless to change this value.</remarks>
		public Layer Layer
		{
			get => this.layer;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				this.layer = this.OnLayerChangedEvent(this.layer, value);
			}
		}

		/// <summary>Gets the <see cref="EntityObject">entity</see> list of the block.</summary>
		/// <remarks>Null entities, attribute definitions or entities already owned by another block or document cannot be added to the list.</remarks>
		public EntityCollection Entities { get; }

		/// <summary>Gets the <see cref="AttributeDefinition">entity</see> list of the block.</summary>
		/// <remarks>
		/// <see langword="null"/> or attribute definitions already owned by another block or document cannot be added to the list.
		/// Additionally Paper Space blocks do not contain attribute definitions.
		/// </remarks>
		public AttributeDefinitionDictionary AttributeDefinitions { get; }

		/// <summary>Gets the owner of the actual <b>DXF</b> object.</summary>
		public new BlockRecord Owner
		{
			get => (BlockRecord)base.Owner;
			internal set => base.Owner = value;
		}

		/// <summary>Gets the block record associated with this block.</summary>
		/// <remarks>It returns the same object as the owner property.</remarks>
		public BlockRecord Record => this.Owner;

		/// <summary>Gets the block-type flags (bit-coded values, may be combined).</summary>
		public BlockTypeFlags Flags { get; internal set; }

		/// <summary>Gets the external reference path name.</summary>
		/// <remarks>
		/// This property is only applicable to externally referenced blocks.
		/// </remarks>
		public string XrefFile { get; }

		/// <summary>Gets if the block is an external reference.</summary>
		public bool IsXRef => this.Flags.HasFlag(BlockTypeFlags.XRef);

		/// <summary>All blocks that starts with "*" are for internal use only.</summary>
		public bool IsForInternalUseOnly { get; private set; }

		#endregion

		#region internal properties

		/// <summary>Gets or sets the block end object.</summary>
		internal EndBlock End { get; }

		#endregion

		#region public methods

		/// <summary>Creates a block from the content of a <see cref="DxfDocument">document</see>.</summary>
		/// <param name="doc">A <see cref="DxfDocument"/> instance.</param>
		/// <param name="name">Name of the new block.</param>
		/// <returns>The block build from the <see cref="DxfDocument">document</see> content.</returns>
		/// <remarks>Only the entities contained in ModelSpace will make part of the block.</remarks>
		public static Block Create(DxfDocument doc, string name)
		{
			if (doc == null)
			{
				throw new ArgumentNullException(nameof(doc));
			}

			Block block = new Block(name)
			{
				Origin = doc.DrawingVariables.InsBase
			};
			block.Record.Units = doc.DrawingVariables.InsUnits;

			// When a Block is created form a DxfDocument or DXF file, any Hatch entity that it may contain will have its associative property set to false.
			// This will avoid problems when the same entity its associated to multiple hatches,
			// or when any of any of its paths is defined by the intersection of several entities.
			foreach (Hatch hatch in doc.Entities.Hatches)
			{
				hatch.UnLinkBoundary();
			}

			// copy the entities of the document,
			// only entities in the ModelSpace will be part of a block
			foreach (EntityObject entity in doc.Layouts[Layout.ModelSpaceName].AssociatedBlock.Entities)
			{
				block.Entities.Add((EntityObject)entity.Clone());
			}

			// copy the attribute definitions to the new block
			foreach (AttributeDefinition attdef in doc.Layouts[Layout.ModelSpaceName].AssociatedBlock.AttributeDefinitions.Values)
			{
				block.AttributeDefinitions.Add((AttributeDefinition)attdef.Clone());
			}

			return block;
		}

		/// <summary>Creates a block from an external <b>DXF</b> file.</summary>
		/// <param name="file">DXF file name.</param>
		/// <returns>The block build from the <b>DXF</b> file content. It will return <see langword="null"/> if the file has not been able to load.</returns>
		/// <remarks>
		/// The name of the block will be the file name without extension, and
		/// only the entities contained in <see cref="ModelSpace"/> will make part of the block.
		/// </remarks>
		public static Block Load(string file) => Load(file, string.Empty, new List<string>());
		/// <summary>Creates a block from an external <b>DXF</b> file.</summary>
		/// <param name="file">DXF file name.</param>
		/// <param name="supportFolders">List of the document support folders.</param>
		/// <returns>The block build from the <b>DXF</b> file content. It will return <see langword="null"/> if the file has not been able to load.</returns>
		/// <remarks>
		/// The name of the block will be the file name without extension, and
		/// only the entities contained in <see cref="ModelSpace"/> will make part of the block.
		/// </remarks>
		public static Block Load(string file, IEnumerable<string> supportFolders) => Load(file, string.Empty, supportFolders);
		/// <summary>Creates a block from an external <b>DXF</b> file.</summary>
		/// <param name="file">DXF file name.</param>
		/// <param name="name">Name of the new block.</param>
		/// <returns>The block build from the <b>DXF</b> file content. It will return <see langword="null"/> if the file has not been able to load.</returns>
		/// <remarks>Only the entities contained in <see cref="ModelSpace"/> will make part of the block.</remarks>
		public static Block Load(string file, string name) => Load(file, name, new List<string>());
		/// <summary>Creates a block from an external <b>DXF</b> file.</summary>
		/// <param name="file">DXF file name.</param>
		/// <param name="name">Name of the new block.</param>
		/// <param name="supportFolders">List of the document support folders.</param>
		/// <returns>The block build from the <b>DXF</b> file content. It will return <see langword="null"/> if the file has not been able to load.</returns>
		/// <remarks>Only the entities contained in <see cref="ModelSpace"/> will make part of the block.</remarks>
		public static Block Load(string file, string name, IEnumerable<string> supportFolders)
		{
#if DEBUG
			DxfDocument dwg = DxfDocument.Load(file, supportFolders);
#else
			DxfDocument dwg;
			try
			{
				dwg = DxfDocument.Load(file, supportFolders);
			}
			catch
			{
				return null;
			}
#endif

			string blkName = string.IsNullOrEmpty(name) ? dwg.Name : name;
			return Create(dwg, blkName);
		}

		/// <summary>Saves a block to a text <b>DXF</b> file.</summary>
		/// <param name="file">DXF file name.</param>
		/// <param name="version">Version of the <b>DXF</b> database version.</param>
		/// <returns>Return <see langword="true"/> if the file has been successfully save; otherwise, <see langword="false"/>.</returns>
		public bool Save(string file, DxfVersion version) => this.Save(file, version, false);
		/// <summary>Saves a block to a <b>DXF</b> file.</summary>
		/// <param name="file">DXF file name.</param>
		/// <param name="version">Version of the <b>DXF</b> database version.</param>
		/// <param name="isBinary">Defines if the file will be saved as binary.</param>
		/// <returns>Return <see langword="true"/> if the file has been successfully save; otherwise, <see langword="false"/>.</returns>
		public bool Save(string file, DxfVersion version, bool isBinary)
		{
			DxfDocument dwg = new DxfDocument(version);
			dwg.DrawingVariables.InsBase = this.Origin;
			dwg.DrawingVariables.InsUnits = this.Record.Units;

			foreach (AttributeDefinition attdef in this.AttributeDefinitions.Values)
			{
				if (!dwg.Layouts[Layout.ModelSpaceName].AssociatedBlock.AttributeDefinitions.ContainsTag(attdef.Tag))
				{
					dwg.Layouts[Layout.ModelSpaceName].AssociatedBlock.AttributeDefinitions.Add((AttributeDefinition)attdef.Clone());
				}
			}

			foreach (EntityObject entity in this.Entities)
			{
				dwg.Layouts[Layout.ModelSpaceName].AssociatedBlock.Entities.Add((EntityObject)entity.Clone());
			}

			return dwg.Save(file, isBinary);
		}

		#endregion

		#region internal methods

		internal new void SetName(string newName, bool checkName)
		{
			// Hack to change the table name without having to check its name.
			// Some invalid characters are used for internal purposes only.
			base.SetName(newName, checkName);
			this.Record.Name = newName;
			this.IsForInternalUseOnly = newName.StartsWith("*");
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override bool HasReferences() => this.Owner.Owner != null && this.Owner.Owner.HasReferences(this.Name);

		/// <inheritdoc/>
		public override List<DxfObjectReference> GetReferences()
		{
			if (this.Owner.Owner == null)
			{
				return null;
			}

			return this.Owner.Owner.GetReferences(this.Name);
		}

		private static TableObject Clone(Block block, string newName, bool checkName)
		{
			if (block.Record.Layout != null && !IsValidName(newName))
			{
				throw new ArgumentException("*Model_Space and *Paper_Space# blocks can only be cloned with a new valid name.");
			}

			Block copy = new Block(newName, null, null, checkName)
			{
				Description = block.description,
				Flags = block.Flags,
				Layer = (Layer)block.Layer.Clone(),
				Origin = block.Origin
			};

			// remove anonymous flag for renamed anonymous blocks
			if (checkName)
			{
				copy.Flags &= ~BlockTypeFlags.AnonymousBlock;
			}

			foreach (EntityObject e in block.Entities)
			{
				copy.Entities.Add((EntityObject)e.Clone());
			}

			foreach (AttributeDefinition a in block.AttributeDefinitions.Values)
			{
				copy.AttributeDefinitions.Add((AttributeDefinition)a.Clone());
			}

			foreach (XData data in block.XData.Values)
			{
				copy.XData.Add((XData)data.Clone());
			}

			foreach (XData data in block.Record.XData.Values)
			{
				copy.Record.XData.Add((XData)data.Clone());
			}

			return copy;
		}
		/// <inheritdoc/>
		public override TableObject Clone(string newName) => Clone(this, newName, true);
		/// <inheritdoc/>
		public override object Clone() => Clone(this, this.Name, !this.Flags.HasFlag(BlockTypeFlags.AnonymousBlock));

		/// <inheritdoc/>
		internal override long AssignHandle(long entityNumber)
		{
			entityNumber = this.Owner.AssignHandle(entityNumber);
			entityNumber = this.End.AssignHandle(entityNumber);
			foreach (AttributeDefinition attdef in this.AttributeDefinitions.Values)
			{
				entityNumber = attdef.AssignHandle(entityNumber);
			}

			return base.AssignHandle(entityNumber);
		}

		#endregion

		#region Entities collection events

		private void Entities_BeforeAddItem(EntityCollection sender, EntityCollectionEventArgs e)
		{
			// null items, entities already owned by another Block, attribute definitions and attributes are not allowed in the entities list.
			if (e.Item == null)
			{
				e.Cancel = true;
			}
			else if (this.Flags.HasFlag(BlockTypeFlags.ExternallyDependent))
			{
				e.Cancel = true;
			}
			else if (e.Item.Owner != null)
			{
				e.Cancel = true;
			}
			else
			{
				e.Cancel = false;
			}
		}

		private void Entities_AddItem(EntityCollection sender, EntityCollectionEventArgs e)
		{
			if (e.Item.Type == EntityType.Leader)
			{
				Leader leader = (Leader)e.Item;
				if (leader.Annotation != null)
				{
					this.Entities.Add(leader.Annotation);
				}
			}
			else if (e.Item.Type == EntityType.Hatch)
			{
				Hatch hatch = (Hatch)e.Item;
				foreach (HatchBoundaryPath path in hatch.BoundaryPaths)
				{
					foreach (EntityObject entity in path.Entities)
					{
						this.Entities.Add(entity);
					}
				}
			}
			else if (e.Item.Type == EntityType.Viewport)
			{
				Viewport viewport = (Viewport)e.Item;
				if (viewport.ClippingBoundary != null)
				{
					this.Entities.Add(viewport.ClippingBoundary);
				}
			}
			this.OnEntityAddedEvent(e.Item);
			e.Item.Owner = this;
		}

		private void Entities_BeforeRemoveItem(EntityCollection sender, EntityCollectionEventArgs e)
		{
			// only items owned by the actual block can be removed
			if (e.Item.Reactors.Count > 0)
			{
				e.Cancel = true;
			}
			else
			{
				e.Cancel = !ReferenceEquals(e.Item.Owner, this);
			}
		}

		private void Entities_RemoveItem(EntityCollection sender, EntityCollectionEventArgs e)
		{
			this.OnEntityRemovedEvent(e.Item);
			e.Item.Owner = null;
		}

		#endregion

		#region Attributes dictionary events

		private void AttributeDefinitions_BeforeAddItem(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e)
		{
			// attributes with the same tag, and attribute definitions already owned by another Block are not allowed in the attributes list.
			if (e.Item == null)
			{
				e.Cancel = true;
			}
			else if (this.Flags.HasFlag(BlockTypeFlags.ExternallyDependent))
			{
				e.Cancel = true;
			}
			else if (this.AttributeDefinitions.ContainsTag(e.Item.Tag))
			{
				e.Cancel = true;
			}
			else if (e.Item.Owner != null)
			{
				e.Cancel = true;
			}
			else
			{
				e.Cancel = false;
			}
		}

		private void AttributeDefinitions_ItemAdd(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e)
		{
			this.OnAttributeDefinitionAddedEvent(e.Item);
			e.Item.Owner = this;
			// the block has attributes
			this.Flags |= BlockTypeFlags.NonConstantAttributeDefinitions;
		}

		private void AttributeDefinitions_BeforeRemoveItem(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e)
		{
			// only attribute definitions owned by the actual block can be removed
			e.Cancel = !ReferenceEquals(e.Item.Owner, this);
		}

		private void AttributeDefinitions_RemoveItem(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e)
		{
			this.OnAttributeDefinitionRemovedEvent(e.Item);
			e.Item.Owner = null;
			if (this.AttributeDefinitions.Count == 0)
			{
				this.Flags &= ~BlockTypeFlags.NonConstantAttributeDefinitions;
			}
		}

		#endregion
	}
}