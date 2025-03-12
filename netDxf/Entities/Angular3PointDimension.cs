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
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a 3 point angular dimension <see cref="EntityObject">entity</see>.</summary>
	public class Angular3PointDimension :
		Dimension
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public Angular3PointDimension()
			: this(Vector2.Zero, Vector2.UnitX, Vector2.UnitY, 0.1)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="arc"><see cref="Arc"/> to measure.</param>
		/// <param name="offset">Distance between the center of the arc and the dimension line.</param>
		public Angular3PointDimension(Arc arc, double offset)
			: this(arc, offset, DimensionStyle.Default)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="arc">Angle <see cref="Arc">arc</see> to measure.</param>
		/// <param name="offset">Distance between the center of the arc and the dimension line.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		public Angular3PointDimension(Arc arc, double offset, DimensionStyle style)
			: base(DimensionType.Angular3Point)
		{
			if (arc == null)
			{
				throw new ArgumentNullException(nameof(arc));
			}

			Vector3 refPoint = MathHelper.Transform(arc.Center, arc.Normal, CoordinateSystem.World, CoordinateSystem.Object);
			this.CenterPoint = new Vector2(refPoint.X, refPoint.Y);
			this.StartPoint = Vector2.Polar(this.CenterPoint, arc.Radius, arc.StartAngle * MathHelper.DegToRad);
			this.EndPoint = Vector2.Polar(this.CenterPoint, arc.Radius, arc.EndAngle * MathHelper.DegToRad);
			this.Offset = offset;
			this.Style = style ?? throw new ArgumentNullException(nameof(style));
			this.Normal = arc.Normal;
			this.Elevation = refPoint.Z;
			this.Update();
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="centerPoint">Center of the angle arc to measure.</param>
		/// <param name="startPoint">Angle start point.</param>
		/// <param name="endPoint">Angle end point.</param>
		/// <param name="offset">Distance between the center point and the dimension line.</param>
		public Angular3PointDimension(Vector2 centerPoint, Vector2 startPoint, Vector2 endPoint, double offset)
			: this(centerPoint, startPoint, endPoint, offset, DimensionStyle.Default)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="centerPoint">Center of the angle arc to measure.</param>
		/// <param name="startPoint">Angle start point.</param>
		/// <param name="endPoint">Angle end point.</param>
		/// <param name="offset">Distance between the center point and the dimension line.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		public Angular3PointDimension(Vector2 centerPoint, Vector2 startPoint, Vector2 endPoint, double offset, DimensionStyle style)
			: base(DimensionType.Angular3Point)
		{
			this.CenterPoint = centerPoint;
			this.StartPoint = startPoint;
			this.EndPoint = endPoint;
			this.Offset = offset;
			this.Style = style ?? throw new ArgumentNullException(nameof(style));
			this.Update();
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the center <see cref="Vector2">point</see> of the arc in <b>OCS</b> (object coordinate system).</summary>
		public Vector2 CenterPoint { get; set; }

		/// <summary>Gets or sets the angle start <see cref="Vector2">point</see> of the dimension in <b>OCS</b> (object coordinate system).</summary>
		public Vector2 StartPoint { get; set; }

		/// <summary>Gets or sets the angle end <see cref="Vector2">point</see> of the dimension in <b>OCS</b> (object coordinate system).</summary>
		public Vector2 EndPoint { get; set; }

		/// <summary>Gets the location of the dimension line arc.</summary>
		public Vector2 ArcDefinitionPoint => this.DefinitionPoint;

		/// <summary>Gets or sets the distance between the center point and the dimension line.</summary>
		/// <remarks>
		/// Positive values will measure the angle between the start point and the end point while negative values will measure the opposite arc angle.
		/// Even thought, zero values are allowed, they are not recommended.
		/// </remarks>
		public double Offset { get; set; }

		/// <inheritdoc/>
		public override double Measurement
		{
			get
			{
				Vector2 dirRef1 = this.StartPoint - this.CenterPoint;
				Vector2 dirRef2 = this.EndPoint - this.CenterPoint;
				if (Vector2.Equals(dirRef1, dirRef2))
				{
					return 0.0;
				}

				if (Vector2.AreParallel(dirRef1, dirRef2))
				{
					return 180.0;
				}

				double angle = Vector2.AngleBetween(dirRef1, dirRef2) * MathHelper.RadToDeg;

				if (this.Offset < 0)
				{
					return 360 - angle;
				}
				return angle;
			}
		}

		#endregion

		#region public methods

		/// <summary>Calculates the dimension offset from a point along the dimension line.</summary>
		/// <param name="point">Point along the dimension line.</param>
		public void SetDimensionLinePosition(Vector2 point)
		{
			double newOffset = Vector2.Distance(this.CenterPoint, point);

			this.Offset = newOffset;
			Vector2 dirPoint = point - this.CenterPoint;
			double cross1 = Vector2.CrossProduct(this.StartPoint - this.CenterPoint, dirPoint);
			double cross2 = Vector2.CrossProduct(this.EndPoint - this.CenterPoint, dirPoint);

			if (!(cross1 >= 0) || !(cross2 < 0))
			{
				this.Offset *= -1;
			}

			double startAngle = this.Offset >= 0 ? Vector2.Angle(this.CenterPoint, this.StartPoint) : Vector2.Angle(this.CenterPoint, this.EndPoint);
			double midRot = startAngle + 0.5 * this.Measurement * MathHelper.DegToRad;
			Vector2 midDim = Vector2.Polar(this.CenterPoint, Math.Abs(this.Offset), midRot);
			this.DefinitionPoint = midDim;

			if (!this.TextPositionManuallySet)
			{
				DimensionStyleOverride styleOverride;
				double textGap = this.Style.TextOffset;
				if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
				{
					textGap = (double)styleOverride.Value;
				}
				double scale = this.Style.DimScaleOverall;
				if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
				{
					scale = (double)styleOverride.Value;
				}

				double gap = textGap * scale;
				this.textRefPoint = midDim + gap * Vector2.Normalize(midDim - this.CenterPoint);
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

			Matrix3 transOW = MathHelper.ArbitraryAxis(this.Normal);
			Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal).Transpose();

			Vector3 v = transOW * new Vector3(this.StartPoint.X, this.StartPoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newStart = new Vector2(v.X, v.Y);
			double newElevation = v.Z;

			v = transOW * new Vector3(this.EndPoint.X, this.EndPoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newEnd = new Vector2(v.X, v.Y);

			v = transOW * new Vector3(this.CenterPoint.X, this.CenterPoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newCenter = new Vector2(v.X, v.Y);

			if (this.TextPositionManuallySet)
			{
				v = transOW * new Vector3(this.textRefPoint.X, this.textRefPoint.Y, this.Elevation);
				v = transformation * v + translation;
				v = transWO * v;
				this.textRefPoint = new Vector2(v.X, v.Y);
			}

			v = transOW * new Vector3(this.DefinitionPoint.X, this.DefinitionPoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			this.DefinitionPoint = new Vector2(v.X, v.Y);

			this.StartPoint = newStart;
			this.EndPoint = newEnd;
			this.CenterPoint = newCenter;
			this.Elevation = newElevation;
			this.Normal = newNormal;

			this.SetDimensionLinePosition(this.DefinitionPoint);
		}

		/// <inheritdoc/>
		protected override void CalculateReferencePoints()
		{
			DimensionStyleOverride styleOverride;
			double startAngle = this.Offset >= 0 ? Vector2.Angle(this.CenterPoint, this.StartPoint) : Vector2.Angle(this.CenterPoint, this.EndPoint);
			double midRot = startAngle + 0.5 * this.Measurement * MathHelper.DegToRad;
			Vector2 midDim = Vector2.Polar(this.CenterPoint, Math.Abs(this.Offset), midRot);

			this.DefinitionPoint = midDim;

			if (this.TextPositionManuallySet)
			{
				DimensionStyleFitTextMove moveText = this.Style.FitTextMove;
				if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
				{
					moveText = (DimensionStyleFitTextMove)styleOverride.Value;
				}

				if (moveText == DimensionStyleFitTextMove.BesideDimLine)
				{
					this.SetDimensionLinePosition(this.textRefPoint);
				}
			}
			else
			{
				double textGap = this.Style.TextOffset;
				if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
				{
					textGap = (double)styleOverride.Value;
				}
				double scale = this.Style.DimScaleOverall;
				if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
				{
					scale = (double)styleOverride.Value;
				}

				double gap = textGap * scale;
				this.textRefPoint = midDim + gap * Vector2.Normalize(midDim - this.CenterPoint);
			}
		}

		/// <inheritdoc/>
		protected override Block BuildBlock(string name) => DimensionBlock.Build(this, name);

		/// <inheritdoc/>
		public override object Clone()
		{
			Angular3PointDimension entity = new Angular3PointDimension
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
				//Dimension properties
				Style = (DimensionStyle)this.Style.Clone(),
				DefinitionPoint = this.DefinitionPoint,
				TextReferencePoint = this.TextReferencePoint,
				TextPositionManuallySet = this.TextPositionManuallySet,
				TextRotation = this.TextRotation,
				AttachmentPoint = this.AttachmentPoint,
				LineSpacingStyle = this.LineSpacingStyle,
				LineSpacingFactor = this.LineSpacingFactor,
				UserText = this.UserText,
				Elevation = this.Elevation,
				//Angular3PointDimension properties
				CenterPoint = this.CenterPoint,
				StartPoint = this.StartPoint,
				EndPoint = this.EndPoint,
				Offset = this.Offset
			};

			foreach (DimensionStyleOverride styleOverride in this.StyleOverrides.Values)
			{
				object copy = styleOverride.Value is ICloneable value ? value.Clone() : styleOverride.Value;
				entity.StyleOverrides.Add(new DimensionStyleOverride(styleOverride.Type, copy));
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