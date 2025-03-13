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
	/// <summary>Represents a shape entity.</summary>
	public class Shape :
		EntityObject
	{
		#region delegates and events

		public delegate void StyleChangedEventHandler(Shape sender, TableObjectChangedEventArgs<ShapeStyle> e);
		public event StyleChangedEventHandler StyleChanged;
		protected virtual ShapeStyle OnStyleChangedEvent(ShapeStyle oldStyle, ShapeStyle newStyle)
		{
			StyleChangedEventHandler ae = this.StyleChanged;
			if (ae != null)
			{
				TableObjectChangedEventArgs<ShapeStyle> eventArgs = new TableObjectChangedEventArgs<ShapeStyle>(oldStyle, newStyle);
				ae(this, eventArgs);
				return eventArgs.NewValue;
			}
			return newStyle;
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Name of the shape which geometry is defined in the shape <see cref="ShapeStyle">style</see>.</param>
		/// <param name="style">Shape <see cref="ShapeStyle">style</see>.</param>
		public Shape(string name, ShapeStyle style)
			: this(name, style, Vector3.Zero, 1.0, 0.0)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Name of the shape which geometry is defined in the shape <see cref="ShapeStyle">style</see>.</param>
		/// <param name="style">Shape <see cref="ShapeStyle">style</see>.</param>
		/// <param name="position">Shape insertion point.</param>
		/// <param name="size">Shape size.</param>
		/// <param name="rotation">Shape rotation.</param>
		public Shape(string name, ShapeStyle style, Vector3 position, double size, double rotation)
			: base(EntityType.Shape, DxfObjectCode.Shape)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}
			_Name = name;
			_Style = style ?? throw new ArgumentNullException(nameof(style));
			this.Position = position;
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(size), size, "The shape size must be greater than zero.");
			}
			_Size = size;
			_Rotation = rotation;
		}

		#endregion

		#region public properties

		private string _Name;
		/// <summary>Gets the shape name.</summary>
		public string Name
		{
			get => _Name;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Name = value;
			}
		}

		private ShapeStyle _Style;
		/// <summary>Gets the <see cref="ShapeStyle">shape style</see>.</summary>
		public ShapeStyle Style
		{
			get => _Style;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Style = this.OnStyleChangedEvent(_Style, value);
			}
		}

		/// <summary>Gets or sets the shape <see cref="Vector3">insertion point</see> in world coordinates.</summary>
		public Vector3 Position { get; set; }

		private double _Size;
		/// <summary>Gets or sets the size of the shape.</summary>
		/// <remarks>
		/// The shape size is relative to the actual size of the shape definition.
		/// The size value works as an scale value applied to the dimensions of the shape definition.
		/// The <b>DXF</b> allows for negative values but that is the same as rotating the shape 180 degrees.<br />
		/// Size values must be greater than zero. Default: 1.0.
		/// </remarks>
		public double Size
		{
			get => _Size;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The shape size must be greater than zero.");
				}
				_Size = value;
			}
		}

		private double _Rotation;
		/// <summary>Gets or sets the shape rotation in degrees.</summary>
		public double Rotation
		{
			get => _Rotation;
			set => _Rotation = MathHelper.NormalizeAngle(value);
		}

		private double _ObliqueAngle = 0.0;
		/// <summary>Gets or sets the shape oblique angle in degrees.</summary>
		public double ObliqueAngle
		{
			get => _ObliqueAngle;
			set => _ObliqueAngle = MathHelper.NormalizeAngle(value);
		}

		private double _WidthFactor = 1.0;
		/// <summary>Gets or sets the shape width factor.</summary>
		/// <remarks>Width factor values cannot be zero. Default: 1.0.</remarks>
		public double WidthFactor
		{
			get => _WidthFactor;
			set
			{
				if (MathHelper.IsZero(value))
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The shape width factor cannot be zero.");
				}
				_WidthFactor = value;
			}
		}

		/// <summary>Gets or set the shape thickness.</summary>
		public double Thickness { get; set; } = 0.0;

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			bool mirrorShape;

			Vector3 newPosition = transformation * this.Position + translation;
			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}

			Matrix3 transOW = MathHelper.ArbitraryAxis(this.Normal);

			Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal);
			transWO = transWO.Transpose();

			List<Vector2> uv = MathHelper.Transform(
				new[]
				{
					Vector2.UnitX * this.WidthFactor * this.Size,
					new Vector2(this.Size * Math.Tan(this.ObliqueAngle * MathHelper.DegToRad), this.Size)
				},
				this.Rotation * MathHelper.DegToRad,
				CoordinateSystem.Object, CoordinateSystem.World);

			Vector3 v;
			v = transOW * new Vector3(uv[0].X, uv[0].Y, 0.0);
			v = transformation * v;
			v = transWO * v;
			Vector2 newUvector = new Vector2(v.X, v.Y);

			v = transOW * new Vector3(uv[1].X, uv[1].Y, 0.0);
			v = transformation * v;
			v = transWO * v;
			Vector2 newVvector = new Vector2(v.X, v.Y);

			double newRotation = Vector2.Angle(newUvector) * MathHelper.RadToDeg;
			double newObliqueAngle = Vector2.Angle(newVvector) * MathHelper.RadToDeg;

			if (Vector2.CrossProduct(newUvector, newVvector) < 0)
			{
				newRotation += 180;
				newObliqueAngle = 270 - (newRotation - newObliqueAngle);
				if (newObliqueAngle >= 360)
				{
					newObliqueAngle -= 360;
				}
				mirrorShape = true;
			}
			else
			{
				newObliqueAngle = 90 + (newRotation - newObliqueAngle);
				mirrorShape = false;
			}

			// the oblique angle is defined between -85 and 85 degrees
			if (newObliqueAngle > 180)
			{
				newObliqueAngle = 180 - newObliqueAngle;
			}

			if (newObliqueAngle < -85)
			{
				newObliqueAngle = -85;
			}
			else if (newObliqueAngle > 85)
			{
				newObliqueAngle = 85;
			}

			// the height must be greater than zero, the cosine is always positive between -85 and 85
			double newHeight = newVvector.Modulus() * Math.Cos(newObliqueAngle * MathHelper.DegToRad);
			newHeight = MathHelper.IsZero(newHeight) ? MathHelper.Epsilon : newHeight;

			// the width factor is defined between 0.01 and 100
			double newWidthFactor = newUvector.Modulus() / newHeight;
			if (newWidthFactor < 0.01)
			{
				newWidthFactor = 0.01;
			}
			else if (newWidthFactor > 100)
			{
				newWidthFactor = 100;
			}

			this.Position = newPosition;
			this.Normal = newNormal;
			this.Rotation = newRotation;
			this.Size = newHeight;
			this.WidthFactor = mirrorShape ? -newWidthFactor : newWidthFactor;
			this.ObliqueAngle = mirrorShape ? -newObliqueAngle : newObliqueAngle;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Shape entity = new Shape(_Name, (ShapeStyle)_Style.Clone())
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
				//Shape properties
				Position = this.Position,
				Size = _Size,
				Rotation = _Rotation,
				ObliqueAngle = _ObliqueAngle,
				Thickness = this.Thickness
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
