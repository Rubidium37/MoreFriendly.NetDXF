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
	/// <summary>Represents a generic polyline <see cref="EntityObject">entity</see>.</summary>
	public class Polyline3D :
		EntityObject
	{
		#region private fields

		private static short defaultSplineSegs = 8;
		private PolylineSmoothType smoothType;

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public Polyline3D()
			: this(new List<Vector3>(), false)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">3d polyline <see cref="Vector3">vertex</see> list.</param>
		public Polyline3D(IEnumerable<Vector3> vertexes)
			: this(vertexes, false)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">3d polyline <see cref="Vector3">vertex</see> list.</param>
		/// <param name="isClosed">Sets if the polyline is closed, by default it will create an open polyline.</param>
		public Polyline3D(IEnumerable<Vector3> vertexes, bool isClosed)
			: base(EntityType.Polyline3D, DxfObjectCode.Polyline)
		{
			if (vertexes == null)
			{
				throw new ArgumentNullException(nameof(vertexes));
			}

			this.Vertexes = new List<Vector3>(vertexes);
			this.Flags = isClosed ? PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM | PolylineTypeFlags.Polyline3D : PolylineTypeFlags.Polyline3D;
			this.smoothType = PolylineSmoothType.NoSmooth;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets if the default SplineSegs value, this value is used by the Explode method when the current <see cref="Polyline2D"/> does not belong to a <b>DXF</b> document.</summary>
		public static short DefaultSplineSegs
		{
			get => defaultSplineSegs;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Values must be greater than 0.");
				}
				defaultSplineSegs = value;
			}
		}

		/// <summary>Gets the polyline <see cref="Vector3">vertex</see> list.</summary>
		public List<Vector3> Vertexes { get; }

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

		/// <summary>Enable or disable if the line type pattern is generated continuously around the vertexes of the polyline.</summary>
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

		/// <summary>Gets or sets the curve smooth type.</summary>
		/// <remarks>
		/// The additional polyline vertexes corresponding to the SplineFit will be created when writing the <b>DXF</b> file.
		/// </remarks>
		public PolylineSmoothType SmoothType
		{
			get => this.smoothType;
			set
			{
				if (value == PolylineSmoothType.NoSmooth)
				{
					this.Flags &= ~PolylineTypeFlags.SplineFit;
				}
				else
				{
					this.Flags |= PolylineTypeFlags.SplineFit;
				}
				this.smoothType = value;
			}
		}

		#endregion

		#region internal properties

		/// <summary>Gets the <see cref="Polyline3D"/> flags.</summary>
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
		}

		/// <summary>Decompose the actual polyline in a list of <see cref="Line">lines</see>.</summary>
		/// <returns>A list of <see cref="Line">lines</see> that made up the polyline.</returns>
		public List<EntityObject> Explode()
		{
			List<EntityObject> entities = new List<EntityObject>();

			if (this.smoothType == PolylineSmoothType.NoSmooth)
			{
				int index = 0;
				foreach (Vector3 vertex in this.Vertexes)
				{
					Vector3 start;
					Vector3 end;

					if (index == this.Vertexes.Count - 1)
					{
						if (!this.IsClosed)
						{
							break;
						}
						start = vertex;
						end = this.Vertexes[0];
					}
					else
					{
						start = vertex;
						end = this.Vertexes[index + 1];
					}

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
					});

					index++;
				}

				return entities;
			}

			int degree = this.smoothType == PolylineSmoothType.Quadratic ? 2 : 3;
			int splineSegs = this.Owner == null ? DefaultSplineSegs : this.Owner.Record.Owner.Owner.DrawingVariables.SplineSegs;
			int precision = this.IsClosed ? splineSegs * this.Vertexes.Count : splineSegs * (this.Vertexes.Count - 1);
			List<Vector3> splinePoints = Spline.NurbsEvaluator(this.Vertexes.ToArray(), null, null, degree, false, this.IsClosed, precision);

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
					EndPoint = end
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
					EndPoint = splinePoints[0]
				});
			}

			return entities;
		}

		/// <summary>Converts the polyline in a list of vertexes.</summary>
		/// <param name="precision">Number of vertexes generated, only applicable for smoothed polylines.</param>
		/// <returns>A list vertexes that represents the polyline.</returns>
		public List<Vector3> PolygonalVertexes(int precision)
		{
			if (precision < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(precision), precision, "The precision must be equal or greater than zero.");
			}

			int degree;
			if (this.smoothType == PolylineSmoothType.Quadratic)
			{
				degree = 2;
			}
			else if (this.smoothType == PolylineSmoothType.Cubic)
			{
				degree = 3;
			}
			else
			{
				List<Vector3> points = new List<Vector3>(this.Vertexes);
				return points;
			}

			// the minimum number of vertexes generated for smoothed polylines is 2
			if (precision < 2)
			{
				precision = 2;
			}

			// closed polylines will be considered as closed and periodic
			return Spline.NurbsEvaluator(this.Vertexes.ToArray(), null, null, degree, false, this.IsClosed, precision);
		}

		/// <summary>Converts the actual <see cref="Polyline3D"/> in a <see cref="Polyline2D"/>.</summary>
		/// <param name="precision">Number of vertexes generated, only applicable for smoothed polylines.</param>
		/// <returns>A <see cref="Polyline2D"/> that represents the polyline.</returns>
		/// <remarks>
		/// The resulting <see cref="Polyline2D"/> will be a projection of the actual polyline into the plane defined by its normal vector.
		/// </remarks>
		public Polyline2D ToPolyline2D(int precision)
		{
			List<Vector3> vertexes3D = this.PolygonalVertexes(precision);
			List<Vector2> vertexes2D = MathHelper.Transform(vertexes3D, this.Normal, out double _);
			Polyline2D polyline2D = new Polyline2D(vertexes2D)
			{
				Layer = (Layer)this.Layer.Clone(),
				Linetype = (Linetype)this.Linetype.Clone(),
				Color = (AciColor)this.Color.Clone(),
				Lineweight = this.Lineweight,
				Transparency = (Transparency)this.Transparency.Clone(),
				LinetypeScale = this.LinetypeScale,
				Normal = this.Normal,
				IsClosed = this.IsClosed
			};

			return polyline2D;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			for (int i = 0; i < this.Vertexes.Count; i++)
			{
				this.Vertexes[i] = transformation * this.Vertexes[i] + translation;
			}

			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}
			this.Normal = newNormal;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Polyline3D entity = new Polyline3D(this.Vertexes)
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
				//Polyline3D properties
				Flags = this.Flags
			};

			foreach (XData data in this.XData.Values)
			{
				entity.XData.Add((XData)data.Clone());
			}

			return entity;
		}

		#endregion
	}
}