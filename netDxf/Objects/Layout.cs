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
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Tables;

namespace netDxf.Objects
{
	/// <summary>Represents a layout.</summary>
	public class Layout :
		TableObject,
		IComparable<Layout>
	{
		#region private fields

		private short tabOrder;

		#endregion

		#region constants

		/// <summary>Layout <see cref="ModelSpace"/> name.</summary>
		public const string ModelSpaceName = "Model";

		/// <summary>Gets the <see cref="ModelSpace"/> layout.</summary>
		/// <remarks>
		/// There can be only one model space layout and it is always called "Model".
		/// </remarks>
		public static Layout ModelSpace => new Layout(ModelSpaceName, Block.ModelSpace, new PlotSettings());

		#endregion

		#region constructor

		/// <summary>Initializes a new layout.</summary>
		/// <param name="name">Layout name.</param>
		public Layout(string name)
			: this(name, null, new PlotSettings())
		{
		}

		private Layout(string name, Block associatedBlock, PlotSettings plotSettings)
			: base(name, DxfObjectCode.Layout, true)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name), "The layout name should be at least one character long.");
			}

			if (name.Equals(ModelSpaceName, StringComparison.OrdinalIgnoreCase))
			{
				this.IsReserved = true;
				this.IsPaperSpace = false;
				this.Viewport = null;
				plotSettings.Flags = PlotFlags.Initializing | PlotFlags.UpdatePaper | PlotFlags.ModelType | PlotFlags.DrawViewportsFirst | PlotFlags.PrintLineweights | PlotFlags.PlotPlotStyles | PlotFlags.UseStandardScale;
			}
			else
			{
				this.IsReserved = false;
				this.IsPaperSpace = true;
				this.Viewport = new Viewport(1)
				{
					ViewCenter = new Vector2(50.0, 100.0),
					Status = ViewportStatusFlags.AdaptiveGridDisplay |
							 ViewportStatusFlags.DisplayGridBeyondDrawingLimits |
							 ViewportStatusFlags.CurrentlyAlwaysEnabled |
							 ViewportStatusFlags.UcsIconVisibility
				};
			}

			this.tabOrder = 0;
			this.AssociatedBlock = associatedBlock;
			this.PlotSettings = plotSettings;
			this.MinLimit = new Vector2(-20.0, -7.5);
			this.MaxLimit = new Vector2(277.0, 202.5);
			this.BasePoint = Vector3.Zero;
			this.MinExtents = new Vector3(25.7, 19.5, 0.0);
			this.MaxExtents = new Vector3(231.3, 175.5, 0.0);
			this.Elevation = 0;
			this.UcsOrigin = Vector3.Zero;
			this.UcsXAxis = Vector3.UnitX;
			this.UcsYAxis = Vector3.UnitY;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the tab order.</summary>
		/// <remarks>
		/// This number is an ordinal indicating this layout's ordering in the tab control that is
		/// attached to the <b>AutoCAD</b> drawing frame window. Note that the "Model" tab always appears
		/// as the first tab regardless of its tab order (always zero).
		/// </remarks>
		public short TabOrder
		{
			get => this.tabOrder;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("The tab order index must be greater than zero.", nameof(value));
				}
				this.tabOrder = value;
			}
		}

		/// <summary>Gets or sets the plot settings</summary>
		public PlotSettings PlotSettings { get; set; }

		/// <summary>Gets or sets the minimum limits for this layout.</summary>
		public Vector2 MinLimit { get; set; }

		/// <summary>Gets or sets the maximum limits for this layout.</summary>
		public Vector2 MaxLimit { get; set; }

		/// <summary>Gets or sets the maximum extents for this layout.</summary>
		public Vector3 MinExtents { get; set; }

		/// <summary>Gets or sets the maximum extents for this layout.</summary>
		public Vector3 MaxExtents { get; set; }

		/// <summary>Gets or sets the insertion base point for this layout.</summary>
		public Vector3 BasePoint { get; set; }

		/// <summary>Gets or sets the elevation.</summary>
		public double Elevation { get; set; }

		/// <summary>Gets or sets the <b>UCS</b> origin.</summary>
		public Vector3 UcsOrigin { get; set; }

		/// <summary>Gets or sets the <b>UCS</b> X axis.</summary>
		public Vector3 UcsXAxis { get; set; }

		/// <summary>Gets or sets the <b>UCS</b> Y axis.</summary>
		public Vector3 UcsYAxis { get; set; }

		/// <summary>Defines if this layout is a paper space.</summary>
		public bool IsPaperSpace { get; }

		/// <summary>
		/// Gets the viewport associated with this layout. This is the viewport with Id 1 that represents the paper space itself,
		/// it has no graphical representation, and does not show the model.
		/// </summary>
		/// <remarks>The <see cref="ModelSpace"/> layout does not require a viewport and it will always return <see langword="null"/>.</remarks>
		public Viewport Viewport { get; internal set; }

		/// <summary>Gets the owner of the actual layout.</summary>
		public new Layouts Owner
		{
			get => (Layouts)base.Owner;
			internal set => base.Owner = value;
		}

		/// <summary>Gets the associated <see cref="ModelSpace"/> or PaperSp
		/// ace block.</summary>
		public Block AssociatedBlock { get; internal set; }

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override bool HasReferences() => this.Owner != null && this.Owner.HasReferences(this.Name);

		/// <inheritdoc/>
		public override List<DxfObjectReference> GetReferences() => this.Owner?.GetReferences(this.Name);

		/// <inheritdoc/>
		public override TableObject Clone(string newName)
		{
			if (!this.IsPaperSpace)
			{
				throw new NotSupportedException("The Model layout cannot be cloned.");
			}

			if (string.Equals(newName, ModelSpaceName, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("The layout name \"Model\" is reserved for the ModelSpace.");
			}

			Layout copy = new Layout(newName, null, (PlotSettings)this.PlotSettings.Clone())
			{
				TabOrder = this.tabOrder,
				MinLimit = this.MinLimit,
				MaxLimit = this.MaxLimit,
				BasePoint = this.BasePoint,
				MinExtents = this.MinExtents,
				MaxExtents = this.MaxExtents,
				Elevation = this.Elevation,
				UcsOrigin = this.UcsOrigin,
				UcsXAxis = this.UcsXAxis,
				UcsYAxis = this.UcsYAxis,
				Viewport = (Viewport)this.Viewport.Clone()
			};

			foreach (XData data in this.XData.Values)
			{
				copy.XData.Add((XData)data.Clone());
			}

			return copy;
		}
		/// <inheritdoc/>
		public override object Clone() => this.Clone(this.Name);

		/// <inheritdoc/>
		internal override long AssignHandle(long entityNumber)
		{
			entityNumber = this.Owner.AssignHandle(entityNumber);
			if (this.IsPaperSpace)
			{
				entityNumber = this.Viewport.AssignHandle(entityNumber);
			}
			return base.AssignHandle(entityNumber);
		}

		#endregion

		#region implements IComparable

		/// <inheritdoc/>
		public int CompareTo(Layout other)
		{
			if (other == null)
			{
				throw new ArgumentNullException(nameof(other));
			}

			return this.tabOrder.CompareTo(other.tabOrder);
		}

		#endregion
	}
}