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

using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a <b>DXF</b> Vertex.</summary>
	/// <remarks>
	/// Under the <b>VERTEX</b> data the <b>DXF</b> stores information about the vertexes of smoothed Polylines2D (non-smoothed Polylines2D are stored as LWPOLYLINE,
	/// Polylines3D (smoothed and non-smoothed), and PolyfaceMeshes.<br />
	/// For internal use only.
	/// </remarks>
	internal class Vertex :
		DxfObject
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public Vertex()
			: base(DxfObjectCode.Vertex)
		{
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the polyline vertex <see cref="Vector3">location</see>.</summary>
		public Vector3 Position { get; set; } = Vector3.Zero;

		/// <summary>Gets or sets the indexes, only applicable when the vertex represents a face of a polyface mesh.</summary>
		public short[] VertexIndexes { get; set; }

		/// <summary>Gets or sets the light weight polyline start segment width.</summary>
		public double StartWidth { get; set; } = 0.0;

		/// <summary>Gets or sets the light weight polyline end segment width.</summary>
		public double EndWidth { get; set; } = 0.0;

		/// <summary>Gets or set the light weight polyline bulge.Accepted values range from 0 to 1.</summary>
		public double Bulge { get; set; } = 0.0;

		/// <summary>Gets or sets the vertex type.</summary>
		public VertexTypeFlags Flags { get; set; } = VertexTypeFlags.Polyline2DVertex;

		/// <summary>Gets or sets the entity color.</summary>
		public AciColor Color { get; set; }

		/// <summary>Gets or sets the entity layer.</summary>
		public Layer Layer { get; set; }

		/// <summary>Gets or sets the entity line type.</summary>
		public Linetype Linetype { get; set; }

		/// <summary>Gets or sets the subclass marker</summary>
		public string SubclassMarker { get; set; } = netDxf.SubclassMarker.Polyline2DVertex;

		#endregion
	}
}