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
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a two dimensional polyline <see cref="EntityObject">entity</see>.</summary>
	/// <remarks>
	/// Two dimensional polylines can hold information about the width of the lines and arcs that compose them.
	/// </remarks>
	public class Polyline2D :
		EntityObject
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public Polyline2D()
			: this(new List<Polyline2DVertex>())
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">Polyline2D <see cref="Vector2">vertex</see> list in object coordinates.</param>
		public Polyline2D(IEnumerable<Vector2> vertexes)
			: this(vertexes, false)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">Polyline2D <see cref="Vector2">vertex</see> list in object coordinates.</param>
		/// <param name="isClosed">Sets if the polyline is closed, by default it will create an open polyline.</param>
		public Polyline2D(IEnumerable<Vector2> vertexes, bool isClosed)
			: base(EntityType.Polyline2D, DxfObjectCode.LwPolyline)
		{
			if (vertexes == null)
			{
				throw new ArgumentNullException(nameof(vertexes));
			}

			this.Vertexes = new List<Polyline2DVertex>();
			foreach (Vector2 vertex in vertexes)
			{
				this.Vertexes.Add(new Polyline2DVertex(vertex));
			}

			this.Flags = isClosed ? PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM : PolylineTypeFlags.OpenPolyline;
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">Polyline2D <see cref="Polyline2DVertex">vertex</see> list in object coordinates.</param>
		public Polyline2D(IEnumerable<Polyline2DVertex> vertexes)
			: this(vertexes, false)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">Polyline2D <see cref="Polyline2DVertex">vertex</see> list in object coordinates.</param>
		/// <param name="isClosed">Sets if the polyline is closed (default: <see langword="false"/>).</param>
		public Polyline2D(IEnumerable<Polyline2DVertex> vertexes, bool isClosed)
			: base(EntityType.Polyline2D, DxfObjectCode.LwPolyline)
		{
			if (vertexes == null)
			{
				throw new ArgumentNullException(nameof(vertexes));
			}

			this.Vertexes = new List<Polyline2DVertex>(vertexes);
			this.Flags = isClosed ? PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM : PolylineTypeFlags.OpenPolyline;
		}

		#endregion

		#region public properties

		private static short _DefaultSplineSegs = 8;
		/// <summary>Gets or sets if the default SplineSegs value.</summary>
		/// <remarks>
		/// This value is used by the Explode method when the current <see cref="Polyline2D"/> does not belong to a <b>DXF</b> document.
		/// </remarks>
		public static short DefaultSplineSegs
		{
			get => _DefaultSplineSegs;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Values must be greater than 0.");
				}
				_DefaultSplineSegs = value;
			}
		}

		/// <summary>Gets or sets the polyline <see cref="Polyline2DVertex">vertex</see> list.</summary>
		public List<Polyline2DVertex> Vertexes { get; }

		/// <summary>Gets or sets if the polyline is closed.</summary>
		public bool IsClosed
		{
			get => this.Flags.HasFlag(PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM);
			set
			{
				if (value)
				{
					this.Flags |= PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM;
				}
				else
				{
					this.Flags &= ~PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM;
				}
			}
		}

		/// <summary>Gets or sets the polyline thickness.</summary>
		public double Thickness { get; set; } = 0.0;

		/// <summary>Gets or sets the polyline elevation.</summary>
		/// <remarks>This is the distance from the origin to the plane of the light weight polyline.</remarks>
		public double Elevation { get; set; } = 0.0;

		/// <summary>Enable or disable if the linetype pattern is generated continuously around the vertexes of the polyline.</summary>
		public bool LinetypeGeneration
		{
			get => this.Flags.HasFlag(PolylineTypeFlags.ContinuousLinetypePattern);
			set
			{
				if (value)
				{
					this.Flags |= PolylineTypeFlags.ContinuousLinetypePattern;
				}
				else
				{
					this.Flags &= ~PolylineTypeFlags.ContinuousLinetypePattern;
				}
			}
		}

		private PolylineSmoothType _SmoothType = PolylineSmoothType.NoSmooth;
		/// <summary>Gets or sets the polyline smooth type.</summary>
		/// <remarks>
		/// The additional polyline vertexes corresponding to the SplineFit will be created when writing the <b>DXF</b> file.
		/// </remarks>
		public PolylineSmoothType SmoothType
		{
			get => _SmoothType;
			set
			{
				if (value == PolylineSmoothType.NoSmooth)
				{
					this.CodeName = DxfObjectCode.LwPolyline;
					this.Flags &= ~PolylineTypeFlags.SplineFit;
				}
				else
				{
					this.CodeName = DxfObjectCode.Polyline;
					this.Flags |= PolylineTypeFlags.SplineFit;
				}
				_SmoothType = value;
			}
		}

		#endregion

		#region internal properties

		/// <summary>Gets the polyline flags.</summary>
		internal PolylineTypeFlags Flags { get; set; }

		#endregion

		#region public methods

		/// <summary>Switch the polyline direction.</summary>
		public void Reverse()
		{
			if (this.Vertexes.Count < 2)
			{
				return;
			}

			this.Vertexes.Reverse();

			double firstBulge = this.Vertexes[0].Bulge;

			for (int i = 0; i < this.Vertexes.Count - 1; i++)
			{
				this.Vertexes[i].Bulge = -this.Vertexes[i + 1].Bulge;
			}

			this.Vertexes[this.Vertexes.Count - 1].Bulge = -firstBulge;
		}

		/// <summary>Sets a constant width for all the polyline segments.</summary>
		/// <param name="width">Polyline width.</param>
		/// <remarks>
		/// Smoothed polylines can only have a constant width, the start width of the first vertex will be used.
		/// </remarks>
		public void SetConstantWidth(double width)
		{
			foreach (Polyline2DVertex v in this.Vertexes)
			{
				v.StartWidth = width;
				v.EndWidth = width;
			}
		}

		/// <summary>Decompose the actual polyline in its internal entities, <see cref="Line">lines</see> and <see cref="Arc">arcs</see>.</summary>
		/// <returns>A list of <see cref="Line">lines</see> and <see cref="Arc">arcs</see> that made up the polyline.</returns>
		public List<EntityObject> Explode()
		{
			List<EntityObject> entities = new List<EntityObject>();

			if (_SmoothType == PolylineSmoothType.NoSmooth)
			{
				int index = 0;
				foreach (Polyline2DVertex vertex in this.Vertexes)
				{
					double bulge = vertex.Bulge;
					Vector2 p1;
					Vector2 p2;

					if (index == this.Vertexes.Count - 1)
					{
						if (!this.IsClosed)
						{
							break;
						}
						p1 = new Vector2(vertex.Position.X, vertex.Position.Y);
						p2 = new Vector2(this.Vertexes[0].Position.X, this.Vertexes[0].Position.Y);
					}
					else
					{
						p1 = new Vector2(vertex.Position.X, vertex.Position.Y);
						p2 = new Vector2(this.Vertexes[index + 1].Position.X, this.Vertexes[index + 1].Position.Y);
					}

					if (MathHelper.IsZero(bulge))
					{
						// the polyline edge is a line
						Vector3 start = MathHelper.Transform(new Vector3(p1.X, p1.Y, this.Elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
						Vector3 end = MathHelper.Transform(new Vector3(p2.X, p2.Y, this.Elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);

						entities.Add(new Line
						{
							Layer = (Layer)this.Layer.Clone(),
							Linetype = (Linetype)this.Linetype.Clone(),
							Color = (AciColor)this.Color.Clone(),
							Lineweight = this.Lineweight,
							Transparency = (Transparency)this.Transparency.Clone(),
							LinetypeScale = this.LinetypeScale,
							Normal = this.Normal,
							StartPoint = start,
							EndPoint = end,
							Thickness = this.Thickness
						});
					}
					else
					{
						// the polyline edge is an arc
						var (center, radius, startAngle, endAngle) = MathHelper.ArcFromBulge(p1, p2, bulge);

						// avoid arcs with very small radius, draw a line instead
						if (MathHelper.IsZero(radius))
						{
							// the polyline edge is a line
							List<Vector3> points = MathHelper.Transform(
								new[]
								{
									new Vector3(p1.X, p1.Y, this.Elevation),
									new Vector3(p2.X, p2.Y, this.Elevation)
								},
								this.Normal,
								CoordinateSystem.Object, CoordinateSystem.World);

							entities.Add(new Line
							{
								Layer = (Layer)this.Layer.Clone(),
								Linetype = (Linetype)this.Linetype.Clone(),
								Color = (AciColor)this.Color.Clone(),
								Lineweight = this.Lineweight,
								Transparency = (Transparency)this.Transparency.Clone(),
								LinetypeScale = this.LinetypeScale,
								Normal = this.Normal,
								StartPoint = points[0],
								EndPoint = points[1],
								Thickness = this.Thickness,
							});
						}
						else
						{
							Vector3 point = MathHelper.Transform(
								new Vector3(center.X, center.Y, this.Elevation),
								this.Normal,
								CoordinateSystem.Object,
								CoordinateSystem.World);

							entities.Add(new Arc
							{
								Layer = (Layer)this.Layer.Clone(),
								Linetype = (Linetype)this.Linetype.Clone(),
								Color = (AciColor)this.Color.Clone(),
								Lineweight = this.Lineweight,
								Transparency = (Transparency)this.Transparency.Clone(),
								LinetypeScale = this.LinetypeScale,
								Normal = this.Normal,
								Center = point,
								Radius = radius,
								StartAngle = startAngle,
								EndAngle = endAngle,
								Thickness = this.Thickness,
							});
						}
					}
					index++;
				}
				return entities;
			}

			Vector3[] wcsVertexes = new Vector3[this.Vertexes.Count];
			Matrix3 trans = MathHelper.ArbitraryAxis(this.Normal);
			for (int i = 0; i < this.Vertexes.Count; i++)
			{
				Vector3 wcsVertex = trans * new Vector3(this.Vertexes[i].Position.X, this.Vertexes[i].Position.Y, this.Elevation);
				wcsVertexes[i] = wcsVertex;
			}

			int degree = _SmoothType == PolylineSmoothType.Quadratic ? 2 : 3;
			int splineSegs = this.Owner == null ? DefaultSplineSegs : this.Owner.Record.Owner.Owner.DrawingVariables.SplineSegs;
			int precision = this.IsClosed ? splineSegs * this.Vertexes.Count : splineSegs * (this.Vertexes.Count - 1);
			List<Vector3> splinePoints = Spline.NurbsEvaluator(wcsVertexes, null, null, degree, false, this.IsClosed, precision);

			for (int i = 1; i < splinePoints.Count; i++)
			{
				Vector3 start = splinePoints[i - 1];
				Vector3 end = splinePoints[i];
				entities.Add(new Line
				{
					Layer = (Layer)this.Layer.Clone(),
					Linetype = (Linetype)this.Linetype.Clone(),
					Color = (AciColor)this.Color.Clone(),
					Lineweight = this.Lineweight,
					Transparency = (Transparency)this.Transparency.Clone(),
					LinetypeScale = this.LinetypeScale,
					Normal = this.Normal,
					StartPoint = start,
					EndPoint = end,
					Thickness = this.Thickness
				});
			}

			if (this.IsClosed)
			{
				entities.Add(new Line
				{
					Layer = (Layer)this.Layer.Clone(),
					Linetype = (Linetype)this.Linetype.Clone(),
					Color = (AciColor)this.Color.Clone(),
					Lineweight = this.Lineweight,
					Transparency = (Transparency)this.Transparency.Clone(),
					LinetypeScale = this.LinetypeScale,
					Normal = this.Normal,
					StartPoint = splinePoints[splinePoints.Count - 1],
					EndPoint = splinePoints[0],
					Thickness = this.Thickness
				});
			}

			return entities;
		}

		/// <summary>Obtains a list of vertexes that represent the polyline approximating the curve segments as necessary.</summary>
		/// <param name="precision">The number of vertexes created for curve segments.</param>
		/// <returns>A list of vertexes expressed in object coordinate system.</returns>
		/// <remarks>
		/// For polylines containing arc segments the precision value defines the number of divisions for a full circle,
		/// therefore, the final number of divisions for the arc will depend on the angle of the arc.<br />
		/// For vertexes with bulge values different than zero a precision of zero means that no approximation will be made.<br />
		/// For smoothed polylines the minimum number of vertexes generated is 2.
		/// </remarks>
		public List<Vector2> PolygonalVertexes(int precision)
			=> this.PolygonalVertexes(precision, MathHelper.Epsilon, MathHelper.Epsilon);

		/// <summary>Obtains a list of vertexes that represent the polyline approximating the curve segments as necessary.</summary>
		/// <param name="precision">The number of vertexes created for curve segments.</param>
		/// <param name="weldThreshold">Tolerance to consider if two new generated vertexes are equal.</param>
		/// <param name="bulgeThreshold">Minimum distance from which approximate curved segments of the polyline.</param>
		/// <returns>A list of vertexes expressed in object coordinate system.</returns>
		/// <remarks>
		/// For polylines containing arc segments the precision value defines the number of divisions for a full circle,
		/// therefore, the final number of divisions for the arc will depend on the angle of the arc.<br />
		/// For vertexes with bulge values different than zero a precision of zero means that no approximation will be made.<br />
		/// For smoothed polylines the minimum number of vertexes generated is 2.
		/// </remarks>
		public List<Vector2> PolygonalVertexes(int precision, double weldThreshold, double bulgeThreshold)
		{
			if (precision < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(precision), precision, "The bulge precision must be equal or greater than zero.");
			}

			List<Vector2> ocsVertexes = new List<Vector2>();
			int degree;
			if (_SmoothType == PolylineSmoothType.Quadratic)
			{
				degree = 2;
			}
			else if (_SmoothType == PolylineSmoothType.Cubic)
			{
				degree = 3;
			}
			else
			{
				int index = 0;

				foreach (Polyline2DVertex vertex in this.Vertexes)
				{
					double bulge = vertex.Bulge;
					Vector2 p1;
					Vector2 p2;

					if (index == this.Vertexes.Count - 1)
					{
						p1 = new Vector2(vertex.Position.X, vertex.Position.Y);
						if (!this.IsClosed)
						{
							ocsVertexes.Add(p1);
							continue;
						}
						p2 = new Vector2(this.Vertexes[0].Position.X, this.Vertexes[0].Position.Y);
					}
					else
					{
						p1 = new Vector2(vertex.Position.X, vertex.Position.Y);
						p2 = new Vector2(this.Vertexes[index + 1].Position.X, this.Vertexes[index + 1].Position.Y);
					}

					if (!p1.Equals(p2, weldThreshold))
					{
						if (MathHelper.IsZero(bulge) || precision == 0)
						{
							ocsVertexes.Add(p1);
						}
						else
						{
							double dist = 0.5 * Vector2.Distance(p1, p2);
							var (center, radius, startAngle, endAngle) = MathHelper.ArcFromBulge(p1, p2, bulge);

							if (dist >= bulgeThreshold || !MathHelper.IsZero(radius))
							{
								double arcAngle = MathHelper.NormalizeAngle(endAngle - startAngle) * MathHelper.DegToRad;
								int arcPrecision = (int)(precision * arcAngle / MathHelper.TwoPI);
								double angle = Math.Sign(bulge) * arcAngle / (arcPrecision + 1);
								ocsVertexes.Add(p1);
								Vector2 prevCurvePoint = p1;
								Vector2 startDir = p1 - center;
								for (int i = 1; i <= arcPrecision; i++)
								{
									Vector2 curvePoint = center + Vector2.Rotate(startDir, i * angle);
									if (!curvePoint.Equals(prevCurvePoint, weldThreshold) && !curvePoint.Equals(p2, weldThreshold))
									{
										ocsVertexes.Add(curvePoint);
										prevCurvePoint = curvePoint;
									}
								}
							}
							else
							{
								ocsVertexes.Add(p1);
							}
						}
					}
					index++;
				}

				return ocsVertexes;
			}

			// the minimum number of vertexes generated for smoothed polylines is 2
			if (precision < 2)
			{
				precision = 2;
			}

			Vector3[] ctrlPoints = new Vector3[this.Vertexes.Count];
			for (int i = 0; i < this.Vertexes.Count; i++)
			{
				Vector2 position = this.Vertexes[i].Position;
				ctrlPoints[i] = new Vector3(position.X, position.Y, 0.0);
			}

			// closed polylines will be considered as closed and periodic
			List<Vector3> points = Spline.NurbsEvaluator(ctrlPoints, null, null, degree, false, this.IsClosed, precision);
			foreach (Vector3 point in points)
			{
				ocsVertexes.Add(new Vector2(point.X, point.Y));
			}

			return ocsVertexes;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			double newElevation = this.Elevation;
			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}

			Matrix3 transOW = MathHelper.ArbitraryAxis(this.Normal);
			Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal).Transpose();

			foreach (Polyline2DVertex vertex in this.Vertexes)
			{
				Vector3 v = transOW * new Vector3(vertex.Position.X, vertex.Position.Y, this.Elevation);
				v = transformation * v + translation;
				v = transWO * v;
				vertex.Position = new Vector2(v.X, v.Y);
				newElevation = v.Z;
			}
			this.Elevation = newElevation;
			this.Normal = newNormal;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Polyline2D entity = new Polyline2D
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
				//LwPolyline properties
				Elevation = this.Elevation,
				Thickness = this.Thickness,
				Flags = this.Flags
			};

			foreach (Polyline2DVertex vertex in this.Vertexes)
			{
				entity.Vertexes.Add((Polyline2DVertex)vertex.Clone());
			}

			foreach (XData data in this.XData.Values)
			{
				entity.XData.Add((XData)data.Clone());
			}

			return entity;
		}

		#endregion
	}
}