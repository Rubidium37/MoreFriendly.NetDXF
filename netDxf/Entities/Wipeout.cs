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
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a wipeout <see cref="EntityObject">entity</see>.</summary>
	/// <remarks>
	/// The Wipeout <b>DXF</b> definition includes three variables for brightness, contrast, and fade but those variables have no effect; in <b>AutoCAD</b> you cannot even change them.<br/>
	/// The Wipeout entity is related with the system variable <b>WIPEOUTFRAME</b> but this variable is not saved in a DXF.
	/// </remarks>
	public class Wipeout :
		EntityObject
	{
		#region constructors

		/// <summary>Initializes a new instance of the class as a rectangular wipeout.</summary>
		/// <param name="x">Rectangle x-coordinate of the bottom-left corner in local coordinates.</param>
		/// <param name="y">Rectangle y-coordinate of the bottom-left corner in local coordinates.</param>
		/// <param name="width">Rectangle width in local coordinates.</param>
		/// <param name="height">Rectangle height in local coordinates.</param>
		public Wipeout(double x, double y, double width, double height)
			: this(new ClippingBoundary(x, y, width, height))
		{
		}
		/// <summary>Initializes a new instance of the class as a rectangular wipeout from two opposite corners.</summary>
		/// <param name="firstCorner">Rectangle firstCorner in local coordinates.</param>
		/// <param name="secondCorner">Rectangle secondCorner in local coordinates.</param>
		public Wipeout(Vector2 firstCorner, Vector2 secondCorner)
			: this(new ClippingBoundary(firstCorner, secondCorner))
		{
		}
		/// <summary>Initializes a new instance of the class as a polygonal wipeout.</summary>
		/// <param name="vertexes">The list of vertexes of the wipeout.</param>
		public Wipeout(IEnumerable<Vector2> vertexes)
			: this(new ClippingBoundary(vertexes))
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="clippingBoundary">The wipeout clipping boundary.</param>
		public Wipeout(ClippingBoundary clippingBoundary)
			: base(EntityType.Wipeout, DxfObjectCode.Wipeout)
		{
			_ClippingBoundary = clippingBoundary ?? throw new ArgumentNullException(nameof(clippingBoundary));
		}

		#endregion

		#region public properties

		private ClippingBoundary _ClippingBoundary;
		/// <summary>Gets or sets the wipeout clipping boundary.</summary>
		public ClippingBoundary ClippingBoundary
		{
			get => _ClippingBoundary;
			set => _ClippingBoundary = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>Gets or sets the wipeout elevation.</summary>
		/// <remarks>This is the distance from the origin to the plane of the wipeout boundary.</remarks>
		public double Elevation { get; set; } = 0.0;

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			double newElevation = this.Elevation;

			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}

			Matrix3 transOW = MathHelper.ArbitraryAxis(this.Normal);
			Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal).Transpose();

			List<Vector2> vertexes = new List<Vector2>();

			foreach (Vector2 vertex in this.ClippingBoundary.Vertexes)
			{
				Vector3 v = transOW * new Vector3(vertex.X, vertex.Y, this.Elevation);
				v = transformation * v + translation;
				v = transWO * v;
				vertexes.Add(new Vector2(v.X, v.Y));
				newElevation = v.Z;
			}

			ClippingBoundary newClipping = this.ClippingBoundary.Type == ClippingBoundaryType.Rectangular
				? new ClippingBoundary(vertexes[0], vertexes[1])
				: new ClippingBoundary(vertexes);

			this.Normal = newNormal;
			this.Elevation = newElevation;
			this.ClippingBoundary = newClipping;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Wipeout entity = new Wipeout((ClippingBoundary)this.ClippingBoundary.Clone())
			{
				//EntityObject properties
				Layer = (Layer)this.Layer.Clone(),
				Linetype = (Linetype)this.Linetype.Clone(),
				Color = (AciColor)this.Color.Clone(),
				Lineweight = this.Lineweight,
				Transparency = (Transparency)this.Transparency.Clone(),
				LinetypeScale = this.LinetypeScale,
				Normal = this.Normal,
				IsVisible = this.IsVisible,
				//Wipeout properties
				Elevation = this.Elevation
			};

			foreach (XData data in this.XData.Values)
			{
				entity.XData.Add((XData)data.Clone());
			}

			return entity;
		}

		#endregion
	}
}