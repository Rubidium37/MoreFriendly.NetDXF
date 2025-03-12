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
using System.Linq;

namespace netDxf.Entities
{
	/// <summary>Represent a loop of a <see cref="Hatch">hatch</see> boundaries.</summary>
	/// <remarks>
	/// The entities that make a loop can be any combination of lines, light weight polylines, polylines, circles, arcs, ellipses, and splines.<br />
	/// The entities that define a loop should define a closed path, they have to be on the same plane as the hatch, and with the same normal;
	/// if these conditions are not met the result might be unpredictable. <br />
	/// Entities expressed in 3d coordinates like lines, polylines, and splines will be projected into its local plane.
	/// All edges that make a <see cref="HatchBoundaryPath"/> are 2d objects, only have X and Y coordinates.
	/// This is why to avoid unexpected results, it is important that all entities that define the paths have the same normal, same reference plane.
	/// </remarks>
	public class HatchBoundaryPath :
		ICloneable
	{
		#region Hatch boundary path edge classes

		/// <summary>Specifies the type of HatchBoundaryPath.Edge.</summary>
		public enum EdgeType
		{
			Polyline = 0,
			Line = 1,
			Arc = 2,
			Ellipse = 3,
			Spline = 4
		}

		/// <summary>Base class for all types of <see cref="HatchBoundaryPath"/> edges.</summary>
		public abstract class Edge :
			ICloneable
		{
			/// <summary>Gets the <see cref="HatchBoundaryPath"/> edge type</summary>
			public EdgeType Type { get; }

			protected Edge(EdgeType type)
			{
				this.Type = type;
			}

			/// <summary>Converts the actual edge to its entity equivalent.</summary>
			/// <returns>An <see cref="EntityObject"/> equivalent to the actual edge.</returns>
			public abstract EntityObject ConvertTo();

			/// <inheritdoc/>
			public abstract object Clone();
		}

		/// <summary>Represents a polyline edge of a HatchBoundaryPath.</summary>
		public class Polyline :
			Edge
		{
			/// <summary>Gets or sets the list of polyline vertexes.</summary>
			/// <remarks>
			/// The position of the vertex is defined by the X and Y coordinates, the Z value represents the bulge at that vertex.
			/// </remarks>
			public Vector3[] Vertexes;

			/// <summary>Gets if the polyline is closed.</summary>
			public bool IsClosed;

			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Polyline"/> class.</summary>
			public Polyline()
				: base(EdgeType.Polyline)
			{
			}
			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Polyline"/> class.</summary>
			/// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
			public Polyline(EntityObject entity)
				: base(EdgeType.Polyline)
			{
				if (entity == null)
				{
					throw new ArgumentNullException(nameof(entity));
				}

				if (entity.Type == EntityType.Polyline2D)
				{
					Entities.Polyline2D poly = (Entities.Polyline2D)entity;
					this.IsClosed = poly.IsClosed;
					this.Vertexes = new Vector3[poly.Vertexes.Count];
					for (int i = 0; i < poly.Vertexes.Count; i++)
					{
						this.Vertexes[i] = new Vector3(poly.Vertexes[i].Position.X, poly.Vertexes[i].Position.Y, poly.Vertexes[i].Bulge);
					}
				}
				else if (entity.Type == EntityType.Polyline3D)
				{
					Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();

					Entities.Polyline3D poly = (Entities.Polyline3D)entity;
					this.IsClosed = poly.IsClosed;
					this.Vertexes = new Vector3[poly.Vertexes.Count];
					for (int i = 0; i < poly.Vertexes.Count; i++)
					{
						Vector3 point = trans * poly.Vertexes[i];
						this.Vertexes[i] = new Vector3(point.X, point.Y, 0.0);
					}
				}
				else
					throw new ArgumentException("The entity is not a Polyline2D or a Polyline3D", nameof(entity));
			}

			/// <summary>Decompose the actual polyline in its internal entities, <see cref="HatchBoundaryPath.Line">lines</see> and <see cref="HatchBoundaryPath.Arc">arcs</see>.</summary>
			/// <returns>A list of <see cref="HatchBoundaryPath.Line">lines</see> and <see cref="HatchBoundaryPath.Arc">arcs</see> that made up the polyline.</returns>
			public List<HatchBoundaryPath.Edge> Explode()
			{
				List<HatchBoundaryPath.Edge> edges = new List<HatchBoundaryPath.Edge>();

				int index = 0;
				foreach (Vector3 vertex in this.Vertexes)
				{
					double bulge = vertex.Z;
					Vector2 p1;
					Vector2 p2;

					if (index == this.Vertexes.Length - 1)
					{
						if (!this.IsClosed)
						{
							break;
						}

						p1 = new Vector2(vertex.X, vertex.Y);
						p2 = new Vector2(this.Vertexes[0].X, this.Vertexes[0].Y);
					}
					else
					{
						p1 = new Vector2(vertex.X, vertex.Y);
						p2 = new Vector2(this.Vertexes[index + 1].X, this.Vertexes[index + 1].Y);
					}

					if (MathHelper.IsZero(bulge))
					{
						// the polyline edge is a line
						HatchBoundaryPath.Line line = new Line
						{
							Start = p1,
							End = p2
						};
						edges.Add(line);
					}
					else
					{
						// the polyline edge is an arc
						Tuple<Vector2, double, double, double> arcData = MathHelper.ArcFromBulge(p1, p2, bulge);
						Vector2 center = arcData.Item1;
						double radius = arcData.Item2;
						double startAngle = arcData.Item3;
						double endAngle = arcData.Item4;

						// avoid arcs with very small radius, draw a line instead
						if (MathHelper.IsZero(radius))
						{
							// the polyline edge is a line
							HatchBoundaryPath.Line line = new Line
							{
								Start = p1,
								End = p2
							};
							edges.Add(line);
						}
						else
						{
							HatchBoundaryPath.Arc arc = new HatchBoundaryPath.Arc
							{
								Center = center,
								Radius = radius,
								StartAngle = startAngle,
								EndAngle = endAngle,
							};
							edges.Add(arc);
						}
					}

					index++;
				}
				return edges;
			}

			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Polyline"/> class.</summary>
			/// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
			public static Polyline ConvertFrom(EntityObject entity) => new Polyline(entity);

			/// <inheritdoc/>
			public override EntityObject ConvertTo()
			{
				List<Polyline2DVertex> points = new List<Polyline2DVertex>(this.Vertexes.Length);
				foreach (Vector3 point in this.Vertexes)
				{
					points.Add(new Polyline2DVertex(point.X, point.Y, point.Z));
				}
				return new Entities.Polyline2D(points, this.IsClosed);
			}

			/// <inheritdoc/>
			public override object Clone()
			{
				Polyline copy = new Polyline
				{
					Vertexes = new Vector3[this.Vertexes.Length]
				};

				for (int i = 0; i < this.Vertexes.Length; i++)
				{
					copy.Vertexes[i] = this.Vertexes[i];
				}
				return copy;
			}
		}

		/// <summary>Represents a line edge of a HatchBoundaryPath.</summary>
		public class Line :
			Edge
		{
			/// <summary>Gets or sets the start point of the line.</summary>
			public Vector2 Start;

			/// <summary>Gets or sets the end point of the line.</summary>
			public Vector2 End;

			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Line"/> class.</summary>
			public Line()
				: base(EdgeType.Line)
			{
			}
			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Line"/> class.</summary>
			/// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
			public Line(EntityObject entity)
				: base(EdgeType.Line)
			{
				if (entity == null)
					throw new ArgumentNullException(nameof(entity));

				Entities.Line line = entity as Entities.Line;
				if (line == null)
					throw new ArgumentException("The entity is not a Line", nameof(entity));

				Vector3 point;
				Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();

				point = trans * line.StartPoint;
				this.Start = new Vector2(point.X, point.Y);
				point = trans * line.EndPoint;
				this.End = new Vector2(point.X, point.Y);
			}

			/// <summary>Creates a BoundaryBoundaryPath from an <see cref="EntityObject">entity</see>.</summary>
			/// <param name="entity">An <see cref="EntityObject">entity</see>.</param>
			/// <returns>A <see cref="HatchBoundaryPath"/> line.</returns>
			public static Line ConvertFrom(EntityObject entity) => new Line(entity);

			/// <inheritdoc/>
			public override EntityObject ConvertTo() => new Entities.Line(this.Start, this.End);

			/// <inheritdoc/>
			public override object Clone()
			{
				Line copy = new Line
				{
					Start = this.Start,
					End = this.End
				};

				return copy;
			}
		}

		/// <summary>Represents an arc edge of a HatchBoundaryPath.</summary>
		public class Arc :
			Edge
		{
			/// <summary>Gets or set the center of the arc.</summary>
			public Vector2 Center;

			/// <summary>Gets or sets the radius of the arc.</summary>
			public double Radius;

			/// <summary>Gets or sets the start angle of the arc.</summary>
			public double StartAngle;

			/// <summary>Gets or sets the end angle of the arc.</summary>
			public double EndAngle;

			/// <summary>Gets or sets if the arc is counter clockwise.</summary>
			public bool IsCounterclockwise;

			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Arc"/> class.</summary>
			public Arc()
				: base(EdgeType.Arc)
			{
			}
			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Arc"/> class.</summary>
			/// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
			public Arc(EntityObject entity)
				: base(EdgeType.Arc)
			{
				if (entity == null)
					throw new ArgumentNullException(nameof(entity));
				Vector3 point;
				Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();
				switch (entity.Type)
				{
					case EntityType.Arc:
						Entities.Arc arc = (Entities.Arc)entity;
						point = trans * arc.Center;
						this.Center = new Vector2(point.X, point.Y);
						this.Radius = arc.Radius;
						this.StartAngle = arc.StartAngle;
						this.EndAngle = arc.EndAngle;
						this.IsCounterclockwise = true;
						break;
					case EntityType.Circle:
						Entities.Circle circle = (Circle)entity;
						point = trans * circle.Center;
						this.Center = new Vector2(point.X, point.Y);
						this.Radius = circle.Radius;
						this.StartAngle = 0.0;
						this.EndAngle = 360.0;
						this.IsCounterclockwise = true;
						break;
					default:
						throw new ArgumentException("The entity is not a Circle or an Arc", nameof(entity));
				}
			}

			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Arc"/> class.</summary>
			/// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
			public static Arc ConvertFrom(EntityObject entity) => new Arc(entity);

			/// <inheritdoc/>
			public override EntityObject ConvertTo()
			{
				if (MathHelper.IsEqual(MathHelper.NormalizeAngle(this.StartAngle), MathHelper.NormalizeAngle(this.EndAngle)))
				{
					return new Entities.Circle(this.Center, this.Radius);
				}

				if (this.IsCounterclockwise)
				{
					return new Entities.Arc(this.Center, this.Radius, this.StartAngle, this.EndAngle);
				}

				return new Entities.Arc(this.Center, this.Radius, 360 - this.EndAngle, 360 - this.StartAngle);
			}

			/// <inheritdoc/>
			public override object Clone()
			{
				Arc copy = new Arc
				{
					Center = this.Center,
					Radius = this.Radius,
					StartAngle = this.StartAngle,
					EndAngle = this.EndAngle,
					IsCounterclockwise = this.IsCounterclockwise
				};

				return copy;
			}
		}

		/// <summary>Represents a ellipse edge of a HatchBoundaryPath.</summary>
		public class Ellipse :
			Edge
		{
			/// <summary>Gets or sets the center of the ellipse.</summary>
			public Vector2 Center;

			/// <summary>Gets or sets the position of the end of the major axis.</summary>
			public Vector2 EndMajorAxis;

			/// <summary>Gets or sets the scale of the minor axis in respect of the major axis.</summary>
			public double MinorRatio;

			/// <summary>Gets or sets the start angle of the ellipse.</summary>
			public double StartAngle;

			/// <summary>Gets or sets the end angle of the ellipse.</summary>
			public double EndAngle;

			/// <summary>Gets or sets if the ellipse is counter clockwise.</summary>
			public bool IsCounterclockwise;

			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Ellipse"/> class.</summary>
			public Ellipse()
				: base(EdgeType.Ellipse)
			{
			}
			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Ellipse"/> class.</summary>
			/// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
			public Ellipse(EntityObject entity)
				: base(EdgeType.Ellipse)
			{
				if (entity == null)
					throw new ArgumentNullException(nameof(entity));

				Entities.Ellipse ellipse = entity as Entities.Ellipse;
				if (ellipse == null)
					throw new ArgumentException("The entity is not an Ellipse", nameof(entity));

				Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();

				Vector3 point = trans * ellipse.Center;
				this.Center = new Vector2(point.X, point.Y);

				double sine = 0.5 * ellipse.MajorAxis * Math.Sin(ellipse.Rotation * MathHelper.DegToRad);
				double cosine = 0.5 * ellipse.MajorAxis * Math.Cos(ellipse.Rotation * MathHelper.DegToRad);
				this.EndMajorAxis = new Vector2(cosine, sine);
				this.MinorRatio = ellipse.MinorAxis / ellipse.MajorAxis;
				if (ellipse.IsFullEllipse)
				{
					this.StartAngle = 0.0;
					this.EndAngle = 360.0;
				}
				else
				{
					this.StartAngle = ellipse.StartAngle;
					this.EndAngle = ellipse.EndAngle;
				}
				this.IsCounterclockwise = true;
			}

			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Ellipse"/> class.</summary>
			/// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
			public static Ellipse ConvertFrom(EntityObject entity) => new Ellipse(entity);

			/// <inheritdoc/>
			public override EntityObject ConvertTo()
			{
				Vector3 center = new Vector3(this.Center.X, this.Center.Y, 0.0);
				Vector3 axisPoint = new Vector3(this.EndMajorAxis.X, this.EndMajorAxis.Y, 0.0);
				Vector3 ocsAxisPoint = MathHelper.Transform(axisPoint,
					Vector3.UnitZ,
					CoordinateSystem.World,
					CoordinateSystem.Object);
				double rotation = Vector2.Angle(new Vector2(ocsAxisPoint.X, ocsAxisPoint.Y)) * MathHelper.RadToDeg;
				double majorAxis = 2 * axisPoint.Modulus();
				return new Entities.Ellipse(center, majorAxis, majorAxis * this.MinorRatio)
				{
					Rotation = rotation,
					StartAngle = this.IsCounterclockwise ? this.StartAngle : 360 - this.EndAngle,
					EndAngle = this.IsCounterclockwise ? this.EndAngle : 360 - this.StartAngle,
				};
			}

			/// <inheritdoc/>
			public override object Clone()
			{
				Ellipse copy = new Ellipse
				{
					Center = this.Center,
					EndMajorAxis = this.EndMajorAxis,
					MinorRatio = this.MinorRatio,
					StartAngle = this.StartAngle,
					EndAngle = this.EndAngle,
					IsCounterclockwise = this.IsCounterclockwise
				};

				return copy;
			}
		}

		/// <summary>Represents a spline edge of a HatchBoundaryPath.</summary>
		public class Spline :
			Edge
		{
			/// <summary>Gets or sets the degree of the spline</summary>
			public short Degree;

			/// <summary>Gets or sets if the spline is rational.</summary>
			public bool IsRational;

			/// <summary>Gets or sets if the spline is periodic.</summary>
			public bool IsPeriodic;

			/// <summary>Gets or sets the list of knots of the spline.</summary>
			public double[] Knots;

			/// <summary>Gets or sets the list of control points of the spline.</summary>
			/// <remarks>
			/// The position of the control point is defined by the X and Y coordinates, the Z value represents its weight.
			/// </remarks>
			public Vector3[] ControlPoints; // location: (x, y) weight: z

			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Spline"/> class.</summary>
			public Spline()
				: base(EdgeType.Spline)
			{
			}
			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Spline"/> class.</summary>
			/// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
			public Spline(EntityObject entity)
				: base(EdgeType.Spline)
			{
				if (entity == null)
				{
					throw new ArgumentNullException(nameof(entity));
				}

				Entities.Spline spline = entity as Entities.Spline;
				if (spline == null)
				{
					throw new ArgumentException("The entity is not an Spline", nameof(entity));
				}

				this.Degree = spline.Degree;
				this.IsRational = true;
				this.IsPeriodic = spline.IsClosedPeriodic;
				if (spline.ControlPoints.Length == 0)
				{
					throw new ArgumentException("The HatchBoundaryPath spline edge requires a spline entity with control points.", nameof(entity));
				}

				Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();

				this.ControlPoints = new Vector3[spline.ControlPoints.Length];
				for (int i = 0; i < spline.ControlPoints.Length; i++)
				{
					Vector3 point = trans * spline.ControlPoints[i];
					this.ControlPoints[i] = new Vector3(point.X, point.Y, spline.Weights[i]);
				}

				this.Knots = new double[spline.Knots.Length];
				for (int i = 0; i < spline.Knots.Length; i++)
				{
					this.Knots[i] = spline.Knots[i];
				}
			}

			/// <summary>Initializes a new instance of the <see cref="HatchBoundaryPath.Spline"/> class.</summary>
			/// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
			public static Spline ConvertFrom(EntityObject entity) => new Spline(entity);

			/// <inheritdoc/>
			public override EntityObject ConvertTo()
			{
				List<Vector3> ctrl = new List<Vector3>();
				List<double> weights = new List<double>();
				List<double> knots = new List<double>(this.Knots);

				foreach (Vector3 point in this.ControlPoints)
				{
					ctrl.Add(new Vector3(point.X, point.Y, 0.0));
					weights.Add(point.Z);
				}
				return new Entities.Spline(ctrl, weights, knots, this.Degree, this.IsPeriodic);
			}

			/// <inheritdoc/>
			public override object Clone()
			{
				Spline copy = new Spline
				{
					Degree = this.Degree,
					IsRational = this.IsRational,
					IsPeriodic = this.IsPeriodic,
					Knots = new double[this.Knots.Length],
					ControlPoints = new Vector3[this.ControlPoints.Length],
				};
				for (int i = 0; i < this.Knots.Length; i++)
				{
					copy.Knots[i] = this.Knots[i];
				}
				for (int i = 0; i < this.ControlPoints.Length; i++)
				{
					copy.ControlPoints[i] = this.ControlPoints[i];
				}
				return copy;
			}
		}

		#endregion

		#region private fields

		private readonly List<EntityObject> entities;
		private readonly List<Edge> edges = new List<Edge>();

		#endregion

		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="edges">List of entities that makes a loop for the hatch boundary paths.</param>
		public HatchBoundaryPath(IEnumerable<EntityObject> edges)
		{
			if (edges == null)
			{
				throw new ArgumentNullException(nameof(edges));
			}
			this.PathType = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;
			this.entities = new List<EntityObject>(edges);
			this.Update();
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="edges">List of edges that makes a loop for the hatch boundary paths.</param>
		public HatchBoundaryPath(IEnumerable<Edge> edges)
		{
			if (edges == null)
			{
				throw new ArgumentNullException(nameof(edges));
			}
			this.PathType = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;
			this.entities = new List<EntityObject>();
			foreach (Edge edge in edges)
			{
				if (edges.Count() == 1 && edge.Type == EdgeType.Polyline)
				{
					this.PathType |= HatchBoundaryPathTypeFlags.Polyline;
					this.edges.Add(edge);
				}
				else
				{
					if (edge.Type == EdgeType.Polyline)
					{
						// Only a single polyline edge can be part of a HatchBoundaryPath. The polyline will be automatically exploded.
						HatchBoundaryPath.Polyline polyline = (HatchBoundaryPath.Polyline)edge;
						this.edges.AddRange(polyline.Explode());
					}
					else
					{
						this.edges.Add(edge);
					}
				}
			}
		}

		#endregion

		#region public properties

		/// <summary>Gets the list of edges that makes a loop for the hatch boundary path.</summary>
		public IReadOnlyList<Edge> Edges => this.edges;

		/// <summary>Gets the boundary path type flag.</summary>
		public HatchBoundaryPathTypeFlags PathType { get; internal set; }

		/// <summary>Gets the list of entities that makes the boundary.</summary>
		/// <remarks>If the boundary path belongs to a non-associative hatch this list will contain zero entities.</remarks>
		public IReadOnlyList<EntityObject> Entities => this.entities;

		#endregion

		#region internal methods

		internal void AddContour(EntityObject entity) => this.entities.Add(entity);

		internal void ClearContour() => this.entities.Clear();

		internal bool RemoveContour(EntityObject entity) => this.entities.Remove(entity);

		#endregion

		#region public methods

		/// <summary>Updates the internal HatchBoundaryPath data.</summary>
		/// <remarks>
		/// It is necessary to manually call this method when changes to the boundary entities are made. This is only applicable to associative hatches,
		/// non-associative hatches has no associated boundary entities.
		/// </remarks>
		public void Update() => this.SetInternalInfo(this.Entities, true);

		#endregion

		#region private methods

		private void SetInternalInfo(IEnumerable<EntityObject> contour, bool clearEdges)
		{
			bool containsPolyline = false;
			if (clearEdges)
			{
				this.edges.Clear();
			}

			foreach (EntityObject entity in contour)
			{
				if (containsPolyline)
				{
					throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
				}

				// it seems that AutoCad does not have problems on creating loops that theoretically does not make sense,
				// like, for example, an internal loop that is made of a single arc.
				// so if AutoCAD is OK with that I am too, the program that make use of this information will take care of this inconsistencies
				switch (entity.Type)
				{
					case EntityType.Arc:
						this.edges.Add(Arc.ConvertFrom(entity));
						break;
					case EntityType.Circle:
						this.edges.Add(Arc.ConvertFrom(entity));
						break;
					case EntityType.Ellipse:
						this.edges.Add(Ellipse.ConvertFrom(entity));
						break;
					case EntityType.Line:
						this.edges.Add(Line.ConvertFrom(entity));
						break;
					case EntityType.Polyline2D:
						Entities.Polyline2D lwpoly = (Entities.Polyline2D)entity;
						if (lwpoly.IsClosed)
						{
							if (this.Edges.Count != 0)
							{
								throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
							}
							this.edges.Add(Polyline.ConvertFrom(entity));
							this.PathType |= HatchBoundaryPathTypeFlags.Polyline;
							containsPolyline = true;
						}
						else
							this.SetInternalInfo(lwpoly.Explode(), false); // open polylines will always be exploded, only one polyline can be present in a path
						break;
					case EntityType.Polyline3D:
						Entities.Polyline3D poly = (Entities.Polyline3D)entity;
						if (poly.IsClosed)
						{
							if (this.Edges.Count != 0)
							{
								throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
							}
							this.edges.Add(Polyline.ConvertFrom(entity));
							this.PathType |= HatchBoundaryPathTypeFlags.Polyline;
							containsPolyline = true;
						}
						else
							this.SetInternalInfo(poly.Explode(), false); // open polylines will always be exploded, only one polyline can be present in a path
						break;
					case EntityType.Spline:
						this.edges.Add(Spline.ConvertFrom(entity));
						break;
					default:
						throw new ArgumentException(string.Format("The entity type {0} cannot be part of a hatch boundary. Only Arc, Circle, Ellipse, Line, Polyline2D, Polyline3D, and Spline entities are allowed.", entity.Type));
				}
			}
		}

		#endregion

		#region ICloneable

		/// <inheritdoc/>
		public object Clone()
		{
			List<Edge> copyEdges = new List<Edge>();
			foreach (Edge edge in this.Edges)
			{
				copyEdges.Add((Edge)edge.Clone());
			}

			return new HatchBoundaryPath(copyEdges);
		}

		#endregion
	}
}