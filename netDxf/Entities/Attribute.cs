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
	/// <summary>Represents an attribute.</summary>
	/// <remarks>
	/// The attribute position, rotation, height and width factor values also includes the transformation of the <see cref="Insert"/> entity to which it belongs.<br />
	/// During the attribute initialization a copy of all attribute definition properties will be copied,
	/// so any changes made to the attribute definition will only be applied to new attribute instances and not to existing ones.
	/// This behavior is to allow imported <see cref="Insert"/> entities to have attributes without definition in the block,
	/// although this might sound not totally correct it is allowed by AutoCad.
	/// </remarks>
	public class Attribute :
		DxfObject,
		ICloneable
	{
		#region delegates and events

		public delegate void LayerChangedEventHandler(Attribute sender, TableObjectChangedEventArgs<Layer> e);

		public event LayerChangedEventHandler LayerChanged;

		protected virtual Layer OnLayerChangedEvent(Layer oldLayer, Layer newLayer)
		{
			LayerChangedEventHandler ae = this.LayerChanged;
			if (ae != null)
			{
				TableObjectChangedEventArgs<Layer> eventArgs = new TableObjectChangedEventArgs<Layer>(oldLayer, newLayer);
				ae(this, eventArgs);
				return eventArgs.NewValue;
			}
			return newLayer;
		}

		public delegate void LinetypeChangedEventHandler(Attribute sender, TableObjectChangedEventArgs<Linetype> e);

		public event LinetypeChangedEventHandler LinetypeChanged;

		protected virtual Linetype OnLinetypeChangedEvent(Linetype oldLinetype, Linetype newLinetype)
		{
			LinetypeChangedEventHandler ae = this.LinetypeChanged;
			if (ae != null)
			{
				TableObjectChangedEventArgs<Linetype> eventArgs = new TableObjectChangedEventArgs<Linetype>(oldLinetype, newLinetype);
				ae(this, eventArgs);
				return eventArgs.NewValue;
			}
			return newLinetype;
		}

		public delegate void TextStyleChangedEventHandler(Attribute sender, TableObjectChangedEventArgs<TextStyle> e);

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

		#region constructor

		internal Attribute(string tag)
			: base(DxfObjectCode.Attribute)
		{
			this.Tag = string.IsNullOrEmpty(tag) ? string.Empty : tag;
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="definition"><see cref="AttributeDefinition">Attribute definition</see>.</param>
		public Attribute(AttributeDefinition definition)
			: base(DxfObjectCode.Attribute)
		{
			if (definition == null)
			{
				throw new ArgumentNullException(nameof(definition));
			}

			_Color = definition.Color;
			_Layer = definition.Layer;
			_Linetype = definition.Linetype;
			this.Lineweight = definition.Lineweight;
			_LinetypeScale = definition.LinetypeScale;
			_Transparency = definition.Transparency;
			this.IsVisible = definition.IsVisible;
			_Normal = definition.Normal;

			this.Definition = definition;
			this.Tag = definition.Tag;
			_Value = definition.Value;
			_Style = definition.Style;
			this.Position = definition.Position;
			this.Flags = definition.Flags;
			_Height = definition.Height;
			_Width = definition.Width;
			_WidthFactor = definition.WidthFactor;
			_ObliqueAngle = definition.ObliqueAngle;
			_Rotation = definition.Rotation;
			this.Alignment = definition.Alignment;
			this.IsBackward = definition.IsBackward;
			this.IsUpsideDown = definition.IsUpsideDown;
		}

		#endregion

		#region public property

		private AciColor _Color;
		/// <summary>Gets or sets the entity <see cref="AciColor">color</see>.</summary>
		public AciColor Color
		{
			get => _Color;
			set => _Color = value ?? throw new ArgumentNullException(nameof(value));
		}

		private Layer _Layer;
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

				_Layer = this.OnLayerChangedEvent(_Layer, value);
			}
		}

		private Linetype _Linetype;
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

				_Linetype = this.OnLinetypeChangedEvent(_Linetype, value);
			}
		}

		/// <summary>Gets or sets the entity line weight, one unit is always 1/100 mm (default = ByLayer).</summary>
		public Lineweight Lineweight { get; set; }

		private Transparency _Transparency;
		/// <summary>Gets or sets layer transparency (default: ByLayer).</summary>
		public Transparency Transparency
		{
			get => _Transparency;
			set => _Transparency = value ?? throw new ArgumentNullException(nameof(value));
		}

		private double _LinetypeScale;
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
		public bool IsVisible { get; set; }

		private Vector3 _Normal;
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

		/// <summary>Gets the owner of the actual <b>DXF</b> object.</summary>
		public new Insert Owner
		{
			get => (Insert)base.Owner;
			internal set => base.Owner = value;
		}

		/// <summary>Gets the attribute definition.</summary>
		/// <remarks>If the insert attribute has no definition it will return <see langword="null"/>.</remarks>
		public AttributeDefinition Definition { get; internal set; }

		/// <summary>Gets the attribute tag.</summary>
		public string Tag { get; }

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

		private double _Width;
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
		/// <summary>Gets or sets the attribute value.</summary>
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

				_Style = this.OnTextStyleChangedEvent(_Style, value);
			}
		}

		/// <summary>Gets or sets the attribute <see cref="Vector3">position</see>.</summary>
		public Vector3 Position { get; set; }

		/// <summary>Gets or sets the attribute flags.</summary>
		public AttributeFlags Flags { get; set; }

		private double _Rotation;
		/// <summary>Gets or sets the attribute text rotation in degrees.</summary>
		public double Rotation
		{
			get => _Rotation;
			set => _Rotation = MathHelper.NormalizeAngle(value);
		}

		/// <summary>Gets or sets the text alignment.</summary>
		public TextAlignment Alignment { get; set; }

		/// <summary>Gets or sets if the attribute text is backward (mirrored in X).</summary>
		public bool IsBackward { get; set; }

		/// <summary>Gets or sets if the attribute text is upside down (mirrored in Y).</summary>
		public bool IsUpsideDown { get; set; }

		#endregion

		#region public methods

		/// <summary>Moves, scales, and/or rotates the current attribute given a 3x3 transformation matrix and a translation vector.</summary>
		/// <param name="transformation">Transformation matrix.</param>
		/// <param name="translation">Translation vector.</param>
		/// <remarks>Matrix3 adopts the convention of using column vectors to represent a transformation matrix.</remarks>
		public void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			bool mirrText;
			if (this.Owner == null)
			{
				mirrText = Text.DefaultMirrText;
			}
			else if (this.Owner.Owner == null)
			{
				mirrText = Text.DefaultMirrText;
			}
			else
			{
				mirrText = this.Owner.Owner.Record.Owner.Owner.DrawingVariables.MirrText;
			}

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
				if (Vector2.CrossProduct(newUvector, newVvector) < 0.0)
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
			Attribute entity = new Attribute(this.Tag)
			{
				//Attribute properties
				Layer = (Layer)this.Layer.Clone(),
				Linetype = (Linetype)this.Linetype.Clone(),
				Color = (AciColor)this.Color.Clone(),
				Lineweight = this.Lineweight,
				Transparency = (Transparency)this.Transparency.Clone(),
				LinetypeScale = this.LinetypeScale,
				Normal = this.Normal,
				IsVisible = this.IsVisible,
				Definition = (AttributeDefinition)this.Definition?.Clone(),
				Height = _Height,
				Width = _Width,
				WidthFactor = _WidthFactor,
				ObliqueAngle = _ObliqueAngle,
				Value = _Value,
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