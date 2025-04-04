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
using netDxf.Entities;

namespace netDxf
{
	/// <summary>
	/// Represent a clipping boundary to display specific portions of
	/// an <see cref="Image"/>,
	/// an <see cref="Underlay"/>,
	/// or a <see cref="Wipeout"/>.
	/// </summary>
	public class ClippingBoundary :
		ICloneable
	{
		#region constructors

		/// <summary>Initializes a new instance of the class as a rectangular clipping boundary.</summary>
		/// <param name="x">Rectangle x-coordinate of the first corner.</param>
		/// <param name="y">Rectangle y-coordinate of the first corner.</param>
		/// <param name="width">Rectangle width.</param>
		/// <param name="height">Rectangle height.</param>
		public ClippingBoundary(double x, double y, double width, double height)
		{
			this.Type = ClippingBoundaryType.Rectangular;
			this.Vertexes = new List<Vector2> { new Vector2(x, y), new Vector2(x + width, y + height) };
		}
		/// <summary>Initializes a new instance of the class as a rectangular clipping boundary from two opposite corners.</summary>
		/// <param name="firstCorner">Rectangle first corner.</param>
		/// <param name="secondCorner">Rectangle second corner.</param>
		public ClippingBoundary(Vector2 firstCorner, Vector2 secondCorner)
		{
			this.Type = ClippingBoundaryType.Rectangular;
			this.Vertexes = new List<Vector2> { firstCorner, secondCorner };
		}
		/// <summary>Initializes a new instance of the class as a polygonal clipping boundary.</summary>
		/// <param name="vertexes">The list of vertexes of the polygonal boundary.</param>
		public ClippingBoundary(IEnumerable<Vector2> vertexes)
		{
			this.Type = ClippingBoundaryType.Polygonal;
			this.Vertexes = new List<Vector2>(vertexes);
			if (this.Vertexes.Count < 3)
			{
				throw new ArgumentOutOfRangeException(nameof(vertexes), this.Vertexes.Count, "The number of vertexes for the polygonal clipping boundary must be equal or greater than three.");
			}
		}

		#endregion

		#region public properties

		/// <summary>Gets the clipping boundary type, rectangular or polygonal.</summary>
		public ClippingBoundaryType Type { get; }

		/// <summary>Gets the list of vertexes of the polygonal boundary, or the opposite vertexes if the boundary is rectangular.</summary>
		public IReadOnlyList<Vector2> Vertexes { get; }

		#endregion

		#region overrides

		/// <inheritdoc/>
		public object Clone()
		{
			return this.Type == ClippingBoundaryType.Rectangular
				? new ClippingBoundary(this.Vertexes[0], this.Vertexes[1])
				: new ClippingBoundary(this.Vertexes);
		}

		#endregion
	}
}