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
	/// <summary>Represents a Text <see cref="EntityObject">entity</see>.</summary>
	public class Text :
		EntityObject
	{
		#region delegates and events

		public delegate void TextStyleChangedEventHandler(Text sender, TableObjectChangedEventArgs<TextStyle> e);
		public event TextStyleChangedEventHandler TextStyleChanged;
		protected virtual TextStyle OnTextStyleChangedEvent(TextStyle oldTextStyle, TextStyle newTextStyle)
		{
			TextStyleChangedEventHandler ae = this.TextStyleChanged;
			if (ae != null)
			{
				TableObjectChangedEventArgs<TextStyle> eventArgs = new TableObjectChangedEventArgs<TextStyle>(oldTextStyle, newTextStyle);
				ae(this, eventArgs);
				return eventArgs.NewValue;
			}
			return newTextStyle;
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public Text()
			: this(string.Empty, Vector3.Zero, 1.0, TextStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="text">Text string.</param>
		public Text(string text)
			: this(text, Vector2.Zero, 1.0, TextStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="text">Text string.</param>
		/// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
		/// <param name="height">Text height.</param>
		public Text(string text, Vector2 position, double height)
			: this(text, new Vector3(position.X, position.Y, 0.0), height, TextStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="text">Text string.</param>
		/// <param name="position">Text <see cref="Vector3">position</see> in world coordinates.</param>
		/// <param name="height">Text height.</param>
		public Text(string text, Vector3 position, double height)
			: this(text, position, height, TextStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="text">Text string.</param>
		/// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
		/// <param name="height">Text height.</param>
		/// <param name="style">Text <see cref="TextStyle">style</see>.</param>
		public Text(string text, Vector2 position, double height, TextStyle style)
			: this(text, new Vector3(position.X, position.Y, 0.0), height, style)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="text">Text string.</param>
		/// <param name="position">Text <see cref="Vector3">position</see> in world coordinates.</param>
		/// <param name="height">Text height.</param>
		/// <param name="style">Text <see cref="TextStyle">style</see>.</param>
		public Text(string text, Vector3 position, double height, TextStyle style)
			: base(EntityType.Text, DxfObjectCode.Text)
		{
			this.Value = text;
			this.Position = position;
			_Style = style ?? throw new ArgumentNullException(nameof(style));
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height), this.Value, "The Text height must be greater than zero.");
			}
			_Height = height;
			_WidthFactor = style.WidthFactor;
			_ObliqueAngle = style.ObliqueAngle;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets if the text will be mirrored when a symmetry is performed, when the current Text entity does not belong to a <b>DXF</b> document.</summary>
		public static bool DefaultMirrText { get; set; }

		/// <summary>Gets or sets Text <see cref="Vector3">position</see> in world coordinates.</summary>
		public Vector3 Position { get; set; }

		private double _Rotation = 0.0;
		/// <summary>Gets or sets the text rotation in degrees.</summary>
		public double Rotation
		{
			get => _Rotation;
			set => _Rotation = MathHelper.NormalizeAngle(value);
		}

		private double _Height;
		/// <summary>Gets or sets the text height.</summary>
		/// <remarks>
		/// Valid values must be greater than zero. Default: 1.0.<br />
		/// When Alignment.Aligned is used this value is not applicable, it will be automatically adjusted so the text will fit in the specified width.
		/// </remarks>
		public double Height
		{
			get => _Height;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The Text height must be greater than zero.");
				}
				_Height = value;
			}
		}

		private double _Width = 1.0;
		/// <summary>Gets or sets the text width, only applicable for text Alignment.Fit and Alignment.Align.</summary>
		/// <remarks>Valid values must be greater than zero. Default: 1.0.</remarks>
		public double Width
		{
			get => _Width;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The Text width must be greater than zero.");
				}
				_Width = value;
			}
		}

		private double _WidthFactor;
		/// <summary>Gets or sets the width factor.</summary>
		/// <remarks>
		/// Valid values range from 0.01 to 100. Default: 1.0.<br />
		/// When Alignment.Fit is used this value is not applicable, it will be automatically adjusted so the text will fit in the specified width.
		/// </remarks>
		public double WidthFactor
		{
			get => _WidthFactor;
			set
			{
				if (value < 0.01 || value > 100.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The Text width factor valid values range from 0.01 to 100.");
				}
				_WidthFactor = value;
			}
		}

		private double _ObliqueAngle;
		/// <summary>Gets or sets the font oblique angle.</summary>
		/// <remarks>Valid values range from -85 to 85. Default: 0.0.</remarks>
		public double ObliqueAngle
		{
			get => _ObliqueAngle;
			set
			{
				if (value < -85.0 || value > 85.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The Text oblique angle valid values range from -85 to 85.");
				}
				_ObliqueAngle = value;
			}
		}

		/// <summary>Gets or sets the text alignment.</summary>
		public TextAlignment Alignment { get; set; } = TextAlignment.BaselineLeft;

		private TextStyle _Style;
		/// <summary>Gets or sets the <see cref="TextStyle">text style</see>.</summary>
		public TextStyle Style
		{
			get => _Style;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Style = this.OnTextStyleChangedEvent(_Style, value);
			}
		}

		/// <summary>Gets or sets the text string.</summary>
		public string Value { get; set; }

		/// <summary>Gets or sets if the text is backward (mirrored in X).</summary>
		public bool IsBackward { get; set; } = false;

		/// <summary>Gets or sets if the text is upside down (mirrored in Y).</summary>
		public bool IsUpsideDown { get; set; } = false;

		#endregion

		#region overrides

		/// <inheritdoc/>
		/// <remarks>
		/// When the current Text entity does not belong to a <b>DXF</b> document, the text will use the DefaultMirrText when a symmetry is performed;
		/// otherwise, the drawing variable MirrText will be used.<br />
		/// A symmetry around the X axis when the text uses an Alignment.BaseLineLeft, Alignment.BaseLineCenter, Alignment.BaseLineRight, Alignment.Fit or an Alignment.Aligned.
		/// A symmetry around the Y axis when the text uses an Alignment.Fit or an Alignment.Aligned.<br />
		/// Matrix3 adopts the convention of using column vectors to represent a transformation matrix.
		/// </remarks>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			bool mirrText = this.Owner == null ? DefaultMirrText : this.Owner.Record.Owner.Owner.DrawingVariables.MirrText;

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
					this.WidthFactor * this.Height * Vector2.UnitX,
					new Vector2(this.Height * Math.Tan(this.ObliqueAngle * MathHelper.DegToRad), this.Height)
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

			if (mirrText)
			{
				if (Vector2.CrossProduct(newUvector, newVvector) < 0)
				{
					newObliqueAngle = 90 - (newRotation - newObliqueAngle);
					if (!(this.Alignment == TextAlignment.Fit || this.Alignment == TextAlignment.Aligned)) newRotation += 180;
					this.IsBackward = !this.IsBackward;
				}
				else
				{
					newObliqueAngle = 90 + (newRotation - newObliqueAngle);
				}
			}
			else
			{
				if (Vector2.CrossProduct(newUvector, newVvector) < 0.0)
				{
					newObliqueAngle = 90 - (newRotation - newObliqueAngle);

					if (Vector2.DotProduct(newUvector, uv[0]) < 0.0)
					{
						newRotation += 180;

						switch (this.Alignment)
						{
							case TextAlignment.TopLeft:
								this.Alignment = TextAlignment.TopRight;
								break;
							case TextAlignment.TopRight:
								this.Alignment = TextAlignment.TopLeft;
								break;
							case TextAlignment.MiddleLeft:
								this.Alignment = TextAlignment.MiddleRight;
								break;
							case TextAlignment.MiddleRight:
								this.Alignment = TextAlignment.MiddleLeft;
								break;
							case TextAlignment.BaselineLeft:
								this.Alignment = TextAlignment.BaselineRight;
								break;
							case TextAlignment.BaselineRight:
								this.Alignment = TextAlignment.BaselineLeft;
								break;
							case TextAlignment.BottomLeft:
								this.Alignment = TextAlignment.BottomRight;
								break;
							case TextAlignment.BottomRight:
								this.Alignment = TextAlignment.BottomLeft;
								break;
						}
					}
					else
					{
						switch (this.Alignment)
						{
							case TextAlignment.TopLeft:
								this.Alignment = TextAlignment.BottomLeft;
								break;
							case TextAlignment.TopCenter:
								this.Alignment = TextAlignment.BottomCenter;
								break;
							case TextAlignment.TopRight:
								this.Alignment = TextAlignment.BottomRight;
								break;
							case TextAlignment.BottomLeft:
								this.Alignment = TextAlignment.TopLeft;
								break;
							case TextAlignment.BottomCenter:
								this.Alignment = TextAlignment.TopCenter;
								break;
							case TextAlignment.BottomRight:
								this.Alignment = TextAlignment.TopRight;
								break;
						}
					}
				}
				else
				{
					newObliqueAngle = 90 + (newRotation - newObliqueAngle);
				}
			}

			// the oblique angle is defined between -85 and 85 degrees
			//if (newObliqueAngle >= 360) newObliqueAngle -= 360;
			newObliqueAngle = MathHelper.NormalizeAngle(newObliqueAngle);
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

			// the height must be greater than zero, the cos is always positive between -85 and 85
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
			this.Height = newHeight;
			this.WidthFactor = newWidthFactor;
			this.ObliqueAngle = newObliqueAngle;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Text entity = new Text
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
				//Text properties
				Position = this.Position,
				Rotation = _Rotation,
				Height = _Height,
				Width = _Width,
				WidthFactor = _WidthFactor,
				ObliqueAngle = _ObliqueAngle,
				Alignment = this.Alignment,
				IsBackward = this.IsBackward,
				IsUpsideDown = this.IsUpsideDown,
				Style = (TextStyle)_Style.Clone(),
				Value = this.Value
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