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
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a <b>DXF</b> Polyline.</summary>
	/// <remarks>
	/// Under the <b>POLYLINE</b> data the <b>DXF</b> stores information about smoothed Polylines2D (non-smoothed Polylines2D are stored as <b>LWPOLYLINE</b>,
	/// Polylines3D (smoothed and non-smoothed), and PolyfaceMeshes.<br />
	/// For internal use only.
	/// </remarks>
	internal class Polyline :
		DxfObject
	{
		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		public Polyline()
			: base(DxfObjectCode.Polyline)
		{
			this.SubclassMarker = netDxf.SubclassMarker.Polyline;
		}

		#endregion

		#region properties

		/// <summary>Gets or sets the polyline subclass marker.</summary>
		public string SubclassMarker { get; set; }

		/// <summary>Gets or sets the polyline layer.</summary>
		public Layer Layer { get; set; }

		/// <summary>Gets or sets the polyline thickness.</summary>
		public double Thickness { get; set; }

		/// <summary>Gets or sets the polyline elevation.</summary>
		public double Elevation { get; set; }

		/// <summary>Gets or sets the polyline normal.</summary>
		public Vector3 Normal { get; set; }

		/// <summary>Gets or sets the polyline color.</summary>
		public AciColor Color { get; set; }

		/// <summary>Gets or sets the polyline <see cref="EndSequence"/> object.</summary>
		public EndSequence EndSequence { get; set; }

		/// <summary>Gets or sets the polyline vertexes list.</summary>
		public List<Vertex> Vertexes { get; set; }

		/// <summary>Gets or sets the polyline flags.</summary>
		public PolylineTypeFlags Flags { get; set; }

		/// <summary>Gets or sets the polyline smooth type.</summary>
		public PolylineSmoothType SmoothType { get; set; }

		/// <summary>Polygon mesh M vertex count.</summary>
		public short M { get; set; }

		/// <summary>Polygon mesh N vertex count.</summary>
		public short N { get; set; }

		/// <summary>Polygon mesh smooth surface M density.</summary>
		public short DensityM { get; set; }

		/// <summary>Polygon mesh smooth surface N density.</summary>
		public short DensityN { get; set; }

		#endregion
	}
}
