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
	/// <summary>Represents an ordinate dimension <see cref="EntityObject">entity</see>.</summary>
	public class OrdinateDimension :
		Dimension
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public OrdinateDimension()
			: this(Vector2.Zero, new Vector2(0.5, 0), new Vector2(1.0, 0), OrdinateDimensionAxis.Y, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="origin">Origin <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="leaderEndPoint">Leader end <see cref="Vector2">point</see> in local coordinates of the ordinate dimension</param>
		/// <remarks>
		/// Uses the difference between the feature location and the leader endpoint to determine whether it is an X or a Y ordinate dimension.
		/// If the difference in the Y ordinate is greater, the dimension measures the X ordinate. Otherwise, it measures the Y ordinate.
		/// </remarks>
		public OrdinateDimension(Vector2 origin, Vector2 featurePoint, Vector2 leaderEndPoint)
			: this(origin, featurePoint, leaderEndPoint, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="origin">Origin <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="leaderEndPoint">Leader end <see cref="Vector2">point</see> in local coordinates of the ordinate dimension</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		/// <remarks>
		/// Uses the difference between the feature location and the leader endpoint to determine whether it is an X or a Y ordinate dimension.
		/// If the difference in the Y ordinate is greater, the dimension measures the X ordinate. Otherwise, it measures the Y ordinate.
		/// </remarks>
		public OrdinateDimension(Vector2 origin, Vector2 featurePoint, Vector2 leaderEndPoint, DimensionStyle style)
			: base(DimensionType.Ordinate)
		{
			this.DefinitionPoint = origin;
			this.FeaturePoint = featurePoint;
			this.LeaderEndPoint = leaderEndPoint;
			_TextReferencePoint = leaderEndPoint;
			Vector2 vec = leaderEndPoint - featurePoint;
			this.Axis = vec.Y > vec.X ? OrdinateDimensionAxis.X : OrdinateDimensionAxis.Y;
			_Rotation = 0.0;
			this.Style = style ?? throw new ArgumentNullException(nameof(style));
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="origin">Origin <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="leaderEndPoint">Leader end <see cref="Vector2">point</see> in local coordinates of the ordinate dimension</param>
		/// <param name="axis">Length of the dimension line.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		public OrdinateDimension(Vector2 origin, Vector2 featurePoint, Vector2 leaderEndPoint, OrdinateDimensionAxis axis, DimensionStyle style)
			: base(DimensionType.Ordinate)
		{
			this.DefinitionPoint = origin;
			this.FeaturePoint = featurePoint;
			this.LeaderEndPoint = leaderEndPoint;
			_TextReferencePoint = leaderEndPoint;
			this.Axis = axis;
			_Rotation = 0.0;
			this.Style = style ?? throw new ArgumentNullException(nameof(style));
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="origin">Origin <see cref="Vector2">point</see> of the ordinate dimension.</param>
		/// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="length">Length of the dimension line.</param>
		/// <param name="axis">Length of the dimension line.</param>
		/// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
		public OrdinateDimension(Vector2 origin, Vector2 featurePoint, double length, OrdinateDimensionAxis axis)
			: this(origin, featurePoint, length, axis, 0.0, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="origin">Origin <see cref="Vector2">point</see> of the ordinate dimension.</param>
		/// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="length">Length of the dimension line.</param>
		/// <param name="axis">Length of the dimension line.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		/// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
		public OrdinateDimension(Vector2 origin, Vector2 featurePoint, double length, OrdinateDimensionAxis axis, DimensionStyle style)
			: this(origin, featurePoint, length, axis, 0.0, style)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="origin">Origin <see cref="Vector2">point</see> of the ordinate dimension.</param>
		/// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="length">Length of the dimension line.</param>
		/// <param name="axis">Length of the dimension line.</param>
		/// <param name="rotation">Angle of rotation in degrees of the dimension lines.</param>
		/// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
		public OrdinateDimension(Vector2 origin, Vector2 featurePoint, double length, OrdinateDimensionAxis axis, double rotation)
			: this(origin, featurePoint, length, axis, rotation, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="origin">Origin <see cref="Vector3">point</see> in world coordinates of the ordinate dimension.</param>
		/// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
		/// <param name="length">Length of the dimension line.</param>
		/// <param name="axis">Local axis that measures the ordinate dimension.</param>
		/// <param name="rotation">Angle of rotation in degrees of the dimension lines.</param>
		/// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
		/// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
		public OrdinateDimension(Vector2 origin, Vector2 featurePoint, double length, OrdinateDimensionAxis axis, double rotation, DimensionStyle style)
			: base(DimensionType.Ordinate)
		{
			this.DefinitionPoint = origin;
			_Rotation = MathHelper.NormalizeAngle(rotation);
			this.FeaturePoint = featurePoint;
			this.Axis = axis;

			this.Style = style ?? throw new ArgumentNullException(nameof(style));

			double angle = rotation * MathHelper.DegToRad;
			if (this.Axis == OrdinateDimensionAxis.X)
			{
				angle += MathHelper.HalfPI;
			}

			this.LeaderEndPoint = Vector2.Polar(featurePoint, length, angle);
			_TextReferencePoint = this.LeaderEndPoint;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the origin <see cref="Vector2">point</see> in local coordinates.</summary>
		public Vector2 Origin
		{
			get => this.DefinitionPoint;
			set => this.DefinitionPoint = value;
		}

		/// <summary>Gets or set the base <see cref="Vector2">point</see> in local coordinates, a point on a feature such as an endpoint, intersection, or center of an object.</summary>
		public Vector2 FeaturePoint { get; set; }

		/// <summary>Gets or sets the leader end <see cref="Vector2">point</see> in local coordinates</summary>
		public Vector2 LeaderEndPoint { get; set; }

		private double _Rotation;
		/// <summary>Gets or sets the angle of rotation in degrees of the ordinate dimension local coordinate system.</summary>
		public double Rotation
		{
			get => _Rotation;
			set => MathHelper.NormalizeAngle(_Rotation = value);
		}

		/// <summary>Gets or sets the local axis that measures the ordinate dimension.</summary>
		public OrdinateDimensionAxis Axis { get; set; }

		/// <inheritdoc/>
		public override double Measurement
		{
			get
			{
				Vector2 dirRef = Vector2.Rotate(this.Axis == OrdinateDimensionAxis.X ? Vector2.UnitY : Vector2.UnitX, _Rotation * MathHelper.DegToRad);
				return MathHelper.PointLineDistance(this.FeaturePoint, this.DefinitionPoint, dirRef);
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

			Vector3 refAxis = transOW * Vector3.UnitX;
			refAxis = transformation * refAxis;
			refAxis = transWO * refAxis;
			double newRotation = Vector2.Angle(new Vector2(refAxis.X, refAxis.Y)) * MathHelper.RadToDeg;

			Vector3 v = transOW * new Vector3(this.FeaturePoint.X, this.FeaturePoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newStart = new Vector2(v.X, v.Y);
			double newElevation = v.Z;

			v = transOW * new Vector3(this.LeaderEndPoint.X, this.LeaderEndPoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			Vector2 newEnd = new Vector2(v.X, v.Y);

			v = transOW * new Vector3(_TextReferencePoint.X, _TextReferencePoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			_TextReferencePoint = new Vector2(v.X, v.Y);

			v = transOW * new Vector3(this.DefinitionPoint.X, this.DefinitionPoint.Y, this.Elevation);
			v = transformation * v + translation;
			v = transWO * v;
			this.DefinitionPoint = new Vector2(v.X, v.Y);

			this.Rotation += newRotation;
			this.FeaturePoint = newStart;
			this.LeaderEndPoint = newEnd;
			this.Elevation = newElevation;
			this.Normal = newNormal;
		}

		/// <inheritdoc/>
		protected override void CalculateReferencePoints()
		{
			if (this.TextPositionManuallySet)
			{
				DimensionStyleFitTextMove moveText = this.Style.FitTextMove;
				DimensionStyleOverride styleOverride;
				if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
				{
					moveText = (DimensionStyleFitTextMove)styleOverride.Value;
				}

				if (moveText != DimensionStyleFitTextMove.OverDimLineWithoutLeader)
				{
					this.LeaderEndPoint = _TextReferencePoint;
				}
			}
			else
			{
				_TextReferencePoint = this.LeaderEndPoint;
			}
		}

		/// <inheritdoc/>
		protected override Block BuildBlock(string name) => DimensionBlock.Build(this, name);

		/// <inheritdoc/>
		public override object Clone()
		{
			OrdinateDimension entity = new OrdinateDimension
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
				//OrdinateDimension properties
				FeaturePoint = this.FeaturePoint,
				LeaderEndPoint = this.LeaderEndPoint,
				Rotation = _Rotation,
				Axis = this.Axis
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