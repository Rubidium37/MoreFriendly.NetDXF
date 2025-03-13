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
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents the base class for a dimension <see cref="EntityObject">entity</see>.</summary>
	/// <reamarks>
	/// Once a dimension is added to the <b>DXF</b> document, its properties should not be modified or the changes will not be reflected in the saved <b>DXF</b> file.
	/// </reamarks>
	public abstract class Dimension :
		EntityObject
	{
		#region delegates and events

		public delegate void DimensionStyleChangedEventHandler(Dimension sender, TableObjectChangedEventArgs<DimensionStyle> e);
		public event DimensionStyleChangedEventHandler DimensionStyleChanged;
		protected virtual DimensionStyle OnDimensionStyleChangedEvent(DimensionStyle oldStyle, DimensionStyle newStyle)
		{
			DimensionStyleChangedEventHandler ae = this.DimensionStyleChanged;
			if (ae != null)
			{
				TableObjectChangedEventArgs<DimensionStyle> eventArgs = new TableObjectChangedEventArgs<DimensionStyle>(oldStyle, newStyle);
				ae(this, eventArgs);
				return eventArgs.NewValue;
			}
			return newStyle;
		}

		public delegate void DimensionBlockChangedEventHandler(Dimension sender, TableObjectChangedEventArgs<Block> e);
		public event DimensionBlockChangedEventHandler DimensionBlockChanged;
		protected virtual Block OnDimensionBlockChangedEvent(Block oldBlock, Block newBlock)
		{
			DimensionBlockChangedEventHandler ae = this.DimensionBlockChanged;
			if (ae != null)
			{
				TableObjectChangedEventArgs<Block> eventArgs = new TableObjectChangedEventArgs<Block>(oldBlock, newBlock);
				ae(this, eventArgs);
				return eventArgs.NewValue;
			}
			return newBlock;
		}

		#endregion

		#region delegates and events for style overrides

		public delegate void DimensionStyleOverrideAddedEventHandler(Dimension sender, DimensionStyleOverrideChangeEventArgs e);
		public event DimensionStyleOverrideAddedEventHandler DimensionStyleOverrideAdded;
		protected virtual void OnDimensionStyleOverrideAddedEvent(DimensionStyleOverride item)
		{
			DimensionStyleOverrideAddedEventHandler ae = this.DimensionStyleOverrideAdded;
			if (ae != null)
			{
				ae(this, new DimensionStyleOverrideChangeEventArgs(item));
			}
		}

		public delegate void DimensionStyleOverrideRemovedEventHandler(Dimension sender, DimensionStyleOverrideChangeEventArgs e);
		public event DimensionStyleOverrideRemovedEventHandler DimensionStyleOverrideRemoved;
		protected virtual void OnDimensionStyleOverrideRemovedEvent(DimensionStyleOverride item)
		{
			DimensionStyleOverrideRemovedEventHandler ae = this.DimensionStyleOverrideRemoved;
			if (ae != null)
			{
				ae(this, new DimensionStyleOverrideChangeEventArgs(item));
			}
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		protected Dimension(DimensionType type)
			: base(EntityType.Dimension, DxfObjectCode.Dimension)
		{
			this.DimensionType = type;
			this.StyleOverrides.BeforeAddItem += this.StyleOverrides_BeforeAddItem;
			this.StyleOverrides.AddItem += this.StyleOverrides_AddItem;
			this.StyleOverrides.BeforeRemoveItem += this.StyleOverrides_BeforeRemoveItem;
			this.StyleOverrides.RemoveItem += this.StyleOverrides_RemoveItem;
		}

		#endregion

		#region internal properties

		/// <summary>Gets the reference <see cref="Vector2">position</see> for the dimension line in local coordinates.</summary>
		internal Vector2 DefinitionPoint { get; set; } = Vector2.Zero;

		#endregion

		#region public properties

		/// <summary>Gets or sets if the text reference point has been set by the user. Set to <see langword="false"/> to reset the dimension text to its original position.</summary>
		public bool TextPositionManuallySet { get; set; }

		protected Vector2 _TextReferencePoint = Vector2.Zero;
		/// <summary>Gets or sets the text reference <see cref="Vector2">position</see>, the middle point of dimension text in local coordinates.</summary>
		/// <remarks>
		/// This value is related to the style property <see cref="DimensionStyle.FitTextMove"/>.
		/// If the style is set to <see cref="DimensionStyleFitTextMove.BesideDimLine"/> the text reference point will take precedence over the offset value to place the dimension line.
		/// In case of <see cref="DimensionType.Ordinate"/> dimensions if the text has been manually set the text position will take precedence over the EndLeaderPoint only if the style
		/// has been set to <see cref="DimensionStyleFitTextMove.OverDimLineWithoutLeader"/>.
		/// </remarks>
		public Vector2 TextReferencePoint
		{
			get => _TextReferencePoint;
			set
			{
				this.TextPositionManuallySet = true;
				_TextReferencePoint = value;
			}
		}

		private DimensionStyle _Style = DimensionStyle.Default;
		/// <summary>Gets or sets the style associated with the dimension.</summary>
		public DimensionStyle Style
		{
			get => _Style;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Style = this.OnDimensionStyleChangedEvent(_Style, value);
			}
		}

		/// <summary>Gets the dimension style overrides list.</summary>
		/// <remarks>Any dimension style value stored in this list will override its corresponding value in the assigned style to the dimension.</remarks>
		public DimensionStyleOverrideDictionary StyleOverrides { get; } = new DimensionStyleOverrideDictionary();

		/// <summary>Gets the dimension type.</summary>
		public DimensionType DimensionType { get; }

		/// <summary>Gets the actual measurement.</summary>
		public abstract double Measurement { get; }

		/// <summary>Gets or sets the dimension text attachment point.</summary>
		public MTextAttachmentPoint AttachmentPoint { get; set; } = MTextAttachmentPoint.MiddleCenter;

		/// <summary>Get or sets the dimension text <see cref="MTextLineSpacingStyle">line spacing style</see>.</summary>
		public MTextLineSpacingStyle LineSpacingStyle { get; set; } = MTextLineSpacingStyle.AtLeast;

		private double _LineSpacingFactor = 1.0;
		/// <summary>Gets or sets the dimension text line spacing factor.</summary>
		/// <remarks>
		/// Percentage of default line spacing to be applied. Valid values range from 0.25 to 4.00, the default value 1.0.
		/// </remarks>
		public double LineSpacingFactor
		{
			get => _LineSpacingFactor;
			set
			{
				if (value < 0.25 || value > 4.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The line spacing factor valid values range from 0.25 to 4.00");
				}
				_LineSpacingFactor = value;
			}
		}

		private Block _Block;
		/// <summary>Gets the block that contains the entities that make up the dimension picture.</summary>
		/// <remarks>
		/// Set this value to <see langword="null"/> to force the program that reads the resulting <b>DXF</b> file to generate the dimension drawing block,
		/// some programs do not even care about this block and will always generate their own dimension drawings.<br />
		/// You can even use your own dimension drawing setting this value with the resulting block.
		/// The assigned block name is irrelevant, it will be automatically modified to accommodate the naming conventions of the blocks for dimension (*D#).<br />
		/// The block will be overwritten when adding the dimension to a <see cref="DxfDocument"/> if <see cref="DxfDocument.BuildDimensionBlocks"/> is set to <see langword="true"/>.
		/// </remarks>
		public Block Block
		{
			get => _Block;
			set => _Block = this.OnDimensionBlockChangedEvent(_Block, value);
		}

		private double _TextRotation = 0.0;
		/// <summary>Gets or sets the rotation angle in degrees of the dimension text away from its default orientation(the direction of the dimension line).</summary>
		public double TextRotation
		{
			get => _TextRotation;
			set => _TextRotation = MathHelper.NormalizeAngle(value);
		}

		private string _UserText = string.Empty;
		/// <summary>Gets or sets the dimension text explicitly.</summary>
		/// <remarks>
		/// Dimension text explicitly entered by the user. Optional; default is the measurement.
		/// If <see langword="null"/> or "&lt;&gt;", the dimension measurement is drawn as the text,
		/// if " " (one blank space), the text is suppressed. Anything else is drawn as the text.
		/// </remarks>
		public string UserText
		{
			get => _UserText;
			set => _UserText = string.IsNullOrEmpty(value) ? string.Empty : value;
		}

		/// <summary>Gets or sets the dimension elevation, its position along its normal.</summary>
		public double Elevation { get; set; } = 0.0;

		#endregion

		#region abstract methods

		/// <summary>Calculate the dimension reference points.</summary>
		protected abstract void CalculateReferencePoints();

		/// <summary>Gets the block that contains the entities that make up the dimension picture.</summary>
		/// <param name="name">Name to be assigned to the generated block.</param>
		/// <returns>The block that represents the actual dimension.</returns>
		protected abstract Block BuildBlock(string name);

		#endregion

		#region public methods

		/// <summary>Updates the internal data of the dimension and if needed it rebuilds the block definition of the actual dimension.</summary>
		/// <remarks>
		/// This method needs to be manually called to reflect any change made to the dimension properties (geometry and/or style).
		/// </remarks>
		public void Update()
		{
			this.CalculateReferencePoints();

			if (_Block != null)
			{
				Block newBlock = this.BuildBlock(_Block.Name);
				_Block = this.OnDimensionBlockChangedEvent(_Block, newBlock);
			}
		}

		#endregion

		#region Dimension style overrides events

		private void StyleOverrides_BeforeAddItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
		{
			if (sender.TryGetValue(e.Item.Type, out DimensionStyleOverride old))
			{
				if (ReferenceEquals(old.Value, e.Item.Value))
				{
					e.Cancel = true;
				}
			}
		}

		private void StyleOverrides_AddItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
			=> this.OnDimensionStyleOverrideAddedEvent(e.Item);

		private void StyleOverrides_BeforeRemoveItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
		{
		}

		private void StyleOverrides_RemoveItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
			=> this.OnDimensionStyleOverrideRemovedEvent(e.Item);

		#endregion
	}
}