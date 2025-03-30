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
	/// <summary>Represents an underlay <see cref="EntityObject">entity</see>.</summary>
	public class Underlay :
		EntityObject
	{
		#region delegates and events

		/// <summary>Generated when a property of <see cref="UnderlayDefinition"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<UnderlayDefinition> BeforeChangingUnderlayDefinitionValue;
		/// <summary>Generates the <see cref="BeforeChangingUnderlayDefinitionValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual UnderlayDefinition OnBeforeChangingUnderlayDefinitionValue(UnderlayDefinition oldValue, UnderlayDefinition newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingUnderlayDefinitionValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<UnderlayDefinition>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		#endregion

		#region constructor

		internal Underlay()
			: base(EntityType.Underlay, DxfObjectCode.Underlay)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="definition"><see cref="UnderlayDefinition">Underlay definition</see>.</param>
		public Underlay(UnderlayDefinition definition)
			: this(definition, Vector3.Zero, 1.0)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="definition"><see cref="UnderlayDefinition">Underlay definition</see>.</param>
		/// <param name="position">Underlay <see cref="Vector3">position</see> in world coordinates.</param>
		public Underlay(UnderlayDefinition definition, Vector3 position)
			: this(definition, position, 1.0)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="definition"><see cref="UnderlayDefinition">Underlay definition</see>.</param>
		/// <param name="position">Underlay <see cref="Vector3">position</see> in world coordinates.</param>
		/// <param name="scale">Underlay scale.</param>
		public Underlay(UnderlayDefinition definition, Vector3 position, double scale)
			: base(EntityType.Underlay, DxfObjectCode.Underlay)
		{
			_Definition = definition ?? throw new ArgumentNullException(nameof(definition));
			this.Position = position;
			if (scale <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(scale), scale, "The Underlay scale must be greater than zero.");
			}
			_Scale = new Vector2(scale);
			switch (_Definition.Type)
			{
				case UnderlayType.DGN:
					this.CodeName = DxfObjectCode.UnderlayDgn;
					break;
				case UnderlayType.DWF:
					this.CodeName = DxfObjectCode.UnderlayDwf;
					break;
				case UnderlayType.PDF:
					this.CodeName = DxfObjectCode.UnderlayPdf;
					break;
			}
		}

		#endregion

		#region public properties

		private UnderlayDefinition _Definition;
		/// <summary>Gets the underlay definition.</summary>
		public UnderlayDefinition Definition
		{
			get => _Definition;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Definition = this.OnBeforeChangingUnderlayDefinitionValue(_Definition, value);

				switch (value.Type)
				{
					case UnderlayType.DGN:
						this.CodeName = DxfObjectCode.UnderlayDgn;
						break;
					case UnderlayType.DWF:
						this.CodeName = DxfObjectCode.UnderlayDwf;
						break;
					case UnderlayType.PDF:
						this.CodeName = DxfObjectCode.UnderlayPdf;
						break;
				}
			}
		}

		/// <summary>Gets or sets the underlay position in world coordinates.</summary>
		public Vector3 Position { get; set; }

		private Vector2 _Scale;
		/// <summary>Gets or sets the underlay scale.</summary>
		/// <remarks>
		/// Any of the vector scale components cannot be zero.<br />
		/// Even thought the <b>DXF</b> has a code for the Z scale it seems that it has no use.
		/// The X and Y components multiplied by the original size of the <b>PDF</b> page represent the width and height of the final underlay.
		/// The Z component even thought it is present in the <b>DXF</b> it seems it has no use.
		/// </remarks>
		public Vector2 Scale
		{
			get => _Scale;
			set
			{
				if (MathHelper.IsZero(value.X) || MathHelper.IsZero(value.Y))
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Any of the vector scale components cannot be zero.");
				}
				_Scale = value;
			}
		}

		private double _Rotation = 0.0;
		/// <summary>Gets or sets the underlay rotation around its normal.</summary>
		public double Rotation
		{
			get => _Rotation;
			set => _Rotation = MathHelper.NormalizeAngle(value);
		}

		private short _Contrast = 100;
		/// <summary>Gets or sets the underlay contrast.</summary>
		/// <remarks>Valid values range from 20 to 100.</remarks>
		public short Contrast
		{
			get => _Contrast;
			set
			{
				if (value < 20 || value > 100)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted contrast values range from 20 to 100.");
				}
				_Contrast = value;
			}
		}

		private short _Fade = 0;
		/// <summary>Gets or sets the underlay fade.</summary>
		/// <remarks>Valid values range from 0 to 80.</remarks>
		public short Fade
		{
			get => _Fade;
			set
			{
				if (value < 0 || value > 80)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted fade values range from 0 to 80.");
				}
				_Fade = value;
			}
		}

		/// <summary>Gets or sets the underlay display options.</summary>
		public UnderlayDisplayFlags DisplayOptions { get; set; } = UnderlayDisplayFlags.ShowUnderlay;

		/// <summary>Gets or sets the underlay clipping boundary.</summary>
		/// <remarks>
		/// Set as <see langword="null"/> to restore the default clipping boundary, show the full underlay without clipping.
		/// </remarks>
		public ClippingBoundary ClippingBoundary { get; set; }

		#endregion

		#region overrides

		/// <inheritdoc/>
		/// <remarks>
		/// Non-uniform scaling for rotated underlays is not supported.
		/// This is not a limitation of the code but the <b>DXF</b> format, unlike the Image there is no way to define the local <b>UV</b> vectors.<br />
		/// Matrix3 adopts the convention of using column vectors to represent a transformation matrix.
		/// </remarks>
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

			List<Vector2> uv = MathHelper.Transform(
				new[]
				{
					this.Scale.X * Vector2.UnitX,
					this.Scale.Y * Vector2.UnitY
				},
				_Rotation * MathHelper.DegToRad,
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

			int sign = Math.Sign(transformation.M11 * transformation.M22 * transformation.M33) < 0 ? -1 : 1;

			double scaleX = sign * newUvector.Modulus();
			scaleX = MathHelper.IsZero(scaleX) ? MathHelper.Epsilon : scaleX;
			double scaleY = newVvector.Modulus();
			scaleY = MathHelper.IsZero(scaleY) ? MathHelper.Epsilon : scaleY;

			Vector2 newScale = new Vector2(scaleX, scaleY);
			double newRotation = Vector2.Angle(sign * newUvector) * MathHelper.RadToDeg;

			this.Position = newPosition;
			this.Normal = newNormal;
			this.Rotation = newRotation;
			this.Scale = newScale;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Underlay entity = new Underlay
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
				//Underlay properties
				Definition = (UnderlayDefinition)_Definition.Clone(),
				Position = this.Position,
				Scale = _Scale,
				Rotation = _Rotation,
				Contrast = _Contrast,
				Fade = _Fade,
				DisplayOptions = this.DisplayOptions,
				ClippingBoundary = this.ClippingBoundary != null ? (ClippingBoundary)this.ClippingBoundary.Clone() : null
			};

			foreach (XData data in this.XData.Values)
				entity.XData.Add((XData)data.Clone());

			return entity;
		}

		#endregion
	}
}