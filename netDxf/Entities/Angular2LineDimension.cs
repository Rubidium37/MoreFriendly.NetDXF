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
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a 3 point angular dimension <see cref="EntityObject">entity</see>.</summary>
	public class Angular2LineDimension :
		Dimension
	{
		#region private fields

		private double offset;

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public Angular2LineDimension()
			: this(Vector2.Zero, Vector2.UnitX, Vector2.Zero, Vector2.UnitY, 0.1)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
		/// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
		/// <param name="offset">Distance between the center point and the dimension line.</param>
		public Angular2LineDimension(Line firstLine, Line secondLine, double offset)
			: this(firstLine, secondLine, offset, Vector3.UnitZ, DimensionStyle.Default)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
		/// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
		/// <param name="offset">Distance between the center point and the dimension line.</param>
		/// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
		public Angular2LineDimension(Line firstLine, Line secondLine, double offset, Vector3 normal)
			: this(firstLine, secondLine, offset, normal, DimensionStyle.Default)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
		/// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
		/// <param name="offset">Distance between the center point and the dimension line.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		public Angular2LineDimension(Line firstLine, Line secondLine, double offset, DimensionStyle style)
			: this(firstLine, secondLine, offset, Vector3.UnitZ, style)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
		/// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
		/// <param name="offset">Distance between the center point and the dimension line.</param>
		/// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		public Angular2LineDimension(Line firstLine, Line secondLine, double offset, Vector3 normal, DimensionStyle style)
			: base(DimensionType.Angular)
		{
			if (firstLine == null)
			{
				throw new ArgumentNullException(nameof(firstLine));
			}

			if (secondLine == null)
			{
				throw new ArgumentNullException(nameof(secondLine));
			}

			if (Vector3.AreParallel(firstLine.Direction, secondLine.Direction))
			{
				throw new ArgumentException("The two lines that define the dimension are parallel.");
			}

			List<Vector3> ocsPoints =
				MathHelper.Transform(
					new[]
					{
						firstLine.StartPoint,
						firstLine.EndPoint,
						secondLine.StartPoint,
						secondLine.EndPoint
					},
					normal, CoordinateSystem.World, CoordinateSystem.Object);

			this.StartFirstLine = new Vector2(ocsPoints[0].X, ocsPoints[0].Y);
			this.EndFirstLine = new Vector2(ocsPoints[1].X, ocsPoints[1].Y);
			this.StartSecondLine = new Vector2(ocsPoints[2].X, ocsPoints[2].Y);
			this.EndSecondLine = new Vector2(ocsPoints[3].X, ocsPoints[3].Y);
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
			}
			this.offset = offset;
			this.Style = style ?? throw new ArgumentNullException(nameof(style));
			this.Normal = normal;
			this.Elevation = ocsPoints[0].Z;
			this.Update();
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
		/// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
		/// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
		/// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
		/// <param name="offset">Distance between the center point and the dimension line.</param>
		public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset)
			: this(startFirstLine, endFirstLine, startSecondLine, endSecondLine, offset, DimensionStyle.Default)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
		/// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
		/// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
		/// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
		/// <param name="offset">Distance between the center point and the dimension line.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset, DimensionStyle style)
			: base(DimensionType.Angular)
		{
			Vector2 dir1 = endFirstLine - startFirstLine;
			Vector2 dir2 = endSecondLine - startSecondLine;
			if (Vector2.AreParallel(dir1, dir2))
			{
				throw new ArgumentException("The two lines that define the dimension are parallel.");
			}

			this.StartFirstLine = startFirstLine;
			this.EndFirstLine = endFirstLine;
			this.StartSecondLine = startSecondLine;
			this.EndSecondLine = endSecondLine;

			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
			}
			this.offset = offset;

			this.Style = style ?? throw new ArgumentNullException(nameof(style));
			this.Update();
		}

		#endregion

		#region public properties

		/// <summary>Gets the center <see cref="Vector2">point</see> of the measured arc in local coordinates.</summary>
		public Vector2 CenterPoint
		{
			get
			{
				return MathHelper.FindIntersection(
					this.StartFirstLine, this.EndFirstLine - this.StartFirstLine,
					this.StartSecondLine, this.EndSecondLine - this.StartSecondLine);
			}
		}

		/// <summary>Start <see cref="Vector2">point</see> of the first line that defines the angle to measure in local coordinates.</summary>
		public Vector2 StartFirstLine { get; set; }

		/// <summary>End <see cref="Vector2">point</see> of the first line that defines the angle to measure in local coordinates.</summary>
		public Vector2 EndFirstLine { get; set; }

		/// <summary>Start <see cref="Vector2">point</see> of the second line that defines the angle to measure in <b>OCS</b> (object coordinate system).</summary>
		public Vector2 StartSecondLine { get; set; }

		/// <summary>End <see cref="Vector2">point</see> of the second line that defines the angle to measure in <b>OCS</b> (object coordinate system).</summary>
		public Vector2 EndSecondLine { get; set; }

		/// <summary>Gets the location of the dimension line arc.</summary>
		public Vector2 ArcDefinitionPoint { get; internal set; }

		/// <summary>Gets or sets the distance between the center point and the dimension line.</summary>
		/// <remarks>
		/// Offset values cannot be negative and, even thought, zero values are allowed, they are not recommended.
		/// </remarks>
		public double Offset
		{
			get => this.offset;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), "The offset value must be equal or greater than zero.");
				}
				this.offset = value;
			}
		}

		/// <inheritdoc/>
		public override double Measurement
		{
			get
			{
				Vector2 dirRef1 = this.EndFirstLine - this.StartFirstLine;
				Vector2 dirRef2 = this.EndSecondLine - this.StartSecondLine;
				return Vector2.AngleBetween(dirRef1, dirRef2) * MathHelper.RadToDeg;
			}
		}

		#endregion

		#region public methods

		/// <summary>Calculates the dimension offset from a point along the dimension line.</summary>
		/// <param name="point">Point along the dimension line.</param>
		/// <remarks>
		/// The start and end points of the reference lines will be modified,
		/// the angle measurement is always made from the direction of the first line to the direction of the second line.
		/// </remarks>
		public void SetDimensionLinePosition(Vector2 point) => this.SetDimensionLinePosition(point, true);

		#endregion

		#region private methods

		private void SetDimensionLinePosition(Vector2 point, bool updateRefs)
		{
			Vector2 dir1 = this.EndFirstLine - this.StartFirstLine;
			Vector2 dir2 = this.EndSecondLine - this.StartSecondLine;
			if (Vector2.AreParallel(dir1, dir2))
			{
				throw new ArgumentException("The two lines that define the dimension are parallel.");
			}

			Vector2 center = this.CenterPoint;

			if (updateRefs)
			{
				double cross = Vector2.CrossProduct(this.EndFirstLine - this.StartFirstLine, this.EndSecondLine - this.StartSecondLine);
				if (cross < 0)
				{
					(this.StartFirstLine, this.StartSecondLine) = (this.StartSecondLine, this.StartFirstLine);
					(this.EndFirstLine, this.EndSecondLine) = (this.EndSecondLine, this.EndFirstLine);

					//MathHelper.Swap(ref this.startFirstLine, ref this.startSecondLine);
					//MathHelper.Swap(ref this.endFirstLine, ref this.endSecondLine);
				}

				Vector2 ref1Start = this.StartFirstLine;
				Vector2 ref1End = this.EndFirstLine;
				Vector2 ref2Start = this.StartSecondLine;
				Vector2 ref2End = this.EndSecondLine;
				Vector2 dirRef1 = ref1End - ref1Start;
				Vector2 dirRef2 = ref2End - ref2Start;

				Vector2 dirOffset = point - center;
				double crossStart = Vector2.CrossProduct(dirRef1, dirOffset);
				double crossEnd = Vector2.CrossProduct(dirRef2, dirOffset);

				if (crossStart >= 0 && crossEnd >= 0)
				{
					this.StartFirstLine = ref2Start;
					this.EndFirstLine = ref2End;
					this.StartSecondLine = ref1End;
					this.EndSecondLine = ref1Start;
				}
				else if (crossStart < 0 && crossEnd >= 0)
				{
					this.StartFirstLine = ref1End;
					this.EndFirstLine = ref1Start;
					this.StartSecondLine = ref2End;
					this.EndSecondLine = ref2Start;
				}
				else if (crossStart < 0 && crossEnd < 0)
				{
					this.StartFirstLine = ref2End;
					this.EndFirstLine = ref2Start;
					this.StartSecondLine = ref1Start;
					this.EndSecondLine = ref1End;
				}
			}

			double newOffset = Vector2.Distance(center, point);
			this.offset = MathHelper.IsZero(newOffset) ? MathHelper.Epsilon : newOffset;

			this.DefinitionPoint = this.EndSecondLine;

			double measure = this.Measurement * MathHelper.DegToRad;
			double startAngle = Vector2.Angle(center, this.EndFirstLine);
			double midRot = startAngle + 0.5 * measure;
			Vector2 midDim = Vector2.Polar(center, this.offset, midRot);
			this.ArcDefinitionPoint = midDim;

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
				this.textRefPoint = midDim + gap * Vector2.Normalize(midDim - center);
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

			Vector3 v = transOW * new Vector3(this.StartFirstLine.X, this.StartFirstLine.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newStart1 = new Vector2(v.X, v.Y);
			double newElevation = v.Z;

			v = transOW * new Vector3(this.EndFirstLine.X, this.EndFirstLine.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newEnd1 = new Vector2(v.X, v.Y);

			v = transOW * new Vector3(this.StartSecondLine.X, this.StartSecondLine.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newStart2 = new Vector2(v.X, v.Y);

			v = transOW * new Vector3(this.EndSecondLine.X, this.EndSecondLine.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newEnd2 = new Vector2(v.X, v.Y);

			Vector2 dir1 = newEnd1 - newStart1;
			Vector2 dir2 = newEnd2 - newStart2;
			if (Vector2.AreParallel(dir1, dir2))
			{
				Debug.Assert(false, "The transformation cannot be applied, the resulting reference lines are parallel.");
				return;
			}

			v = transOW * new Vector3(this.ArcDefinitionPoint.X, this.ArcDefinitionPoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newArcDefPoint = new Vector2(v.X, v.Y);

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

			this.StartFirstLine = newStart1;
			this.EndFirstLine = newEnd1;
			this.StartSecondLine = newStart2;
			this.EndSecondLine = newEnd2;
			this.ArcDefinitionPoint = newArcDefPoint;
			this.Elevation = newElevation;
			this.Normal = newNormal;

			this.SetDimensionLinePosition(newArcDefPoint);
		}

		/// <inheritdoc/>
		protected override void CalculateReferencePoints()
		{
			Vector2 dir1 = this.EndFirstLine - this.StartFirstLine;
			Vector2 dir2 = this.EndSecondLine - this.StartSecondLine;
			if (Vector2.AreParallel(dir1, dir2))
			{
				throw new ArgumentException("The two lines that define the dimension are parallel.");
			}

			DimensionStyleOverride styleOverride;

			double measure = this.Measurement * MathHelper.DegToRad;
			Vector2 center = this.CenterPoint;

			double startAngle = Vector2.Angle(center, this.EndFirstLine);
			double midRot = startAngle + 0.5 * measure;
			Vector2 midDim = Vector2.Polar(center, this.offset, midRot);

			this.DefinitionPoint = this.EndSecondLine;
			this.ArcDefinitionPoint = midDim;

			if (this.TextPositionManuallySet)
			{
				DimensionStyleFitTextMove moveText = this.Style.FitTextMove;
				if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
				{
					moveText = (DimensionStyleFitTextMove)styleOverride.Value;
				}

				if (moveText == DimensionStyleFitTextMove.BesideDimLine)
				{
					this.SetDimensionLinePosition(this.textRefPoint, false);
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
				this.textRefPoint = midDim + gap * Vector2.Normalize(midDim - center);
			}
		}

		/// <inheritdoc/>
		protected override Block BuildBlock(string name) => DimensionBlock.Build(this, name);

		/// <inheritdoc/>
		public override object Clone()
		{
			Angular2LineDimension entity = new Angular2LineDimension
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
				//Angular2LineDimension properties
				StartFirstLine = this.StartFirstLine,
				EndFirstLine = this.EndFirstLine,
				StartSecondLine = this.StartSecondLine,
				EndSecondLine = this.EndSecondLine,
				Offset = this.offset,
				ArcDefinitionPoint = this.ArcDefinitionPoint
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