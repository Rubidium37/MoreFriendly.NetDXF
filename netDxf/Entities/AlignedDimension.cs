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
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a dimension <see cref="EntityObject">entity</see> that is aligned the reference line.</summary>
	public class AlignedDimension :
		Dimension
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public AlignedDimension()
			: this(Vector2.Zero, Vector2.UnitX, 0.1)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
		/// <param name="offset">Distance between the reference line and the dimension line.</param>
		/// <remarks>The reference points define the distance to be measure.</remarks>
		public AlignedDimension(Line referenceLine, double offset)
			: this(referenceLine, offset, Vector3.UnitZ, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
		/// <param name="offset">Distance between the reference line and the dimension line.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		/// <remarks>The reference points define the distance to be measure.</remarks>
		public AlignedDimension(Line referenceLine, double offset, DimensionStyle style)
			: this(referenceLine, offset, Vector3.UnitZ, style)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
		/// <param name="offset">Distance between the reference line and the dimension line.</param>
		/// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
		/// <remarks>The reference points define the distance to be measure.</remarks>
		public AlignedDimension(Line referenceLine, double offset, Vector3 normal)
			: this(referenceLine, offset, normal, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
		/// <param name="offset">Distance between the reference line and the dimension line.</param>
		/// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		/// <remarks>The reference line define the distance to be measure.</remarks>
		public AlignedDimension(Line referenceLine, double offset, Vector3 normal, DimensionStyle style)
			: base(DimensionType.Aligned)
		{
			if (referenceLine == null)
			{
				throw new ArgumentNullException(nameof(referenceLine));
			}

			List<Vector3> ocsPoints = MathHelper.Transform(
				new List<Vector3> { referenceLine.StartPoint, referenceLine.EndPoint },
				normal,
				CoordinateSystem.World,
				CoordinateSystem.Object);
			this.FirstReferencePoint = new Vector2(ocsPoints[0].X, ocsPoints[0].Y);
			this.SecondReferencePoint = new Vector2(ocsPoints[1].X, ocsPoints[1].Y);
			this.Offset = offset;
			this.Style = style ?? throw new ArgumentNullException(nameof(style));
			this.Normal = normal;
			this.Elevation = ocsPoints[0].Z;
			this.Update();
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstPoint">First reference <see cref="Vector2">point</see> of the dimension.</param>
		/// <param name="secondPoint">Second reference <see cref="Vector2">point</see> of the dimension.</param>
		/// <param name="offset">Distance between the reference line and the dimension line.</param>
		/// <remarks>The reference points define the distance to be measure.</remarks>
		public AlignedDimension(Vector2 firstPoint, Vector2 secondPoint, double offset)
			: this(firstPoint, secondPoint, offset, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstPoint">First reference <see cref="Vector2">point</see> of the dimension.</param>
		/// <param name="secondPoint">Second reference <see cref="Vector2">point</see> of the dimension.</param>
		/// <param name="offset">Distance between the reference line and the dimension line.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		/// <remarks>The reference points define the distance to be measure.</remarks>
		public AlignedDimension(Vector2 firstPoint, Vector2 secondPoint, double offset, DimensionStyle style)
			: base(DimensionType.Aligned)
		{
			this.FirstReferencePoint = firstPoint;
			this.SecondReferencePoint = secondPoint;
			this.Offset = offset;
			this.Style = style ?? throw new ArgumentNullException(nameof(style));

			this.Update();
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the first definition point of the dimension in <b>OCS</b> (object coordinate system).</summary>
		public Vector2 FirstReferencePoint { get; set; }

		/// <summary>Gets or sets the second definition point of the dimension in <b>OCS</b> (object coordinate system).</summary>
		public Vector2 SecondReferencePoint { get; set; }

		/// <summary>Gets the location of the dimension line.</summary>
		public Vector2 DimLinePosition => this.DefinitionPoint;

		/// <summary>Gets or sets the distance between the reference line and the dimension line.</summary>
		/// <remarks>
		/// The positive side at which the dimension line is drawn depends of the direction of its reference line.
		/// </remarks>
		public double Offset { get; set; }

		/// <inheritdoc/>
		public override double Measurement => Vector2.Distance(this.FirstReferencePoint, this.SecondReferencePoint);

		#endregion

		#region public methods

		/// <summary>Calculates the dimension offset from a point along the dimension line.</summary>
		/// <param name="point">Point along the dimension line.</param>
		public void SetDimensionLinePosition(Vector2 point)
		{
			Vector2 refDir = this.SecondReferencePoint - this.FirstReferencePoint;
			Vector2 offsetDir = point - this.FirstReferencePoint;

			double cross = Vector2.CrossProduct(refDir, offsetDir);
			refDir.Normalize();

			Vector2 vec = Vector2.Perpendicular(refDir);
			this.Offset = Math.Sign(cross) * MathHelper.PointLineDistance(point, this.FirstReferencePoint, refDir);
			this.DefinitionPoint = this.SecondReferencePoint + this.Offset * vec;

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

				double gap = this.Offset + textGap * scale;
				_TextReferencePoint = Vector2.MidPoint(this.FirstReferencePoint, this.SecondReferencePoint) + gap * vec;
			}
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal)) newNormal = this.Normal;

			Matrix3 transOW = MathHelper.ArbitraryAxis(this.Normal);
			Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal).Transpose();

			Vector3 v;

			v = transOW * new Vector3(this.FirstReferencePoint.X, this.FirstReferencePoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newStart = new Vector2(v.X, v.Y);
			double newElevation = v.Z;

			v = transOW * new Vector3(this.SecondReferencePoint.X, this.SecondReferencePoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newEnd = new Vector2(v.X, v.Y);

			if (this.TextPositionManuallySet)
			{
				v = transOW * new Vector3(_TextReferencePoint.X, _TextReferencePoint.Y, this.Elevation);
				v = transformation * v + translation;
				v = transWO * v;
				_TextReferencePoint = new Vector2(v.X, v.Y);
			}

			v = transOW * new Vector3(this.DefinitionPoint.X, this.DefinitionPoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			this.DefinitionPoint = new Vector2(v.X, v.Y);

			this.FirstReferencePoint = newStart;
			this.SecondReferencePoint = newEnd;
			this.Elevation = newElevation;
			this.Normal = newNormal;

			this.SetDimensionLinePosition(this.DefinitionPoint);
		}

		/// <inheritdoc/>
		protected override void CalculateReferencePoints()
		{
			DimensionStyleOverride styleOverride;

			Vector2 ref1 = this.FirstReferencePoint;
			Vector2 ref2 = this.SecondReferencePoint;
			Vector2 dirRef = ref2 - ref1;
			Vector2 dirDesp = Vector2.Normalize(Vector2.Perpendicular(dirRef));
			Vector2 vec = this.Offset * dirDesp;
			Vector2 dimRef1 = ref1 + vec;
			Vector2 dimRef2 = ref2 + vec;

			this.DefinitionPoint = dimRef2;

			if (this.TextPositionManuallySet)
			{
				DimensionStyleFitTextMove moveText = this.Style.FitTextMove;
				if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
				{
					moveText = (DimensionStyleFitTextMove)styleOverride.Value;
				}

				if (moveText == DimensionStyleFitTextMove.BesideDimLine)
				{
					this.SetDimensionLinePosition(_TextReferencePoint);
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
				_TextReferencePoint = Vector2.MidPoint(dimRef1, dimRef2) + gap * dirDesp;
			}
		}

		/// <inheritdoc/>
		protected override Block BuildBlock(string name) => DimensionBlock.Build(this, name);

		/// <inheritdoc/>
		public override object Clone()
		{
			AlignedDimension entity = new AlignedDimension
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
				//AlignedDimension properties
				FirstReferencePoint = this.FirstReferencePoint,
				SecondReferencePoint = this.SecondReferencePoint,
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