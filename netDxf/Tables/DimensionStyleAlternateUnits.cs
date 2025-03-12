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
using netDxf.Units;

namespace netDxf.Tables
{
	/// <summary>Represents the way alternate units are formatted in dimension entities.</summary>
	/// <remarks>Alternative units are not applicable for angular dimensions.</remarks>
	public class DimensionStyleAlternateUnits :
		ICloneable
	{
		#region private fields

		private short dimaltd;
		private double dimaltf;
		private double dimaltrnd;
		private string dimPrefix;
		private string dimSuffix;

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public DimensionStyleAlternateUnits()
		{
			this.Enabled = false;
			this.dimaltd = 2;
			this.dimPrefix = string.Empty;
			this.dimSuffix = string.Empty;
			this.dimaltf = 25.4;
			this.LengthUnits = LinearUnitType.Decimal;
			this.StackUnits = false;
			this.SuppressLinearLeadingZeros = false;
			this.SuppressLinearTrailingZeros = false;
			this.SuppressZeroFeet = true;
			this.SuppressZeroInches = true;
			this.dimaltrnd = 0.0;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets if the alternate measurement units are added to the dimension text. (<b>DIMALT</b>)</summary>
		public bool Enabled { get; set; }

		/// <summary>Sets the number of decimal places displayed for the alternate units of a dimension. (<b>DIMALTD</b>)</summary>
		/// <remarks>
		/// Default: 4<br/>
		/// It is recommended to use values in the range 0 to 8.<br/>
		/// For architectural and fractional the precision used for the minimum fraction is 1/2^LinearDecimalPlaces.
		/// </remarks>
		public short LengthPrecision
		{
			get => this.dimaltd;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The length precision must be equals or greater than zero.");
				}
				this.dimaltd = value;
			}
		}

		/// <summary>Specifies the text prefix for the dimension. (<b>DIMAPOST</b>)</summary>
		public string Prefix
		{
			get => this.dimPrefix;
			set => this.dimPrefix = value ?? string.Empty;
		}

		/// <summary>Specifies the text suffix for the dimension. (<b>DIMAPOST</b>)</summary>
		public string Suffix
		{
			get => this.dimSuffix;
			set => this.dimSuffix = value ?? string.Empty;
		}

		/// <summary>Gets or sets the multiplier used as the conversion factor between primary and alternate units. (<b>DIMALTF</b>)</summary>
		/// <remarks>
		/// to convert inches to millimeters, enter 25.4.
		/// The value has no effect on angular dimensions, and it is not applied to the rounding value or the plus or minus tolerance values.
		/// </remarks>
		public double Multiplier
		{
			get => this.dimaltf;
			set
			{
				if (value <= 0.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The multiplier for alternate units must be greater than zero0.");
				}
				this.dimaltf = value;
			}
		}

		/// <summary>Gets or sets the alternate units for all dimension types except angular. (<b>DIMALTU</b>)</summary>
		/// <remarks>
		/// Scientific<br/>
		/// Decimal<br/>
		/// Engineering<br/>
		/// Architectural<br/>
		/// Fractional
		/// </remarks>
		public LinearUnitType LengthUnits { get; set; }

		/// <summary>Gets or set if the <see cref="LinearUnitType.Architectural"/> or <see cref="LinearUnitType.Fractional"/> linear units will be shown stacked or not. (<b>DIMALTU</b>)</summary>
		/// <remarks>
		/// This value only is applicable if the <see cref="LengthUnits"/> property has been set to <see cref="LinearUnitType.Architectural"/> or <see cref="LinearUnitType.Fractional"/>,
		/// for any other value this parameter is not applicable.
		/// </remarks>
		public bool StackUnits { get; set; }

		/// <summary>Suppresses leading zeros in linear decimal alternate units. (<b>DIMALTZ</b>)</summary>
		/// <remarks>This value is part of the <b>DIMALTZ</b> variable.</remarks>
		public bool SuppressLinearLeadingZeros { get; set; }

		/// <summary>Suppresses trailing zeros in linear decimal alternate units. (<b>DIMALTZ</b>)</summary>
		/// <remarks>This value is part of the <b>DIMALTZ</b> variable.</remarks>
		public bool SuppressLinearTrailingZeros { get; set; }

		/// <summary>Suppresses zero feet in architectural alternate units. (<b>DIMALTZ</b>)</summary>
		/// <remarks>This value is part of the <b>DIMALTZ</b> variable.</remarks>
		public bool SuppressZeroFeet { get; set; }

		/// <summary>Suppresses zero inches in architectural alternate units. (<b>DIMALTZ</b>)</summary>
		/// <remarks>This value is part of the <b>DIMALTZ</b> variable.</remarks>
		public bool SuppressZeroInches { get; set; }

		/// <summary>Gets or sets the value to round all dimensioning distances. (<b>DIMALTRND</b>)</summary>
		/// <remarks>
		/// Default: 0 (no rounding off).<br/>
		/// If <b>DIMRND</b> is set to 0.25, all distances round to the nearest 0.25 unit.
		/// If you set <b>DIMRND</b> to 1.0, all distances round to the nearest integer.
		/// Note that the number of digits edited after the decimal point depends on the precision set by DIMDEC.
		/// <b>DIMRND</b> does not apply to angular dimensions.
		/// </remarks>
		public double Roundoff
		{
			get => this.dimaltrnd;
			set
			{
				if (value < 0.000001 && !MathHelper.IsZero(value, double.Epsilon))
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The nearest value to round all distances must be equal or greater than 0.000001 or zero (no rounding off).");
				}
				this.dimaltrnd = value;
			}
		}

		#endregion

		#region implements ICloneable

		/// <inheritdoc/>
		public object Clone()
		{
			DimensionStyleAlternateUnits copy = new DimensionStyleAlternateUnits()
			{
				Enabled = this.Enabled,
				LengthUnits = this.LengthUnits,
				StackUnits = this.StackUnits,
				LengthPrecision = this.dimaltd,
				Multiplier = this.dimaltf,
				Roundoff = this.dimaltrnd,
				Prefix = this.dimPrefix,
				Suffix = this.dimSuffix,
				SuppressLinearLeadingZeros = this.SuppressLinearLeadingZeros,
				SuppressLinearTrailingZeros = this.SuppressLinearTrailingZeros,
				SuppressZeroFeet = this.SuppressZeroFeet,
				SuppressZeroInches = this.SuppressZeroInches
			};

			return copy;
		}

		#endregion
	}
}