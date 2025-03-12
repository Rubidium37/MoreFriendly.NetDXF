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

using System.Collections.Generic;

namespace netDxf.Entities
{
	/// <summary>Represents a <see cref="MLine">multiline</see> vertex.</summary>
	public class MLineVertex
	{
		#region constructors

		internal MLineVertex(Vector2 location, Vector2 direction, Vector2 miter, List<double>[] distances)
		{
			this.Position = location;
			this.Direction = direction;
			this.Miter = miter;
			this.Distances = distances;
		}

		#endregion

		#region public properties

		/// <summary>Gets the <b>MLine</b> vertex position.</summary>
		/// <remarks>
		/// If this property is modified the function MLine.Update() will need to be called manually to update the internal information.
		/// </remarks>
		public Vector2 Position { get; set; }

		/// <summary>Gets the <b>MLine</b> vertex direction.</summary>
		public Vector2 Direction { get; }

		/// <summary>Gets the <b>MLine</b> vertex miter.</summary>
		public Vector2 Miter { get; }

		/// <summary>Gets the <see cref="MLine">multiline</see> vertex distances lists.</summary>
		/// <remarks>
		/// <para>
		/// There is a list for every MLineStyle element, and every list contains an array of real values
		/// that parametrize the start and end point of every element of the style.
		/// </para>
		/// <para>
		/// The first value (index 0) represents the distance from the segment vertex along the miter vector to the
		/// point where the line element's path intersects the miter vector.<br />
		/// The second value (index 1) is the distance along the line element's direction from the point,
		/// defined by the first value, to the actual start of the line element.<br />
		/// The successive values list the start and stop points of the line element breaks or cuts in this segment of the multiline.
		/// </para>
		/// </remarks>
		public List<double>[] Distances { get; }

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
			=> string.Format("{0}: ({1})", "MLineVertex", this.Position);

		/// <summary>Creates a new MLineVertex that is a copy of the current instance.</summary>
		/// <returns>A new MLineVertex that is a copy of this instance.</returns>
		public object Clone()
		{
			List<double>[] copyDistances = new List<double>[this.Distances.Length];
			for (int i = 0; i < this.Distances.Length; i++)
			{
				copyDistances[i] = new List<double>();
				copyDistances[i].AddRange(this.Distances[i]);
			}
			return new MLineVertex(this.Position, this.Direction, this.Miter, copyDistances);
		}

		#endregion
	}
}