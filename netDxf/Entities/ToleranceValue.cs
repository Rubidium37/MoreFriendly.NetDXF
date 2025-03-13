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
	/// <summary>Represents a tolerance, indicates the amount by which the geometric characteristic can deviate from a perfect form.</summary>
	public class ToleranceValue :
		ICloneable
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public ToleranceValue()
		{
			this.ShowDiameterSymbol = false;
			_Value = string.Empty;
			this.MaterialCondition = ToleranceMaterialCondition.None;
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="showDiameterSymbol">Show a diameter symbol before the tolerance value.</param>
		/// <param name="value">Tolerance value.</param>
		/// <param name="materialCondition">Tolerance material condition.</param>
		public ToleranceValue(bool showDiameterSymbol, string value, ToleranceMaterialCondition materialCondition)
		{
			this.ShowDiameterSymbol = showDiameterSymbol;
			_Value = string.IsNullOrEmpty(value) ? string.Empty : value;
			this.MaterialCondition = materialCondition;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets if the tolerance diameter symbol will be shown.</summary>
		public bool ShowDiameterSymbol { get; set; }

		private string _Value;
		/// <summary>Gets or sets the tolerance value.</summary>
		public string Value
		{
			get => _Value;
			set => _Value = string.IsNullOrEmpty(value) ? string.Empty : value;
		}

		/// <summary>Gets or sets the tolerance material condition.</summary>
		public ToleranceMaterialCondition MaterialCondition { get; set; }

		#endregion

		#region ICloneable

		/// <inheritdoc/>
		public object Clone()
			=> new ToleranceValue
			{
				ShowDiameterSymbol = this.ShowDiameterSymbol,
				Value = _Value,
				MaterialCondition = this.MaterialCondition
			};

		#endregion
	}
}