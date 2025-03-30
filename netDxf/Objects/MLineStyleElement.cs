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
using System.Runtime.CompilerServices;
using netDxf.Tables;

namespace netDxf.Objects
{
	/// <summary>Represent each of the elements that make up a MLineStyle.</summary>
	public class MLineStyleElement :
		IComparable<MLineStyleElement>,
		IEquatable<MLineStyleElement>,
		ICloneable
	{
		#region delegates and events

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

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="offset">Element offset.</param>
		public MLineStyleElement(double offset)
			: this(offset, AciColor.ByLayer, Linetype.ByLayer)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="offset">Element offset.</param>
		/// <param name="color">Element color.</param>
		/// <param name="linetype">Element line type.</param>
		public MLineStyleElement(double offset, AciColor color, Linetype linetype)
		{
			this.Offset = offset;
			_Color = color;
			_Linetype = linetype;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the element offset.</summary>
		public double Offset { get; set; }

		private AciColor _Color;
		/// <summary>Gets or sets the element color.</summary>
		/// <remarks>
		/// AutoCad2000 <b>DXF</b> version does not support <see langword="true"/> colors for MLineStyleElement color.
		/// </remarks>
		public AciColor Color
		{
			get => _Color;
			set => _Color = value ?? throw new ArgumentNullException(nameof(value));
		}

		private Linetype _Linetype;
		/// <summary>Gets or sets the element line type.</summary>
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

		#endregion

		#region implements IComparable

		/// <inheritdoc/>
		/// <remarks>
		/// The MLineStyleElements are ordered from larger to smaller offset values.
		/// A 32-bit signed integer that indicates the relative order of the objects being compared.
		/// The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.
		/// Zero This object is equal to other. Greater than zero This object is greater than other.
		/// </remarks>
		public int CompareTo(MLineStyleElement other)
		{
			if (other == null)
			{
				throw new ArgumentNullException(nameof(other));
			}

			return -this.Offset.CompareTo(other.Offset);
		}

		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is MLineStyleElement other && this.Equals(other);
		/// <inheritdoc/>
		public bool Equals(MLineStyleElement other)
		{
			if (other == null)
			{
				return false;
			}

			return MathHelper.IsEqual(this.Offset, other.Offset);
		}

		/// <inheritdoc/>
		public override int GetHashCode() => this.Offset.GetHashCode();

		#endregion

		#region implements ICloneable

		/// <inheritdoc/>
		public object Clone()
			=> new MLineStyleElement(this.Offset)
			{
				Color = (AciColor)this.Color.Clone(),
				Linetype = (Linetype)_Linetype.Clone()
			};

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
			=> string.Format("{0}, color:{1}, line type:{2}", this.Offset, _Color, _Linetype);

		#endregion
	}
}