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
	/// <summary>Represents an entry in a tolerance entity.</summary>
	/// <remarks>
	/// Each entry can be made of up to two tolerance values and three datum references, plus a symbol that represents the geometric characteristics.
	/// </remarks>
	public class ToleranceEntry :
		ICloneable
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public ToleranceEntry()
		{
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the geometric characteristics symbol.</summary>
		public ToleranceGeometricSymbol GeometricSymbol { get; set; }

		/// <summary>Gets or sets the first tolerance value.</summary>
		public ToleranceValue Tolerance1 { get; set; }

		/// <summary>Gets or sets the second tolerance value.</summary>
		public ToleranceValue Tolerance2 { get; set; }

		/// <summary>Gets or sets the first datum reference value.</summary>
		public DatumReferenceValue Datum1 { get; set; }

		/// <summary>Gets or sets the second datum reference value.</summary>
		public DatumReferenceValue Datum2 { get; set; }

		/// <summary>Gets or sets the third datum reference value.</summary>
		public DatumReferenceValue Datum3 { get; set; }

		#endregion

		#region ICloneable

		/// <inheritdoc/>
		public object Clone()
			=> new ToleranceEntry
			{
				GeometricSymbol = this.GeometricSymbol,
				Tolerance1 = (ToleranceValue)this.Tolerance1?.Clone(),
				Tolerance2 = (ToleranceValue)this.Tolerance2?.Clone(),
				Datum1 = (DatumReferenceValue)this.Datum1?.Clone(),
				Datum2 = (DatumReferenceValue)this.Datum2?.Clone(),
				Datum3 = (DatumReferenceValue)this.Datum3?.Clone(),
			};

		#endregion
	}
}