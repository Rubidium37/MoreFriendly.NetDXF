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
	/// <summary>Base class for the three kinds of linetype segments simple, text, and shape.</summary>
	public abstract class LinetypeSegment :
		ICloneable
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="type">Type of the linetype segment.</param>
		/// <param name="length">Dash or space length of the segment.</param>
		protected LinetypeSegment(LinetypeSegmentType type, double length)
		{
			this.Type = type;
			this.Length = length;
		}

		#endregion

		#region public properties

		/// <summary>Gets the linetype segment simple, text, or shape.</summary>
		public LinetypeSegmentType Type { get; }

		/// <summary>Gets or sets the dash, dot or space length.</summary>
		/// <remarks>
		/// A positive decimal number denotes a pen-down (dash) segment of that length.
		/// A negative decimal number denotes a pen-up (space) segment of that length.
		/// A dash length of 0 draws a dot.
		/// </remarks>
		public double Length { get; set; }

		#endregion

		#region implements ICloneable

		/// <inheritdoc/>
		public abstract object Clone();

		#endregion
	}
}
