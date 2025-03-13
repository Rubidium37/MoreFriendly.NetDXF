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
using System.Threading;

namespace netDxf.Entities
{
	/// <summary>Represents an edge of a <see cref="EntityObject">mesh</see> entity.</summary>
	public class MeshEdge :
		ICloneable
	{
		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="startVertexIndex">The edge start vertex index.</param>
		/// <param name="endVertexIndex">The edge end vertex index.</param>
		public MeshEdge(int startVertexIndex, int endVertexIndex)
			: this(startVertexIndex, endVertexIndex, 0.0)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="startVertexIndex">The edge start vertex index.</param>
		/// <param name="endVertexIndex">The edge end vertex index.</param>
		/// <param name="crease">The highest smoothing level at which the crease is retained (default: 0.0).</param>
		public MeshEdge(int startVertexIndex, int endVertexIndex, double crease)
		{
			if (startVertexIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(startVertexIndex), startVertexIndex, "The vertex index must be positive.");
			_StartVertexIndex = startVertexIndex;

			if (endVertexIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(endVertexIndex), endVertexIndex, "The vertex index must be positive.");
			_EndVertexIndex = endVertexIndex;
			_Crease = crease < 0.0 ? -1.0 : crease;
		}

		#endregion

		#region public properties

		private int _StartVertexIndex;
		/// <summary>Gets or sets the edge start vertex index.</summary>
		/// <remarks>
		/// This value must be positive represent the position of the vertex in the mesh vertex list.
		/// </remarks>
		public int StartVertexIndex
		{
			get => _StartVertexIndex;
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The vertex index must be must be equals or greater than zero.");
				_StartVertexIndex = value;
			}
		}

		private int _EndVertexIndex;
		/// <summary>Gets or sets the edge end vertex index.</summary>
		public int EndVertexIndex
		{
			get => _EndVertexIndex;
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The vertex index must be must be equals or greater than zero.");
				_EndVertexIndex = value;
			}
		}

		private double _Crease;
		/// <summary>Get or set the highest smoothing level at which the crease is retained. If the smoothing level exceeds this value, the crease is also smoothed.</summary>
		/// <remarks>
		/// Enter a value of 0 to remove an existing crease (no edge sharpening).<br/>
		/// Enter a value of -1 (any negative number will be reset to -1) to specify that the crease is always retained, even if the object or sub-object is smoothed or refined.
		/// </remarks>
		public double Crease
		{
			get => _Crease;
			set => _Crease = value < 0 ? -1 : value;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
			=> string.Format("{0}: ({1}{4} {2}) crease={3}", "SplineVertex", _StartVertexIndex, _EndVertexIndex, _Crease, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);

		/// <summary>Obtains a string that represents the mesh edge.</summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> object implementation that supplies culture-specific formatting information. </param>
		/// <returns>A string text.</returns>
		public string ToString(IFormatProvider provider)
			=> string.Format("{0}: ({1}{4} {2}) crease={3}", "SplineVertex", _StartVertexIndex.ToString(provider), _EndVertexIndex.ToString(provider), _Crease.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);

		/// <inheritdoc/>
		public object Clone()
			=> new MeshEdge(_StartVertexIndex, _EndVertexIndex, _Crease);

		#endregion
	}
}