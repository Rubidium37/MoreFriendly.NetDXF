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
	/// <summary>Represents a generic entity.</summary>
	public abstract class EntityObject :
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

		#endregion

		#region constructors

		protected EntityObject(EntityType type, string dxfCode)
			: base(dxfCode)
		{
			this.Type = type;
		}

		#endregion

		#region public properties

		private readonly List<DxfObject> _Reactors = new List<DxfObject>();
		/// <summary>Gets the list of <b>DXF</b> objects that has been attached to this entity.</summary>
		public IReadOnlyList<DxfObject> Reactors => _Reactors;

		/// <summary>Gets the entity <see cref="EntityType">type</see>.</summary>
		public EntityType Type { get; }

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

		/// <summary>Gets or sets the entity <see cref="Lineweight">line weight</see>, one unit is always 1/100 mm (default = ByLayer).</summary>
		public Lineweight Lineweight { get; set; } = Lineweight.ByLayer;

		private Transparency _Transparency = Transparency.ByLayer;
		/// <summary>Gets or sets layer <see cref="Transparency">transparency</see> (default: ByLayer).</summary>
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

		/// <summary>Gets the owner of the actual <b>DXF</b> object.</summary>
		public new Block Owner
		{
			get => (Block)base.Owner;
			internal set => base.Owner = value;
		}

		#endregion

		#region internal methods

		internal void AddReactor(DxfObject o) => _Reactors.Add(o);

		internal bool RemoveReactor(DxfObject o) => _Reactors.Remove(o);

		#endregion

		#region public methods

		/// <summary>Moves, scales, and/or rotates the current entity given a 3x3 transformation matrix and a translation vector.</summary>
		/// <param name="transformation">Transformation matrix.</param>
		/// <param name="translation">Translation vector.</param>
		/// <remarks>Matrix3 adopts the convention of using column vectors to represent a transformation matrix.</remarks>
		public abstract void TransformBy(Matrix3 transformation, Vector3 translation);

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
		public override string ToString() => this.Type.ToString();

		#endregion

		#region ICloneable

		/// <inheritdoc/>
		public abstract object Clone();

		#endregion
	}
}