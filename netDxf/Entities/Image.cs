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
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a raster image <see cref="EntityObject">entity</see>.</summary>
	public class Image :
		EntityObject
	{
		#region delegates and events

		/// <summary>Generated when a property of <see cref="ImageDefinition"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<ImageDefinition> BeforeChangingImageDefinitionValue;
		/// <summary>Generates the <see cref="BeforeChangingImageDefinitionValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual ImageDefinition OnBeforeChangingImageDefinitionValue(ImageDefinition oldValue, ImageDefinition newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingImageDefinitionValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<ImageDefinition>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		#endregion

		#region constructors

		internal Image()
			: base(EntityType.Image, DxfObjectCode.Image)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="imageDefinition">Image definition.</param>
		/// <param name="position">Image <see cref="Vector2">position</see> in world coordinates.</param>
		/// <param name="size">Image <see cref="Vector2">size</see> in world coordinates.</param>
		public Image(ImageDefinition imageDefinition, Vector2 position, Vector2 size)
			: this(imageDefinition, new Vector3(position.X, position.Y, 0.0), size.X, size.Y)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="imageDefinition">Image definition.</param>
		/// <param name="position">Image <see cref="Vector3">position</see> in world coordinates.</param>
		/// <param name="size">Image <see cref="Vector2">size</see> in world coordinates.</param>
		public Image(ImageDefinition imageDefinition, Vector3 position, Vector2 size)
			: this(imageDefinition, position, size.X, size.Y)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="imageDefinition">Image definition.</param>
		/// <param name="position">Image <see cref="Vector2">position</see> in world coordinates.</param>
		/// <param name="width">Image width in world coordinates.</param>
		/// <param name="height">Image height in world coordinates.</param>
		public Image(ImageDefinition imageDefinition, Vector2 position, double width, double height)
			: this(imageDefinition, new Vector3(position.X, position.Y, 0.0), width, height)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="imageDefinition">Image definition.</param>
		/// <param name="position">Image <see cref="Vector3">position</see> in world coordinates.</param>
		/// <param name="width">Image width in world coordinates.</param>
		/// <param name="height">Image height in world coordinates.</param>
		public Image(ImageDefinition imageDefinition, Vector3 position, double width, double height)
			: base(EntityType.Image, DxfObjectCode.Image)
		{
			_Definition = imageDefinition ?? throw new ArgumentNullException(nameof(imageDefinition));
			this.Position = position;
			_Uvector = Vector2.UnitX;
			_Vvector = Vector2.UnitY;
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width), width, "The Image width must be greater than zero.");
			}
			_Width = width;
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height), height, "The Image height must be greater than zero.");
			}
			_Height = height;
			this.Clipping = false;
			_Brightness = 50;
			_Contrast = 50;
			_Fade = 0;
			this.DisplayOptions = ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowImageWhenNotAlignedWithScreen | ImageDisplayFlags.UseClippingBoundary;
			_ClippingBoundary = new ClippingBoundary(0, 0, imageDefinition.Width, imageDefinition.Height);
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the image <see cref="Vector3">position</see> in world coordinates.</summary>
		public Vector3 Position { get; set; }

		private Vector2 _Uvector;
		/// <summary>Gets or sets the image <see cref="Vector2">U-vector</see>.</summary>
		public Vector2 Uvector
		{
			get => _Uvector;
			set
			{
				if (Vector2.Equals(Vector2.Zero, value))
				{
					throw new ArgumentException("The U vector can not be the zero vector.", nameof(value));
				}

				_Uvector = Vector2.Normalize(value);
			}
		}

		private Vector2 _Vvector;
		/// <summary>Gets or sets the image <see cref="Vector2">V-vector</see>.</summary>
		public Vector2 Vvector
		{
			get => _Vvector;
			set
			{
				if (Vector2.Equals(Vector2.Zero, value))
				{
					throw new ArgumentException("The V vector can not be the zero vector.", nameof(value));
				}

				_Vvector = Vector2.Normalize(value);
			}
		}

		private double _Height;
		/// <summary>Gets or sets the height of the image in drawing units.</summary>
		public double Height
		{
			get => _Height;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The Image height must be greater than zero.");
				}
				_Height = value;
			}
		}

		private double _Width;
		/// <summary>Gets or sets the width of the image in drawing units.</summary>
		public double Width
		{
			get => _Width;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The Image width must be greater than zero.");
				}
				_Width = value;
			}
		}

		/// <summary>Gets or sets the image rotation in degrees.</summary>
		/// <remarks>The image rotation is the angle of the U-vector.</remarks>
		public double Rotation
		{
			get
			{
				return Vector2.Angle(_Uvector) * MathHelper.RadToDeg;
			}
			set
			{
				List<Vector2> uv = MathHelper.Transform(new List<Vector2> { _Uvector, _Vvector },
					MathHelper.NormalizeAngle(value) * MathHelper.DegToRad,
					CoordinateSystem.Object, CoordinateSystem.World);
				_Uvector = uv[0];
				_Vvector = uv[1];
			}
		}

		private ImageDefinition _Definition;
		/// <summary>Gets the <see cref="ImageDefinition">image definition</see>.</summary>
		public ImageDefinition Definition
		{
			get => _Definition;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Definition = this.OnBeforeChangingImageDefinitionValue(_Definition, value);
			}
		}

		/// <summary>Gets or sets the clipping state: <see langword="false"/> = off, <see langword="true"/> = on.</summary>
		public bool Clipping { get; set; }

		private short _Brightness;
		/// <summary>Gets or sets the brightness value (0-100; default = 50)</summary>
		public short Brightness
		{
			get => _Brightness;
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted brightness values range from 0 to 100.");
				}
				_Brightness = value;
			}
		}

		private short _Contrast;
		/// <summary>Gets or sets the contrast value (0-100; default = 50)</summary>
		public short Contrast
		{
			get => _Contrast;
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted contrast values range from 0 to 100.");
				}
				_Contrast = value;
			}
		}

		private short _Fade;
		/// <summary>Gets or sets the fade value (0-100; default = 0)</summary>
		public short Fade
		{
			get => _Fade;
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted fade values range from 0 to 100.");
				}
				_Fade = value;
			}
		}

		/// <summary>Gets or sets the image display options.</summary>
		public ImageDisplayFlags DisplayOptions { get; set; }

		private ClippingBoundary _ClippingBoundary;
		/// <summary>Gets or sets the image clipping boundary.</summary>
		/// <remarks>
		/// The vertexes coordinates of the clipping boundary are expressed in local coordinates of the image in pixels.
		/// Set as <see langword="null"/> to restore the default clipping boundary, full image.
		/// </remarks>
		public ClippingBoundary ClippingBoundary
		{
			get => _ClippingBoundary;
			set => _ClippingBoundary = value ?? new ClippingBoundary(0, 0, this.Definition.Width, this.Definition.Height);
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			Vector3 newPosition = transformation * this.Position + translation;
			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}

			Matrix3 transOW = MathHelper.ArbitraryAxis(this.Normal);

			Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal);
			transWO = transWO.Transpose();

			Vector3 v;
			v = transOW * new Vector3(this.Uvector.X * this.Width, this.Uvector.Y * this.Width, 0.0);
			v = transformation * v;
			v = transWO * v;
			Vector2 newUvector = new Vector2(v.X, v.Y);

			double newWidth;
			if (Vector2.Equals(Vector2.Zero, newUvector))
			{
				newUvector = this.Uvector;
				newWidth = MathHelper.Epsilon;
			}
			else
			{
				newWidth = newUvector.Modulus();
			}

			v = transOW * new Vector3(this.Vvector.X * this.Height, this.Vvector.Y * this.Height, 0.0);
			v = transformation * v;
			v = transWO * v;
			Vector2 newVvector = new Vector2(v.X, v.Y);

			double newHeight;
			if (Vector2.Equals(Vector2.Zero, newVvector))
			{
				newVvector = this.Uvector;
				newHeight = MathHelper.Epsilon;
			}
			else
			{
				newHeight = newVvector.Modulus();
			}

			this.Position = newPosition;
			this.Normal = newNormal;
			this.Uvector = newUvector;
			this.Vvector = newVvector;
			this.Width = newWidth;
			this.Height = newHeight;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Image entity = new Image
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
				//Image properties
				Position = this.Position,
				Height = _Height,
				Width = _Width,
				Uvector = _Uvector,
				Vvector = _Vvector,
				//Rotation = _Rotation,
				Definition = (ImageDefinition)_Definition.Clone(),
				Clipping = this.Clipping,
				Brightness = _Brightness,
				Contrast = _Contrast,
				Fade = _Fade,
				DisplayOptions = this.DisplayOptions,
				ClippingBoundary = (ClippingBoundary)_ClippingBoundary.Clone()
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