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
using System.Runtime.CompilerServices;
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents an attribute definition.</summary>
	/// <remarks>
	/// <b>AutoCAD</b> allows to have duplicate tags in the attribute definitions list, but this library does not.
	/// To have duplicate tags is not recommended in any way, since there will be now way to know which is the definition associated to the insert attribute.
	/// </remarks>
	public class AttributeDefinition :
		DxfObject,
		ICloneable
	{
		#region delegates and events

		/// <summary>Generated when a property of <see cref="Layer"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<Layer> BeforeChangingLayerValue;
		/// <summary>Generates the <see cref="BeforeChangingLayerValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual Layer OnBeforeChangingLayerValue(Layer oldValue, Layer newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingLayerValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<Layer>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		/// <summary>Generated when a property of <see cref="Linetype"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<Linetype> BeforeChangingLinetypeValue;
		/// <summary>Generates the <see cref="BeforeChangingLinetypeValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual Linetype OnBeforeChangingLinetypeValue(Linetype oldValue, Linetype newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingLinetypeValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<Linetype>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		/// <summary>Generated when a property of <see cref="TextStyle"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<TextStyle> BeforeChangingTextStyleValue;
		/// <summary>Generates the <see cref="BeforeChangingTextStyleValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual TextStyle OnBeforeChangingBeforeValueTextStyle(TextStyle oldValue, TextStyle newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingTextStyleValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<TextStyle>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		#endregion

		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="tag">Attribute identifier.</param>
		public AttributeDefinition(string tag)
			: this(tag, TextStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="tag">Attribute identifier.</param>
		/// <param name="style">Attribute <see cref="TextStyle">text style</see>.</param>
		public AttributeDefinition(string tag, TextStyle style)
			: this(tag, MathHelper.IsZero(style.Height) ? 1.0 : style.Height, style)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="tag">Attribute identifier.</param>
		/// <param name="textHeight">Height of the attribute definition text.</param>
		/// <param name="style">Attribute <see cref="TextStyle">text style</see>.</param>
		public AttributeDefinition(string tag, double textHeight, TextStyle style)
			: base(DxfObjectCode.AttributeDefinition)
		{
			if (string.IsNullOrEmpty(tag))
			{
				throw new ArgumentNullException(nameof(tag));
			}

			this.Tag = tag;
			_Style = style ?? throw new ArgumentNullException(nameof(style));
			if (textHeight <= 0.0)
			{
				throw new ArgumentOutOfRangeException(nameof(textHeight), _Value, "The attribute definition text height must be greater than zero.");
			}
			_Height = textHeight;
			_WidthFactor = style.WidthFactor;
			_ObliqueAngle = style.ObliqueAngle;
		}

		#endregion

		#region public property

		private AciColor _Color = AciColor.ByLayer;
		/// <summary>Gets or sets the entity <see cref="AciColor">color</see>.</summary>
		public AciColor Color
		{
			get => _Color;
			set => _Color = value ?? throw new ArgumentNullException(nameof(value));
		}

		private Layer _Layer = Layer.Default;
		/// <summary>Gets or sets the entity <see cref="Layer">layer</see>.</summary>
		public Layer Layer
		{
			get => _Layer;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Layer = this.OnBeforeChangingLayerValue(_Layer, value);
			}
		}

		private Linetype _Linetype = Linetype.ByLayer;
		/// <summary>Gets or sets the entity <see cref="Linetype">line type</see>.</summary>
		public Linetype Linetype
		{
			get => _Linetype;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Linetype = this.OnBeforeChangingLinetypeValue(_Linetype, value);
			}
		}

		/// <summary>Gets or sets the entity line weight, one unit is always 1/100 mm (default = ByLayer).</summary>
		public Lineweight Lineweight { get; set; } = Lineweight.ByLayer;

		private Transparency _Transparency = Transparency.ByLayer;
		/// <summary>Gets or sets layer transparency (default: ByLayer).</summary>
		public Transparency Transparency
		{
			get => _Transparency;
			set => _Transparency = value ?? throw new ArgumentNullException(nameof(value));
		}

		private double _LinetypeScale = 1.0;
		/// <summary>Gets or sets the entity line type scale.</summary>
		public double LinetypeScale
		{
			get => _LinetypeScale;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The line type scale must be greater than zero.");
				}
				_LinetypeScale = value;
			}
		}

		/// <summary>Gets or set the entity visibility.</summary>
		public bool IsVisible { get; set; } = true;

		private Vector3 _Normal = Vector3.UnitZ;
		/// <summary>Gets or sets the entity <see cref="Vector3">normal</see>.</summary>
		public Vector3 Normal
		{
			get => _Normal;
			set
			{
				_Normal = Vector3.Normalize(value);
				if (Vector3.IsZero(_Normal))
				{
					throw new ArgumentException("The normal can not be the zero vector.", nameof(value));
				}
			}
		}

		/// <summary>Gets the attribute identifier.</summary>
		/// <remarks>
		/// Even thought the official <b>DXF</b> documentation clearly says that the attribute definition tag cannot contain spaces,
		/// most programs seems to allow them, but I cannot guarantee that all will behave this way.
		/// </remarks>
		public string Tag { get; }

		/// <summary>Gets or sets the attribute information text.</summary>
		/// <remarks>This is the text prompt shown to introduce the attribute value when new Insert entities are inserted into the drawing.</remarks>
		public string Prompt { get; set; } = string.Empty;

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
					throw new ArgumentOutOfRangeException(nameof(value), value, "The height should be greater than zero.");
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
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The width factor should be greater than zero.");
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
					throw new ArgumentOutOfRangeException(nameof(value), value, "The oblique angle valid values range from -85 to 85.");
				}
				_ObliqueAngle = value;
			}
		}

		private string _Value;
		/// <summary>Gets or sets the attribute default value.</summary>
		public string Value
		{
			get => _Value;
			set => _Value = string.IsNullOrEmpty(value) ? string.Empty : value;
		}

		private TextStyle _Style;
		/// <summary>Gets or sets the attribute text style.</summary>
		/// <remarks>
		/// The <see cref="TextStyle">text style</see> defines the basic properties of the information text.
		/// </remarks>
		public TextStyle Style
		{
			get => _Style;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Style = this.OnBeforeChangingBeforeValueTextStyle(_Style, value);
			}
		}

		/// <summary>Gets or sets the attribute <see cref="Vector3">position</see> in object coordinates.</summary>
		public Vector3 Position { get; set; } = Vector3.Zero;

		/// <summary>Gets or sets the attribute flags.</summary>
		public AttributeFlags Flags { get; set; } = AttributeFlags.None;

		private double _Rotation = 0.0;
		/// <summary>Gets or sets the attribute text rotation in degrees.</summary>
		public double Rotation
		{
			get => _Rotation;
			set => _Rotation = MathHelper.NormalizeAngle(value);
		}

		/// <summary>Gets or sets the text alignment.</summary>
		public TextAlignment Alignment { get; set; } = TextAlignment.BaselineLeft;

		/// <summary>Gets or sets if the attribute definition text is backward (mirrored in X).</summary>
		public bool IsBackward { get; set; } = false;

		/// <summary>Gets or sets if the attribute definition text is upside down (mirrored in Y).</summary>
		public bool IsUpsideDown { get; set; } = false;

		/// <summary>Gets the owner of the actual <b>DXF</b> object.</summary>
		public new Block Owner
		{
			get => (Block)base.Owner;
			internal set => base.Owner = value;
		}

		#endregion

		#region public methods

		/// <summary>Moves, scales, and/or rotates the current attribute definition given a 3x3 transformation matrix and a translation vector.</summary>
		/// <param name="transformation">Transformation matrix.</param>
		/// <param name="translation">Translation vector.</param>
		/// <remarks>Matrix3 adopts the convention of using column vectors to represent a transformation matrix.</remarks>
		public void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			bool mirrText = this.Owner == null ? Text.DefaultMirrText : this.Owner.Record.Owner.Owner.DrawingVariables.MirrText;

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
					if (!(this.Alignment == TextAlignment.Fit || this.Alignment == TextAlignment.Aligned))
					{
						newRotation += 180;
					}
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

		/// <summary>Moves, scales, and/or rotates the current entity given a 4x4 transformation matrix.</summary>
		/// <param name="transformation">Transformation matrix.</param>
		/// <remarks>Matrix4 adopts the convention of using column vectors to represent a transformation matrix.</remarks>
		public void TransformBy(Matrix4 transformation)
		{
			Matrix3 m = new Matrix3(transformation.M11, transformation.M12, transformation.M13,
				transformation.M21, transformation.M22, transformation.M23,
				transformation.M31, transformation.M32, transformation.M33);
			Vector3 v = new Vector3(transformation.M14, transformation.M24, transformation.M34);

			this.TransformBy(m, v);
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public object Clone()
		{
			AttributeDefinition entity = new AttributeDefinition(this.Tag)
			{
				//Attribute definition properties
				Layer = (Layer)this.Layer.Clone(),
				Linetype = (Linetype)this.Linetype.Clone(),
				Color = (AciColor)this.Color.Clone(),
				Lineweight = this.Lineweight,
				Transparency = (Transparency)this.Transparency.Clone(),
				LinetypeScale = this.LinetypeScale,
				Normal = this.Normal,
				IsVisible = this.IsVisible,
				Prompt = this.Prompt,
				Value = _Value,
				Height = _Height,
				Width = _Width,
				WidthFactor = _WidthFactor,
				ObliqueAngle = _ObliqueAngle,
				Style = (TextStyle)_Style.Clone(),
				Position = this.Position,
				Flags = this.Flags,
				Rotation = _Rotation,
				Alignment = this.Alignment,
				IsBackward = this.IsBackward,
				IsUpsideDown = this.IsUpsideDown
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
