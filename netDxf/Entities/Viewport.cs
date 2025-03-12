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
using netDxf.Collections;
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a view in paper space of the model.</summary>
	/// <remarks>
	/// The viewport with id equals 1 is the view of the paper space layout itself and it does not show the model.
	/// </remarks>
	public class Viewport :
		EntityObject
	{
		#region delegates and events

		public delegate void ClippingBoundaryAddedEventHandler(Viewport sender, EntityChangeEventArgs e);

		public event ClippingBoundaryAddedEventHandler ClippingBoundaryAdded;

		protected virtual void OnClippingBoundaryAddedEvent(EntityObject item)
		{
			ClippingBoundaryAddedEventHandler ae = this.ClippingBoundaryAdded;
			if (ae != null)
				ae(this, new EntityChangeEventArgs(item));
		}

		public delegate void ClippingBoundaryRemovedEventHandler(Viewport sender, EntityChangeEventArgs e);

		public event ClippingBoundaryRemovedEventHandler ClippingBoundaryRemoved;

		protected virtual void OnClippingBoundaryRemovedEvent(EntityObject item)
		{
			ClippingBoundaryRemovedEventHandler ae = this.ClippingBoundaryRemoved;
			if (ae != null)
				ae(this, new EntityChangeEventArgs(item));
		}

		#endregion

		#region private fields

		private short stacking;
		private EntityObject boundary;

		#endregion

		#region constructors

		/// <summary>Initializes a new viewport object.</summary>
		public Viewport()
			: this(2)
		{
			this.Status |= ViewportStatusFlags.GridMode;
		}

		public Viewport(Vector2 bottomLeftCorner, Vector2 topRightCorner)
			: this(2)
		{
			this.Center = new Vector3((topRightCorner.X + bottomLeftCorner.X) * 0.5, (topRightCorner.Y + bottomLeftCorner.Y) * 0.5, 0);
			this.Width = (topRightCorner.X - bottomLeftCorner.X) * 0.5;
			this.Height = (topRightCorner.Y - bottomLeftCorner.Y) * 0.5;
		}

		public Viewport(Vector2 center, double width, double height)
			: this(2)
		{
			this.Center = new Vector3(center.X, center.Y, 0.0);
			this.Width = width;
			this.Height = height;
		}

		public Viewport(EntityObject clippingBoundary)
			: this(2)
		{
			this.ClippingBoundary = clippingBoundary;
		}

		internal Viewport(short id)
			: base(EntityType.Viewport, DxfObjectCode.Viewport)
		{
			this.Center = Vector3.Zero;
			this.Width = 297;
			this.Height = 210;
			this.Stacking = id;
			this.Id = id;
			this.ViewCenter = Vector2.Zero;
			this.SnapBase = Vector2.Zero;
			this.SnapSpacing = new Vector2(10.0);
			this.GridSpacing = new Vector2(10.0);
			this.ViewDirection = Vector3.UnitZ;
			this.ViewTarget = Vector3.Zero;
			this.LensLength = 50.0;
			this.FrontClipPlane = 0.0;
			this.BackClipPlane = 0.0;
			this.ViewHeight = 250;
			this.SnapAngle = 0.0;
			this.TwistAngle = 0.0;
			this.CircleZoomPercent = 1000;
			this.Status = ViewportStatusFlags.AdaptiveGridDisplay |
							ViewportStatusFlags.DisplayGridBeyondDrawingLimits |
							ViewportStatusFlags.CurrentlyAlwaysEnabled |
							ViewportStatusFlags.UcsIconVisibility |
							ViewportStatusFlags.GridMode;
			this.FrozenLayers = new ObservableCollection<Layer>();
			this.FrozenLayers.BeforeAddItem += this.FrozenLayers_BeforeAddItem;
			this.UcsOrigin = Vector3.Zero;
			this.UcsXAxis = Vector3.UnitX;
			this.UcsYAxis = Vector3.UnitY;
			this.Elevation = 0.0;
			this.boundary = null;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the center point in paper space units.</summary>
		public Vector3 Center { get; set; }

		/// <summary>Gets or sets the width in paper space units.</summary>
		public double Width { get; set; }

		/// <summary>Gets or sets the height in paper space units.</summary>
		public double Height { get; set; }

		/// <summary>
		/// Viewport status field:<br />
		/// -1 = On, but is fully off screen, or is one of the viewports that is not active because the $MAXACTVP count is currently being exceeded.<br />
		/// 0 = Off<br />
		/// 1 = Stacking value reserved for the layout view.
		/// positive value = On and active. The value indicates the order of stacking for the viewports, where 1 is the active viewport, 2 is the next, and so forth.
		/// </summary>
		public short Stacking
		{
			get => this.stacking;
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException(nameof(value), "The stacking value must be greater than -1.");
				}
				this.stacking = value;
			}
		}

		/// <summary>Gets or sets the viewport ID.</summary>
		internal short Id { get; set; }

		/// <summary>Gets or sets the view center point (in DCS).</summary>
		public Vector2 ViewCenter { get; set; }

		/// <summary>Gets or sets the snap base point.</summary>
		public Vector2 SnapBase { get; set; }

		/// <summary>Gets or sets the snap spacing.</summary>
		public Vector2 SnapSpacing { get; set; }

		/// <summary>Gets or sets the grid spacing.</summary>
		public Vector2 GridSpacing { get; set; }

		/// <summary>Gets or sets the view direction vector (in WCS).</summary>
		public Vector3 ViewDirection { get; set; }

		/// <summary>Gets or sets the view target point (in WCS).</summary>
		public Vector3 ViewTarget { get; set; }

		/// <summary>Gets or sets the perspective lens length.</summary>
		public double LensLength { get; set; }

		/// <summary>Gets or sets the front clip plane Z value.</summary>
		public double FrontClipPlane { get; set; }

		/// <summary>Gets or sets the back clip plane Z value.</summary>
		public double BackClipPlane { get; set; }

		/// <summary>Gets or sets the view height (in model space units).</summary>
		public double ViewHeight { get; set; }

		/// <summary>Gets or sets the snap angle.</summary>
		public double SnapAngle { get; set; }

		/// <summary>Gets or sets the view twist angle.</summary>
		public double TwistAngle { get; set; }

		/// <summary>Gets or sets the circle zoom percent.</summary>
		public short CircleZoomPercent { get; set; }

		/// <summary>Gets the list of layers that are frozen in this viewport.</summary>
		/// <remarks>
		/// The FrozenLayers list cannot contain <see langword="null"/> items and layers that belong to different documents.
		/// Even if duplicate items should not cause any problems, it is not allowed to have two layers with the same name in the list.
		/// </remarks>
		public ObservableCollection<Layer> FrozenLayers { get; }

		/// <summary>Gets or sets the <see cref="ViewportStatusFlags">viewport status flags</see>:</summary>
		public ViewportStatusFlags Status { get; set; }

		/// <summary>Gets or sets the <b>UCS</b> origin.</summary>
		public Vector3 UcsOrigin { get; set; }

		/// <summary>Gets or sets the <b>UCS</b> X axis.</summary>
		public Vector3 UcsXAxis { get; set; }

		/// <summary>Gets or sets the <b>UCS</b> Y axis.</summary>
		public Vector3 UcsYAxis { get; set; }

		/// <summary>Gets or sets the elevation.</summary>
		public double Elevation { get; set; }

		/// <summary>Entity that serves as the viewport clipping boundary (only present if viewport is non-rectangular).</summary>
		/// <remarks>
		/// <b>AutoCAD</b> does not allow the creation of viewports from open shapes such as LwPolylines, Polylines, or ellipse arcs;
		/// but if they are edited afterward, making them open, it will not complain, and they will work without problems.
		/// So, it is possible to use open shapes as clipping boundaries, even if it is not recommended.
		/// It might not be supported by all programs that read <b>DXF</b> files and a redraw of the layout might be required to show them correctly inside AutoCad.<br />
		/// Only X and Y coordinates will be used the entity normal will be considered as UnitZ.<br />
		/// When the viewport is added to the document this entity will be added too.
		/// </remarks>
		public EntityObject ClippingBoundary
		{
			get => this.boundary;
			set
			{
				if (value != null)
				{
					BoundingRectangle abbr;
					switch (value.Type)
					{
						case EntityType.Circle:
							Circle circle = (Circle)value;
							abbr = new BoundingRectangle(new Vector2(circle.Center.X, circle.Center.Y), circle.Radius);
							break;
						case EntityType.Ellipse:
							Ellipse ellipse = (Ellipse)value;
							abbr = new BoundingRectangle(new Vector2(ellipse.Center.X, ellipse.Center.Y), ellipse.MajorAxis, ellipse.MinorAxis, ellipse.Rotation);
							break;
						case EntityType.Polyline2D:
							Polyline2D poly2D = (Polyline2D)value;
							abbr = new BoundingRectangle(poly2D.PolygonalVertexes(6, MathHelper.Epsilon, MathHelper.Epsilon));
							break;
						case EntityType.Polyline3D:
							Polyline3D poly3D = (Polyline3D)value;
							List<Vector2> pPoints = new List<Vector2>();
							foreach (Vector3 point in poly3D.Vertexes)
							{
								pPoints.Add(new Vector2(point.X, point.Y));
							}
							abbr = new BoundingRectangle(pPoints);
							break;
						case EntityType.Spline:
							Spline spline = (Spline)value;
							List<Vector2> sPoints = new List<Vector2>();
							foreach (Vector3 point in spline.ControlPoints)
							{
								sPoints.Add(new Vector2(point.X, point.Y));
							}
							abbr = new BoundingRectangle(sPoints);
							break;
						default:
							throw new ArgumentException("Only lightweight polylines, polylines, circles, ellipses and splines are allowed as a viewport clipping boundary.");
					}

					this.Width = abbr.Width;
					this.Height = abbr.Height;
					this.Center = new Vector3(abbr.Center.X, abbr.Center.Y, 0.0);
					this.Status |= ViewportStatusFlags.NonRectangularClipping;
				}
				else
				{
					this.Status &= ~ViewportStatusFlags.NonRectangularClipping;
				}

				// nothing else to do if it is the same
				if (ReferenceEquals(this.boundary, value))
					return;

				// remove the previous clipping boundary
				if (this.boundary != null)
				{
					this.boundary.RemoveReactor(this);
					this.OnClippingBoundaryRemovedEvent(this.boundary);
				}

				// add the new clipping boundary
				if (value != null)
				{
					value.AddReactor(this);
					this.OnClippingBoundaryAddedEvent(value);
				}

				this.boundary = value;
			}
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}
			this.Normal = newNormal;

			EntityObject clippingEntity = this.ClippingBoundary;
			if (clippingEntity == null)
			{
				if (transformation.IsIdentity)
				{
					this.Center += translation;
					return;
				}

				// when a view port is transformed a Polyline2D will be generated
				List<Polyline2DVertex> vertexes = new List<Polyline2DVertex>
				{
					new Polyline2DVertex(this.Center.X - this.Width * 0.5, this.Center.Y - this.Height * 0.5),
					new Polyline2DVertex(this.Center.X + this.Width * 0.5, this.Center.Y - this.Height * 0.5),
					new Polyline2DVertex(this.Center.X + this.Width * 0.5, this.Center.Y + this.Height * 0.5),
					new Polyline2DVertex(this.Center.X - this.Width * 0.5, this.Center.Y + this.Height * 0.5)
				};
				clippingEntity = new Polyline2D(vertexes, true);
			}

			clippingEntity.TransformBy(transformation, translation);
			this.ClippingBoundary = clippingEntity;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Viewport viewport = new Viewport
			{
				//EntityObject properties
				Layer = (Layer)this.Layer.Clone(),
				Linetype = (Linetype)this.Linetype.Clone(),
				Color = (AciColor)this.Color.Clone(),
				Lineweight = this.Lineweight,
				Transparency = (Transparency)this.Transparency.Clone(),
				LinetypeScale = this.LinetypeScale,
				Normal = this.Normal,
				IsVisible = this.IsVisible,
				//viewport properties
				ClippingBoundary = (EntityObject)this.boundary?.Clone(),
				Center = this.Center,
				Width = this.Width,
				Height = this.Height,
				Stacking = this.Stacking,
				Id = this.Id,
				ViewCenter = this.ViewCenter,
				SnapBase = this.SnapBase,
				SnapSpacing = this.SnapSpacing,
				GridSpacing = this.GridSpacing,
				ViewDirection = this.ViewDirection,
				ViewTarget = this.ViewTarget,
				LensLength = this.LensLength,
				FrontClipPlane = this.FrontClipPlane,
				BackClipPlane = this.BackClipPlane,
				ViewHeight = this.ViewHeight,
				SnapAngle = this.SnapAngle,
				TwistAngle = this.TwistAngle,
				CircleZoomPercent = this.CircleZoomPercent,
				Status = this.Status,
				UcsOrigin = this.UcsOrigin,
				UcsXAxis = this.UcsXAxis,
				UcsYAxis = this.UcsYAxis,
				Elevation = this.Elevation
			};

			foreach (Layer layer in this.FrozenLayers)
				viewport.FrozenLayers.Add((Layer)layer.Clone());

			foreach (XData data in this.XData.Values)
				viewport.XData.Add((XData)data.Clone());

			return viewport;
		}

		#endregion

		#region FrozenLayers events

		private void FrozenLayers_BeforeAddItem(ObservableCollection<Layer> sender, ObservableCollectionEventArgs<Layer> e)
		{
			if (e.Item == null)
			{
				// the frozen layer list cannot contain null items
				e.Cancel = true;
			}
			else if (this.Owner != null && e.Item.Owner == null)
			{
				// the frozen layer and the viewport must belong to the same document
				e.Cancel = true;
			}
			else if (this.Owner == null && e.Item.Owner != null)
			{
				// the frozen layer and the viewport must belong to the same document
				e.Cancel = true;
			}
			else if (this.Owner != null && e.Item.Owner != null)
			{
				// the frozen layer and the viewport must belong to the same document
				if (!ReferenceEquals(this.Owner.Owner.Owner.Owner, e.Item.Owner.Owner))
				{
					e.Cancel = true;
				}
			}
			else if (this.FrozenLayers.Contains(e.Item))
			{
				// the frozen layer list cannot contain duplicates
				e.Cancel = true;
			}
		}

		#endregion
	}
}