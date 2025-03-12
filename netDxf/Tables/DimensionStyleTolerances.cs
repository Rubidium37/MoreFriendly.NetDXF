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

namespace netDxf.Tables
{
	/// <summary>Represents the way tolerances are formatted in dimension entities</summary>
	public class DimensionStyleTolerances :
		ICloneable
	{
		#region private fields

		private short dimtdec;
		private short dimalttd;

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public DimensionStyleTolerances()
		{
			this.DisplayMethod = DimensionStyleTolerancesDisplayMethod.None;
			this.LowerLimit = 0.0;
			this.UpperLimit = 0.0;
			this.VerticalPlacement = DimensionStyleTolerancesVerticalPlacement.Middle;
			this.dimtdec = 4;
			this.SuppressLinearLeadingZeros = false;
			this.SuppressLinearTrailingZeros = false;
			this.SuppressZeroFeet = true;
			this.SuppressZeroInches = true;
			this.dimalttd = 2;
			this.AlternateSuppressLinearLeadingZeros = false;
			this.AlternateSuppressLinearTrailingZeros = false;
			this.AlternateSuppressZeroFeet = true;
			this.AlternateSuppressZeroInches = true;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the method for calculating the tolerance. (<b>DIMTOL</b>)</summary>
		/// <remarks>
		/// Default: None
		/// </remarks>
		public DimensionStyleTolerancesDisplayMethod DisplayMethod { get; set; }

		/// <summary>Gets or sets the maximum or upper tolerance value. When you select <see cref="DimensionStyleTolerancesDisplayMethod.Symmetrical"/> in <see cref="DisplayMethod"/>, this value is used for the tolerance. (<b>DIMTP</b>)</summary>
		/// <remarks>
		/// Default: 0.0
		/// </remarks>
		public double UpperLimit { get; set; }

		/// <summary>Gets or sets the minimum or lower tolerance value. (<b>DIMTM</b>)</summary>
		/// <remarks>
		/// Default: 0.0
		/// </remarks>
		public double LowerLimit { get; set; }

		/// <summary>Gets or sets the text vertical placement for symmetrical and deviation tolerances. (<b>DIMTOLJ</b>)</summary>
		/// <remarks>
		/// Default: Middle
		/// </remarks>
		public DimensionStyleTolerancesVerticalPlacement VerticalPlacement { get; set; }

		/// <summary>Gets or sets the number of decimal places. (<b>DIMTDEC</b>)</summary>
		/// <remarks>
		/// Default: 4<br/>
		/// It is recommended to use values in the range 0 to 8.
		/// </remarks>
		public short Precision
		{
			get => this.dimtdec;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The tolerance precision must be equals or greater than zero.");
				}
				this.dimtdec = value;
			}
		}

		/// <summary>Suppresses leading zeros in linear decimal tolerance units. (<b>DIMTZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMTZIN</b> variable.
		/// </remarks>
		public bool SuppressLinearLeadingZeros { get; set; }

		/// <summary>Suppresses trailing zeros in linear decimal tolerance units. (<b>DIMTZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMTZIN</b> variable.
		/// </remarks>
		public bool SuppressLinearTrailingZeros { get; set; }

		/// <summary>Suppresses zero feet in architectural tolerance units. (<b>DIMTZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMTZIN</b> variable.
		/// </remarks>
		public bool SuppressZeroFeet { get; set; }

		/// <summary>Suppresses zero inches in architectural tolerance units. (<b>DIMTZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMTZIN</b> variable.
		/// </remarks>
		public bool SuppressZeroInches { get; set; }

		/// <summary>Gets or sets the number of decimal places of the tolerance alternate units. (<b>DIMALTTD</b>)</summary>
		/// <remarks>
		/// Default: 2<br/>
		/// It is recommended to use values in the range 0 to 8.
		/// </remarks>
		public short AlternatePrecision
		{
			get => this.dimalttd;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The alternate precision must be equals or greater than zero.");
				}
				this.dimalttd = value;
			}
		}

		/// <summary>Suppresses leading zeros in linear decimal alternate tolerance units. (<b>DIMALTTZ</b>)</summary>
		/// <remarks>This value is part of the <b>DIMALTTZ</b> variable.</remarks>
		public bool AlternateSuppressLinearLeadingZeros { get; set; }

		/// <summary>Suppresses trailing zeros in linear decimal alternate tolerance units. (<b>DIMALTTZ</b>)</summary>
		/// <remarks>This value is part of the <b>DIMALTTZ</b> variable.</remarks>
		public bool AlternateSuppressLinearTrailingZeros { get; set; }

		/// <summary>Suppresses zero feet in architectural alternate tolerance units. (<b>DIMALTTZ</b>)</summary>
		/// <remarks>This value is part of the <b>DIMALTTZ</b> variable.</remarks>
		public bool AlternateSuppressZeroFeet { get; set; }

		/// <summary>Suppresses zero inches in architectural alternate tolerance units. (<b>DIMALTTZ</b>)</summary>
		/// <remarks>This value is part of the <b>DIMALTTZ</b> variable.</remarks>
		public bool AlternateSuppressZeroInches { get; set; }

		#endregion

		#region implements ICloneable

		/// <inheritdoc/>
		public object Clone()
			=> new DimensionStyleTolerances
			{
				DisplayMethod = this.DisplayMethod,
				UpperLimit = this.UpperLimit,
				LowerLimit = this.LowerLimit,
				VerticalPlacement = this.VerticalPlacement,
				Precision = this.dimtdec,
				SuppressLinearLeadingZeros = this.SuppressLinearLeadingZeros,
				SuppressLinearTrailingZeros = this.SuppressLinearTrailingZeros,
				SuppressZeroFeet = this.SuppressZeroFeet,
				SuppressZeroInches = this.SuppressZeroInches,
				AlternatePrecision = this.dimalttd,
				AlternateSuppressLinearLeadingZeros = this.AlternateSuppressLinearLeadingZeros,
				AlternateSuppressLinearTrailingZeros = this.AlternateSuppressLinearTrailingZeros,
				AlternateSuppressZeroFeet = this.AlternateSuppressZeroFeet,
				AlternateSuppressZeroInches = this.AlternateSuppressZeroInches,
			};

		#endregion
	}
}