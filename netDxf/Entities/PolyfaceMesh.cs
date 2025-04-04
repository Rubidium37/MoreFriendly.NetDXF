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
using System.Linq;
using System.Runtime.CompilerServices;
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a polyface mesh <see cref="EntityObject">entity</see>.</summary>
	public class PolyfaceMesh :
		EntityObject
	{
		#region delegates and events

		/// <summary>Generated when a property of <see cref="Layer"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<Layer> BeforeChangingFacesLayerValue;
		/// <summary>Generates the <see cref="BeforeChangingFacesLayerValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual Layer OnBeforeChangingFacesLayerValue(Layer oldValue, Layer newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingFacesLayerValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<Layer>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">Polyface mesh <see cref="Vector3">vertex</see> list.</param>
		/// <param name="faces">Polyface mesh faces list, a maximum of 4 indexes per face.</param>
		public PolyfaceMesh(IEnumerable<Vector3> vertexes, IEnumerable<short[]> faces)
			: base(EntityType.PolyfaceMesh, DxfObjectCode.Polyline)
		{
			this.Flags = PolylineTypeFlags.PolyfaceMesh;
			if (vertexes == null)
			{
				throw new ArgumentNullException(nameof(vertexes));
			}
			this.Vertexes = vertexes.ToArray();
			if (this.Vertexes.Length < 3)
			{
				throw new ArgumentOutOfRangeException(nameof(vertexes), this.Vertexes.Length, "The polyface mesh faces list requires at least three points.");
			}

			if (faces == null)
			{
				throw new ArgumentNullException(nameof(faces));
			}

			int numFaces = faces.Count();
			_Faces = new PolyfaceMeshFace[numFaces];
			for (int i = 0; i < numFaces; i++)
			{
				_Faces[i] = new PolyfaceMeshFace(faces.ElementAt(i));
			}
			if (this.Faces.Count < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(vertexes), this.Faces.Count, "The polyface mesh faces list requires at least one face.");
			}
			foreach (PolyfaceMeshFace face in this.Faces)
			{
				face.BeforeChangingLayerValue += this.Faces_Item_BeforeChangingLayerValue;
			}
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">Polyface mesh <see cref="Vector3">vertex</see> list.</param>
		/// <param name="faces">Polyface mesh faces list, a maximum of 4 indexes per face.</param>
		public PolyfaceMesh(IEnumerable<Vector3> vertexes, IEnumerable<PolyfaceMeshFace> faces)
			: base(EntityType.PolyfaceMesh, DxfObjectCode.Polyline)
		{
			this.Flags = PolylineTypeFlags.PolyfaceMesh;
			if (vertexes == null)
			{
				throw new ArgumentNullException(nameof(vertexes));
			}
			this.Vertexes = vertexes.ToArray();
			if (this.Vertexes.Length < 3)
			{
				throw new ArgumentOutOfRangeException(nameof(vertexes), this.Vertexes.Length, "The polyface mesh faces list requires at least three points.");
			}

			if (faces == null)
			{
				throw new ArgumentNullException(nameof(faces));
			}

			_Faces = faces.ToArray();
			if (this.Faces.Count < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(vertexes), this.Faces.Count, "The polyface mesh faces list requires at least one face.");
			}

			foreach (PolyfaceMeshFace face in this.Faces)
			{
				face.BeforeChangingLayerValue += this.Faces_Item_BeforeChangingLayerValue;
			}
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the polyface mesh <see cref="Vector3">vertexes</see>.</summary>
		public Vector3[] Vertexes { get; }

		private readonly PolyfaceMeshFace[] _Faces;
		/// <summary>Gets or sets the polyface mesh <see cref="PolyfaceMeshFace">faces</see>.</summary>
		public IReadOnlyList<PolyfaceMeshFace> Faces => _Faces;

		#endregion

		#region internal properties

		/// <summary>Gets the polyface mesh flags.</summary>
		internal PolylineTypeFlags Flags { get; set; }

		#endregion

		#region public methods

		/// <summary>
		/// Decompose the actual polyface mesh faces in <see cref="Point">points</see> (one vertex polyface mesh face),
		/// <see cref="Line">lines</see> (two vertexes polyface mesh face) and <see cref="Face3D">3d faces</see> (three or four vertexes polyface mesh face).
		/// </summary>
		/// <returns>A list of <see cref="Face3D">3d faces</see> that made up the polyface mesh.</returns>
		public List<EntityObject> Explode()
		{
			List<EntityObject> entities = new List<EntityObject>();

			foreach (PolyfaceMeshFace face in this.Faces)
			{
				AciColor faceColor = face.Color == null ? this.Color : face.Color;
				Layer faceLayer = face.Layer == null ? this.Layer : face.Layer;

				if (face.VertexIndexes.Length == 1)
				{
					Point point = new Point
					{
						Layer = (Layer)faceLayer.Clone(),
						Linetype = (Linetype)this.Linetype.Clone(),
						Color = (AciColor)faceColor.Clone(),
						Lineweight = this.Lineweight,
						Transparency = (Transparency)this.Transparency.Clone(),
						LinetypeScale = this.LinetypeScale,
						Normal = this.Normal,
						Position = this.Vertexes[Math.Abs(face.VertexIndexes[0]) - 1],
					};
					entities.Add(point);
					continue;
				}
				if (face.VertexIndexes.Length == 2)
				{
					Line line = new Line
					{
						Layer = (Layer)faceLayer.Clone(),
						Linetype = (Linetype)this.Linetype.Clone(),
						Color = (AciColor)faceColor.Clone(),
						Lineweight = this.Lineweight,
						Transparency = (Transparency)this.Transparency.Clone(),
						LinetypeScale = this.LinetypeScale,
						Normal = this.Normal,
						StartPoint = this.Vertexes[Math.Abs(face.VertexIndexes[0]) - 1],
						EndPoint = this.Vertexes[Math.Abs(face.VertexIndexes[1]) - 1],
					};
					entities.Add(line);
					continue;
				}

				Face3DEdgeFlags edgeVisibility = Face3DEdgeFlags.None;

				short indexV1 = face.VertexIndexes[0];
				short indexV2 = face.VertexIndexes[1];
				short indexV3 = face.VertexIndexes[2];
				// Polyface mesh faces are made of 3 or 4 vertexes, we will repeat the third vertex if the number of face vertexes is three
				int indexV4 = face.VertexIndexes.Length == 3 ? face.VertexIndexes[2] : face.VertexIndexes[3];

				if (indexV1 < 0)
				{
					edgeVisibility |= Face3DEdgeFlags.First;
				}

				if (indexV2 < 0)
				{
					edgeVisibility |= Face3DEdgeFlags.Second;
				}

				if (indexV3 < 0)
				{
					edgeVisibility |= Face3DEdgeFlags.Third;
				}

				if (indexV4 < 0)
				{
					edgeVisibility |= Face3DEdgeFlags.Fourth;
				}

				Vector3 v1 = this.Vertexes[Math.Abs(indexV1) - 1];
				Vector3 v2 = this.Vertexes[Math.Abs(indexV2) - 1];
				Vector3 v3 = this.Vertexes[Math.Abs(indexV3) - 1];
				Vector3 v4 = this.Vertexes[Math.Abs(indexV4) - 1];

				Face3D face3D = new Face3D
				{
					Layer = (Layer)faceLayer.Clone(),
					Linetype = (Linetype)this.Linetype.Clone(),
					Color = (AciColor)faceColor.Clone(),
					Lineweight = this.Lineweight,
					Transparency = (Transparency)this.Transparency.Clone(),
					LinetypeScale = this.LinetypeScale,
					Normal = this.Normal,
					FirstVertex = v1,
					SecondVertex = v2,
					ThirdVertex = v3,
					FourthVertex = v4,
					EdgeFlags = edgeVisibility,
				};

				entities.Add(face3D);
			}

			return entities;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			for (int i = 0; i < this.Vertexes.Length; i++)
			{
				this.Vertexes[i] = transformation * this.Vertexes[i] + translation;
			}

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
			PolyfaceMesh entity = new PolyfaceMesh(this.Vertexes, this.Faces)
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
				//PolyfaceMesh properties
				Flags = this.Flags

			};

			foreach (XData data in this.XData.Values)
			{
				entity.XData.Add((XData)data.Clone());
			}

			return entity;
		}

		#endregion

		#region PolyfaceMeshFace events

		private void Faces_Item_BeforeChangingLayerValue(object sender, BeforeValueChangeEventArgs<Layer> e)
			=> e.NewValue = this.OnBeforeChangingFacesLayerValue(e.OldValue, e.NewValue, $"{nameof(this.Faces)}.{e.PropertyName}");

		#endregion
	}
}