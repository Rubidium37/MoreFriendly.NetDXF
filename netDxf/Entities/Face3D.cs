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
	/// <summary>Represents a 3d Face <see cref="EntityObject">entity</see>.</summary>
	public class Face3D :
		EntityObject
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public Face3D()
			: this(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstVertex">Face3D <see cref="Vector2">first vertex</see>.</param>
		/// <param name="secondVertex">Face3D <see cref="Vector2">second vertex</see>.</param>
		/// <param name="thirdVertex">Face3D <see cref="Vector2">third vertex</see>.</param>
		public Face3D(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex)
			: this(new Vector3(firstVertex.X, firstVertex.Y, 0.0),
				new Vector3(secondVertex.X, secondVertex.Y, 0.0),
				new Vector3(thirdVertex.X, thirdVertex.Y, 0.0),
				new Vector3(thirdVertex.X, thirdVertex.Y, 0.0))
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstVertex">Face3D <see cref="Vector2">first vertex</see>.</param>
		/// <param name="secondVertex">Face3D <see cref="Vector2">second vertex</see>.</param>
		/// <param name="thirdVertex">Face3D <see cref="Vector2">third vertex</see>.</param>
		/// <param name="fourthVertex">Face3D <see cref="Vector2">fourth vertex</see>.</param>
		public Face3D(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex, Vector2 fourthVertex)
			: this(new Vector3(firstVertex.X, firstVertex.Y, 0.0),
				new Vector3(secondVertex.X, secondVertex.Y, 0.0),
				new Vector3(thirdVertex.X, thirdVertex.Y, 0.0),
				new Vector3(fourthVertex.X, fourthVertex.Y, 0.0))
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstVertex">Face3D <see cref="Vector3">first vertex</see>.</param>
		/// <param name="secondVertex">Face3D <see cref="Vector3">second vertex</see>.</param>
		/// <param name="thirdVertex">Face3D <see cref="Vector3">third vertex</see>.</param>
		public Face3D(Vector3 firstVertex, Vector3 secondVertex, Vector3 thirdVertex)
			: this(firstVertex, secondVertex, thirdVertex, thirdVertex)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="firstVertex">Face3D <see cref="Vector3">first vertex</see>.</param>
		/// <param name="secondVertex">Face3D <see cref="Vector3">second vertex</see>.</param>
		/// <param name="thirdVertex">Face3D <see cref="Vector3">third vertex</see>.</param>
		/// <param name="fourthVertex">Face3D <see cref="Vector3">fourth vertex</see>.</param>
		public Face3D(Vector3 firstVertex, Vector3 secondVertex, Vector3 thirdVertex, Vector3 fourthVertex)
			: base(EntityType.Face3D, DxfObjectCode.Face3d)
		{
			this.FirstVertex = firstVertex;
			this.SecondVertex = secondVertex;
			this.ThirdVertex = thirdVertex;
			this.FourthVertex = fourthVertex;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the first <b>Face3D</b> <see cref="Vector3">vertex</see>.</summary>
		public Vector3 FirstVertex { get; set; }

		/// <summary>Gets or sets the second <b>Face3D</b> <see cref="Vector3">vertex</see>.</summary>
		public Vector3 SecondVertex { get; set; }

		/// <summary>Gets or sets the third <b>Face3D</b> <see cref="Vector3">vertex</see>.</summary>
		public Vector3 ThirdVertex { get; set; }

		/// <summary>Gets or sets the fourth <b>Face3D</b> <see cref="Vector3">vertex</see>.</summary>
		public Vector3 FourthVertex { get; set; }

		/// <summary>Gets or sets the <b>Face3D</b> edge visibility.</summary>
		public Face3DEdgeFlags EdgeFlags { get; set; } = Face3DEdgeFlags.None;

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			this.FirstVertex = transformation * this.FirstVertex + translation;
			this.SecondVertex = transformation * this.SecondVertex + translation;
			this.ThirdVertex = transformation * this.ThirdVertex + translation;
			this.FourthVertex = transformation * this.FourthVertex + translation;

			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}
			this.Normal = newNormal;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Face3D entity = new Face3D
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
				//Face3d properties
				FirstVertex = this.FirstVertex,
				SecondVertex = this.SecondVertex,
				ThirdVertex = this.ThirdVertex,
				FourthVertex = this.FourthVertex,
				EdgeFlags = this.EdgeFlags
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