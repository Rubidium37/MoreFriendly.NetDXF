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
	/// <summary>Represents a mesh <see cref="EntityObject">entity</see>.</summary>
	/// <remarks>
	/// Use this entity to overcome the limitations of the PolyfaceMesh, but, keep in mind that this entity was first introduced in <b>AutoCAD</b> 2010.<br/>
	/// The maximum number of faces a mesh can have is 16000000 (16 millions).
	/// </remarks>
	public class Mesh :
		EntityObject
	{
		private const int MaxFaces = 16000000;

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">Mesh vertex list.</param>
		/// <param name="faces">Mesh faces list.</param>
		public Mesh(IEnumerable<Vector3> vertexes, IEnumerable<int[]> faces)
			: this(vertexes, faces, null)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">Mesh vertex list.</param>
		/// <param name="faces">Mesh faces list.</param>
		/// <param name="edges">Mesh edges list, this parameter is only really useful when it is required to assign creases values to edges.</param>
		public Mesh(IEnumerable<Vector3> vertexes, IEnumerable<int[]> faces, IEnumerable<MeshEdge> edges)
			: base(EntityType.Mesh, DxfObjectCode.Mesh)
		{
			if (vertexes == null)
			{
				throw new ArgumentNullException(nameof(vertexes));
			}
			this.Vertexes = new List<Vector3>(vertexes);
			if (faces == null)
			{
				throw new ArgumentNullException(nameof(faces));
			}
			this.Faces = new List<int[]>(faces);
			if (this.Faces.Count > MaxFaces)
			{
				throw new ArgumentOutOfRangeException(nameof(faces), this.Faces.Count, string.Format("The maximum number of faces in a mesh is {0}", MaxFaces));
			}
			this.Edges = edges == null ? new List<MeshEdge>() : new List<MeshEdge>(edges);
		}

		#endregion

		#region public properties

		/// <summary>Gets the mesh vertexes list.</summary>
		public List<Vector3> Vertexes { get; }

		/// <summary>Gets the mesh faces list.</summary>
		public List<int[]> Faces { get; }

		/// <summary>Gets the mesh edges list.</summary>
		public List<MeshEdge> Edges { get; }

		/// <summary>Gets or sets the mesh subdivision level.</summary>
		/// <remarks>
		/// The valid range is from 0 to 255. The recommended range is 0-5 to prevent creating extremely dense meshes.
		/// </remarks>
		public byte SubdivisionLevel { get; set; }

		#endregion

		#region public methods

		///// <summary>
		///// Decompose the actual mesh into <see cref="Face3D">faces 3D</see>.
		///// </summary>
		///// <returns>A list of <see cref="Face3D">faces 3D</see> that made up the mesh.</returns>
		//public List<Face3D> Explode()
		//{
		//TODO: requires triangulate polygon into triangles for faces with more than 4 vertexes

		//	List<Face3D> faces3D = new List<Face3D>();

		//	foreach (int[] face in _Faces)
		//	{
		//		faces3D.Add(new Face3D(this.Vertexes[face[0]], this.Vertexes[face[1]], this.Vertexes[face[2]], this.Vertexes[face[3]]));
		//	}

		//	return faces3D;
		//}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			for (int i = 0; i < this.Vertexes.Count; i++)
			{
				this.Vertexes[i] = transformation * this.Vertexes[i] + translation;
			}

			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal)) newNormal = this.Normal;
			this.Normal = newNormal;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			List<Vector3> copyVertexes = new List<Vector3>(this.Vertexes.Count);
			List<int[]> copyFaces = new List<int[]>(this.Faces.Count);
			List<MeshEdge> copyEdges = null;

			copyVertexes.AddRange(this.Vertexes);
			foreach (int[] face in this.Faces)
			{
				int[] copyFace = new int[face.Length];
				face.CopyTo(copyFace, 0);
				copyFaces.Add(copyFace);
			}
			if (this.Edges != null)
			{
				copyEdges = new List<MeshEdge>(this.Edges.Count);
				foreach (MeshEdge meshEdge in this.Edges)
				{
					copyEdges.Add((MeshEdge)meshEdge.Clone());
				}
			}

			Mesh entity = new Mesh(copyVertexes, copyFaces, copyEdges)
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
				//Mesh properties
				SubdivisionLevel = this.SubdivisionLevel
			};

			foreach (XData data in this.XData.Values)
				entity.XData.Add((XData)data.Clone());

			return entity;
		}

		#endregion
	}
}