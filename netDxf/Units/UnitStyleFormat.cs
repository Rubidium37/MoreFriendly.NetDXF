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

namespace netDxf.Units
{
	/// <summary>Represents the parameters to convert linear and angular units to its string representation.</summary>
	public class UnitStyleFormat
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public UnitStyleFormat()
		{
		}

		#endregion

		#region public properties

		private short _LinearDecimalPlaces = 2;
		/// <summary>Gets or sets the number of decimal places for linear units.</summary>
		/// <remarks>
		/// For architectural and fractional the precision used for the minimum fraction is 1/2^LinearDecimalPlaces.
		/// </remarks>
		public short LinearDecimalPlaces
		{
			get => _LinearDecimalPlaces;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The number of decimal places must be equals or greater than zero.");
				}
				_LinearDecimalPlaces = value;
			}
		}

		private short _AngularDecimalPlaces = 0;
		/// <summary>Gets or sets the number of decimal places for angular units.</summary>
		public short AngularDecimalPlaces
		{
			get => _AngularDecimalPlaces;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The number of decimal places must be equals or greater than zero.");
				}
				_AngularDecimalPlaces = value;
			}
		}

		/// <summary>Gets or set the decimal separator.</summary>
		public string DecimalSeparator { get; set; } = ".";

		/// <summary>Gets or sets the separator between feet and inches.</summary>
		public string FeetInchesSeparator { get; set; } = "-";

		/// <summary>Gets or set the angle degrees symbol.</summary>
		public string DegreesSymbol { get; set; } = "°";

		/// <summary>Gets or set the angle minutes symbol.</summary>
		public string MinutesSymbol { get; set; } = "\'";

		/// <summary>Gets or set the angle seconds symbol.</summary>
		public string SecondsSymbol { get; set; } = "\"";

		/// <summary>Gets or set the angle radians symbol.</summary>
		public string RadiansSymbol { get; set; } = "r";

		/// <summary>Gets or set the angle gradians symbol.</summary>
		public string GradiansSymbol { get; set; } = "g";

		/// <summary>Gets or set the feet symbol.</summary>
		public string FeetSymbol { get; set; } = "\'";

		/// <summary>Gets or set the inches symbol.</summary>
		public string InchesSymbol { get; set; } = "\"";

		private double _FractionHeightScale = 1.0;
		/// <summary>Gets or sets the scale of fractions relative to dimension text height.</summary>
		public double FractionHeightScale
		{
			get => _FractionHeightScale;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The fraction height scale must be greater than zero.");
				}
				_FractionHeightScale = value;
			}
		}

		/// <summary>Gets or sets the fraction format for architectural or fractional units.</summary>
		/// <remarks>
		/// Horizontal stacking<br/>
		/// Diagonal stacking<br/>
		/// Not stacked (for example, 1/2)
		/// </remarks>
		public FractionFormatType FractionType { get; set; } = FractionFormatType.Horizontal;

		/// <summary>Suppresses leading zeros in linear decimal dimensions (for example, 0.5000 becomes .5000).</summary>
		public bool SuppressLinearLeadingZeros { get; set; } = false;

		/// <summary>Suppresses trailing zeros in linear decimal dimensions (for example, 12.5000 becomes 12.5).</summary>
		public bool SuppressLinearTrailingZeros { get; set; } = false;

		/// <summary>Suppresses leading zeros in angular decimal dimensions (for example, 0.5000 becomes .5000).</summary>
		public bool SuppressAngularLeadingZeros { get; set; } = false;

		/// <summary>Suppresses trailing zeros in angular decimal dimensions (for example, 12.5000 becomes 12.5).</summary>
		public bool SuppressAngularTrailingZeros { get; set; } = false;

		/// <summary>Suppresses zero feet in architectural dimensions.</summary>
		public bool SuppressZeroFeet { get; set; } = true;

		/// <summary>Suppresses zero inches in architectural dimensions.</summary>
		public bool SuppressZeroInches { get; set; } = true;

		#endregion
	}
}