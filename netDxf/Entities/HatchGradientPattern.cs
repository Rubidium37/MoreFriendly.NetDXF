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

namespace netDxf.Entities
{
	/// <summary>Represents the hatch gradient pattern style.</summary>
	/// <remarks>
	/// Gradient patterns are only supported by AutoCad2004 and higher <b>DXF</b> versions. It will default to a solid pattern if saved as AutoCad2000.
	/// </remarks>
	public class HatchGradientPattern :
		HatchPattern
	{
		#region constructors

		/// <summary>Initializes a new instance of the class as a default linear gradient.</summary>
		public HatchGradientPattern()
			: this(string.Empty)
		{
		}
		/// <summary>Initializes a new instance of the class as a default linear gradient.</summary>
		/// <param name="description">Description of the pattern (optional, this information is not saved in the <b>DXF</b> file). By default it will use the supplied name.</param>
		public HatchGradientPattern(string description)
			: base("SOLID", description)
		{
			_Color1 = AciColor.Blue;
			_Color2 = AciColor.Yellow;
			_SingleColor = false;
			this.GradientType = HatchGradientPatternType.Linear;
			_Tint = 1.0;
		}
		/// <summary>Initializes a new instance of the class as a single color gradient.</summary>
		/// <param name="color">Gradient <see cref="AciColor">color</see>.</param>
		/// <param name="tint">Gradient tint.</param>
		/// <param name="type">Gradient <see cref="HatchGradientPatternType">type</see>.</param>
		public HatchGradientPattern(AciColor color, double tint, HatchGradientPatternType type)
			: this(color, tint, type, string.Empty)
		{
		}
		/// <summary>Initializes a new instance of the class as a single color gradient.</summary>
		/// <param name="color">Gradient <see cref="AciColor">color</see>.</param>
		/// <param name="tint">Gradient tint.</param>
		/// <param name="type">Gradient <see cref="HatchGradientPatternType">type</see>.</param>
		/// <param name="description">Description of the pattern (optional, this information is not saved in the <b>DXF</b> file). By default it will use the supplied name.</param>
		public HatchGradientPattern(AciColor color, double tint, HatchGradientPatternType type, string description)
			: base("SOLID", description)
		{
			_Color1 = color ?? throw new ArgumentNullException(nameof(color));
			_Color2 = this.Color2FromTint(tint);
			_SingleColor = true;
			this.GradientType = type;
			_Tint = tint;
		}
		/// <summary>Initializes a new instance of the class as a two color gradient.</summary>
		/// <param name="color1">Gradient <see cref="AciColor">color</see> 1.</param>
		/// <param name="color2">Gradient <see cref="AciColor">color</see> 2.</param>
		/// <param name="type">Gradient <see cref="HatchGradientPatternType">type</see>.</param>
		public HatchGradientPattern(AciColor color1, AciColor color2, HatchGradientPatternType type)
			: this(color1, color2, type, string.Empty)
		{
		}
		/// <summary>Initializes a new instance of the class as a two color gradient.</summary>
		/// <param name="color1">Gradient <see cref="AciColor">color</see> 1.</param>
		/// <param name="color2">Gradient <see cref="AciColor">color</see> 2.</param>
		/// <param name="type">Gradient <see cref="HatchGradientPatternType">type</see>.</param>
		/// <param name="description">Description of the pattern (optional, this information is not saved in the <b>DXF</b> file). By default it will use the supplied name.</param>
		public HatchGradientPattern(AciColor color1, AciColor color2, HatchGradientPatternType type, string description)
			: base("SOLID", description)
		{
			_Color1 = color1 ?? throw new ArgumentNullException(nameof(color1));
			_Color2 = color2 ?? throw new ArgumentNullException(nameof(color2));
			_SingleColor = false;
			this.GradientType = type;
			_Tint = 1.0;
		}

		#endregion

		#region public properties

		/// <summary>Gets or set the gradient pattern <see cref="HatchGradientPatternType">type</see>.</summary>
		public HatchGradientPatternType GradientType { get; set; }

		private AciColor _Color1;
		/// <summary>Gets or sets the gradient <see cref="AciColor">color</see> 1.</summary>
		public AciColor Color1
		{
			get => _Color1;
			set => _Color1 = value ?? throw new ArgumentNullException(nameof(value));
		}

		private AciColor _Color2;
		/// <summary>Gets or sets the gradient <see cref="AciColor">color</see> 2.</summary>
		/// <remarks>
		/// If color 2 is defined, automatically the single color property will be set to <see langword="false"/>.
		/// </remarks>
		public AciColor Color2
		{
			get => _Color2;
			set
			{
				_SingleColor = false;
				_Color2 = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		private bool _SingleColor;
		/// <summary>Gets or sets the gradient pattern color type.</summary>
		public bool SingleColor
		{
			get => _SingleColor;
			set
			{
				if (value)
					this.Color2 = this.Color2FromTint(_Tint);
				_SingleColor = value;
			}
		}

		private double _Tint;
		/// <summary>Gets or sets the gradient pattern tint.</summary>
		/// <remarks>It only applies to single color gradient patterns.</remarks>
		public double Tint
		{
			get => _Tint;
			set
			{
				if (_SingleColor)
					this.Color2 = this.Color2FromTint(value);
				_Tint = value;
			}
		}

		/// <summary>Gets or sets if the pattern is centered or not.</summary>
		/// <remarks>
		/// Each gradient has two definitions, shifted and unsifted. A shift value describes the blend of the two definitions that should be used.
		/// A value of 0.0 (false) means only the unsifted version should be used, and a value of 1.0 (true) means that only the shifted version should be used.
		/// </remarks>
		public bool Centered { get; set; } = true;

		#endregion

		#region private methods

		private AciColor Color2FromTint(double value)
		{
			AciColor.ToHsl(_Color1, out double h, out double s, out double _);
			return AciColor.FromHsl(h, s, value);
		}

		#endregion

		#region ICloneable

		/// <inheritdoc/>
		public override object Clone()
		{
			HatchGradientPattern copy = new HatchGradientPattern
			{
				// Pattern
				Fill = this.Fill,
				Type = this.Type,
				Origin = this.Origin,
				Angle = this.Angle,
				Scale = this.Scale,
				Style = this.Style,
				// GraientPattern
				GradientType = this.GradientType,
				Color1 = (AciColor)_Color1.Clone(),
				Color2 = (AciColor)_Color2.Clone(),
				SingleColor = _SingleColor,
				Tint = _Tint,
				Centered = this.Centered
			};

			return copy;
		}

		#endregion
	}
}